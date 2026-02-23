import { DoctorAPI, AuthAPI, SystemAPI } from "./api.js";

const $ = (id) => document.getElementById(id);
const setText = (id, v, fb = "—") => { const el = $(id); if (el) el.textContent = (v ?? "").toString().trim() || fb; };
const fmtDateTR = (iso) => {
    if (!iso || !iso.includes("-")) return iso || "";
    const [y, m, d] = iso.split("T")[0].split("-");
    return `${d}.${m}.${y}`;
};

async function loadProfile() {
    const userId = localStorage.getItem("userId");
    if (!userId) {
        Swal.fire("Oturum yok. Lütfen giriş yapın.");
        location.replace("/giris.html");
        return null;
    }

    try {
        const me = await AuthAPI.me(userId);
        const title = me.role === "Doctor" ? "Doktor" : (me.role || "Doktor");
        const fullTitleAndName = `${title} ${me.fullName}`;

        setText("docName", fullTitleAndName);
        setText("p-fullname", fullTitleAndName);
        setText("p-email", me.email);
        setText("p-phone", me.phone);
        setText("p-department", me.department);

        localStorage.setItem("userFullName", me.fullName || "");
        if (me.userId) localStorage.setItem("doctorId", String(me.userId));
        if (me.role) localStorage.setItem("role", me.role);

        return me;
    } catch (err) {
        console.error("Profil yükleme hatası:", err);
        return null;
    }
}

async function loadAppointments() {
    const doctorId = localStorage.getItem("doctorId");
    if (!doctorId) return console.warn("Doktor ID bulunamadı.");

    try {
        const list = await DoctorAPI.appointments(doctorId);

        let countToday = 0, countPending = 0, countCompleted = 0, countRx = 0;
        const todayStr = new Date().toISOString().split('T')[0];
        let upcomingList = [], pastList = [], rxList = [];

        (list ?? []).forEach(a => {
            if (a.date === todayStr) countToday++;
            if (a.status === 'Onaylandı' || a.status === 'Bekliyor') countPending++;
            if (a.status === 'Tamamlandı') countCompleted++;
            if (a.prescription) countRx++;

            const isPast = ['Tamamlandı', 'İptal Edildi', 'Cancelled'].includes(a.status);

            if (isPast) {
                pastList.push(a);
                if (a.prescription) rxList.push(a);
            } else {
                upcomingList.push(a);
            }
        });

        upcomingList.sort((a, b) => new Date(`${a.date}T${a.time}`) - new Date(`${b.date}T${b.time}`));
        pastList.sort((a, b) => new Date(`${b.date}T${b.time}`) - new Date(`${a.date}T${a.time}`));
        rxList.sort((a, b) => new Date(`${b.date}T${b.time}`) - new Date(`${a.date}T${a.time}`));

        const upcomingHTML = upcomingList.map(a => `
            <tr>
                <td class="ps-4">${a.patient?.nationalID || "—"}</td>
                <td><strong>${a.patient?.firstName || ''} ${a.patient?.lastName || ''}</strong></td>
                <td>${fmtDateTR(a.date)}</td>
                <td>${a.time ? a.time.slice(0, 5) : ''}</td>
                <td class="text-center"><span class="badge bg-warning text-dark">${a.status}</span></td>
                <td class="text-end pe-4">
                    <button onclick="openPrescriptionModal(${a.appointmentID})" class="btn btn-info btn-sm text-white shadow-sm"><i class="bi bi-search"></i> İncele</button>
                </td>
            </tr>`).join("");

        const pastHTML = pastList.map(a => {
            const rxText = a.prescription ? `<b>${a.prescription.diagnosis || a.prescription.Diagnosis}</b><br><small class="text-muted">${a.prescription.description || a.prescription.Description}</small>` : "—";
            return `
            <tr>
                <td class="ps-4"><strong>${a.patient?.firstName || ''} ${a.patient?.lastName || ''}</strong><br><small class="text-muted">${fmtDateTR(a.date)} • ${a.time ? a.time.slice(0, 5) : ''}</small></td>
                <td>${rxText}</td>
                <td class="text-center"><span class="badge ${a.status === 'Tamamlandı' ? 'bg-success' : 'bg-danger'}">${a.status}</span></td>
                <td class="text-end pe-4"><button class="btn btn-secondary btn-sm shadow-sm" disabled>Tamamlandı</button></td>
            </tr>`;
        }).join("");

        const rxHTML = rxList.map(a => {
            let medsList = "—";

            const medicinesArray = a.prescription?.medicines || a.prescription?.prescription_Medicines || a.prescription?.prescriptionMedicines || [];

            if (medicinesArray.length > 0) {
                medsList = medicinesArray.map(m => {
                    const medName = m.name || m.Name || m.medicineName || m.MedicineName || `İlaç ID: ${m.medicineID || m.MedicineID || '?'}`;
                    const medUsage = m.description || m.Description || m.usage || "";

                    return `• <strong>${medName}</strong> <small class="text-muted">(${medUsage})</small>`;
                }).join("<br>");
            }

            const rxId = a.prescription?.id || a.prescription?.prescriptionID || a.prescription?.PrescriptionID || 0;

            return `
            <tr>
                <td class="ps-4"><strong>${a.patient?.firstName || ''} ${a.patient?.lastName || ''}</strong><br><small class="text-muted">${fmtDateTR(a.date)} • ${a.time ? a.time.slice(0, 5) : ''}</small></td>
                <td><strong>#RX-${rxId.toString().padStart(4, '0')}</strong></td>
                <td>${medsList}</td>
                <td class="text-center"><span class="badge bg-success">Sisteme İşlendi</span></td>
                <td class="text-end pe-4"><button class="btn btn-outline-primary btn-sm shadow-sm" onclick="Swal.fire('Reçete detayı yazdırılıyor...')"><i class="bi bi-printer"></i> Yazdır</button></td>
            </tr>`;
        }).join("");

        setText("stat-today", countToday);
        setText("stat-pending", countPending);
        setText("stat-completed", countCompleted);
        setText("stat-rx", countRx);

        $("apptTbody").innerHTML = upcomingHTML || `<tr><td colspan="6" class="text-center text-muted py-3">Bekleyen randevu yok</td></tr>`;
        $("examTbody").innerHTML = pastHTML || `<tr><td colspan="4" class="text-center text-muted py-3">Geçmiş kayıt bulunmuyor</td></tr>`;
        $("rxTbody").innerHTML = rxHTML || `<tr><td colspan="5" class="text-center text-muted py-3">Henüz yazılmış bir reçete bulunmuyor</td></tr>`;

    } catch (e) {
        console.error("Randevular alınamadı:", e);
    }
}

document.addEventListener("DOMContentLoaded", async () => {
    const me = await loadProfile();
    if (me) await loadAppointments();

    const btnEdit = $("btnEditProfile");
    if (btnEdit) {
        btnEdit.addEventListener("click", () => {
            $("e-email").value = $("p-email").textContent === "—" ? "" : $("p-email").textContent;
            $("e-phone").value = $("p-phone").textContent === "—" ? "" : $("p-phone").textContent;
            $("editProfileModal").style.display = "flex";
        });
    }

    const btnCloseEdit = $("btnCloseEditModal");
    if (btnCloseEdit) btnCloseEdit.addEventListener("click", () => $("editProfileModal").style.setProperty("display", "none", "important"));

    const editForm = $("editProfileForm");
    if (editForm) {
        editForm.addEventListener("submit", async (e) => {
            e.preventDefault();
            const payload = { email: $("e-email").value, phoneNumber: $("e-phone").value };
            try {
                await DoctorAPI.updateProfile(localStorage.getItem("doctorId"), payload);
                Swal.fire("Profil başarıyla güncellendi.");
                $("editProfileModal").style.setProperty("display", "none", "important");
                location.reload();
            } catch (err) {
                Swal.fire("Güncelleme başarısız: " + err.message);
            }
        });
    }

    const rxForm = $("prescriptionForm");
    if (rxForm) {
        rxForm.addEventListener("submit", async (e) => {
            e.preventDefault();
            const apptId = $("modalApptId").value;
            const payload = {
                Diagnosis: $("m-diagnosis").value,
                Description: $("m-description").value,
                prescription_Medicines: []
            };

            document.querySelectorAll(".medicine-row-item").forEach(row => {
                const medId = row.querySelector(".med-select").value;
                const desc = row.querySelector(".med-usage").value;
                if (medId) payload.prescription_Medicines.push({ MedicineID: parseInt(medId), Description: desc });
            });

            try {
                await DoctorAPI.addPrescription(apptId, payload);
                Swal.fire("Reçete başarıyla kaydedildi!");
                window.closePrescriptionModal();
                await loadAppointments();
            } catch (err) {
                Swal.fire("Reçete kaydedilemedi: " + err.message);
            }
        });
    }
});

window.openPrescriptionModal = function (apptId) {
    $("modalApptId").value = apptId;
    $("medicineList").innerHTML = "";
    window.addMedicineRow();
    $("prescriptionModal").style.setProperty("display", "flex", "important");
};

window.closePrescriptionModal = () => $("prescriptionModal").style.setProperty("display", "none", "important");

window.addMedicineRow = async () => {
    const list = $("medicineList");
    const div = document.createElement("div");

    div.className = "row g-2 mb-2 align-items-center opacity-0 medicine-row-item";
    div.style.transition = "opacity 0.3s ease-in";

    try {
        const medicines = await SystemAPI.medicines();

        div.innerHTML = `
            <div class="col-md-6">
                <select class="form-select form-select-sm shadow-sm med-select" required>
                    <option value="">İlaç Seçin...</option>
                    ${medicines.map(m => `<option value="${m.medicineID || m.MedicineID}">${m.medicineName || m.MedicineName}</option>`).join("")}
                </select>
            </div>
            <div class="col-md-5">
                <input type="text" class="form-control form-control-sm shadow-sm med-usage" placeholder="Kullanım (Örn: 2x1 Tok)" required>
            </div>
            <div class="col-md-1 text-end">
                <button type="button" onclick="this.parentElement.parentElement.remove()" class="btn btn-outline-danger btn-sm border-0 shadow-none">
                    <i class="bi bi-trash3"></i>
                </button>
            </div>
        `;

        list.appendChild(div);
        setTimeout(() => div.classList.replace("opacity-0", "opacity-100"), 10);

    } catch (err) {
        console.error("İlaçlar yüklenemedi:", err);
        div.innerHTML = `
            <div class="col-12">
                <div class="alert alert-danger py-1 px-2 mb-0 small">
                    <i class="bi bi-exclamation-circle me-1"></i> İlaç listesi veritabanından alınamadı.
                </div>
            </div>
        `;
        list.appendChild(div);
        setTimeout(() => div.classList.replace("opacity-0", "opacity-100"), 10);
    }
};