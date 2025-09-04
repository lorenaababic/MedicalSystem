using System;
using MedicalSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalSystem.Data
{
    public class MedicalSystemDbContext : DbContext
    {
        public MedicalSystemDbContext(DbContextOptions<MedicalSystemDbContext> options) : base(options)
        {
        }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<MedicalDocumentation> MedicalDocumentations { get; set; }
        public DbSet<Examination> Examinations { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<MedicalImage> MedicalImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Patient configuration
            modelBuilder.Entity<Patient>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Oib).IsUnique();
                entity.HasIndex(e => e.LastName);
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // MedicalDocumentation configuration
            modelBuilder.Entity<MedicalDocumentation>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.PatientId);
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.Patient)
                    .WithMany(p => p.MedicalHistory)
                    .HasForeignKey(e => e.PatientId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Examination configuration
            modelBuilder.Entity<Examination>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.PatientId);
                entity.HasIndex(e => e.ExaminationDate);
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.Patient)
                    .WithMany(p => p.Examinations)
                    .HasForeignKey(e => e.PatientId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Prescription configuration
            modelBuilder.Entity<Prescription>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.PatientId);
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.Patient)
                    .WithMany(p => p.Prescriptions)
                    .HasForeignKey(e => e.PatientId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Examination)
                    .WithMany(ex => ex.Prescriptions)
                    .HasForeignKey(e => e.ExaminationId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // MedicalImage configuration
            modelBuilder.Entity<MedicalImage>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.PatientId);
                entity.HasIndex(e => e.ExaminationId);
                entity.Property(e => e.UploadDate)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Relationship with Patient
                entity.HasOne(e => e.Patient)
                    .WithMany(p => p.MedicalImages)
                    .HasForeignKey(e => e.PatientId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relationship with Examination (optional)
                entity.HasOne(e => e.Examination)
                    .WithMany(ex => ex.MedicalImages)
                    .HasForeignKey(e => e.ExaminationId)
                    .OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}