namespace MisAPI.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


[Table("refresh_tokens")]
public class RefreshToken
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonIgnore]
    [Column("id")]
    public Guid Id { get; set; }
    
    [Column("token")]
    public required string Token { get; set; }
    
    [Column("expires")]
    public DateTime Expires { get; set; }
    
    [Column("is_expired")]
    public bool IsExpired => DateTime.UtcNow >= Expires;
    
    [Column("revoked")]
    public bool Revoked { get; set; }
    
    [Column("doctorId")]
    public required Guid DoctorId { get; set; }
}