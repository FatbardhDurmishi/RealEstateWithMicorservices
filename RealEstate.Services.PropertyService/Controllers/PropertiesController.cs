using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Services.PropertyService.Constants;
using RealEstate.Services.PropertyService.Models;
using RealEstate.Services.PropertyService.Models.Dtos;
using RealEstate.Services.PropertyService.Repositories.IRepositories;

namespace RealEstate.Services.PropertyService.Controllers
{
    [Route("api/property")]
    [ApiController]
    public class PropertiesController : ControllerBase
    {
        private readonly BlobServiceClient blobServiceClient;
        private readonly BlobContainerClient containerClient;
        private readonly string accountName = "riinvestdetyra";
        private readonly string accountKey = "ar97PiHstwAOJfq9Op8Op7b1jrmVVniJc0xvUoGnfsQAK9dBBoWm5MnM2o8jRYSJQ7b//JC8oGFv+AStXgiw5A==";
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
            containerClient = blobServiceClient.GetBlobContainerClient("pictures");
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
            await _propertyRepository.Add(property);
            return Ok(property);
        }

        [HttpPost("UploadImages/{propertyId}")]
        public async Task<IActionResult> UploadImages([FromForm(Name = "CoverImage")] IFormFile CoverImage, [FromForm(Name = "PropertyImages")] IFormFileCollection PropertyImages, int propertyId)
        {
            var property = await _propertyRepository.GetFirstOrDefault(x => x.Id == propertyId);
            if (property == null)
            {
                return BadRequest();
            }
            if (CoverImage != null)
            {
                bool result = await UploadToBlob(containerClient, CoverImage);
                if (!result)
                {
                    return BadRequest(result);
                }
                property.CoverImageUrl = CoverImage.FileName;
                await _propertyRepository.Update(property);


            }
            foreach (var image in PropertyImages)
            {
                var Image = new PropertyImage
                {
                    ImageUrl = image.FileName,
                    PropertyId = propertyId
                };
                if (image.FileName != CoverImage.FileName)
                {
                    bool result = await UploadToBlob(containerClient, image);
                    if (result)
                    {
                        await _propertyImageRepository.Add(Image);
                    }
                }
                else
                {
                    await _propertyImageRepository.Add(Image);
                }


            }
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
            var property = await _propertyRepository.GetFirstOrDefault(x => x.Id == propertyId);
            if (property == null)
            {
                return NotFound();
            }
            foreach (var imageId in parameters.imagesToDelete)
            {
                int Id = imageId;
                var image = await _propertyImageRepository.GetFirstOrDefault(x => x.Id == Id);
                if (image != null)
                {
                    await _propertyImageRepository.Remove(image);
                    await DeleteBlob(containerClient, image.ImageUrl);
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
                await DeleteBlob(containerClient, property.CoverImageUrl);
                property.CoverImageUrl = parameters.addPropertyDto.coverImageUrl;
            }
            property.TransactionType = parameters.addPropertyDto.transactionType;
            if (parameters.addPropertyDto.userId != null)
            {
                property.UserId = parameters.addPropertyDto.userId;
            }
            await _propertyRepository.Update(property);
            return Ok(property);
        }

        [HttpGet("GetProperties/{currentUserId}/{currentUserRole}")]
        public async Task<IActionResult> GetProperties(string currentUserId, string currentUserRole)
        {
            var propertiesList = new List<Property>();
            if (string.IsNullOrEmpty(currentUserId))
            {
                var properties = await _propertyRepository.GetAll(includeProperties: "PropertyType");
                propertiesList = properties?.ToList();
                if (propertiesList != null)
                {
                    foreach (var property in propertiesList)
                    {
                        property.CoverImageBlobUrl = await GetBlobUrl(containerClient, property.CoverImageUrl);
                    }
                    return Ok(propertiesList);
                }
                return Ok(propertiesList);
            }
            if (currentUserRole == RoleConstants.Role_User_Comp)
            {
                var usersResponse = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/user/GetUsersByCompanyId/{currentUserId}");
                if (usersResponse.IsSuccessStatusCode)
                {
                    var users = await usersResponse.Content.ReadFromJsonAsync<List<UserDto>>();
                    if (users.Any())
                    {
                        var properties = await _propertyRepository.GetAll(x => users.Any(y => x.UserId == y.Id), includeProperties: "PropertyType");
                        propertiesList = properties?.ToList();
                        if (propertiesList != null)
                        {
                            foreach (var property in propertiesList)
                            {
                                if (property.Status != PropertyStatus.Rented)
                                {
                                    property.ShowButtons = true;
                                }
                                property.CoverImageBlobUrl = await GetBlobUrl(containerClient, property.CoverImageUrl);
                                foreach (var user in users)
                                {
                                    if (property.UserId == user.Id)
                                    {
                                        property.User = user;
                                    }
                                }
                            }

                            properties = await _propertyRepository.GetAll(x => x.UserId == currentUserId);
                            propertiesList = properties?.ToList();
                            //make api call to get the current user
                            var userResponse = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/user/GetUser/{currentUserId}");
                            if (userResponse.IsSuccessStatusCode && propertiesList != null)
                            {
                                var user = await userResponse.Content.ReadFromJsonAsync<UserDto>();
                                foreach (var property in propertiesList)
                                {
                                    property.User = user;
                                }
                            }

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
                var properties = await _propertyRepository.GetAll(x => x.UserId == currentUserId, includeProperties: "PropertyType");
                propertiesList = properties?.ToList();
                if (propertiesList != null)
                {
                    foreach (var property in propertiesList)
                    {
                        property.CoverImageBlobUrl = await GetBlobUrl(containerClient, property.CoverImageUrl);
                    }
                    return Ok(propertiesList);
                }
                return NoContent();
            }
        }

        [HttpGet("GetProperty/{id}")]
        public async Task<IActionResult> GetProperty(int id)
        {
            var property = await _propertyRepository.GetFirstOrDefault(x => x.Id == id, includeProperties: "PropertyImages,PropertyType");
            if (property == null)
            {
                return NotFound();
            }
            foreach (var image in property.PropertyImages)
            {
                string imageUrl = await GetBlobUrl(containerClient, image.ImageUrl);
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    property.BlobUrls.Add(imageUrl);

                }
            }
            return Ok(property);
        }

        [HttpGet("GetPropertiesForIndex/{currentUserId?}/{currentUserRole?}")]
        public async Task<IActionResult> GetPropertiesForIndex(string? currentUserId, string? currentUserRole)
        {
            var propertiesList = new List<Property>();
            if (currentUserRole == RoleConstants.Role_User_Indi)
            {
                var properties = await _propertyRepository.GetAll(x => x.UserId != currentUserId && x.Status == PropertyStatus.Free, includeProperties: "PropertyType");
                propertiesList = properties.ToList();
                if (propertiesList != null)
                {
                    foreach (var property in propertiesList)
                    {
                        property.CoverImageBlobUrl = await GetBlobUrl(containerClient, property.CoverImageUrl);
                    }
                    return Ok(propertiesList);
                }


            }
            else
            {
                var usersResponse = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/user/GetUsersByCompanyId/{currentUserId}");
                if (usersResponse.IsSuccessStatusCode)
                {
                    var users = await usersResponse.Content.ReadFromJsonAsync<List<UserDto>>();
                    var properties = await _propertyRepository.GetAll(x => x.Status == PropertyStatus.Free && x.UserId != currentUserId, includeProperties: "PropertyType");
                    propertiesList = properties?.Where(x => users.Any(y => x.UserId != y.Id)).ToList();
                    if (propertiesList != null)
                    {
                        foreach (var property in propertiesList)
                        {
                            property.CoverImageBlobUrl = await GetBlobUrl(containerClient, property.CoverImageUrl);
                        }
                        return Ok(propertiesList);
                    }
                }
            }

            return Ok(propertiesList);
        }

        [HttpGet("GetPropertiesForIndex")]
        public async Task<IActionResult> GetPropertiesForIndex()
        {
            var properties = await _propertyRepository.GetAll(includeProperties: "PropertyType");
            if (properties != null)
            {
                foreach (var property in properties)
                {
                    property.CoverImageBlobUrl = await GetBlobUrl(containerClient, property.CoverImageUrl);
                }
                return Ok(properties);

            }
            return NoContent();
        }

        [HttpDelete("DeleteProperty/{id}")]
        public async Task<IActionResult> DeleteProperty(int id)
        {
            var property = await _propertyRepository.GetFirstOrDefault(x => x.Id == id);
            if (property == null)
            {
                return NotFound();
            }
            var propertyImages = await _propertyImageRepository.GetAll(x => x.PropertyId == id);
            await _propertyRepository.Remove(property);
            await DeleteBlob(containerClient, property.CoverImageUrl);
            await _propertyImageRepository.RemoveRange(propertyImages);
            foreach (var image in propertyImages)
            {
                await DeleteBlob(containerClient, image.ImageUrl);
            }
            //make api call to delete transactions related to this property
            await _httpClient.DeleteAsync($"{APIGatewayUrl.URL}api/transaction/DeleteTransactionByPropertyId/{property.Id}");

            return Ok("Property Deleted Successfully");
        }

        [HttpPost("UpdatePropertyStatus")]
        public async Task<IActionResult> UpdatePropertyStatus([FromBody] dynamic parameters)
        {
            int propertyId = parameters.propertyId;
            string status = parameters.status;
            var property = await _propertyRepository.GetFirstOrDefault(x => x.Id == propertyId);
            if (property == null)
            {
                return NotFound();
            }
            _propertyRepository.UpdateStatus(property, status);
            await _propertyRepository.SaveChanges();
            return Ok();
        }

        [HttpPost("UpdatePropertyOwner")]
        public async Task<IActionResult> UpdatePropertyOwner([FromBody] dynamic parameters)
        {
            int propertyId = parameters.propertyId;
            string userId = parameters.userId;
            var property = await _propertyRepository.GetFirstOrDefault(x => x.Id == propertyId);
            if (property == null)
            {
                return NotFound();
            }
            _propertyRepository.UpdateOwner(property, userId);
            await _propertyRepository.SaveChanges();
            return Ok();
        }

        [HttpDelete("DeletePropertiesByUserId/{userId}")]
        public async Task<IActionResult> DeletePropertiesByUserId(string userId)
        {
            if (userId == null)
                return NotFound();
            var properties = await _propertyRepository.GetAll(x => x.UserId == userId);

            if (properties != null)
            {
                foreach (var property in properties)
                {
                    var propertyImages = await _propertyImageRepository.GetAll(x => x.PropertyId
                    == property.Id);
                    foreach (var image in propertyImages)
                    {
                        await DeleteBlob(containerClient, image.ImageUrl);
                    }
                    await _propertyImageRepository.RemoveRange(propertyImages);
                }

                await _propertyRepository.RemoveRange(properties);
            }

            return Ok();
        }



        [ApiExplorerSettings(IgnoreApi = true)]
        private async Task<bool> UploadToBlob(BlobContainerClient containerClient, IFormFile file)
        {
            try
            {
                var blobClient = containerClient.GetBlobClient(file.FileName);
                var blobExists = await blobClient.ExistsAsync();

                if (blobExists)
                {
                    return true;
                }

                using (var inputStream = file.OpenReadStream())
                {
                    await containerClient.UploadBlobAsync(file.FileName, inputStream);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        private static async Task DeleteBlob(BlobContainerClient containerClient, string blobName)
        {
            var blobClient = containerClient.GetBlobClient(blobName);
            var blobExists = await blobClient.ExistsAsync();
            if (blobExists)
            {
                await containerClient.DeleteBlobAsync(blobName);
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        private static async Task<string> GetBlobUrl(BlobContainerClient containerClient, string blobName)
        {
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            var blobExists = await blobClient.ExistsAsync();
            if (!blobExists)
            {
                return string.Empty;
            }
            DateTimeOffset expiresOn = DateTimeOffset.UtcNow.AddHours(1);

            Uri blobSasUri = blobClient.GenerateSasUri(BlobSasPermissions.Read, expiresOn);

            string blobUrl = blobSasUri.ToString();
            return blobUrl;
        }
    }
}