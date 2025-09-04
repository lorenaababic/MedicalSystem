using System;
using MedicalSystem.Models;
using MedicalSystem.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MedicalSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicalDocumentationController : ControllerBase
    {
        private readonly IMedicalDocumentationRepository _medicalDocumentationRepository;

        public MedicalDocumentationController(IMedicalDocumentationRepository medicalDocumentationRepository)
        {
            _medicalDocumentationRepository = medicalDocumentationRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MedicalDocumentation>>> GetAllMedicalDocumentations()
        {
            try
            {
                var documentations = await _medicalDocumentationRepository.GetAllAsync();
                return Ok(documentations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MedicalDocumentation>> GetMedicalDocumentation(long id)
        {
            try
            {
                var documentation = await _medicalDocumentationRepository.GetByIdAsync(id);
                if (documentation == null)
                {
                    return NotFound();
                }
                return Ok(documentation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("patient/{patientId}")]
        public async Task<ActionResult<IEnumerable<MedicalDocumentation>>> GetMedicalDocumentationByPatient(long patientId)
        {
            try
            {
                var documentations = await _medicalDocumentationRepository.GetByPatientIdAsync(patientId);
                return Ok(documentations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<MedicalDocumentation>> CreateMedicalDocumentation([FromBody] MedicalDocumentation medicalDocumentation)
        {
            try
            {
                if (medicalDocumentation.PatientId <= 0 || string.IsNullOrEmpty(medicalDocumentation.DiseaseName))
                {
                    return BadRequest("PatientId and DiseaseName are required");
                }

                var createdDocumentation = await _medicalDocumentationRepository.CreateAsync(medicalDocumentation);
                return CreatedAtAction(nameof(GetMedicalDocumentation), new { id = createdDocumentation.Id }, createdDocumentation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<MedicalDocumentation>> UpdateMedicalDocumentation(long id, [FromBody] MedicalDocumentation medicalDocumentation)
        {
            try
            {
                if (id != medicalDocumentation.Id)
                {
                    return BadRequest("ID mismatch");
                }

                var existingDocumentation = await _medicalDocumentationRepository.GetByIdAsync(id);
                if (existingDocumentation == null)
                {
                    return NotFound();
                }

                var updatedDocumentation = await _medicalDocumentationRepository.UpdateAsync(medicalDocumentation);
                return Ok(updatedDocumentation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMedicalDocumentation(long id)
        {
            try
            {
                var success = await _medicalDocumentationRepository.DeleteAsync(id);
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

