﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BicardBackend.Models;

namespace BicardBackend.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<MedService> Meds { get; set; }
        public DbSet<SubMedService> Subs { get; set; }
        public DbSet<SubMedServiceDoctor> SubsDoctors { get;set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Faq> Faqs { get; set; }
        public DbSet<Vacancy> Vacancies { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Info> Info { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<AboutClinic> AboutClinic { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SubMedServiceDoctor>()
                .HasKey(s => new { s.DoctorId, s.SubMedServiceId });

            // Other configurations...

            modelBuilder.Entity<SubMedServiceDoctor>()
                .HasOne(sd => sd.Doctor)
                .WithMany(d => d.SubMedServiceDoctors)
                .HasForeignKey(sd => sd.DoctorId);

            modelBuilder.Entity<SubMedServiceDoctor>()
                .HasOne(sd => sd.SubMedService)
                .WithMany(sm => sm.SubMedServiceDoctors)
                .HasForeignKey(sd => sd.SubMedServiceId);

            modelBuilder.Entity<SubMedService>()
            .HasOne(b => b.MedService)
            .WithMany(a => a.SubMedServices)
            .HasForeignKey(b => b.MedServiceId);
            modelBuilder.Entity<Feedback>()
            .HasOne(b => b.User)
            .WithMany(a => a.Feedbacks)
            .HasForeignKey(b => b.UserId);
        }

    }
}
