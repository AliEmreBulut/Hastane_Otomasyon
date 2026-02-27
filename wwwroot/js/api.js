const API_BASE = "/api";

async function request(path, { method = "GET", body, headers = {} } = {}) {
    const res = await fetch(`${API_BASE}${path}`, {
        method,
        headers: { "Content-Type": "application/json", ...headers },
        body: body ? JSON.stringify(body) : undefined,
        credentials: "include"
    });
    let data = null;
    try { data = await res.json(); } catch (e) { }
    if (!res.ok) {
        const msg = data?.message || data?.error || res.statusText;
        throw new Error(msg);
    }
    return data;
}

export const AuthAPI = {
    me: async (userId) => {
        const res = await fetch(`${API_BASE}/Auth/me?userId=${encodeURIComponent(userId)}`, { credentials: "include" });
        if (!res.ok) throw new Error("Kimlik doğrulama başarısız");
        return await res.json();
    }
};

//Patient
export const PatientAPI = {
    register: (payload) => request("/Patient/Register", { method: "POST", body: payload }),
    login: (payload) => request("/Patient/Login", { method: "POST", body: payload }),
    myProfile: (pid) => request(`/Patient/MyProfile/${pid}`),
    updateProfile: (pid, payload) => request(`/Patient/UpdateProfile/${pid}`, { method: "PUT", body: payload }),
    myAppointments: (pid) => request(`/Patient/appointments/${pid}`),
    availableSlots: (doctorId, date) => request(`/Patient/AvailableSlots/${doctorId}/${date}`),
    takeAppointment: (payload) => request("/Patient/TakeAppointment", { method: "POST", body: payload }),
    cancelAppointment: (id) => request(`/Patient/CancelAppointment/${id}`, { method: "PUT" })
};




// Doctor
export const DoctorAPI = {
    patients: (doctorId) => request(`/Doctor/patients/${doctorId}`),
    appointments: (doctorId) => request(`/Doctor/appointments/${doctorId}`),
    schedule: (doctorId, date) => request(`/Doctor/schedule/${doctorId}/${date}`),
    addPrescription: (appointmentId, payload) => request(`/Doctor/AddPrescription/${appointmentId}`, { method: "POST", body: payload }),
    updateProfile: (doctorId, payload) => request(`/Doctor/UpdateProfile/${doctorId}`, { method: "PUT", body: payload })
};

// Admin
export const AdminAPI = {
      users:         ()      => request("/Admin/users"),
      patients:      ()      => request("/Admin/patients"),
      patient:       (id)    => request(`/Admin/patient/${id}`),
      updatePatient: (id,p)  => request(`/Admin/patient/${id}`, { method: "PUT", body: p }),
      deletePatient: (id)    => request(`/Admin/patient/${id}`, { method: "DELETE" }),
      doctors:       ()      => request("/Admin/doctors"),
      doctor:        (id)    => request(`/Admin/doctor/${id}`),
      createDoctor:  (p)     => request("/Admin/doctor", { method: "POST", body: p }),
      deleteDoctor:  (id)    => request(`/Admin/doctor/${id}`, { method: "DELETE" }),
      appointments:  ()      => request("/Admin/appointments"),
      deleteAppointment: (id) => request(`/Admin/appointment/${id}`, { method: "DELETE" }),
      updateDoctor: (id, p) => request(`/Admin/doctor/${id}`, { method: "PUT", body: p }),
      departments: () => request("/Admin/departments"),
};

export const SystemAPI = {
    medicines: () => request("/Medicines")
};