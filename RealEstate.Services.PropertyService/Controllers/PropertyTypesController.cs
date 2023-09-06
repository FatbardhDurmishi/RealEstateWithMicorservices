using Microsoft.AspNetCore.Mvc;
using RealEstate.Services.PropertyService.Models;
using RealEstate.Services.PropertyService.Repositories.IRepositories;

namespace RealEstate.Services.PropertyService.Controllers
{
    [Route("api/propertyTypes")]
    [ApiController]
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
            var propertyTypes = await _propertyTypeRepository.GetAll();
            return Ok(propertyTypes);
        }

        [HttpPost("AddPropertyType")]
        public async Task<IActionResult> AddPropertyType(PropertyType propertyType)
        {
            if (propertyType == null)
            {
                return BadRequest();
            }
            await _propertyTypeRepository.Add(propertyType);
            return Ok();
        }

        [HttpPut("UpdatePropertyType")]
        public async Task<IActionResult> UpdatePropertyType(PropertyType propertyType)
        {
            if (propertyType.Id == 0)
            {
                return BadRequest();
            }
            await _propertyTypeRepository.Update(propertyType);
            return Ok();
        }

        [HttpGet("GetPropertyType/{id}")]
        public async Task<IActionResult> GetPropertyType(int id)
        {
            var propertyType = await _propertyTypeRepository.GetFirstOrDefault(x => x.Id == id);
            if (propertyType == null)
            {
                return NotFound();
            }
            return Ok(propertyType);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePropertyType(int id)
        {
            var propertyType = await _propertyTypeRepository.GetFirstOrDefault(x => x.Id == id);
            if (propertyType == null)
            {
                return NotFound();
            }
            await _propertyTypeRepository.Remove(propertyType);
            return Ok();
        }
    }
}