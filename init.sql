-- --------------------------------------------------------
-- Sunucu:                       127.0.0.1
-- Sunucu sürümü:                11.4.2-MariaDB - mariadb.org binary distribution
-- Sunucu İşletim Sistemi:       Win64
-- HeidiSQL Sürüm:               12.6.0.6765
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


-- hospitaldb için veritabanı yapısı dökülüyor
CREATE DATABASE IF NOT EXISTS `hospitaldb` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci */;
USE `hospitaldb`;

-- tablo yapısı dökülüyor hospitaldb.admin
CREATE TABLE IF NOT EXISTS `admin` (
  `UserID` int(11) NOT NULL,
  PRIMARY KEY (`UserID`),
  CONSTRAINT `FK_admin_user_UserID` FOREIGN KEY (`UserID`) REFERENCES `user` (`UserID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- hospitaldb.admin: ~0 rows (yaklaşık) tablosu için veriler indiriliyor
INSERT INTO `admin` (`UserID`) VALUES
	(1);

-- tablo yapısı dökülüyor hospitaldb.appointment
CREATE TABLE IF NOT EXISTS `appointment` (
  `AppointmentID` int(11) NOT NULL AUTO_INCREMENT,
  `PatientID` int(11) NOT NULL,
  `DoctorID` int(11) DEFAULT NULL,
  `Date` date DEFAULT NULL,
  `Time` time DEFAULT NULL,
  `Status` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`AppointmentID`),
  KEY `FK_appointment_patient_UserID` (`PatientID`),
  KEY `FK_appointment_doctor_UserID` (`DoctorID`),
  CONSTRAINT `FK_appointment_doctor_UserID` FOREIGN KEY (`DoctorID`) REFERENCES `doctor` (`UserID`) ON UPDATE CASCADE,
  CONSTRAINT `FK_appointment_patient_UserID` FOREIGN KEY (`PatientID`) REFERENCES `patient` (`UserID`) ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- hospitaldb.appointment: ~15 rows (yaklaşık) tablosu için veriler indiriliyor
INSERT INTO `appointment` (`AppointmentID`, `PatientID`, `DoctorID`, `Date`, `Time`, `Status`) VALUES
	(1, 12, 2, '2026-01-15', '09:00:00', 'Tamamlandı'),
	(2, 13, 3, '2026-01-20', '10:00:00', 'Tamamlandı'),
	(3, 14, 4, '2026-02-01', '11:00:00', 'Tamamlandı'),
	(4, 12, 5, '2026-03-01', '09:00:00', 'Tamamlandı'),
	(5, 15, 6, '2026-03-05', '14:00:00', 'İptal Edildi'),
	(6, 13, 7, '2026-03-10', '15:00:00', 'Onaylandı'),
	(7, 12, 2, '2026-02-24', '13:30:00', 'Tamamlandı'),
	(8, 12, 7, '2026-02-24', '11:00:00', 'İptal Edildi'),
	(9, 15, 5, '2026-02-25', '13:30:00', 'Tamamlandı'),
	(10, 15, 3, '2026-02-27', '14:00:00', 'Onaylandı'),
	(11, 15, 4, '2026-02-26', '13:30:00', 'Onaylandı'),
	(12, 12, 5, '2026-02-25', '11:00:00', 'Tamamlandı'),
	(13, 18, 17, '2026-02-25', '14:00:00', 'Tamamlandı'),
	(14, 16, 3, '2026-02-25', '11:30:00', 'İptal Edildi'),
	(15, 16, 3, '2026-02-26', '13:30:00', 'Tamamlandı');

-- tablo yapısı dökülüyor hospitaldb.department
CREATE TABLE IF NOT EXISTS `department` (
  `DepartmentID` int(11) NOT NULL AUTO_INCREMENT,
  `DepartmentName` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`DepartmentID`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- hospitaldb.department: ~10 rows (yaklaşık) tablosu için veriler indiriliyor
INSERT INTO `department` (`DepartmentID`, `DepartmentName`) VALUES
	(1, 'Dahiliye'),
	(2, 'Kardiyoloji'),
	(3, 'Nöroloji'),
	(4, 'Ortopedi'),
	(5, 'Pediatri'),
	(6, 'Göz Hastalıkları'),
	(7, 'Kulak Burun Boğaz'),
	(8, 'Dermatoloji'),
	(9, 'Psikiyatri'),
	(10, 'Genel Cerrahi');

-- tablo yapısı dökülüyor hospitaldb.doctor
CREATE TABLE IF NOT EXISTS `doctor` (
  `FirstName` varchar(100) DEFAULT NULL,
  `LastName` varchar(100) DEFAULT NULL,
  `Email` varchar(100) DEFAULT NULL,
  `PhoneNumber` varchar(20) DEFAULT NULL,
  `UserID` int(11) NOT NULL,
  `DepartmentID` int(11) DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT 0,
  PRIMARY KEY (`UserID`),
  UNIQUE KEY `uq_doctor_userid` (`UserID`),
  KEY `DepartmentID` (`DepartmentID`),
  CONSTRAINT `doctor_ibfk_1` FOREIGN KEY (`UserID`) REFERENCES `user` (`UserID`),
  CONSTRAINT `doctor_ibfk_2` FOREIGN KEY (`DepartmentID`) REFERENCES `department` (`DepartmentID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- hospitaldb.doctor: ~10 rows (yaklaşık) tablosu için veriler indiriliyor
INSERT INTO `doctor` (`FirstName`, `LastName`, `Email`, `PhoneNumber`, `UserID`, `DepartmentID`, `IsActive`) VALUES
	('Ayşe', 'Kaya', 'ayse.kaya@buluthastanesi.com', '05301234567', 2, 1, 1),
	('Mehmet', 'Demir', 'mehmet.demir@buluthastanesi.com', '05302234567', 3, 2, 1),
	('Fatma', 'Şahin', 'fatma.sahin@buluthastanesi.com', '05303234567', 4, 3, 1),
	('Ali', 'Yıldız', 'ali.yildiz@buluthastanesi.com', '05304234567', 5, 4, 1),
	('Zeynep', 'Çelik', 'zeynep.celik@buluthastanesi.com', '05305234567', 6, 5, 1),
	('Mustafa', 'Arslan', 'mustafa.arslan@buluthastanesi.com', '05306234567', 7, 6, 1),
	('Hasan', 'Yılmaz', 'hasan.yilmaz@buluthastanesi.com', '05308234567', 9, 8, 1),
	('Selin', 'Aydın', 'selin.aydin@buluthastanesi.com', '05309234567', 10, 9, 1),
	('Samet', 'Kaya', 'mehmet.kaya@gmail.com', '05481534579', 17, 5, 1),
	('Nazife', 'Bulut', 'nazife.bulut@gmail.com', '05247984651', 19, 4, 1);

-- tablo yapısı dökülüyor hospitaldb.medicine
CREATE TABLE IF NOT EXISTS `medicine` (
  `MedicineID` int(11) NOT NULL AUTO_INCREMENT,
  `MedicineName` varchar(100) DEFAULT NULL,
  `Description` text DEFAULT NULL,
  PRIMARY KEY (`MedicineID`)
) ENGINE=InnoDB AUTO_INCREMENT=15 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- hospitaldb.medicine: ~14 rows (yaklaşık) tablosu için veriler indiriliyor
INSERT INTO `medicine` (`MedicineID`, `MedicineName`, `Description`) VALUES
	(1, 'Parasetamol', 'Hafif ve orta şiddetli ağrılar ile ateş düşürülmesi için kullanılır.'),
	(2, 'İbuprofen', 'Ağrı kesici, ateş düşürücü ve anti-enflamatuar özelliklere sahip ilaç.'),
	(3, 'Amoksisilin', 'Çeşitli bakteriyel enfeksiyonların tedavisinde kullanılan bir antibiyotiktir.'),
	(4, 'Aspirin', 'Ağrı kesici, ateş düşürücü ve kan sulandırıcı olarak kullanılır.'),
	(5, 'Metformin', 'Tip 2 diyabet hastalarında kan şekeri kontrolü için kullanılır.'),
	(6, 'Lansoprazol', 'Mide asidini azaltmak için kullanılan proton pompası inhibitörüdür.'),
	(7, 'Sertralin', 'Depresyon ve anksiyete bozukluklarının tedavisinde kullanılan antidepresan.'),
	(8, 'Parasetamol', 'Hafif ve orta şiddetli ağrılar ile ateş düşürülmesi için kullanılır.'),
	(9, 'İbuprofen', 'Ağrı kesici, ateş düşürücü ve anti-enflamatuar özelliklere sahip ilaç.'),
	(10, 'Amoksisilin', 'Çeşitli bakteriyel enfeksiyonların tedavisinde kullanılan bir antibiyotiktir.'),
	(11, 'Aspirin', 'Ağrı kesici, ateş düşürücü ve kan sulandırıcı olarak kullanılır.'),
	(12, 'Metformin', 'Tip 2 diyabet hastalarında kan şekeri kontrolü için kullanılır.'),
	(13, 'Lansoprazol', 'Mide asidini azaltmak için kullanılan proton pompası inhibitörüdür.'),
	(14, 'Sertralin', 'Depresyon ve anksiyete bozukluklarının tedavisinde kullanılan antidepresan.');

-- tablo yapısı dökülüyor hospitaldb.patient
CREATE TABLE IF NOT EXISTS `patient` (
  `FirstName` varchar(100) DEFAULT NULL,
  `LastName` varchar(100) DEFAULT NULL,
  `NationalID` varchar(11) DEFAULT NULL,
  `BirthDate` date DEFAULT NULL,
  `Gender` varchar(10) DEFAULT NULL,
  `PhoneNumber` varchar(20) DEFAULT NULL,
  `Email` varchar(100) DEFAULT NULL,
  `Address` text DEFAULT NULL,
  `UserID` int(11) NOT NULL,
  `BloodType` varchar(10) NOT NULL DEFAULT '',
  PRIMARY KEY (`UserID`),
  UNIQUE KEY `NationalID` (`NationalID`),
  UNIQUE KEY `uq_patient_userid` (`UserID`),
  CONSTRAINT `patient_ibfk_1` FOREIGN KEY (`UserID`) REFERENCES `user` (`UserID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- hospitaldb.patient: ~5 rows (yaklaşık) tablosu için veriler indiriliyor
INSERT INTO `patient` (`FirstName`, `LastName`, `NationalID`, `BirthDate`, `Gender`, `PhoneNumber`, `Email`, `Address`, `UserID`, `BloodType`) VALUES
	('Ali', 'Kara', '12345678901', '1990-05-15', 'Erkek', '05321234567', 'ali.kara@gmail.com', 'Ankara, Çankaya', 12, 'A-'),
	('Ayşe', 'Demir', '23456789012', '1985-08-22', 'Kadın', '05322234567', 'ayse.demir@gmail.com', 'İstanbul, Kadıköy', 13, 'B+'),
	('Mehmet', 'Yılmaz', '34567890123', '1978-03-10', 'Erkek', '05323234567', 'mehmet.yilmaz@gmail.com', 'İzmir, Bornova', 14, '0+'),
	('Fatma', 'Şahin', '45678901234', '1995-11-30', 'Kadın', '05324234567', 'fatma.sahin@gmail.com', 'Bursa, Nilüfer', 15, 'AB+'),
	('Yaren', 'Balaban', '15678945631', '2002-06-19', 'Kadın', '05385247896', 'yaren.balaban@gmail.com', 'Afyon, Merkez', 16, 'B-'),
	('Emirhan', 'Altunok', '15678524568', '2005-06-08', 'Erkek', '05786957445', 'emirhan.altunok@gmail.com', 'Amasya, Merkez', 18, 'AB+');

-- tablo yapısı dökülüyor hospitaldb.prescription
CREATE TABLE IF NOT EXISTS `prescription` (
  `PrescriptionID` int(11) NOT NULL AUTO_INCREMENT,
  `AppointmentID` int(11) DEFAULT NULL,
  `Description` varchar(500) DEFAULT NULL,
  `Diagnosis` text DEFAULT NULL,
  `Date` date DEFAULT NULL,
  PRIMARY KEY (`PrescriptionID`),
  KEY `AppointmentID` (`AppointmentID`),
  CONSTRAINT `prescription_ibfk_1` FOREIGN KEY (`AppointmentID`) REFERENCES `appointment` (`AppointmentID`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- hospitaldb.prescription: ~6 rows (yaklaşık) tablosu için veriler indiriliyor
INSERT INTO `prescription` (`PrescriptionID`, `AppointmentID`, `Description`, `Diagnosis`, `Date`) VALUES
	(1, 7, 'soğuk terleme', 'Grip', '2026-02-22'),
	(2, 9, 'Şiddetli baş ağrısı', 'Baş ağrısı', '2026-02-23'),
	(3, 4, 'Şiddetli kalp ağrısı', 'Kalp Krizi', '2026-02-23'),
	(4, 12, 'Kolu ağrıyomuş', 'Kol ağrısı', '2026-02-23'),
	(5, 13, 'Üşütmüş', 'Grip', '2026-02-23'),
	(6, 15, 'Kalbinin ritmi bozuk', 'Ritim Bozukluğu', '2026-02-23');

-- tablo yapısı dökülüyor hospitaldb.prescription_medicine
CREATE TABLE IF NOT EXISTS `prescription_medicine` (
  `PrescriptionID` int(11) NOT NULL,
  `MedicineID` int(11) NOT NULL,
  `Description` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`PrescriptionID`,`MedicineID`),
  KEY `MedicineID` (`MedicineID`),
  CONSTRAINT `prescription_medicine_ibfk_1` FOREIGN KEY (`PrescriptionID`) REFERENCES `prescription` (`PrescriptionID`),
  CONSTRAINT `prescription_medicine_ibfk_2` FOREIGN KEY (`MedicineID`) REFERENCES `medicine` (`MedicineID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- hospitaldb.prescription_medicine: ~4 rows (yaklaşık) tablosu için veriler indiriliyor
INSERT INTO `prescription_medicine` (`PrescriptionID`, `MedicineID`, `Description`) VALUES
	(1, 3, '2x1 Tok'),
	(4, 6, '2x2'),
	(5, 2, '2x1 Tok'),
	(6, 12, '2x1 Aç');

-- tablo yapısı dökülüyor hospitaldb.role
CREATE TABLE IF NOT EXISTS `role` (
  `RoleID` int(11) NOT NULL AUTO_INCREMENT,
  `RoleName` varchar(50) NOT NULL,
  PRIMARY KEY (`RoleID`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- hospitaldb.role: ~3 rows (yaklaşık) tablosu için veriler indiriliyor
INSERT INTO `role` (`RoleID`, `RoleName`) VALUES
	(1, 'Admin'),
	(2, 'Doctor'),
	(3, 'Patient');

-- tablo yapısı dökülüyor hospitaldb.user
CREATE TABLE IF NOT EXISTS `user` (
  `UserID` int(11) NOT NULL AUTO_INCREMENT,
  `UserName` varchar(100) NOT NULL,
  `Password` varchar(60) NOT NULL,
  `RoleID` int(11) DEFAULT NULL,
  PRIMARY KEY (`UserID`),
  KEY `RoleID` (`RoleID`),
  CONSTRAINT `user_ibfk_1` FOREIGN KEY (`RoleID`) REFERENCES `role` (`RoleID`)
) ENGINE=InnoDB AUTO_INCREMENT=20 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- hospitaldb.user: ~15 rows (yaklaşık) tablosu için veriler indiriliyor
INSERT INTO `user` (`UserID`, `UserName`, `Password`, `RoleID`) VALUES
	(1, 'admin', '$2a$11$gVuVR6wyCnxzdhGWJF2UheQ8ZlG3d/CNWspQWhtiBd1U2PGCF3anq', 1),
	(2, 'dr.ayse', '$2a$11$gVuVR6wyCnxzdhGWJF2UheQ8ZlG3d/CNWspQWhtiBd1U2PGCF3anq', 2),
	(3, 'dr.mehmet', '$2a$11$gVuVR6wyCnxzdhGWJF2UheQ8ZlG3d/CNWspQWhtiBd1U2PGCF3anq', 2),
	(4, 'dr.fatma', '$2a$11$gVuVR6wyCnxzdhGWJF2UheQ8ZlG3d/CNWspQWhtiBd1U2PGCF3anq', 2),
	(5, 'dr.ali', '$2a$11$gVuVR6wyCnxzdhGWJF2UheQ8ZlG3d/CNWspQWhtiBd1U2PGCF3anq', 2),
	(6, 'dr.zeynep', '$2a$11$gVuVR6wyCnxzdhGWJF2UheQ8ZlG3d/CNWspQWhtiBd1U2PGCF3anq', 2),
	(7, 'dr.mustafa', '$2a$11$gVuVR6wyCnxzdhGWJF2UheQ8ZlG3d/CNWspQWhtiBd1U2PGCF3anq', 2),
	(9, 'dr.hasan', '$2a$11$gVuVR6wyCnxzdhGWJF2UheQ8ZlG3d/CNWspQWhtiBd1U2PGCF3anq', 2),
	(10, 'dr.selin', '$2a$11$gVuVR6wyCnxzdhGWJF2UheQ8ZlG3d/CNWspQWhtiBd1U2PGCF3anq', 2),
	(12, 'hasta.ali', '$2a$11$gVuVR6wyCnxzdhGWJF2UheQ8ZlG3d/CNWspQWhtiBd1U2PGCF3anq', 3),
	(13, 'hasta.ayse', '$2a$11$gVuVR6wyCnxzdhGWJF2UheQ8ZlG3d/CNWspQWhtiBd1U2PGCF3anq', 3),
	(14, 'hasta.mehmet', '$2a$11$gVuVR6wyCnxzdhGWJF2UheQ8ZlG3d/CNWspQWhtiBd1U2PGCF3anq', 3),
	(15, 'hasta.fatma', '$2a$11$gVuVR6wyCnxzdhGWJF2UheQ8ZlG3d/CNWspQWhtiBd1U2PGCF3anq', 3),
	(16, 'hasta.yaren', '$2a$11$Dz77P7KB7Gpy1Lauhu.JOu5XQ3406FkgzmlFMFaCl9IXtiS9DkPIS', 3),
	(17, 'dr.samet', '$2a$11$1B/GZMfVFMzBeuvK1TA2f.0RT8EFz3ERf/1KeI1kVj2M5jIEDnp6.', 2),
	(18, 'hasta.emirhan', '$2a$11$wyUe90Eo6sWWEL9Yqt.Zgumq5yyEcr2/yfwSR.34IaWyCx1vksXQW', 3),
	(19, 'dr.nazife', '$2a$11$.MGH4VnvkadCSj23vXSDqOs4Q0hEgcdHkWbbrtkjzy12fYFS/63Ou', 2);

-- tablo yapısı dökülüyor hospitaldb.__efmigrationshistory
CREATE TABLE IF NOT EXISTS `__efmigrationshistory` (
  `MigrationId` varchar(150) NOT NULL,
  `ProductVersion` varchar(32) NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- hospitaldb.__efmigrationshistory: ~4 rows (yaklaşık) tablosu için veriler indiriliyor
INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`) VALUES
	('20251008112251_Baseline', '9.0.9'),
	('20251008112515_Add_Whatever', '9.0.9'),
	('20260221142135_AddAdminTable', '9.0.9'),
	('20260221162012_AddDescriptionToPrescriptions', '9.0.9'),
	('20260221212758_FixPasswordLength', '9.0.9'),
	('20260222212138_AddIsActiveToDoctors', '9.0.9');

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
