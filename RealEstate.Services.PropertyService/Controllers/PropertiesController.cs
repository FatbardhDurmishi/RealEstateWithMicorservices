﻿using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Services.PropertyService.Constants;
using RealEstate.Services.PropertyService.Helpers;
using RealEstate.Services.PropertyService.Models;
using RealEstate.Services.PropertyService.Models.Dtos;
using RealEstate.Services.PropertyService.Repositories.IRepositories;

namespace RealEstate.Services.PropertyService.Controllers
{
    [Route("api/property")]
    [ApiController]
    [Authorize]
    public class PropertiesController : ControllerBase
    {
        private readonly BlobServiceClient blobServiceClient;
        private readonly BlobContainerClient containerClient;
        private readonly string accountName = "estateville";
        private readonly string accountKey = "yj2rtRQLS0zZsUg7zov/rmbN8aiQx0g10kVcxPuq/R8NQ3L19dmzYvN1os7G8OifyC0+YK64PeL2+ASt49OmQQ==";
        private readonly StorageSharedKeyCredential credential;
        private readonly IPropertyRepository _propertyRepository;
        private readonly HttpClient _httpClient;
        private readonly IPropertyImageRepository _propertyImageRepository;
        private readonly IConfiguration _configuration;

        public PropertiesController(IPropertyRepository propertyRepository, HttpClient httpClient, IPropertyImageRepository propertyImageRepository, IConfiguration configuration)
        {
            _configuration = configuration;

            credential = new StorageSharedKeyCredential(accountName, accountKey);
            blobServiceClient = new BlobServiceClient(_configuration.GetConnectionString("AzureStorageConnection"));
            containerClient = blobServiceClient.GetBlobContainerClient("estateville");
            _propertyRepository = propertyRepository;
            _httpClient = httpClient;
            _propertyImageRepository = propertyImageRepository;
        }

        [HttpPost("addProperty")]
        public async Task<IActionResult> AddProperty(PropertyDto addPropertyDto)
        {
            if (addPropertyDto == null)
            {
                return BadRequest();
            }
            var property = new Property()
            {
                Name = addPropertyDto.Name,
                Description = addPropertyDto.Description,
                BedRooms = addPropertyDto.BedRooms,
                BathRooms = addPropertyDto.BedRooms,
                Area = addPropertyDto.Area,
                Price = addPropertyDto.Price,
                State = addPropertyDto.State,
                Status = PropertyStatus.Free,
                City = addPropertyDto.City,
                StreetAddress = addPropertyDto.StreetAddress,
                CoverImageUrl = addPropertyDto.CoverImageUrl,
                TransactionType = addPropertyDto.TransactionType,
                PropertyTypeId = addPropertyDto.PropertyTypeId,
            };
            if (addPropertyDto.CurrentUserRole == RoleConstants.Role_User_Indi)
            {
                property.UserId = addPropertyDto.CurrentUserId;
            }
            else
            {
                property.UserId = addPropertyDto.UserId;
            }
            await _propertyRepository.AddAsync(property);
            await _propertyRepository.SaveChangesAsync();
            _propertyRepository.Dispose();
            return Ok(property);
        }

        [HttpPost("UploadImages/{propertyId}")]
        public async Task<IActionResult> UploadImages([FromForm(Name = "CoverImage")] IFormFile CoverImage, [FromForm(Name = "PropertyImages")] IFormFileCollection PropertyImages, int propertyId)
        {
            var property = await _propertyRepository.GetFirstOrDefaultAsync(x => x.Id == propertyId);
            if (property == null)
            {
                return BadRequest();
            }
            if (CoverImage != null)
            {
                bool result = await AzureBlobActions.UploadToBlob(containerClient, CoverImage);
                if (!result)
                {
                    return BadRequest(result);
                }
                property.CoverImageUrl = CoverImage.FileName;
                _propertyRepository.Update(property);


            }
            foreach (var image in PropertyImages)
            {
                var Image = new PropertyImage
                {
                    ImageUrl = image.FileName,
                    PropertyId = propertyId
                };
                bool result = await AzureBlobActions.UploadToBlob(containerClient, image);
                if (result)
                {
                    await _propertyImageRepository.AddAsync(Image);
                }

            }
            await _propertyImageRepository.SaveChangesAsync();
            _propertyImageRepository.Dispose();
            return Ok();
        }

        [HttpPut("UpdateProperty")]
        public async Task<IActionResult> UpdateProperty([FromBody] dynamic parameters)
        {
            int propertyId = parameters.addPropertyDto.id;
            if (propertyId == 0)
            {
                return BadRequest();
            }
            var property = await _propertyRepository.GetFirstOrDefaultAsync(x => x.Id == propertyId);
            if (property == null)
            {
                return NotFound();
            }
            foreach (var imageId in parameters.imagesToDelete)
            {
                int Id = imageId;
                var image = await _propertyImageRepository.GetFirstOrDefaultAsync(x => x.Id == Id);
                if (image != null)
                {
                    _propertyImageRepository.Remove(image);
                    await AzureBlobActions.DeleteBlob(containerClient, image.ImageUrl);
                }
            }
            property.Name = parameters.addPropertyDto.name;
            property.Description = parameters.addPropertyDto.description;
            property.City = parameters.addPropertyDto.city;
            property.State = parameters.addPropertyDto.state;
            property.StreetAddress = parameters.addPropertyDto.streetAddress;
            property.Price = parameters.addPropertyDto.price;
            property.BedRooms = parameters.addPropertyDto.bedRooms;
            property.BathRooms = parameters.addPropertyDto.bathRooms;
            property.Area = parameters.addPropertyDto.area;
            property.PropertyTypeId = parameters.addPropertyDto.propertyTypeId;
            if (parameters.addPropertyDto.Status != null)
            {
                property.Status = parameters.addPropertyDto.status;
            }
            if (parameters.addPropertyDto.coverImageUrl != null)
            {
                await AzureBlobActions.DeleteBlob(containerClient, property.CoverImageUrl);
                property.CoverImageUrl = parameters.addPropertyDto.coverImageUrl;
            }
            property.TransactionType = parameters.addPropertyDto.transactionType;
            if (parameters.addPropertyDto.userId != null)
            {
                property.UserId = parameters.addPropertyDto.userId;
            }
            _propertyRepository.Update(property);
            await _propertyRepository.SaveChangesAsync();
            _propertyRepository.Dispose();

            return Ok(property);
        }

        [HttpGet("GetProperties/{currentUserId}/{currentUserRole}")]
        public async Task<IActionResult> GetProperties(string currentUserId, string currentUserRole)
        {
            var propertiesList = new List<Property>();
            if (string.IsNullOrEmpty(currentUserId))
            {
                var properties = await _propertyRepository.GetAllAsync(includeProperties: "PropertyType");
                propertiesList = properties?.ToList();
                if (propertiesList != null)
                {
                    foreach (var property in propertiesList)
                    {
                        property.CoverImageBlobUrl = await AzureBlobActions.GetBlobUrl(containerClient, property.CoverImageUrl);
                    }
                    _propertyRepository.Dispose();
                    return Ok(propertiesList);
                }
                _propertyRepository.Dispose();
                return Ok(propertiesList);
            }
            if (currentUserRole == RoleConstants.Role_User_Comp)
            {
                var usersResponse = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/user/GetUsersByCompanyId/{currentUserId}");
                if (usersResponse.IsSuccessStatusCode)
                {
                    var users = await usersResponse.Content.ReadFromJsonAsync<List<UserDto>>();
                    if (users!.Any())
                    {
                        var properties = await _propertyRepository.GetAllAsync(x => users!.Any(y => x.UserId == y.Id), includeProperties: "PropertyType");
                        propertiesList = properties?.ToList();
                        if (propertiesList != null)
                        {
                            foreach (var property in propertiesList)
                            {
                                if (property.Status != PropertyStatus.Rented)
                                {
                                    property.ShowButtons = true;
                                }
                                property.CoverImageBlobUrl = await AzureBlobActions.GetBlobUrl(containerClient, property.CoverImageUrl);
                                foreach (var user in users!)
                                {
                                    if (property.UserId == user.Id)
                                    {
                                        property.User = user;
                                    }
                                }
                            }

                            properties = await _propertyRepository.GetAllAsync(x => x.UserId == currentUserId);
                            propertiesList = properties?.ToList();
                            //make api call to get the current user
                            var userResponse = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/user/GetUser/{currentUserId}");
                            if (userResponse.IsSuccessStatusCode && propertiesList != null)
                            {
                                var user = await userResponse.Content.ReadFromJsonAsync<UserDto>();
                                foreach (var property in propertiesList)
                                {
                                    property.User = user!;
                                }
                            }
                            _propertyRepository.Dispose();
                            return Ok(propertiesList);
                        }
                    }

                    return NoContent();
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                var properties = await _propertyRepository.GetAllAsync(x => x.UserId == currentUserId, includeProperties: "PropertyType");
                propertiesList = properties?.ToList();
                if (propertiesList != null)
                {
                    foreach (var property in propertiesList)
                    {
                        property.CoverImageBlobUrl = await AzureBlobActions.GetBlobUrl(containerClient, property.CoverImageUrl);
                    }
                    _propertyRepository.Dispose();
                    return Ok(propertiesList);
                }
                return NoContent();
            }
        }

        [HttpGet("GetProperty/{id}")]
        public async Task<IActionResult> GetProperty(int id)
        {
            var property = await _propertyRepository.GetFirstOrDefaultAsync(x => x.Id == id, includeProperties: "PropertyImages,PropertyType");
            if (property == null)
            {
                _propertyRepository.Dispose();
                return NotFound();
            }
            foreach (var image in property.PropertyImages)
            {
                string imageUrl = await AzureBlobActions.GetBlobUrl(containerClient, image.ImageUrl);
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    property.BlobUrls.Add(imageUrl);

                }
            }
            _propertyRepository.Dispose();
            return Ok(property);
        }

        [HttpGet("GetPropertiesForIndex/{currentUserId?}/{currentUserRole?}")]
        public async Task<IActionResult> GetPropertiesForIndex(string? currentUserId, string? currentUserRole)
        {
            var propertiesList = new List<Property>();
            if (currentUserRole == RoleConstants.Role_User_Indi)
            {
                var properties = await _propertyRepository.GetAllAsync(x => x.UserId != currentUserId && x.Status == PropertyStatus.Free, includeProperties: "PropertyType");
                propertiesList = properties.ToList();
                if (propertiesList != null)
                {
                    foreach (var property in propertiesList)
                    {
                        property.CoverImageBlobUrl = await AzureBlobActions.GetBlobUrl(containerClient, property.CoverImageUrl);
                    }
                    _propertyRepository.Dispose();
                    return Ok(propertiesList);
                }
            }
            else
            {
                var usersResponse = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/user/GetUsersByCompanyId/{currentUserId}");
                if (usersResponse.IsSuccessStatusCode)
                {
                    var users = await usersResponse.Content.ReadFromJsonAsync<List<UserDto>>();
                    var properties = await _propertyRepository.GetAllAsync(x => x.Status == PropertyStatus.Free && x.UserId != currentUserId, includeProperties: "PropertyType");
                    propertiesList = properties?.Where(x => users == null || !users.Any(y => x.UserId == y.Id)).ToList();
                    if (propertiesList != null)
                    {
                        foreach (var property in propertiesList)
                        {
                            property.CoverImageBlobUrl = await AzureBlobActions.GetBlobUrl(containerClient, property.CoverImageUrl);
                        }
                        _propertyRepository.Dispose();
                        return Ok(propertiesList);
                    }
                }
            }
            _propertyRepository.Dispose();
            return Ok(propertiesList);
        }

        [HttpGet("GetPropertiesForIndex")]
        public async Task<IActionResult> GetPropertiesForIndex()
        {
            var properties = await _propertyRepository.GetAllAsync(x => x.Status == PropertyStatus.Free, includeProperties: "PropertyType");
            if (properties != null)
            {
                foreach (var property in properties)
                {
                    property.CoverImageBlobUrl = await AzureBlobActions.GetBlobUrl(containerClient, property.CoverImageUrl);
                }
                _propertyRepository.Dispose();
                return Ok(properties);

            }
            _propertyRepository.Dispose();
            return NoContent();
        }

        [HttpDelete("DeleteProperty/{id}")]
        public async Task<IActionResult> DeleteProperty(int id)
        {
            var property = await _propertyRepository.GetFirstOrDefaultAsync(x => x.Id == id);
            if (property == null)
            {
                return NotFound();
            }
            var propertyImages = await _propertyImageRepository.GetAllAsync(x => x.PropertyId == id);
            _propertyRepository.Remove(property);
            await AzureBlobActions.DeleteBlob(containerClient, property.CoverImageUrl);
            _propertyImageRepository.RemoveRange(propertyImages);
            foreach (var image in propertyImages)
            {
                await AzureBlobActions.DeleteBlob(containerClient, image.ImageUrl);
            }
            await _propertyRepository.SaveChangesAsync();
            _propertyRepository.Dispose();
            //make api call to delete transactions related to this property
            await _httpClient.DeleteAsync($"{APIGatewayUrl.URL}api/transaction/DeleteTransactionByPropertyId/{property.Id}");

            return Ok("Property Deleted Successfully");
        }

        [HttpPost("UpdatePropertyStatus")]
        public async Task<IActionResult> UpdatePropertyStatus([FromBody] dynamic parameters)
        {
            int propertyId = parameters.propertyId;
            string status = parameters.status;
            var property = await _propertyRepository.GetFirstOrDefaultAsync(x => x.Id == propertyId);
            if (property == null)
            {
                return NotFound();
            }
            _propertyRepository.UpdateStatus(property, status);
            await _propertyRepository.SaveChangesAsync();
            _propertyRepository.Dispose();
            return Ok();
        }

        [HttpPost("UpdatePropertyOwner")]
        public async Task<IActionResult> UpdatePropertyOwner([FromBody] dynamic parameters)
        {
            int propertyId = parameters.propertyId;
            string userId = parameters.userId;
            var property = await _propertyRepository.GetFirstOrDefaultAsync(x => x.Id == propertyId);
            if (property == null)
            {
                return NotFound();
            }
            _propertyRepository.UpdateOwner(property, userId);
            await _propertyRepository.SaveChangesAsync();
            _propertyRepository.Dispose();
            return Ok();
        }

        [HttpDelete("DeletePropertiesByUserId/{userId}")]
        public async Task<IActionResult> DeletePropertiesByUserId(string userId)
        {
            if (userId == null)
                return NotFound();
            var properties = await _propertyRepository.GetAllAsync(x => x.UserId == userId);

            if (properties != null)
            {
                foreach (var property in properties)
                {
                    var propertyImages = await _propertyImageRepository.GetAllAsync(x => x.PropertyId
                    == property.Id);
                    foreach (var image in propertyImages)
                    {
                        await AzureBlobActions.DeleteBlob(containerClient, image.ImageUrl);
                    }
                    _propertyImageRepository.RemoveRange(propertyImages);
                }

                _propertyRepository.RemoveRange(properties);
            }
            await _propertyRepository.SaveChangesAsync();
            _propertyRepository.Dispose();
            return Ok();
        }
    }
}