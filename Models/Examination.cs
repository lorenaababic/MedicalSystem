using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Npgsql;

namespace MedicalSystem.Models
{
    [Table("examinations")]
    public class Examination
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required(ErrorMessage = "Patient ID is required")]
        [Column("patient_id")]
        public long PatientId { get; set; }

        [Required(ErrorMessage = "Examination date is required")]
        [Column("examination_date")]
        public DateTime ExaminationDate { get; set; }

        [Required(ErrorMessage = "Examination time is required")]
        [Column("examination_time")]
        public TimeSpan ExaminationTime { get; set; }

        [Required(ErrorMessage = "Examination type is required")]
        [RegularExpression("^(GP|KRV|X-RAY|CT|MR|ULTRA|EKG|ECHO|EYE|DERM|DENTA|MAMMO|NEURO)$",
            ErrorMessage = "Invalid examination type")]
        [Column("examination_type")]
        public string ExaminationType { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        [Column("description")]
        public string Description { get; set; } = string.Empty;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }
        public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
        public virtual ICollection<MedicalImage> MedicalImages { get; set; } = new List<MedicalImage>();

        public static Examination FromSqlReader(NpgsqlDataReader reader)
        {
            long id = long.Parse(reader["id"].ToString() ?? "0");
            long patientId = long.Parse(reader["patient_id"].ToString() ?? "0");
            DateTime examinationDate = DateTime.Parse(reader["examination_date"]?.ToString() ?? DateTime.MinValue.ToString());
            TimeSpan examinationTime = TimeSpan.Parse(reader["examination_time"]?.ToString() ?? "00:00:00");
            string examinationType = reader["examination_type"]?.ToString() ?? string.Empty;
            string description = reader["description"]?.ToString() ?? string.Empty;
            DateTime createdAt = DateTime.Parse(reader["created_at"]?.ToString() ?? DateTime.MinValue.ToString());

            return new Examination
            {
                Id = id,
                PatientId = patientId,
                ExaminationDate = examinationDate,
                ExaminationTime = examinationTime,
                ExaminationType = examinationType,
                Description = description,
                CreatedAt = createdAt
            };
        }

        public override string ToString()
            => $"{Id}|{ExaminationType} | {ExaminationDate:yyyy-MM-dd} {ExaminationTime}";
    }
}

