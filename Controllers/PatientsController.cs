using Microsoft.AspNetCore.Mvc;
using MedicalSystem.Models;
using MedicalSystem.Repositories.Interfaces;
using MedicalSystem.Services;
using Npgsql;

namespace MedicalSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IPatientService _patientService;
        private readonly IConfiguration _configuration;

        // Lazy paradigma za repository
        private readonly Lazy<IPatientRepository> _patientRepository;

        public PatientsController(IRepositoryFactory repositoryFactory, IPatientService patientService, IConfiguration configuration)
        {
            _repositoryFactory = repositoryFactory;
            _patientService = patientService;
            _configuration = configuration;

            // Lazy loading - repository se kreira tek kad je potreban
            _patientRepository = new Lazy<IPatientRepository>(() => _repositoryFactory.CreatePatientRepository());
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Patient>>> GetAllPatients()
        {
            try
            {
                var patients = await _patientRepository.Value.GetAllAsync();
                return Ok(patients);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Patient>> GetPatient(long id)
        {
            try
            {
                var patient = await _patientRepository.Value.GetByIdAsync(id);
                if (patient == null)
                {
                    return NotFound();
                }
                return Ok(patient);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Nova funkcionalnost - potpuni prikaz pacijenta s povezanim podacima
        [HttpGet("{id}/details")]
        public async Task<ActionResult<PatientDetailsDto>> GetPatientWithDetails(long id)
        {
            try
            {
                var patientDetails = await _patientService.GetPatientWithDetailsAsync(id);
                return Ok(patientDetails);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { Error = "Not found", Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = "Internal server error",
                    Message = ex.Message
                });
            }

        }

        // CSV Export funkcionalnost
        [HttpGet("export/csv")]
        public async Task<IActionResult> ExportPatientsToCSV()
        {
            try
            {
                var csvData = await _patientService.ExportPatientsToCSVAsync();

                return File(csvData, "text/csv", $"patients_export_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Patient>>> SearchPatients([FromQuery] string lastName = "", [FromQuery] string oib = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(oib))
                {
                    var patientByOib = await _patientRepository.Value.GetByOibAsync(oib);
                    return Ok(patientByOib != null ? new[] { patientByOib } : Array.Empty<Patient>());
                }

                if (!string.IsNullOrEmpty(lastName))
                {
                    var patients = await _patientRepository.Value.SearchByLastNameAsync(lastName);
                    return Ok(patients);
                }

                return BadRequest("Please provide lastName or oib parameter for search");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Patient>> CreatePatient([FromBody] Patient patient)
        {
            try
            {
                if (string.IsNullOrEmpty(patient.FirstName) ||
                    string.IsNullOrEmpty(patient.LastName) ||
                    string.IsNullOrEmpty(patient.Oib))
                {
                    return BadRequest("FirstName, LastName, and Oib are required");
                }

                var createdPatient = await _patientRepository.Value.CreateAsync(patient);
                return CreatedAtAction(nameof(GetPatient), new { id = createdPatient.Id }, createdPatient);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Patient>> UpdatePatient(long id, [FromBody] Patient patient)
        {
            try
            {
                if (id != patient.Id)
                {
                    return BadRequest("ID mismatch");
                }

                var existingPatient = await _patientRepository.Value.GetByIdAsync(id);
                if (existingPatient == null)
                {
                    return NotFound();
                }

                var updatedPatient = await _patientRepository.Value.UpdateAsync(patient);
                return Ok(updatedPatient);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePatient(long id)
        {
            try
            {
                var success = await _patientRepository.Value.DeleteAsync(id);
                if (!success)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("test-connection")]
        public async Task<ActionResult> TestConnection()
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");

                using var connection = new NpgsqlConnection(connectionString);
                await connection.OpenAsync();

                var sql = "SELECT COUNT(*) FROM patients";
                using var command = new NpgsqlCommand(sql, connection);
                var count = await command.ExecuteScalarAsync();

                return Ok(new
                {
                    Status = "Connected successfully",
                    Host = connection.Host,
                    Database = connection.Database,
                    PatientCount = count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = ex.Message,
                    InnerError = ex.InnerException?.Message
                });
            }
        }
    }
}