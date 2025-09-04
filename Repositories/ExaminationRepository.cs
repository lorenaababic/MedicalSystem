using System;
using MedicalSystem.Models;
using MedicalSystem.Repositories.Interfaces;
using Npgsql;

namespace MedicalSystem.Repositories
{
    public class ExaminationRepository : IExaminationRepository
    {
        private readonly string _connectionString;

        public ExaminationRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<IEnumerable<Examination>> GetAllAsync()
        {
            var examinations = new List<Examination>();

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = "SELECT id, patient_id, examination_date, examination_time, examination_type, description, created_at FROM examinations ORDER BY examination_date DESC, examination_time DESC";
            using var command = new NpgsqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                examinations.Add(Examination.FromSqlReader(reader));
            }

            return examinations;
        }

        public async Task<Examination?> GetByIdAsync(long id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = "SELECT id, patient_id, examination_date, examination_time, examination_type, description, created_at FROM examinations WHERE id = @id";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return Examination.FromSqlReader(reader);
            }

            return null;
        }

        public async Task<IEnumerable<Examination>> GetByPatientIdAsync(long patientId)
        {
            var examinations = new List<Examination>();

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = "SELECT id, patient_id, examination_date, examination_time, examination_type, description, created_at FROM examinations WHERE patient_id = @patientId ORDER BY examination_date DESC, examination_time DESC";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@patientId", patientId);

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                examinations.Add(Examination.FromSqlReader(reader));
            }

            return examinations;
        }

        public async Task<IEnumerable<Examination>> GetByTypeAsync(string examinationType)
        {
            var examinations = new List<Examination>();

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = "SELECT id, patient_id, examination_date, examination_time, examination_type, description, created_at FROM examinations WHERE examination_type = @examinationType ORDER BY examination_date DESC";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@examinationType", examinationType);

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                examinations.Add(Examination.FromSqlReader(reader));
            }

            return examinations;
        }

        public async Task<Examination> CreateAsync(Examination examination)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = @"
                INSERT INTO examinations (patient_id, examination_date, examination_time, examination_type, description)
                VALUES (@patientId, @examinationDate, @examinationTime, @examinationType, @description)
                RETURNING id, created_at";

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@patientId", examination.PatientId);
            command.Parameters.AddWithValue("@examinationDate", examination.ExaminationDate);
            command.Parameters.AddWithValue("@examinationTime", examination.ExaminationTime);
            command.Parameters.AddWithValue("@examinationType", examination.ExaminationType);
            command.Parameters.AddWithValue("@description", examination.Description ?? string.Empty);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                examination.Id = (long)reader["id"];
                examination.CreatedAt = (DateTime)reader["created_at"];
            }

            return examination;
        }

        public async Task<Examination> UpdateAsync(Examination examination)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = @"
                UPDATE examinations 
                SET patient_id = @patientId, 
                    examination_date = @examinationDate, 
                    examination_time = @examinationTime, 
                    examination_type = @examinationType, 
                    description = @description
                WHERE id = @id";

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", examination.Id);
            command.Parameters.AddWithValue("@patientId", examination.PatientId);
            command.Parameters.AddWithValue("@examinationDate", examination.ExaminationDate);
            command.Parameters.AddWithValue("@examinationTime", examination.ExaminationTime);
            command.Parameters.AddWithValue("@examinationType", examination.ExaminationType);
            command.Parameters.AddWithValue("@description", examination.Description ?? string.Empty);

            await command.ExecuteNonQueryAsync();
            return examination;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = "DELETE FROM examinations WHERE id = @id";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
    }
}

