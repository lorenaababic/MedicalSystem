using System;
using MedicalSystem.Models;
using MedicalSystem.Repositories.Interfaces;
using Npgsql;

namespace MedicalSystem.Repositories
{
    public class PrescriptionRepository : IPrescriptionRepository
    {
        private readonly string _connectionString;

        public PrescriptionRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<IEnumerable<Prescription>> GetAllAsync()
        {
            var prescriptions = new List<Prescription>();

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open(); // SYNCHRONNI

            var sql = "SELECT id, patient_id, examination_id, medication_name, dosage, issue_date, notes, created_at FROM prescriptions ORDER BY issue_date DESC";
            using var command = new NpgsqlCommand(sql, connection);
            using var reader = command.ExecuteReader(); // SYNCHRONNI

            while (reader.Read()) // SYNCHRONNI
            {
                prescriptions.Add(Prescription.FromSqlReader(reader));
            }

            return prescriptions;
        }

        public async Task<Prescription?> GetByIdAsync(long id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = "SELECT id, patient_id, examination_id, medication_name, dosage, issue_date, notes, created_at FROM prescriptions WHERE id = @id";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return Prescription.FromSqlReader(reader);
            }

            return null;
        }

        public async Task<IEnumerable<Prescription>> GetByPatientIdAsync(long patientId)
        {
            var prescriptions = new List<Prescription>();

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = "SELECT id, patient_id, examination_id, medication_name, dosage, issue_date, notes, created_at FROM prescriptions WHERE patient_id = @patientId ORDER BY issue_date DESC";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@patientId", patientId);

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                prescriptions.Add(Prescription.FromSqlReader(reader));
            }

            return prescriptions;
        }

        public async Task<IEnumerable<Prescription>> GetByExaminationIdAsync(long examinationId)
        {
            var prescriptions = new List<Prescription>();

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = "SELECT id, patient_id, examination_id, medication_name, dosage, issue_date, notes, created_at FROM prescriptions WHERE examination_id = @examinationId";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@examinationId", examinationId);

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                prescriptions.Add(Prescription.FromSqlReader(reader));
            }

            return prescriptions;
        }

        public async Task<Prescription> CreateAsync(Prescription prescription)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = @"
                INSERT INTO prescriptions (patient_id, examination_id, medication_name, dosage, issue_date, notes)
                VALUES (@patientId, @examinationId, @medicationName, @dosage, @issueDate, @notes)
                RETURNING id, created_at";

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@patientId", prescription.PatientId);
            command.Parameters.AddWithValue("@examinationId", (object?)prescription.ExaminationId ?? DBNull.Value);
            command.Parameters.AddWithValue("@medicationName", prescription.MedicationName);
            command.Parameters.AddWithValue("@dosage", prescription.Dosage);
            command.Parameters.AddWithValue("@issueDate", prescription.IssueDate);
            command.Parameters.AddWithValue("@notes", prescription.Notes ?? string.Empty);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                prescription.Id = (long)reader["id"];
                prescription.CreatedAt = (DateTime)reader["created_at"];
            }

            return prescription;
        }

        public async Task<Prescription> UpdateAsync(Prescription prescription)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = @"
                UPDATE prescriptions 
                SET patient_id = @patientId, 
                    examination_id = @examinationId, 
                    medication_name = @medicationName, 
                    dosage = @dosage, 
                    issue_date = @issueDate, 
                    notes = @notes
                WHERE id = @id";

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", prescription.Id);
            command.Parameters.AddWithValue("@patientId", prescription.PatientId);
            command.Parameters.AddWithValue("@examinationId", (object?)prescription.ExaminationId ?? DBNull.Value);
            command.Parameters.AddWithValue("@medicationName", prescription.MedicationName);
            command.Parameters.AddWithValue("@dosage", prescription.Dosage);
            command.Parameters.AddWithValue("@issueDate", prescription.IssueDate);
            command.Parameters.AddWithValue("@notes", prescription.Notes ?? string.Empty);

            await command.ExecuteNonQueryAsync();
            return prescription;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = "DELETE FROM prescriptions WHERE id = @id";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
    }
}

