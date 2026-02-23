import { AdminAPI } from "./api.js";

const $ = (id) => document.getElementById(id);

const fmtDateTR = (iso) => {
    if (!iso || !iso.includes("-")) return iso || "";
    const [y, m, d] = iso.split("T")[0].split("-");
    return `${d}.${m}.${y}`;
};

async function loadDashboard() {
    try {
        const [patients, doctors, appointments,departments] = await Promise.all([
            AdminAPI.patients().catch(() => []),
            AdminAPI.doctors().catch(() => []),
            AdminAPI.appointments().catch(() => []),
           
        ]);

        if ($("stat-patients")) $("stat-patients").innerText = patients.length || 0;
        if ($("stat-doctors")) $("stat-doctors").innerText = doctors.length || 0;
        if ($("stat-appts")) $("stat-appts").innerText = appointments.length || 0;

        // Doktorlar Tablosu
        const tDoctors = $("tDoctors");
        if (tDoctors) {
            tDoctors.innerHTML = (doctors || []).map(d => `
            <tr class="user-row" 
                data-doc='${JSON.stringify(d).replace(/'/g, "&#39;")}'>
                <td class="ps-4"><strong>${d.firstName || ''} ${d.lastName || ''}</strong></td>
                <td>${d.department?.departmentName || d.Department?.DepartmentName || 'Belirtilmemiş'}</td>
                <td>${d.email || '-'}</td>
                <td class="text-center"><span class="badge bg-success">Aktif</span></td>
                <td class="text-end pe-4">
                    <button onclick="deleteUser(${d.userID || d.UserID}, 'Doctor')" class="btn btn-danger btn-sm shadow-sm">Sil</button>
                </td>
            </tr>
        `).join("") || `<tr><td colspan="5" class="text-center text-muted py-3">Kayıtlı doktor yok</td></tr>`;

            document.querySelector("#tDoctors").addEventListener("click", (e) => {
                const btn = e.target.closest(".btn-edit-doc");
                if (!btn) return;
                const row = btn.closest("tr[data-doc]");
                const doc = JSON.parse(row.dataset.doc);
                openEditDoctorModal(doc);
            });
        }

        // Hastalar Tablosu
        const tPatients = $("tPatients");
        if (tPatients) {
            tPatients.innerHTML = (patients || []).map(p => `
                <tr class="user-row">
                    <td class="ps-4"><strong>${p.firstName || ''} ${p.lastName || ''}</strong></td>
                    <td>${p.nationalID || p.NationalID || '-'}</td>
                    <td>${p.email || '-'}</td>
                    <td class="text-center"><span class="badge bg-success">Aktif</span></td>
                    <td class="text-end pe-4">
                        <button onclick="deleteUser(${p.userID || p.UserID}, 'Patient')" class="btn btn-danger btn-sm shadow-sm">Sil</button>
                    </td>
                </tr>
            `).join("") || `<tr><td colspan="5" class="text-center text-muted py-3">Kayıtlı hasta yok</td></tr>`;
        }

        // Randevular Tablosu
        const tAppts = $("tAppts");
        if (tAppts) {
            tAppts.innerHTML = (appointments || []).map(a => {
                const dateStr = fmtDateTR(a.date);
                const docName = a.doctor ? `${a.doctor.firstName || ''} ${a.doctor.lastName || ''}`.trim() : 'Bilinmiyor';
                const patName = a.patient ? `${a.patient.firstName || ''} ${a.patient.lastName || ''}`.trim() : 'Bilinmiyor';
                const clinic = a.doctor?.department?.departmentName || a.doctor?.Department?.DepartmentName || '-';
                const appStatus = a.status || a.Status || '-';

                return `
                <tr>
                    <td class="ps-4">${dateStr} • ${a.time ? a.time.slice(0, 5) : ''}</td>
                    <td><strong>${clinic}</strong></td>
                    <td>👨‍⚕️ ${docName}</td>
                    <td>🧍 ${patName}</td>
                    <td > ${appStatus}</td>
                    <td class="text-end pe-4">
                        <button onclick="deleteAppointment(${a.appointmentID || a.AppointmentID})" class="btn btn-danger btn-sm shadow-sm">Sil</button>
                    </td>
                </tr>
                `;
            }).join("") || `<tr><td colspan="5" class="text-center text-muted py-3">Sistemde randevu bulunmuyor</td></tr>`;
        }

        // Klinikler Tablosu
        const tClinics = $("tClinics");
        if (tClinics && doctors) {
            const clinicMap = {};
            doctors.forEach(d => {
                const depName = d.department?.departmentName || d.Department?.DepartmentName || 'Bilinmeyen Klinik';
                clinicMap[depName] = (clinicMap[depName] || 0) + 1;
            });

            tClinics.innerHTML = Object.keys(clinicMap).map(cName => `
                <tr>
                    <td class="ps-4"><strong>${cName}</strong></td>
                    <td class="text-center">${clinicMap[cName]} Doktor</td>
                    <td class="text-end pe-4"><span class="badge bg-success">Aktif</span></td>
                </tr>
            `).join("") || `<tr><td colspan="3" class="text-center text-muted py-3">Klinik verisi yok</td></tr>`;
        }

    } catch (err) {
        console.error("Dashboard yüklenirken hata oluştu:", err);
    }
}

document.addEventListener("DOMContentLoaded", loadDashboard);

window.submitDoctor = async function (e) {
    e.preventDefault();

    const dto = {
        userName: $("d-user").value,
        password: $("d-pass").value,
        firstName: $("d-first").value,
        lastName: $("d-last").value,
        email: $("d-mail").value,
        phoneNumber: $("d-phone").value,
        departmentID: parseInt($("d-dep").value)
    };

    try {
        await AdminAPI.createDoctor(dto);
        Swal.fire("Yeni doktor başarıyla sisteme eklendi!");
        $("doctorModal").style.setProperty('display', 'none', 'important');
        $("addDoctorForm").reset();
        await loadDashboard();
    } catch (error) {
        Swal.fire("Doktor eklenirken hata oluştu: " + error.message);
    }
};

window.deleteUser = async function (id, roleName) {
    if (roleName === "Admin") {
        Swal.fire("Hata", "Sistem yöneticisi silinemez!", "error");
        return;
    }

    const roleText = roleName === 'Doctor' ? 'Doktoru' : 'Hastayı';

    const result = await Swal.fire({
        title: 'Emin misiniz?',
        text: `Bu ${roleText} kalıcı olarak silinecek. Onaylıyor musunuz?`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#6c757d',
        confirmButtonText: '<i class="bi bi-trash"></i> Evet, Sil',
        cancelButtonText: 'Vazgeç'
    });

    if (!result.isConfirmed) return;

    try {
        if (roleName === "Doctor") {
            await AdminAPI.deleteDoctor(id);
        } else {
            await AdminAPI.deletePatient(id);
        }
        Swal.fire("Silindi!", "Kullanıcı başarıyla silindi.", "success");
        await loadDashboard();
    } catch (error) {
        Swal.fire("Hata!", "Silme işlemi başarısız. Bu kullanıcının aktif randevuları olabilir. Hata: " + (error.message || "Bilinmeyen Hata"), "error");
    }
};

window.deleteAppointment = async function (id) {
    const result = await Swal.fire({
        title: 'Randevuyu Sil',
        text: "Bu randevuyu sistemden kalıcı olarak silmek istediğinize emin misiniz?",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#6c757d',
        confirmButtonText: '<i class="bi bi-trash"></i> Evet, Sil',
        cancelButtonText: 'Vazgeç'
    });

    if (!result.isConfirmed) return;

    try {
        await AdminAPI.deleteAppointment(id);
        Swal.fire("Başarılı!", "Randevu sistemden silindi.", "success");
        await loadDashboard();
    } catch (error) {
        Swal.fire("Silinemedi!", "Bu randevuya yazılmış bir reçete olduğu için silinemiyor olabilir. Hata: " + (error.message || "Sunucu hatası"), "error");
    }
};
window.filterUsers = function () {
    const input = $("u-ara")?.value.toLowerCase() || "";
    document.querySelectorAll(".user-row").forEach(row => {
        row.style.display = row.innerText.toLowerCase().includes(input) ? "" : "none";
    });
};


    