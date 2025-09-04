using System;
using MedicalSystem.Models;
using MedicalSystem.Repositories.Interfaces;
using Npgsql;

namespace MedicalSystem.Repositories
{
    public class MedicalDocumentationRepository : IMedicalDocumentationRepository
    {
        private readonly string _connectionString;

        public MedicalDocumentationRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<IEnumerable<MedicalDocumentation>> GetAllAsync()
        {
            var documentations = new List<MedicalDocumentation>();

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            var sql = "SELECT id, patient_id, disease_name, start_date, end_date, created_at FROM medical_documentation ORDER BY created_at DESC";
            using var command = new NpgsqlCommand(sql, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                documentations.Add(MedicalDocumentation.FromSqlReader(reader));
            }

            return documentations;
        }

        public async Task<MedicalDocumentation?> GetByIdAsync(long id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            var sql = "SELECT id, patient_id, disease_name, start_date, end_date, created_at FROM medical_documentation WHERE id = @id";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return MedicalDocumentation.FromSqlReader(reader);
            }

            return null;
        }

        public async Task<IEnumerable<MedicalDocumentation>> GetByPatientIdAsync(long patientId)
        {
            var documentations = new List<MedicalDocumentation>();

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            var sql = "SELECT id, patient_id, disease_name, start_date, end_date, created_at FROM medical_documentation WHERE patient_id = @patientId ORDER BY start_date DESC";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@patientId", patientId);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                documentations.Add(MedicalDocumentation.FromSqlReader(reader));
            }

            return documentations;
        }

        public async Task<MedicalDocumentation> CreateAsync(MedicalDocumentation medicalDocumentation)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            var sql = @"
                INSERT INTO medical_documentation (patient_id, disease_name, start_date, end_date)
                VALUES (@patientId, @diseaseName, @startDate, @endDate)
                RETURNING id, created_at";

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@patientId", medicalDocumentation.PatientId);
            command.Parameters.AddWithValue("@diseaseName", medicalDocumentation.DiseaseName);
            command.Parameters.AddWithValue("@startDate", medicalDocumentation.StartDate);
            command.Parameters.AddWithValue("@endDate", (object?)medicalDocumentation.EndDate ?? DBNull.Value);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                medicalDocumentation.Id = (long)reader["id"];
                medicalDocumentation.CreatedAt = (DateTime)reader["created_at"];
            }

            return medicalDocumentation;
        }

        public async Task<MedicalDocumentation> UpdateAsync(MedicalDocumentation medicalDocumentation)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            var sql = @"
                UPDATE medical_documentation 
                SET patient_id = @patientId, 
                    disease_name = @diseaseName, 
                    start_date = @startDate, 
                    end_date = @endDate
                WHERE id = @id";

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", medicalDocumentation.Id);
            command.Parameters.AddWithValue("@patientId", medicalDocumentation.PatientId);
            command.Parameters.AddWithValue("@diseaseName", medicalDocumentation.DiseaseName);
            command.Parameters.AddWithValue("@startDate", medicalDocumentation.StartDate);
            command.Parameters.AddWithValue("@endDate", (object?)medicalDocumentation.EndDate ?? DBNull.Value);

            command.ExecuteNonQuery();
            return medicalDocumentation;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            var sql = "DELETE FROM medical_documentation WHERE id = @id";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            var rowsAffected = command.ExecuteNonQuery();
            return rowsAffected > 0;
        }
    }
}