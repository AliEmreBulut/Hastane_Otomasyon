using Hastane_Otomasyon.Models;
using Microsoft.EntityFrameworkCore;

namespace Hastane_Otomasyon.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

   
        public DbSet<User> User { get; set; }
        public DbSet<Admin> Admin { get; set; }
        public DbSet<Department> Department { get; set; }
        public DbSet<Doctor> Doctor { get; set; }
        public DbSet<Appointment> Appointment { get; set; }
        public DbSet<Medicine> Medicine { get; set; }
        public DbSet<Patient> Patient { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Prescription> Prescription { get; set; }
        public DbSet<PrescriptionMedicine> PrescriptionMedicine { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<User>().ToTable("user");
            modelBuilder.Entity<Patient>().ToTable("patient"); 
            modelBuilder.Entity<Doctor>().ToTable("doctor");   
            modelBuilder.Entity<Admin>().ToTable("admin"); 
            modelBuilder.Entity<Department>().ToTable("department");
            modelBuilder.Entity<Appointment>().ToTable("appointment");
            modelBuilder.Entity<Medicine>().ToTable("medicine");
            modelBuilder.Entity<Prescription>().ToTable("prescription");
            modelBuilder.Entity<Role>().ToTable("role");
            modelBuilder.Entity<PrescriptionMedicine>().ToTable("prescription_medicine");

            // USER 
            modelBuilder.Entity<User>(e =>
            {
                e.HasKey(u => u.UserID);
            });

            // PATIENT : USER 
            modelBuilder.Entity<Patient>(e =>
            {
                e.Property(p => p.BirthDate).HasColumnType("date");
            });

            // DOCTOR : USER
            modelBuilder.Entity<Doctor>(e =>
            {
                e.HasOne(d => d.Department)
                 .WithMany(dp => dp.Doctors)
                 .HasForeignKey(d => d.DepartmentID)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ROLE (User n-1 Role)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleID);

            // APPOINTMENT
            modelBuilder.Entity<Appointment>(e =>
            {
                e.HasKey(a => a.AppointmentID);
                e.Property(a => a.Date).HasColumnType("date");
                e.Property(a => a.Time).HasColumnType("time");

                
                e.HasOne(a => a.Patient)
                 .WithMany(p => p.Appointments)
                 .HasForeignKey(a => a.PatientID)
                 .OnDelete(DeleteBehavior.Restrict);

               
                e.HasOne(a => a.Doctor)
                 .WithMany(d => d.Appointments)
                 .HasForeignKey(a => a.DoctorID)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // PRESCRIPTION (1-1)
            modelBuilder.Entity<Prescription>(e =>
            {
                e.HasOne(p => p.Appointment)
                 .WithOne(a => a.Prescription)
                 .HasForeignKey<Prescription>(p => p.AppointmentID);
            });

            // N-N
            modelBuilder.Entity<PrescriptionMedicine>(e =>
            {
                e.HasKey(pm => new { pm.PrescriptionID, pm.MedicineID });
                e.HasOne(pm => pm.Prescription)
                 .WithMany(p => p.prescription_Medicines)
                 .HasForeignKey(pm => pm.PrescriptionID);
                e.HasOne(pm => pm.Medicine)
                 .WithMany(m => m.Prescriptions)
                 .HasForeignKey(pm => pm.MedicineID);
            });

            base.OnModelCreating(modelBuilder);
        }

    }
}
