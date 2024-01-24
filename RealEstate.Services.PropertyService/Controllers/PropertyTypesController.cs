using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Services.PropertyService.Models;
using RealEstate.Services.PropertyService.Repositories.IRepositories;

namespace RealEstate.Services.PropertyService.Controllers
{
    [Route("api/propertyTypes")]
    [ApiController]
    [Authorize]
    public class PropertyTypesController : ControllerBase
    {
        private readonly IPropertyTypeRepository _propertyTypeRepository;

        public PropertyTypesController(IPropertyTypeRepository propertyTypeRepository)
        {
            _propertyTypeRepository = propertyTypeRepository;
        }

        [HttpGet("GetPropertyTypes")]
        public async Task<IActionResult> GetPropertyTypes()
        {
            var propertyTypes = await _propertyTypeRepository.GetAllAsync();
            return Ok(propertyTypes);
        }

        [HttpPost("AddPropertyType")]
        public async Task<IActionResult> AddPropertyType(PropertyType propertyType)
        {
            if (propertyType == null)
            {
                return BadRequest();
            }
            await _propertyTypeRepository.AddAsync(propertyType);
            await _propertyTypeRepository.SaveChangesAsync();
            _propertyTypeRepository.Dispose();
            return Ok();
        }

        [HttpPut("UpdatePropertyType")]
        public async Task<IActionResult> UpdatePropertyType(PropertyType propertyType)
        {
            if (propertyType.Id == 0)
            {
                return BadRequest();
            }
            _propertyTypeRepository.Update(propertyType);
            await _propertyTypeRepository.SaveChangesAsync();
            _propertyTypeRepository.Dispose();
            return Ok();
        }

        [HttpGet("GetPropertyType/{id}")]
        public async Task<IActionResult> GetPropertyType(int id)
        {
            var propertyType = await _propertyTypeRepository.GetFirstOrDefaultAsync(x => x.Id == id);
            if (propertyType == null)
            {
                return NotFound();
            }
            return Ok(propertyType);
        }

        [HttpDelete("DeletePropertyType/{id}")]
        public async Task<IActionResult> DeletePropertyType(int id)
        {
            var propertyType = await _propertyTypeRepository.GetFirstOrDefaultAsync(x => x.Id == id);
            if (propertyType == null)
            {
                return NotFound();
            }
            _propertyTypeRepository.Remove(propertyType);
            await _propertyTypeRepository.SaveChangesAsync();
            _propertyTypeRepository.Dispose();
            return Ok();
        }
    }
}