using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Npgsql;

namespace MedicalSystem.Models
{
    [Table("medical_documentation")]
    public class MedicalDocumentation
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required(ErrorMessage = "Patient ID is required")]
        [Column("patient_id")]
        public long PatientId { get; set; }

        [Required(ErrorMessage = "Disease name is required")]
        [StringLength(200, ErrorMessage = "Disease name cannot exceed 200 characters")]
        [Column("disease_name")]
        public string DiseaseName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Start date is required")]
        [Column("start_date")]
        public DateTime StartDate { get; set; }

        [Column("end_date")]
        public DateTime? EndDate { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }

        public static MedicalDocumentation FromSqlReader(NpgsqlDataReader reader)
        {
            return new MedicalDocumentation
            {
                Id = long.Parse(reader["id"].ToString() ?? "0"),
                PatientId = long.Parse(reader["patient_id"].ToString() ?? "0"),
                DiseaseName = reader["disease_name"]?.ToString() ?? string.Empty,
                StartDate = DateTime.Parse(reader["start_date"]?.ToString() ?? DateTime.MinValue.ToString()),
                EndDate = reader["end_date"] == DBNull.Value ? null : DateTime.Parse(reader["end_date"].ToString()),
                CreatedAt = DateTime.Parse(reader["created_at"]?.ToString() ?? DateTime.MinValue.ToString())
            };
        }

        public override string ToString()
            => $"{Id}|{DiseaseName} | {StartDate:yyyy-MM-dd} - {(EndDate?.ToString("yyyy-MM-dd") ?? "Ongoing")}";
    }
}