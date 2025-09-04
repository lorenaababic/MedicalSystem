using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Npgsql;

namespace MedicalSystem.Models
{
    [Table("medical_images")]
    public class MedicalImage
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required(ErrorMessage = "Patient ID is required")]
        [Column("patient_id")]
        public long PatientId { get; set; }

        [Column("examination_id")]
        public long? ExaminationId { get; set; } // Make optional as images can exist without examination

        [Required(ErrorMessage = "File name is required")]
        [StringLength(255, ErrorMessage = "File name cannot exceed 255 characters")]
        [Column("file_name")]
        public string FileName { get; set; } = string.Empty;

        [Required(ErrorMessage = "File path is required")]
        [StringLength(500, ErrorMessage = "File path cannot exceed 500 characters")]
        [Column("file_path")]
        public string FilePath { get; set; } = string.Empty;

        [Required(ErrorMessage = "File type is required")]
        [StringLength(10, ErrorMessage = "File type cannot exceed 10 characters")]
        [RegularExpression("^(JPG|JPEG|PNG|GIF|BMP|TIFF|WEBP|PDF|DCM)$",
            ErrorMessage = "Invalid file type")]
        [Column("file_type")]
        public string FileType { get; set; } = string.Empty;

        [Required(ErrorMessage = "File size is required")]
        [Range(1, 10485760, ErrorMessage = "File size must be between 1 byte and 10MB")]
        [Column("file_size")]
        public long FileSize { get; set; }

        [Column("upload_date")]
        public DateTime UploadDate { get; set; }

        // Navigation properties
        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }

        [ForeignKey("ExaminationId")]
        public virtual Examination? Examination { get; set; }

        public static MedicalImage FromSqlReader(NpgsqlDataReader reader)
        {
            long id = long.Parse(reader["id"].ToString() ?? "0");
            long patientId = long.Parse(reader["patient_id"].ToString() ?? "0");
            long? examinationId = reader["examination_id"] == DBNull.Value ? null : long.Parse(reader["examination_id"].ToString() ?? "0");
            string fileName = reader["file_name"]?.ToString() ?? string.Empty;
            string filePath = reader["file_path"]?.ToString() ?? string.Empty;
            string fileType = reader["file_type"]?.ToString() ?? string.Empty;
            long fileSize = long.Parse(reader["file_size"]?.ToString() ?? "0");
            DateTime uploadDate = DateTime.Parse(reader["upload_date"]?.ToString() ?? DateTime.MinValue.ToString());

            return new MedicalImage
            {
                Id = id,
                PatientId = patientId,
                ExaminationId = examinationId,
                FileName = fileName,
                FilePath = filePath,
                FileType = fileType,
                FileSize = fileSize,
                UploadDate = uploadDate
            };
        }

        public override string ToString()
            => $"{Id}|{FileName} ({FileType}) | {FileSize} bytes";
    }
}