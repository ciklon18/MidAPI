using Microsoft.EntityFrameworkCore;
using MisAPI.Entities;

namespace MisAPI.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    public DbSet<Doctor> Doctors { get; set; } = null!;
    public DbSet<Speciality> Specialties { get; set; } = null!;
    public DbSet<Mkb10> Mkb10 { get; set; } = null!;
    public DbSet<Mkb10Root> Mkb10Roots { get; set; } = null!;

    public DbSet<Patient> Patients { get; set; } = null!;
    public DbSet<Inspection> Inspections { get; set; } = null!;
    public DbSet<Diagnosis> Diagnoses { get; set; } = null!;
    public DbSet<Comment> Comments { get; set; } = null!;


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    
        modelBuilder.Entity<Patient>()
            .HasMany(p => p.Inspections)
            .WithOne(i => i.Patient)
            .HasForeignKey(i => i.PatientId)
            .OnDelete(DeleteBehavior.Cascade);
    
        modelBuilder.Entity<Inspection>()
            .HasMany(i => i.Diagnoses)
            .WithOne(d => d.Inspection)
            .HasForeignKey(d => d.InspectionId)
            .OnDelete(DeleteBehavior.Cascade);
        
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