using System;
using MedicalSystem.Models;
using MedicalSystem.Repositories.Interfaces;
using Npgsql;

namespace MedicalSystem.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly string _connectionString;

        public PatientRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<IEnumerable<Patient>> GetAllAsync()
        {
            var patients = new List<Patient>();

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            // ISPRAVKA: Dodajem patient_number u sve SQL upite
            var sql = "SELECT id, first_name, last_name, oib, gender, date_of_birth, created_at, patient_number FROM patients ORDER BY last_name, first_name";
            using var command = new NpgsqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                patients.Add(Patient.FromSqlReader(reader));
            }

            return patients;
        }

        public async Task<Patient?> GetByIdAsync(long id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            // ISPRAVKA: Dodajem patient_number
            var sql = "SELECT id, first_name, last_name, oib, gender, date_of_birth, created_at, patient_number FROM patients WHERE id = @id";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return Patient.FromSqlReader(reader);
            }

            return null;
        }

        public async Task<Patient?> GetByOibAsync(string oib)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            // ISPRAVKA: Dodajem patient_number
            var sql = "SELECT id, first_name, last_name, oib, gender, date_of_birth, created_at, patient_number FROM patients WHERE oib = @oib";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@oib", oib);

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return Patient.FromSqlReader(reader);
            }

            return null;
        }

        public async Task<IEnumerable<Patient>> SearchByLastNameAsync(string lastName)
        {
            var patients = new List<Patient>();

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            // ISPRAVKA: Dodajem patient_number
            var sql = "SELECT id, first_name, last_name, oib, gender, date_of_birth, created_at, patient_number FROM patients WHERE LOWER(last_name) LIKE LOWER(@lastName) ORDER BY last_name, first_name";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@lastName", $"%{lastName}%");

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                patients.Add(Patient.FromSqlReader(reader));
            }

            return patients;
        }

        public async Task<Patient> CreateAsync(Patient patient)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            // ISPRAVKA: Dodajem patient_number u INSERT
            var sql = @"
                INSERT INTO patients (first_name, last_name, oib, gender, date_of_birth, patient_number)
                VALUES (@firstName, @lastName, @oib, @gender, @dateOfBirth, @patientNumber)
                RETURNING id, created_at";

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@firstName", patient.FirstName);
            command.Parameters.AddWithValue("@lastName", patient.LastName);
            command.Parameters.AddWithValue("@oib", patient.Oib);
            command.Parameters.AddWithValue("@gender", patient.Gender);
            command.Parameters.AddWithValue("@dateOfBirth", patient.DateOfBirth);
            command.Parameters.AddWithValue("@patientNumber", (object?)patient.PatientNumber ?? DBNull.Value);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                patient.Id = Convert.ToInt64(reader["id"]);
                patient.CreatedAt = (DateTime)reader["created_at"];
            }

            return patient;
        }

        public async Task<Patient> UpdateAsync(Patient patient)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            // ISPRAVKA: Dodajem patient_number u UPDATE
            var sql = @"
                UPDATE patients 
                SET first_name = @firstName, 
                    last_name = @lastName, 
                    oib = @oib, 
                    gender = @gender, 
                    date_of_birth = @dateOfBirth,
                    patient_number = @patientNumber
                WHERE id = @id";

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", patient.Id);
            command.Parameters.AddWithValue("@firstName", patient.FirstName);
            command.Parameters.AddWithValue("@lastName", patient.LastName);
            command.Parameters.AddWithValue("@oib", patient.Oib);
            command.Parameters.AddWithValue("@gender", patient.Gender);
            command.Parameters.AddWithValue("@dateOfBirth", patient.DateOfBirth);
            command.Parameters.AddWithValue("@patientNumber", (object?)patient.PatientNumber ?? DBNull.Value);

            await command.ExecuteNonQueryAsync();
            return patient;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = "DELETE FROM patients WHERE id = @id";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
    }
}