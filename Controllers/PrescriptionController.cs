using System;
using MedicalSystem.Models;
using MedicalSystem.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MedicalSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrescriptionController : ControllerBase
    {
        private readonly IPrescriptionRepository _prescriptionRepository;

        public PrescriptionController(IPrescriptionRepository prescriptionRepository)
        {
            _prescriptionRepository = prescriptionRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Prescription>>> GetAllPrescriptions()
        {
            try
            {
                var prescriptions = await _prescriptionRepository.GetAllAsync();
                return Ok(prescriptions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Prescription>> GetPrescription(long id)
        {
            try
            {
                var prescription = await _prescriptionRepository.GetByIdAsync(id);
                if (prescription == null)
                {
                    return NotFound();
                }
                return Ok(prescription);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("patient/{patientId}")]
        public async Task<ActionResult<IEnumerable<Prescription>>> GetPrescriptionsByPatient(long patientId)
        {
            try
            {
                var prescriptions = await _prescriptionRepository.GetByPatientIdAsync(patientId);
                return Ok(prescriptions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("examination/{examinationId}")]
        public async Task<ActionResult<IEnumerable<Prescription>>> GetPrescriptionsByExamination(long examinationId)
        {
            try
            {
                var prescriptions = await _prescriptionRepository.GetByExaminationIdAsync(examinationId);
                return Ok(prescriptions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Prescription>> CreatePrescription([FromBody] Prescription prescription)
        {
            try
            {
                if (prescription.PatientId <= 0 || string.IsNullOrEmpty(prescription.MedicationName))
                {
                    return BadRequest("PatientId and MedicationName are required");
                }

                var createdPrescription = await _prescriptionRepository.CreateAsync(prescription);
                return CreatedAtAction(nameof(GetPrescription), new { id = createdPrescription.Id }, createdPrescription);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Prescription>> UpdatePrescription(long id, [FromBody] Prescription prescription)
        {
            try
            {
                if (id != prescription.Id)
                {
                    return BadRequest("ID mismatch");
                }

                var existingPrescription = await _prescriptionRepository.GetByIdAsync(id);
                if (existingPrescription == null)
                {
                    return NotFound();
                }

                var updatedPrescription = await _prescriptionRepository.UpdateAsync(prescription);
                return Ok(updatedPrescription);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePrescription(long id)
        {
            try
            {
                var success = await _prescriptionRepository.DeleteAsync(id);
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
    }
}

