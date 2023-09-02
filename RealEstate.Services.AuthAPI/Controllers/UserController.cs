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


        public UserController(IUserRepository userRepository, UserManager<ApplicationUser> userManager)
        {
            _userRepository = userRepository;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<UserDto>> GetUsers(string? currentUserId)
        {
            var users = await _userRepository.GetAll();
            return Ok(users);
        }

        [HttpGet("{companyId}")]
        public async Task<ActionResult<UserDto>> GetUsersByCompanyId(string companyId)
        {
            var users = await _userRepository.GetAll(x=>x.CompanyId==companyId);
            return Ok(users);
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] dynamic parameters)
        {
            if(parameters.User == null)
            {
                return BadRequest();
            }


            var user = new ApplicationUser
            {
                Name = parameters.User.Name,
                Email = parameters.User.Email,
                StreetAddres = parameters.User.StreetAddres,
                City = parameters.User.City,
                State = parameters.User.State,
                PostalCode = parameters.User.PostalCode,
                PhoneNumber = parameters.User.PhoneNumber,
            };
            if (parameters.CurrentUserRole == RoleConstants.Role_User_Comp)
            {
                user.CompanyId = parameters.CurrentUserId;
                user.Role = RoleConstants.Role_User_Indi;
            }
            else
            {
                user.Role = parameters.User.Role;
            }
            var result= await _userManager.CreateAsync(user, parameters.User.Password);
            if(!result.Succeeded)
            {
                return BadRequest();
            }
            await _userManager.AddToRoleAsync(user, user.Role);
            return Ok(user);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<UserDto>> DeleteUser(string id)
        {
            var user = await _userRepository.GetFirstOrDefault(x=>x.Id==id);
            if(user == null)
            {
                return NotFound();
            }
            await _userRepository.Remove(user);
            return Ok(user);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(string id)
        {
            var user = await _userRepository.GetFirstOrDefault(x => x.Id == id);
            if(user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UserDto>> UpdateUser(string id, UserDto userDto)
        {

            var user = await _userRepository.GetFirstOrDefault(x => x.Id == id);
            if(user == null)
            {
                return NotFound();
            }
            user.Name = userDto.Name;
            user.Email = userDto.Email;
            user.StreetAddres = userDto.StreetAddres;
            user.City = userDto.City;
            user.State = userDto.State;
            user.PostalCode = userDto.PostalCode;
            user.PhoneNumber = userDto.PhoneNumber;
            user.Role = userDto.Role;
            await _userRepository.Update(user);
            return Ok(user);
        }

    }
}
