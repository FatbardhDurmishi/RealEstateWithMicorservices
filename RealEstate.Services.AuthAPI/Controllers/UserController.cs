using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Services.AuthAPI.Constants;
using RealEstate.Services.AuthAPI.Models;
using RealEstate.Services.AuthAPI.Models.Dto;
using RealEstate.Services.AuthAPI.Repositories.IRepository;
using System.ComponentModel.Design;
using System.Data;
using System.IdentityModel.Tokens.Jwt;

namespace RealEstate.Services.AuthAPI.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly HttpClient _httpClient;

        public UserController(IUserRepository userRepository, UserManager<ApplicationUser> userManager, HttpClient httpClient)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _httpClient = httpClient;
        }

        [HttpGet("GetUsers/{currentUserId}/{currentUserRole}")]
        public async Task<ActionResult<UserDto>> GetUsers(string currentUserId, string currentUserRole)
        {
            if (currentUserRole == RoleConstants.Role_User_Comp)
            {
                var usersForCompany = await _userRepository.GetFirstOrDefaultAsync(x => x.Id == currentUserId);
                _userRepository.Dispose();
                return Ok(usersForCompany);
            }

            var usersForAdmin = await _userRepository.GetAllAsync(x => x.Id != currentUserId);
            _userRepository.Dispose();
            return Ok(usersForAdmin);
        }

        [HttpGet("GetUsersByCompanyId/{companyId}")]
        public async Task<ActionResult<UserDto>> GetUsersByCompanyId(string companyId)
        {
            var users = await _userRepository.GetAllAsync(x => x.CompanyId == companyId);
            _userRepository.Dispose();
            return Ok(users);
        }

        [HttpPost("CreateUser")]
        public async Task<ActionResult> CreateUser([FromBody] CreateUserDto parameters)
        {
            var user = new ApplicationUser()
            {
                Name = parameters.User.Name,
                UserName = parameters.User.Email,
                Email = parameters.User.Email,
                StreetAddres = parameters.User.StreetAddres!,
                City = parameters.User.City!,
                State = parameters.User.State!,
                PostalCode = parameters.User.PostalCode!,
                PhoneNumber = parameters.User.PhoneNumber!,
            };
            if (parameters.CurrentUserRole == RoleConstants.Role_User_Comp)
            {
                user.CompanyId = parameters.CurrentUserId;
                user.Role = RoleConstants.Role_User_Indi;
            }
            else
            {
                user.Role = parameters.User.Role!;
            }
            var result = await _userManager.CreateAsync(user, parameters.User.Password);
            if (!result.Succeeded)
            {
                return BadRequest();
            }
            await _userManager.AddToRoleAsync(user, user.Role);
            return Ok(user);
        }

        [HttpDelete("DeleteUser/{id}")]
        public async Task<ActionResult<UserDto>> DeleteUser(string id)
        {
            var user = await _userRepository.GetFirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            //make a call to propertyService to delete all of user properties
            var deletePropertiesResponse = await _httpClient.DeleteAsync($"{APIGatewayUrl.URL}api/property/DeletePropertiesByUserId/{user.Id}");
            if (deletePropertiesResponse.IsSuccessStatusCode)
            {
                _userRepository.Remove(user);
                await _userRepository.SaveChangesAsync();
                _userRepository.Dispose();
                return Ok();
            }
            return BadRequest();
        }

        [HttpGet("GetUser/{id}")]
        public async Task<ActionResult<UserDto>> GetUser(string id)
        {
            var user = await _userRepository.GetFirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            _userRepository.Dispose();
            return Ok(user);
        }

        [HttpPut("UpdateUser")]
        public async Task<ActionResult<RegisterDto>> UpdateUser(RegisterDto registerDto)
        {
            var user = await _userRepository.GetFirstOrDefaultAsync(x => x.Id == registerDto.Id);
            if (user == null)
            {
                return NotFound();
            }

            user.Name = registerDto.Name;
            user.Email = registerDto.Email;
            user.StreetAddres = registerDto.StreetAddres!;
            user.City = registerDto.City!;
            user.State = registerDto.State!;
            user.PostalCode = registerDto.PostalCode!;
            user.PhoneNumber = registerDto.PhoneNumber!;
            user.Role = registerDto.Role!;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                if (user.Role != registerDto.Role)
                {

                    var roleResult = await _userManager.RemoveFromRoleAsync(user, registerDto.Role!);
                    if (roleResult.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, user.Role);
                    }
                }
                await _userRepository.SaveChangesAsync();
                _userRepository.Dispose();
                return Ok(user);
            }
            return BadRequest();

        }
    }
}