using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Npgsql;

namespace MedicalSystem.Models
{
    [Table("patients")]
    public class Patient
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
        [Column("first_name")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
        [Column("last_name")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "OIB is required")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "OIB must be exactly 11 digits")]
        [Column("oib")]
        public string Oib { get; set; } = string.Empty;

        [Required(ErrorMessage = "Gender is required")]
        [RegularExpression("^[MF]$", ErrorMessage = "Gender must be M or F")]
        [Column("gender")]
        public string Gender { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of birth is required")]
        [Column("date_of_birth")]
        public DateTime DateOfBirth { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        // NEW FIELD - Ishod 5 (migration simulation)
        [StringLength(20, ErrorMessage = "Patient number cannot exceed 20 characters")]
        [Column("patient_number")]
        public string? PatientNumber { get; set; }

        // Navigation properties for relationships
        public virtual ICollection<MedicalDocumentation> MedicalHistory { get; set; } = new List<MedicalDocumentation>();
        public virtual ICollection<Examination> Examinations { get; set; } = new List<Examination>();
        public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
        public virtual ICollection<MedicalImage> MedicalImages { get; set; } = new List<MedicalImage>();


        public static Patient FromSqlReader(NpgsqlDataReader reader)
        {
            // Jednostavniji i sigurniji pristup
            return new Patient
            {
                Id = Convert.ToInt64(reader["id"]),
                FirstName = reader["first_name"]?.ToString() ?? string.Empty,
                LastName = reader["last_name"]?.ToString() ?? string.Empty,
                Oib = reader["oib"]?.ToString() ?? string.Empty,
                Gender = reader["gender"]?.ToString() ?? string.Empty,
                DateOfBirth = Convert.ToDateTime(reader["date_of_birth"]),
                CreatedAt = Convert.ToDateTime(reader["created_at"]),
                // Sigurno čitanje patient_number - može biti null u bazi
                PatientNumber = reader["patient_number"] == DBNull.Value ? null : reader["patient_number"]?.ToString()
            };
        }

        public override string ToString()
            => $"{Id}|{FirstName} {LastName} ({Oib}) | {Gender} | {DateOfBirth:yyyy-MM-dd}";
    }
}