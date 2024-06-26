﻿using BicardBackend.Data;
using BicardBackend.DTOs;
using BicardBackend.Models;
using BicardBackend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BicardBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedServicesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;
        public MedServicesController(ApplicationDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }
        [HttpGet("GetMedServiceById/{id}")]
        public async Task<IActionResult> GetMedServiceById(int id)
        {
            var medService = await _context.Meds.FindAsync(id);
            if (medService == null)
            {
                return NotFound();
            }
            return Ok(medService);
        }
        [HttpGet("GetSubMedServiceById/{id}")]
        public async Task<IActionResult> GetSubMedServiceById(int id)
        {
            var subMedService = await _context.Subs.FindAsync(id);
            if (subMedService == null)
            {
                return NotFound();
            }
            return Ok(subMedService);
        }
        [HttpGet("GetListOfMedServices")]
        public async Task<IActionResult> GetListOfMedServices()
        {
            var list = _context.Meds.ToList();
            return Ok(list);
        }
        [HttpGet("GetListOfSubMedServices/{medServiceId}")]
        public async Task<IActionResult> GetListOfSubMedServices(int medServiceId)
        {
            var list = _context.Subs.Where(s => s.MedServiceId == medServiceId).ToList();
            return Ok(list);
        }
        [HttpGet("GetListOfAllSubMedServices")]
        public async Task<IActionResult> GetListOfAllSubMedServices()
        {
            var list = await _context.Subs
                .ToListAsync();
            return Ok(list);
        }
        [HttpPost("CreateMedService")]
        public async Task<IActionResult> CreateMedService([FromForm] MedServiceDto dto)
        {
            if (ModelState.IsValid)
            {
                MedService model = new() 
                {
                    Name = dto.Name,
                    ShortDescription = dto.ShortDescription,
                    LongDescription = dto.LongDescription,
                    PathToPhoto = await _fileService.SaveFileAsync(dto.Photo, "MedService")
                };
                
                await _context.Meds.AddAsync(model);
                
                await _context.SaveChangesAsync();
                return Ok(model);
            }
            return BadRequest("Model state is invalid.");
        }
        [HttpPost("CreateSubMedService")]
        public async Task<IActionResult> CreateSubMedService(SubMedServiceDto dto)
        {
            if (ModelState.IsValid)
            {
                SubMedService model = new()
                {
                    Name = dto.Name,
                    Price = dto.Price,
                    MedServiceId = dto.MedServiceId,
                };
                await _context.Subs.AddAsync(model);
                await _context.SaveChangesAsync();
                return Ok("SubMedService created.");
            }
            return BadRequest("Model state is invalid.");
        }
        [HttpPut("UpdateMedService")]
        public async Task<IActionResult> UpdateMedService(int id, [FromForm] MedServiceDto dto)
        {
            if (ModelState.IsValid)
            {
                var medService = await _context.Meds.FindAsync(id);

                if (medService == null)
                {
                    return NotFound("MedService not found.");
                }
                medService.Name = dto.Name ?? medService.Name;
                medService.ShortDescription = dto.ShortDescription ?? medService.ShortDescription;
                medService.LongDescription = dto.LongDescription ?? medService.LongDescription;
                if (dto.Photo != null) 
                {
                    _fileService.DeleteFile(medService.PathToPhoto);
                    medService.PathToPhoto = await _fileService.SaveFileAsync(dto.Photo, "MedService");
                }
                
                await _context.SaveChangesAsync();

                return Ok();
            }

            return BadRequest("Model state is invalid.");
        }
        [HttpPut("UpdateSubMedService")]
        public async Task<IActionResult> UpdateSubMedService(int id, SubMedServiceDto model)
        {
            if (ModelState.IsValid)
            {
                // Step 1: Find the existing entity by its ID
                var subMedService = await _context.Subs.FindAsync(id);

                if (subMedService == null)
                {
                    // Step 2: Handle the case where the entity with the given ID is not found
                    return NotFound("SubMedService not found.");
                }

                // Step 3: Update the properties of the existing entity
                _context.Entry(subMedService).CurrentValues.SetValues(model);

                // Step 4: Set the state to Modified to inform EF that it should consider the entity as modified
                _context.Entry(subMedService).State = EntityState.Modified;

                // Step 5: Save changes to the database
                await _context.SaveChangesAsync();

                // Step 6: Return a success response
                return Ok("SubMedService updated successfully.");
            }

            // Handle the case where the model state is invalid
            return BadRequest("Model state is invalid.");
        }
        [HttpDelete("DeleteMedService/{id}")]
        public async Task<IActionResult> DeleteMedService(int id)
        {
            // Step 1: Find the existing entity by its ID
            var medService = await _context.Meds.FindAsync(id);

            if (medService == null)
            {
                // Step 2: Handle the case where the entity with the given ID is not found
                return NotFound("MedService not found.");
            }
            _fileService.DeleteFile(medService.PathToPhoto);
            // Step 3: Remove the entity from the DbSet
            _context.Meds.Remove(medService);

            // Step 4: Save changes to the database
            await _context.SaveChangesAsync();

            // Step 5: Return a success response
            return Ok("MedService deleted successfully.");
        }
        [HttpDelete("DeleteSubMedService/{id}")]
        public async Task<IActionResult> DeleteSubMedService(int id)
        {
            // Step 1: Find the existing entity by its ID
            var subMedService = await _context.Subs.FindAsync(id);

            if (subMedService == null)
            {
                // Step 2: Handle the case where the entity with the given ID is not found
                return NotFound("SubMedService not found.");
            }

            // Step 3: Remove the entity from the DbSet
            _context.Subs.Remove(subMedService);

            // Step 4: Save changes to the database
            await _context.SaveChangesAsync();

            // Step 5: Return a success response
            return Ok("SubMedService deleted successfully.");
        }

    }
}
