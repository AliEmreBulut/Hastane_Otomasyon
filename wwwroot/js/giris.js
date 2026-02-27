const API_BASE = "/api";
const LOGIN_URL = new URL("api/Login/login", location.origin).href;

const PANEL_PATHS = {
    Patient: "/sayfalar/hasta-panel.html",
    Doctor: "/sayfalar/doktor-panel.html",
    Admin: "/sayfalar/admin-panel.html",
};

const LoginAPI = {
    login: async (payload) => {
        const formData = new FormData();
        formData.append("username", payload.username ?? payload.UserName ?? "");
        formData.append("password", payload.password ?? payload.Password ?? "");

        const res = await fetch(LOGIN_URL, {
            method: "POST",
            body: formData,
            credentials: "include",
        });

        if (!res.ok) {
            const errorText = await res.text().catch(() => "");
            throw new Error(errorText || "Giriş Başarısız");
        }

        const ct = res.headers.get("content-type") || "";
        if (ct.includes("application/json")) return await res.json();

        if (res.redirected && res.url) return { redirectTo: res.url };

        const maybeUrl = await res.text().catch(() => "");
        if (/^\/|^https?:\/\//.test(maybeUrl)) return { redirectTo: maybeUrl };

        return { ok: true };
    }
};


const PatientAPI = {
    register: async (payload) => {
        const url = `${API_BASE}/Patient/Register`;

        const res = await fetch(url, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload),
            credentials: "include",
        });

        if (!res.ok) throw new Error(await res.text().catch(() => "Kayıt başarısız"));
        return await res.json();
    }
};



const form = document.getElementById("loginForm");

form.addEventListener("submit", async (e) => {
    e.preventDefault();

    const kullanici = form.elements.namedItem("username");
    const sifre = form.elements.namedItem("password");

    const username = kullanici?.value?.trim() ?? "";
    const password = sifre?.value ?? "";

    if (!username || !password) {
        Swal.fire("Kullanıcı adı ve şifre zorunlu");
        // Teşhis için:
        console.log({ kullanici, sifre, username, password });
        return;
    }

    try {
        const result = await LoginAPI.login({ username, password });

        if (result?.fullName) localStorage.setItem("userFullName", result.fullName);
        if (result?.userId) localStorage.setItem("userId", String(result.userId));
        if (result?.role) localStorage.setItem("role", result.role);
        if (result?.redirectTo) {
            location.replace(result.redirectTo);
        } else {
            Swal.fire("Giriş başarılı ama yönlendirme bilgisi gelmedi.");
            console.warn("Login response:", result);
        }
    } catch (err) {
        Swal.fire(err?.message || "Giriş başarısız.");
        console.error(err);
    }
});


const registerForm = document.getElementById("registerForm");
if (registerForm) {
    registerForm.addEventListener("submit", async (e) => {
        e.preventDefault();
        const f = e.currentTarget;

        const birthDateInput = f.querySelector("input[name='BirthDate'],input[name='birthdate']");
        const birthDateValue = birthDateInput ? birthDateInput.value : "";
        if (!birthDateValue) return Swal.fire("Lütfen doğum tarihini seçin");

        const payload = {
            FirstName: f.FirstName?.value ?? f.firstName?.value ?? "",
            LastName: f.LastName?.value ?? f.lastName?.value ?? "",
            NationalID: f.NationalID?.value ?? f.nationalId?.value ?? "",
            Email: f.Email?.value ?? f.email?.value ?? "",
            PhoneNumber: f.PhoneNumber?.value ?? f.phoneNumber?.value ?? "",
            Address: f.Address?.value ?? f.address?.value ?? "",
            Gender: f.Gender?.value ?? f.gender?.value ?? "Belirtilmedi",
            BirthDate: birthDateValue,
            UserName: f.UserName?.value ?? f.username?.value ?? "",
            Password: f.Password?.value ?? f.password?.value ?? "",
            BloodType: f.BloodType?.value ?? ""
        };

        try {
            const data = await PatientAPI.register(payload);
            Swal.fire(data?.Message || "Kayıt başarılı");
        } catch (err) {
            console.error(err);
            Swal.fire("Kayıt hatası: " + (err?.message || "Bir hata oluştu"));
        }
    });
}
