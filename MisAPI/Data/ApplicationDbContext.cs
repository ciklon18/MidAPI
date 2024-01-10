﻿using Microsoft.EntityFrameworkCore;
using MisAPI.Entities;

namespace MisAPI.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    public DbSet<Doctor> Doctors { get; set; } = null!;
    public DbSet<Speciality> Specialities { get; set; } = null!;
    public DbSet<Icd10> Icd10 { get; set; } = null!;
    public DbSet<Icd10Root> Icd10Roots { get; set; } = null!;

    public DbSet<Patient> Patients { get; set; } = null!;
    public DbSet<Inspection> Inspections { get; set; } = null!;
    public DbSet<Diagnosis> Diagnoses { get; set; } = null!;
    public DbSet<Comment> Comments { get; set; } = null!;
    public DbSet<Consultation> Consultations { get; set; } = null!;
    public DbSet<Notification> Notifications { get; set; } = null!;


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        
        modelBuilder.Entity<Consultation>()
            .HasOne(i => i.RootComment)
            .WithOne(c => c.Consultation)
            .HasForeignKey<Consultation>(i => i.RootCommentId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Speciality>()
            .HasMany(s => s.Doctors)
            .WithOne(d => d.Speciality)
            .HasForeignKey(d => d.SpecialityId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}