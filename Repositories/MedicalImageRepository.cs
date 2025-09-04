using System;
using MedicalSystem.Models;
using MedicalSystem.Repositories.Interfaces;
using Npgsql;

namespace MedicalSystem.Repositories
{
    public class MedicalImageRepository : IMedicalImageRepository
    {
        private readonly string _connectionString;

        public MedicalImageRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<IEnumerable<MedicalImage>> GetAllAsync()
        {
            var images = new List<MedicalImage>();

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            var sql = "SELECT id, examination_id, file_name, file_path, file_type, file_size, upload_date FROM medical_images ORDER BY upload_date DESC";
            using var command = new NpgsqlCommand(sql, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                images.Add(MedicalImage.FromSqlReader(reader));
            }

            return images;
        }

        public async Task<MedicalImage?> GetByIdAsync(long id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            var sql = "SELECT id, examination_id, file_name, file_path, file_type, file_size, upload_date FROM medical_images WHERE id = @id";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return MedicalImage.FromSqlReader(reader);
            }

            return null;
        }

        public async Task<IEnumerable<MedicalImage>> GetByExaminationIdAsync(long examinationId)
        {
            var images = new List<MedicalImage>();

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            var sql = "SELECT id, examination_id, file_name, file_path, file_type, file_size, upload_date FROM medical_images WHERE examination_id = @examinationId ORDER BY upload_date DESC";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@examinationId", examinationId);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                images.Add(MedicalImage.FromSqlReader(reader));
            }

            return images;
        }

        public async Task<MedicalImage> CreateAsync(MedicalImage medicalImage)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            var sql = @"
                INSERT INTO medical_images (examination_id, file_name, file_path, file_type, file_size)
                VALUES (@examinationId, @fileName, @filePath, @fileType, @fileSize)
                RETURNING id, upload_date";

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@examinationId", medicalImage.ExaminationId);
            command.Parameters.AddWithValue("@fileName", medicalImage.FileName);
            command.Parameters.AddWithValue("@filePath", medicalImage.FilePath);
            command.Parameters.AddWithValue("@fileType", medicalImage.FileType);
            command.Parameters.AddWithValue("@fileSize", medicalImage.FileSize);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                medicalImage.Id = (long)reader["id"];
                medicalImage.UploadDate = (DateTime)reader["upload_date"];
            }

            return medicalImage;
        }

        public async Task<MedicalImage> UpdateAsync(MedicalImage medicalImage)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            var sql = @"
                UPDATE medical_images 
                SET examination_id = @examinationId, 
                    file_name = @fileName, 
                    file_path = @filePath, 
                    file_type = @fileType, 
                    file_size = @fileSize
                WHERE id = @id";

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", medicalImage.Id);
            command.Parameters.AddWithValue("@examinationId", medicalImage.ExaminationId);
            command.Parameters.AddWithValue("@fileName", medicalImage.FileName);
            command.Parameters.AddWithValue("@filePath", medicalImage.FilePath);
            command.Parameters.AddWithValue("@fileType", medicalImage.FileType);
            command.Parameters.AddWithValue("@fileSize", medicalImage.FileSize);

            command.ExecuteNonQuery();
            return medicalImage;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            var sql = "DELETE FROM medical_images WHERE id = @id";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            var rowsAffected = command.ExecuteNonQuery();
            return rowsAffected > 0;
        }
    }
}

