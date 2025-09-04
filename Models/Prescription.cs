using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Npgsql;

namespace MedicalSystem.Models
{
    [Table("prescriptions")]
    public class Prescription
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required(ErrorMessage = "Patient ID is required")]
        [Column("patient_id")]
        public long PatientId { get; set; }

        [Column("examination_id")]
        public long? ExaminationId { get; set; }

        [Required(ErrorMessage = "Medication name is required")]
        [StringLength(200, ErrorMessage = "Medication name cannot exceed 200 characters")]
        [Column("medication_name")]
        public string MedicationName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Dosage is required")]
        [StringLength(100, ErrorMessage = "Dosage cannot exceed 100 characters")]
        [Column("dosage")]
        public string Dosage { get; set; } = string.Empty;

        [Required(ErrorMessage = "Issue date is required")]
        [Column("issue_date")]
        public DateTime IssueDate { get; set; }

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        [Column("notes")]
        public string Notes { get; set; } = string.Empty;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }

        [ForeignKey("ExaminationId")]
        public virtual Examination? Examination { get; set; }

        public static Prescription FromSqlReader(NpgsqlDataReader reader)
        {
            long id = long.Parse(reader["id"].ToString() ?? "0");
            long patientId = long.Parse(reader["patient_id"].ToString() ?? "0");
            long? examinationId = reader["examination_id"] == DBNull.Value ? null : long.Parse(reader["examination_id"].ToString() ?? "0");
            string medicationName = reader["medication_name"]?.ToString() ?? string.Empty;
            string dosage = reader["dosage"]?.ToString() ?? string.Empty;
            DateTime issueDate = DateTime.Parse(reader["issue_date"]?.ToString() ?? DateTime.MinValue.ToString());
            string notes = reader["notes"]?.ToString() ?? string.Empty;
            DateTime createdAt = DateTime.Parse(reader["created_at"]?.ToString() ?? DateTime.MinValue.ToString());

            return new Prescription
            {
                Id = id,
                PatientId = patientId,
                ExaminationId = examinationId,
                MedicationName = medicationName,
                Dosage = dosage,
                IssueDate = issueDate,
                Notes = notes,
                CreatedAt = createdAt
            };
        }

        public override string ToString()
            => $"{Id}|{MedicationName} - {Dosage} | {IssueDate:yyyy-MM-dd}";
    }

}

