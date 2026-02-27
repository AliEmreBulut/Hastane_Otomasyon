# 🏥 Hastane Otomasyon Sistemi (Hospital Management System)

Bu proje, hastane süreçlerini dijitalleştirmek, hasta-doktor etkileşimini kolaylaştırmak ve sistem yöneticilerine tam kontrol sağlamak amacıyla geliştirilmiş kapsamlı bir **Full-Stack** web uygulamasıdır. Backend tarafında **ASP.NET Core Web API**, veritabanı yönetiminde **Entity Framework Core (MariaDB)** ve frontend tarafında modern, dinamik bir **Vanilla JS + Bootstrap** mimarisi kullanılmıştır. Proje, **Docker** kullanılarak tamamen konteynerize edilmiş olup tek bir komutla her ortamda çalışmaya hazırdır.

## 🚀 Özellikler ve Kullanıcı Rolleri

Sistem, üç farklı kullanıcı rolü üzerinden güvenli ve özelleştirilmiş deneyim sunar:

### 🛡️ Admin (Sistem Yöneticisi) Paneli
* **Genel Özet (Dashboard):** Sistemdeki aktif hasta, doktor ve randevu sayılarını anlık takip etme.
* **Doktor ve Hasta Yönetimi:** Sisteme yeni doktor ekleme, mevcut doktor ve hasta kayıtlarını görüntüleme ve güvenli pasife alma (Soft Delete) işlemleri.
* **Randevu Kontrolü:** Tüm hastanedeki randevuları filtreleme ve gerektiğinde iptal etme yetkisi.
* **Klinik Yönetimi:** Departmanlara göre doktor dağılımlarını görüntüleme.

### 👨‍⚕️ Doktor Paneli
* **Randevu Takibi:** Günlük ve geçmiş randevuları listeleme, hasta detaylarını ve teşhisleri inceleme.
* **Reçete ve Teşhis Yönetimi:** Tamamlanan muayeneler için veritabanına kayıtlı ilaçları aratarak reçete oluşturma (N-N ilişkili dinamik ilaç listesi) ve doktor notu ekleme.
* **Profil Yönetimi:** İletişim bilgilerini (E-posta, Telefon) güvenli bir şekilde güncelleme.

### 🧍 Hasta Paneli
* **Kayıt ve Kimlik Doğrulama:** Kimlik no ve BCrypt ile şifrelenmiş parolalar ile güvenli giriş/kayıt işlemleri.
* **Dinamik Randevu Sistemi:** İlgili departman ve doktoru seçtikten sonra, doktorun **yalnızca boş saatlerini (Available Slots)** görerek çakışmasız randevu alabilme.
* **Tıbbi Geçmiş:** Geçmiş randevuları, konulan teşhisleri ve yazılan reçeteleri detaylıca görüntüleyebilme.
* **Profil Yönetimi:** Kan grubu, adres ve iletişim gibi kişisel bilgileri yönetme.

## 🛠️ Kullanılan Teknolojiler
* **Backend:** C#, ASP.NET Core 8+ Web API
* **ORM:** Entity Framework Core (Code-First)
* **Veritabanı:** MariaDB (Pomelo.EntityFrameworkCore.MySql)
* **DevOps:** Docker & Docker Compose
* **Güvenlik:** BCrypt.Net (Parola Hashleme)
* **Frontend:** HTML5, CSS3, Vanilla JavaScript (Fetch API), Bootstrap 5
* **UI Eklentileri:** SweetAlert2 (Gelişmiş bildirimler), Bootstrap Icons

## 🗄️ Veritabanı Mimarisi
Projede **Domain-Driven Design (DDD)** prensiplerinden esinlenilmiş ve Entity Framework üzerinde **Table-Per-Type (TPT)** kalıtım stratejisi uygulanmıştır:
* Temel `User` tablosundan `Admin`, `Doctor` ve `Patient` tabloları türetilmiştir.
* `Doctor` ile `Department` arasında (1-N) ilişki kurulmuştur.
* `Appointment` (Randevu) tablosu, `Doctor` ve `Patient` tabloları ile ilişkili olup süreçlerin merkezinde yer alır.
* `Prescription` (Reçete) ile `Medicine` (İlaç) arasında çoka-çok (N-N) ilişki `PrescriptionMedicine` ara tablosu ile yönetilmektedir.

## 🐳 Kurulum ve Çalıştırma (Docker İle Tek Komut)

Proje tamamen Dockerize edilmiştir. Bilgisayarınızda herhangi bir .NET SDK veya veritabanı sunucusu kurulu olmasına gerek yoktur. Sadece **Docker Desktop**'ın çalışır durumda olması yeterlidir.

**1. Projeyi Klonlayın:**
```bash
git clone [https://github.com/AliEmreBulut/Hastane_Otomasyon.git](https://github.com/AliEmreBulut/Hastane_Otomasyon.git)
cd Hastane_Otomasyon/Hastane_Otomasyon
```
*(Not: İçeriğinde `docker-compose.yml` dosyasının bulunduğu ana dizinde olduğunuzdan emin olun).*

**2. Sistemi Ayağa Kaldırın:**
Terminalde aşağıdaki komutu çalıştırın:
```bash
docker compose up --build -d
```
*Bu komut; backend uygulamasını derler, MariaDB veritabanını ayağa kaldırır ve `init.sql` dosyasını okuyarak veritabanı tablolarını ve örnek verileri otomatik olarak oluşturur.*

**3. Uygulamaya Erişin:**
İşlem tamamlandığında (servisler `Running` durumuna geçtiğinde) tarayıcınızı açın ve şu adrese gidin:
👉 **http://localhost:8080**

---

### 🔑 Test Hesapları
Sistemi hızlıca test edebilmeniz için veritabanı aşağıdaki varsayılan hesaplarla (ve örnek randevu/reçete verileriyle) kurulur:

* **Admin Girişi:** Kullanıcı Adı: `admin` | Şifre: `123456`
* **Doktor Girişi:** Kullanıcı Adı: `dr.ayse` | Şifre: `123456`
* **Hasta Girişi:** Kullanıcı Adı: `hasta.ali` | Şifre: `123456`
