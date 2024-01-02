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
    public DbSet<Specialty> Specialties { get; set; } = null!;
    public DbSet<Mkb10> Mkb10 { get; set; } = null!;
    
}