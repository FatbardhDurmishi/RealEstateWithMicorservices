using Microsoft.AspNetCore.Mvc;
using RealEstate.Services.PropertyService.Models;
using RealEstate.Services.PropertyService.Repositories.IRepositories;

namespace RealEstate.Services.PropertyService.Controllers
{
    [Route("api/propertyType")]
    [ApiController]
    public class PropertyTypesController : ControllerBase
    {
        private readonly IPropertyTypeRepository _propertyTypeRepository;

        public PropertyTypesController(IPropertyTypeRepository propertyTypeRepository)
        {
            _propertyTypeRepository = propertyTypeRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetPropertyTypes()
        {
            var propertyTypes = await _propertyTypeRepository.GetAll();
            return Ok(propertyTypes);
        }

        [HttpPost]
        public async Task<IActionResult> AddPropertyType(PropertyType propertyType)
        {
            if (propertyType == null)
            {
                return BadRequest();
            }
            await _propertyTypeRepository.Add(propertyType);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePropertyType(PropertyType propertyType)
        {
            if (propertyType == null)
            {
                return BadRequest();
            }
            await _propertyTypeRepository.Update(propertyType);
            return Ok();
        }
    }
}