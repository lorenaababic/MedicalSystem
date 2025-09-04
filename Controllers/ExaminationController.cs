using System;
using MedicalSystem.Models;
using MedicalSystem.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MedicalSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExaminationController : ControllerBase
    {
        private readonly IExaminationRepository _examinationRepository;

        public ExaminationController(IExaminationRepository examinationRepository)
        {
            _examinationRepository = examinationRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Examination>>> GetAllExaminations()
        {
            try
            {
                var examinations = await _examinationRepository.GetAllAsync();
                return Ok(examinations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Examination>> GetExamination(long id)
        {
            try
            {
                var examination = await _examinationRepository.GetByIdAsync(id);
                if (examination == null)
                {
                    return NotFound();
                }
                return Ok(examination);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("patient/{patientId}")]
        public async Task<ActionResult<IEnumerable<Examination>>> GetExaminationsByPatient(long patientId)
        {
            try
            {
                var examinations = await _examinationRepository.GetByPatientIdAsync(patientId);
                return Ok(examinations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("type/{examinationType}")]
        public async Task<ActionResult<IEnumerable<Examination>>> GetExaminationsByType(string examinationType)
        {
            try
            {
                var examinations = await _examinationRepository.GetByTypeAsync(examinationType);
                return Ok(examinations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Examination>> CreateExamination([FromBody] Examination examination)
        {
            try
            {
                if (examination.PatientId <= 0 || string.IsNullOrEmpty(examination.ExaminationType))
                {
                    return BadRequest("PatientId and ExaminationType are required");
                }

                var createdExamination = await _examinationRepository.CreateAsync(examination);
                return CreatedAtAction(nameof(GetExamination), new { id = createdExamination.Id }, createdExamination);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Examination>> UpdateExamination(long id, [FromBody] Examination examination)
        {
            try
            {
                if (id != examination.Id)
                {
                    return BadRequest("ID mismatch");
                }

                var existingExamination = await _examinationRepository.GetByIdAsync(id);
                if (existingExamination == null)
                {
                    return NotFound();
                }

                var updatedExamination = await _examinationRepository.UpdateAsync(examination);
                return Ok(updatedExamination);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteExamination(long id)
        {
            try
            {
                var success = await _examinationRepository.DeleteAsync(id);
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

