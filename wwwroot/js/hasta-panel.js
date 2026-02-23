import { PatientAPI, AuthAPI, AdminAPI } from "./api.js";

const $ = (id) => document.getElementById(id);
const setText = (id, v, fb = "—") => { const el = $(id); if (el) el.textContent = (v && v.toString().trim()) || fb; };
const fmtDateTR = (iso) => {
    if (!iso || !iso.includes("-")) return iso || "";
    const [y, m, d] = iso.split("-");
    return `${d}.${m}.${y}`;
};
const fmtDate = (d) => {
    if (!d || d.toString().startsWith("0001")) return "—";
    return d.toString().includes("T") ? d.toString().slice(0, 10) : d;
};

async function loadProfile() {
    const userId = localStorage.getItem("userId");
    if (!userId) {
        Swal.fire("Oturum bilgisi bulunamadı, lütfen giriş yapın.");
        location.replace("/giris.html");
        return null;
    }

    try {
        const me = await AuthAPI.me(userId);
        setText("ptName", me.fullName || "Hasta");
        setText("p-email", me.email);
        setText("p-phone", me.phone);
        setText("p-birthdate", fmtDate(me.birthDate));
        setText("p-nationalid", me.nationalId);
        setText("p-address", me.address);
        setText("p-bloodtype", me.bloodType);

        localStorage.setItem("userFullName", me.fullName || "");
        localStorage.setItem("patientId", String(me.userId));
        if (me.role) localStorage.setItem("role", me.role);

        return me;
    } catch (err) {
        console.error("Profil yükleme hatası:", err);
        setText("ptName", localStorage.getItem("userFullName") || "Hasta");
        return null;
    }
}

async function loadDoctorsAndDepartments() {
    try {
        const doctors = await AdminAPI.doctors();
        window.allDoctors = doctors;

        const deptMap = new Map();
        doctors.forEach((d) => {
            const dep = d.department || d.Department || {};
            const depId = dep.departmentID ?? dep.DepartmentID ?? d.departmentId ?? d.departmentID;
            const depName = dep.departmentName ?? dep.DepartmentName ?? d.departmentName;
            if (depId && depName) deptMap.set(String(depId), depName);
        });

        const selDept = $("r-department");
        if (selDept) {
            selDept.innerHTML = '<option value="">Bölüm Seçiniz</option>' +
                [...deptMap.entries()].map(([id, name]) => `<option value="${id}">${name}</option>`).join("");
        }

        // Doktor Dropdown
        updateDoctorDropdown(doctors);
    } catch (e) {
        console.error("Doktor/Bölüm yükleme hatası:", e);
    }
}

function updateDoctorDropdown(doctors) {
    const selDoctor = $("r-doctor");
    if (!selDoctor) return;

    if (!doctors || doctors.length === 0) {
        selDoctor.innerHTML = '<option value="">Doktor bulunamadı</option>';
        return;
    }

    selDoctor.innerHTML = '<option value="">Doktor Seçiniz</option>' +
        doctors.map((d) => {
            const dept = d.department?.departmentName || d.Department?.DepartmentName || "";
            const name = `${d.firstName ?? d.FirstName ?? ""} ${d.lastName ?? d.LastName ?? ""}`.trim();
            const uid = d.userID ?? d.UserID;
            return `<option value="${uid}">${name}${dept ? " - " + dept : ""}</option>`;
        }).join("");
}

async function loadMyAppointments() {
    const patientId = localStorage.getItem("patientId");
    if (!patientId) return;

    try {
        const appointments = await PatientAPI.myAppointments(patientId);

        let countUpcoming = 0, countCompleted = 0, countRx = 0, countCancelled = 0;
        let upcomingList = [], pastList = [];

        appointments.forEach(a => {
            const isPast = a.status === 'Tamamlandı' || a.status === 'İptal Edildi' || a.status === 'Cancelled';

            if (a.status === 'Onaylandı' || a.status === 'Bekliyor') countUpcoming++;
            if (a.status === 'Tamamlandı') countCompleted++;
            if (a.status === 'İptal Edildi' || a.status === 'Cancelled') countCancelled++;
            if (a.prescription) countRx++;

            isPast ? pastList.push(a) : upcomingList.push(a);
        });

        upcomingList.sort((a, b) => new Date(`${a.date}T${a.time}`) - new Date(`${b.date}T${b.time}`));
        pastList.sort((a, b) => new Date(`${b.date}T${b.time}`) - new Date(`${a.date}T${a.time}`));

        const upcomingHTML = upcomingList.map(a => `
            <tr>
                <td class="ps-4">${fmtDateTR(a.date)} • ${a.time.slice(0, 5)}</td>
                <td>${a.doctor?.department || '—'}</td>
                <td>${a.doctor?.fullName || '—'}</td>
                <td><span class="badge ${a.status === 'Onaylandı' ? 'bg-success' : 'bg-warning text-dark'}">${a.status}</span></td>
                <td class="pe-4">
                    <button type="button" class="btn btn-danger btn-sm" onclick="cancelAppointment(${a.appointmentID})">İptal</button>
                </td>
            </tr>`).join("");

        const pastHTML = pastList.map(a => {
            const actionCell = a.status === 'Tamamlandı'
                ? `<button class="btn btn-outline-primary btn-sm rounded-pill px-3" onclick='showPrescription(${JSON.stringify(a)})'><i class="bi bi-eye"></i> Reçete</button>`
                : `<span class="badge bg-danger">${a.status}</span>`;

            return `
            <tr>
                <td class="ps-4">${fmtDateTR(a.date)}<br><small class="text-muted">${a.time.slice(0, 5)}</small></td>
                <td><strong>${a.doctor?.department || '—'}</strong><br><small>${a.doctor?.fullName || '—'}</small></td>
                <td>${a.prescription ? `<span class="text-primary fw-medium">${a.prescription.diagnosis}</span>` : '<span class="text-muted small">Kayıt Yok</span>'}</td>
                <td class="text-end pe-4">${actionCell}</td>
            </tr>`;
        }).join("");

        setText("stat-upcoming", countUpcoming);
        setText("stat-completed", countCompleted);
        setText("stat-rx", countRx);
        setText("stat-cancelled", countCancelled);

        const upcomingTbody = $("upcomingTbody");
        const pastTbody = $("pastTbody");

        if (upcomingTbody) upcomingTbody.innerHTML = upcomingHTML || `<tr><td colspan="5" style="text-align:center; color:#999;">Yaklaşan randevunuz bulunmuyor.</td></tr>`;
        if (pastTbody) pastTbody.innerHTML = pastHTML || `<tr><td colspan="5" style="text-align:center; color:#999;">Geçmiş muayeneniz bulunmuyor.</td></tr>`;

    } catch (err) {
        console.error("Randevuları yüklerken hata:", err);
    }
}

document.addEventListener("DOMContentLoaded", async () => {
    const me = await loadProfile();
    if (!me) return;

    await loadDoctorsAndDepartments();
    await loadMyAppointments();

    // Bölüm değiştiğinde doktorları filtrele
    const selDept = $("r-department");
    if (selDept) {
        selDept.addEventListener("change", () => {
            const deptId = selDept.value;
            const filtered = (window.allDoctors || []).filter(d => {
                const dep = d.department || d.Department || {};
                const id = dep.departmentID ?? dep.DepartmentID ?? d.departmentId ?? d.departmentID;
                return !deptId || String(id) === String(deptId);
            });
            updateDoctorDropdown(filtered);
        });
    }
    const selDoctor = $("r-doctor");
    if (selDoctor) {
        selDoctor.addEventListener("change", () => {
            const docId = selDoctor.value;
            if (!docId) return;
            const doctor = (window.allDoctors || []).find(d => String(d.userID || d.UserID) === String(docId));
            if (doctor) {
                const depId = doctor.department?.departmentID ?? doctor.Department?.DepartmentID ?? doctor.departmentId ?? doctor.departmentID;
                if (depId) $("r-department").value = depId;
            }
        });
    }

    // Randevu Oluşturma Formu
    const appointmentForm = document.querySelector("#randevu-olustur form");
    if (appointmentForm) {
        appointmentForm.addEventListener("submit", async (e) => {
            e.preventDefault();
            const payload = {
                patientId: Number(localStorage.getItem("patientId")),
                doctorId: Number($("r-doctor")?.value),
                date: $("r-date")?.value,
                time: $("r-time")?.value.split(':').length === 2 ? $("r-time").value + ':00' : $("r-time").value
            };

            if (!payload.patientId || !payload.doctorId || !payload.date || !payload.time) {
                return Swal.fire("Lütfen tüm alanları doldurun");
            }

            try {
                const data = await PatientAPI.takeAppointment(payload);
                Swal.fire(data?.message || "Randevu başarıyla oluşturuldu!");
                e.target.reset();
                $("availableSlots").style.display = "none";
                await loadMyAppointments();
            } catch (err) {
                Swal.fire("Randevu hatası: " + err.message);
            }
        });
    }

    // Boş Saatleri Getir
    const btnCheckSlots = $("btnCheckSlots");
    if (btnCheckSlots) {
        btnCheckSlots.addEventListener("click", async () => {
            const doctorId = $("r-doctor")?.value;
            const date = $("r-date")?.value;
            if (!doctorId || !date) return Swal.fire("Önce doktor ve tarih seçin");

            try {
                const slots = await PatientAPI.availableSlots(doctorId, date);
                const slotList = $("slotList");

                if (!Array.isArray(slots) || slots.length === 0) {
                    slotList.innerHTML = "<p style='color:#999;'>Bu tarihte müsait saat yok</p>";
                } else {
                    slotList.innerHTML = slots.map(s => `<button type="button" class="btn btn-sm slot-btn" data-time="${s}">${s}</button>`).join(" ");
                }

                slotList.onclick = (ev) => {
                    if (ev.target.classList.contains("slot-btn")) {
                        $("r-time").value = ev.target.getAttribute("data-time");
                    }
                };
                $("availableSlots").style.display = "block";
            } catch (e) {
                Swal.fire("Saatler yüklenemedi: " + e.message);
            }
        });
    }

    // Profil Düzenleme Formu
    const editForm = $("editProfileForm");
    if (editForm) {
        editForm.onsubmit = async (e) => {
            e.preventDefault();
            const payload = {
                Email: $("e-email").value,
                PhoneNumber: $("e-phone").value,
                BloodType: $("e-bloodtype").value,
                Address: $("e-address").value
            };

            try {
                await PatientAPI.updateProfile(localStorage.getItem("patientId"), payload);
                Swal.fire("İletişim bilgileriniz başarıyla güncellendi.");
                window.closeEditModal();
                location.reload();
            } catch (err) {
                Swal.fire("Güncelleme başarısız: " + err.message);
            }
        };
    }
});

window.cancelAppointment = async function (appointmentId) {
    if (!Swal.fire("Bu randevuyu iptal etmek istediğinize emin misiniz?")) return;
    try {
        await PatientAPI.cancelAppointment(appointmentId);
        Swal.fire("Randevunuz iptal edildi.");
        await loadMyAppointments();
    } catch (err) {
        Swal.fire("İptal işlemi başarısız: " + err.message);
    }
};

window.openEditModal = () => {
    $("e-tc").value = $("p-nationalid").textContent;
    $("e-birthdate").value = $("p-birthdate").textContent;
    $("e-email").value = $("p-email").textContent;
    $("e-phone").value = $("p-phone").textContent;
    $("e-address").value = $("p-address").textContent;
    $("e-bloodtype").value = $("p-bloodtype").textContent === "—" ? "" : $("p-bloodtype").textContent;
    $("editProfileModal").style.setProperty("display", "flex", "important");
};

window.closeEditModal = () => {
    $("editProfileModal").style.setProperty("display", "none", "important");
};


window.showPrescription = (appt) => {
    if (!appt.prescription) {
        return Swal.fire("Bilgi", "Bu muayeneye ait reçete bulunamadı.", "info");
    }

    $("view-diagnosis").textContent = appt.prescription.diagnosis || appt.prescription.Diagnosis || "Tanı Belirtilmemiş";
    $("view-description").textContent = appt.prescription.description || appt.prescription.Description || "Doktor notu bulunmuyor.";

    const medList = $("view-medicines");
    medList.innerHTML = "";

    const medicinesArray = appt.prescription.medicines || appt.prescription.Medicines || [];

    if (medicinesArray && medicinesArray.length > 0) {
        medicinesArray.forEach(m => {
            const medName = m.name || m.Name || m.medicineName || m.MedicineName || "Bilinmeyen İlaç";
            const medUsage = m.usage || m.Usage || m.description || m.Description || "";

            const item = document.createElement("div");
            item.className = "list-group-item px-0 border-0 d-flex justify-content-between align-items-center";
            item.innerHTML = `
                <div>
                    <i class="bi bi-check2-circle text-success me-2"></i>
                    <span class="fw-bold">${medName}</span>
                </div>
                <span class="badge bg-light text-dark border">${medUsage}</span>
            `;
            medList.appendChild(item);
        });
    } else {
        medList.innerHTML = "<p class='text-muted small'>İlaç listesi boş.</p>";
    }

    $("prescriptionDetailModal").style.setProperty("display", "flex", "important");
};
window.closePrescriptionModal = () => {
    $("prescriptionDetailModal").style.setProperty("display", "none", "important");
};