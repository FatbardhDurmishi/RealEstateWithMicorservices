using Azure.Identity;
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
        private readonly string accoutName = "riinvestdetyra";
        private readonly string accountKey = "ar97PiHstwAOJfq9Op8Op7b1jrmVVniJc0xvUoGnfsQAK9dBBoWm5MnM2o8jRYSJQ7b//JC8oGFv+AStXgiw5A==";
        private readonly StorageSharedKeyCredential credential;
        private readonly IPropertyRepository _propertyRepository;
        private readonly HttpClient _httpClient;
        private readonly IPropertyImageRepository _propertyImageRepository;

        public PropertiesController(IPropertyRepository propertyRepository, HttpClient httpClient, IPropertyImageRepository propertyImageRepository)
        {
            credential = new StorageSharedKeyCredential(accoutName, accountKey);
            blobServiceClient = new BlobServiceClient(
    new Uri("https://riinvestdetyra.blob.core.windows.net"), new DefaultAzureCredential());

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
            return Ok();
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
                property.CoverImageUrl = CoverImage.FileName;
                await _propertyRepository.Update(property);
                await UploadToBlob(containerClient, CoverImage);
            }
            foreach (var image in PropertyImages)
            {
                var Image = new PropertyImage
                {
                    ImageUrl = image.FileName,
                    PropertyId = propertyId
                };
                await _propertyImageRepository.Add(Image);
                await UploadToBlob(containerClient, image);
            }
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProperty([FromBody] dynamic parameters)
        {
            if (parameters.AddPropertyDto == null)
            {
                return BadRequest();
            }
            int propertyId = parameters.AddPropertyDto.PropertyId;
            var property = await _propertyRepository.GetFirstOrDefault(x => x.Id == propertyId);
            if (property == null)
            {
                return NotFound();
            }
            if (parameters.AddPropertyDto.CoverImage != null)
            {
                property.CoverImageUrl = parameters.AddPropertyDto.CoverImage.FileName;
                await UploadToBlob(containerClient, parameters.AddPropertyDto.CoverImage);
            }
            if (parameters.AddPropertyDto.PropertyImages != null)
            {
                foreach (var image in parameters.AddPropertyDto.PropertyImages)
                {
                    var Image = new PropertyImage
                    {
                        ImageUrl = image.FileName
                    };
                    property.PropertyImages.Add(Image);
                    await UploadToBlob(containerClient, image);
                }
            }
            if (parameters.ImagesToDelete != null)
            {
                foreach (var imageId in parameters.ImagesToDelete)
                {
                    int Id = imageId;
                    var image = await _propertyImageRepository.GetFirstOrDefault(x => x.Id == Id);
                    if (image != null)
                    {
                        await _propertyImageRepository.Remove(image);
                        await DeleteBlob(containerClient, image.ImageUrl);
                    }
                }
            }
            property.Name = parameters.AddPropertyDto.Property.Name;
            property.Description = parameters.AddPropertyDto.Property.Description;
            property.City = parameters.AddPropertyDto.Property.City;
            property.State = parameters.AddPropertyDto.Property.State;
            property.StreetAddress = parameters.AddPropertyDto.Property.StreetAddress;
            property.Price = parameters.AddPropertyDto.Property.Price;
            property.BedRooms = parameters.AddPropertyDto.Property.BedRooms;
            property.BathRooms = parameters.AddPropertyDto.Property.BathRooms;
            property.Area = parameters.AddPropertyDto.Property.Area;
            property.PropertyTypeId = parameters.AddPropertyDto.Property.PropertyTypeId;
            property.Status = parameters.AddPropertyDto.Property.Status;
            property.TransactionType = parameters.AddPropertyDto.Property.TransactionType;
            property.UserId = parameters.AddPropertyDto.Property.UserId;
            await _propertyRepository.Update(property);
            return Ok();
        }

        [HttpGet("GetProperties")]
        public async Task<IActionResult> GetProperties(string? currentUserId, string? currentUserRole)
        {
            if (string.IsNullOrEmpty(currentUserId))
            {
                var properties = await _propertyRepository.GetAll(includeProperties: "PropertyImages,PropertyType");
                foreach (var property in properties)
                {
                    property.CoverImageBlobUrl = await GetBlobUrl(containerClient, property.CoverImageUrl);
                }
                if (properties != null)
                {
                    return Ok(properties);
                }
                return NoContent();
            }
            if (currentUserRole == RoleConstants.Role_User_Comp)
            {
                var response = await _httpClient.GetAsync($"{APIBaseUrls.AuthAPIBaseUrl}api/user/GetUsersByCompanyId/{currentUserId}");
                if (response.IsSuccessStatusCode)
                {
                    var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();
                    var properties = await _propertyRepository.GetAll(x => users.Any(y => x.UserId == y.Id), includeProperties: "PropertyType");
                    if (properties != null)
                    {
                        foreach (var property in properties)
                        {
                            property.CoverImageBlobUrl = await GetBlobUrl(containerClient, property.CoverImageUrl);
                            foreach (var user in users)
                            {
                                if (property.UserId == user.Id)
                                {
                                    property.User = user;
                                }
                            }
                        }
                        return Ok(properties);
                    }
                    return NoContent();
                }
                else
                {
                    return BadRequest(response);
                }
            }
            else
            {
                var response = await _httpClient.GetAsync($"{APIBaseUrls.AuthAPIBaseUrl}/api/user/GetUsers");
                if (response.IsSuccessStatusCode)
                {
                    var users = response.Content.ReadFromJsonAsync<List<UserDto>>();
                    var properties = await _propertyRepository.GetAll(x => x.UserId == currentUserId, includeProperties: "PropertyType");
                    if (properties != null)
                    {
                        foreach (var property in properties)
                        {
                            property.CoverImageBlobUrl = await GetBlobUrl(containerClient, property.CoverImageUrl);
                            if (users.Result.Any(x => x.Id == property.UserId))
                            {
                                property.User = users.Result.FirstOrDefault(x => x.Id == property.UserId);
                            }
                        }
                        return Ok(properties);
                    }
                    return NoContent();
                }
                else
                {
                    return BadRequest(response);
                }
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProperty(int id)
        {
            var property = await _propertyRepository.GetFirstOrDefault(x => x.Id == id, includeProperties: "PropertyImages,PropertyType");
            if (property == null)
            {
                return NotFound();
            }
            foreach (var image in property.PropertyImages)
            {
                property.BlobUrls.Add(await GetBlobUrl(containerClient, image.ImageUrl));
            }
            return Ok(property);
        }

        [HttpDelete("{id}")]
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
            await _httpClient.DeleteAsync($"{APIBaseUrls.TransactionAPIBaseUrl}api/transaction/deleteByPropertyId/{property.Id}");

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
            return Ok();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        private static async Task UploadToBlob(BlobContainerClient containerClient, IFormFile file)
        {
            // Get a reference to a blob
            BlobClient blobClient = containerClient.GetBlobClient(file.FileName);
            // Open the file and upload its data
            var InputStream = file.OpenReadStream();
            await containerClient.UploadBlobAsync(file.FileName, InputStream);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        private static async Task DeleteBlob(BlobContainerClient containerClient, string blobName)
        {
            await containerClient.DeleteBlobAsync(blobName);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        private static async Task<string> GetBlobUrl(BlobContainerClient containerClient, string blobName)
        {
            string accountName = "riinvestdetyra";
            string accountKey = "ar97PiHstwAOJfq9Op8Op7b1jrmVVniJc0xvUoGnfsQAK9dBBoWm5MnM2o8jRYSJQ7b//JC8oGFv+AStXgiw5A==";
            StorageSharedKeyCredential credential = new StorageSharedKeyCredential(accountName, accountKey);
            //UserDelegationKey key = await blobServiceClient.GetUserDelegationKeyAsync(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddDays(7),CancellationToken.None);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            // Create a SAS token that's valid for one hour.
            BlobSasBuilder sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = containerClient.Name,
                BlobName = blobName,
                Resource = "b",
                StartsOn = DateTimeOffset.UtcNow,
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
            };
            sasBuilder.SetPermissions(BlobSasPermissions.Read);
            BlobUriBuilder blobUriBuilder = new BlobUriBuilder(blobClient.Uri)
            {
                // Specify the user delegation key.
                Sas = sasBuilder.ToSasQueryParameters(credential)
            };
            //blobUrl = containerClient.GenerateSasUri(sasBuilder).ToString();
            return blobUriBuilder.ToString();
        }
    }
}