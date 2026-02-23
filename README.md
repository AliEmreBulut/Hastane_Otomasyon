# 🏥 Hastane Otomasyon Sistemi (Hospital Management System)

Bu proje, hastane süreçlerini dijitalleştirmek, hasta-doktor etkileşimini kolaylaştırmak ve sistem yöneticilerine tam kontrol sağlamak amacıyla geliştirilmiş kapsamlı bir **Full-Stack** web uygulamasıdır. Backend tarafında **ASP.NET Core Web API**, veritabanı yönetiminde **Entity Framework Core (MySQL/MariaDB)** ve frontend tarafında modern, dinamik bir **Vanilla JS + Bootstrap** mimarisi kullanılmıştır.

## 🚀 Özellikler ve Kullanıcı Rolleri

Sistem, üç farklı kullanıcı rolü üzerinden güvenli ve özelleştirilmiş deneyim sunar:

### 🛡️ Admin (Sistem Yöneticisi) Paneli
* **Genel Özet (Dashboard):** Sistemdeki aktif hasta, doktor ve randevu sayılarını anlık takip etme.
* **Doktor ve Hasta Yönetimi:** Sisteme yeni doktor ekleme, mevcut doktor ve hasta kayıtlarını görüntüleme, güncelleme ve silme (CRUD işlemleri).
* **Randevu Kontrolü:** Tüm hastanedeki randevuları filtreleme ve gerektiğinde iptal etme yetkisi.
* **Klinik Yönetimi:** Departmanlara göre doktor dağılımlarını görüntüleme.

### 👨‍⚕️ Doktor Paneli
* **Randevu Takibi:** Günlük ve geçmiş randevuları listeleme, hasta detaylarını ve teşhisleri inceleme.
* **Reçete ve Teşhis Yönetimi:** Tamamlanan muayeneler için veritabanına kayıtlı ilaçları aratarak reçete oluşturma (N-N ilişkili dinamik ilaç listesi) ve doktor notu ekleme.
* **Profil Yönetimi:** İletişim bilgilerini (E-posta, Telefon) güvenli bir şekilde güncelleme.

### 🧍 Hasta Paneli
* **Kayıt ve Kimlik Doğrulama:** TC Kimlik no ve BCrypt ile şifrelenmiş parolalar ile güvenli giriş/kayıt işlemleri.
* **Dinamik Randevu Sistemi:** İlgili departman ve doktoru seçtikten sonra, doktorun **yalnızca boş saatlerini (Available Slots)** görerek çakışmasız randevu alabilme.
* **Tıbbi Geçmiş:** Geçmiş randevuları, konulan teşhisleri ve yazılan reçeteleri detaylıca görüntüleyebilme.
* **Profil Yönetimi:** Kan grubu, adres ve iletişim gibi kişisel bilgileri yönetme.

## 🛠️ Kullanılan Teknolojiler

* **Backend:** C#, ASP.NET Core 8+ Web API
* **ORM:** Entity Framework Core (Code-First)
* **Veritabanı:** MySQL / MariaDB (Pomelo.EntityFrameworkCore.MySql)
* **Güvenlik:** BCrypt.Net (Parola Hashleme)
* **Frontend:** HTML5, CSS3, Vanilla JavaScript (Fetch API), Bootstrap 5
* **UI Eklentileri:** SweetAlert2 (Gelişmiş bildirimler), Bootstrap Icons

## 🗄️ Veritabanı Mimarisi

Projede **Domain-Driven Design (DDD)** prensiplerinden esinlenilmiş ve Entity Framework üzerinde **Table-Per-Type (TPT)** kalıtım stratejisi uygulanmıştır:
* Temel `User` tablosundan `Admin`, `Doctor` ve `Patient` tabloları türetilmiştir.
* `Doctor` ile `Department` arasında (1-N) ilişki kurulmuştur.
* `Appointment` (Randevu) tablosu, `Doctor` ve `Patient` tabloları ile ilişkili olup süreçlerin merkezinde yer alır.
* `Prescription` (Reçete) ile `Medicine` (İlaç) arasında çoka-çok (N-N) ilişki `PrescriptionMedicine` ara tablosu ile yönetilmektedir.

## ⚙️ Kurulum ve Çalıştırma

Projeyi yerel bilgisayarınızda çalıştırmak için aşağıdaki adımları izleyin:

1. **Projeyi Klonlayın:**
   ```bash
   git clone [https://github.com/AliEmreBulut/Hastane_Otomasyon.git](https://github.com/AliEmreBulut/Hastane_Otomasyon.git)
   cd Hastane_Otomasyon