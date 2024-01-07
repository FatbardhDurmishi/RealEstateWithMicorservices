using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Services.AuthAPI.Constants;
using RealEstate.Services.AuthAPI.Data;
using RealEstate.Services.AuthAPI.Models;
using RealEstate.Services.AuthAPI.Models.Dto;
using RealEstate.Services.AuthAPI.Repositories.IRepository;
using RealEstate.Services.AuthAPI.Service.IService;

namespace RealEstate.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IUserRepository _userRepository;

        public AuthAPIController(AppDbContext context, UserManager<ApplicationUser> userManager, IJwtTokenGenerator jwtTokenGenerator, RoleManager<IdentityRole> roleManager, IUserRepository userRepository)
        {
            _context = context;
            _userManager = userManager;
            _jwtTokenGenerator = jwtTokenGenerator;
            _roleManager = roleManager;
            _userRepository = userRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            ApplicationUser user = new()
            {
                Name = registerDto.Name,
                Email = registerDto.Email,
                UserName = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber!,
                StreetAddres = registerDto.StreetAddres!,
                City = registerDto.City!,
                State = registerDto.State!,
                PostalCode = registerDto.PostalCode!,
            };
            if (registerDto.Role == "false")
            {
                user.Role = RoleConstants.Role_User_Indi;
            }
            else
            {
                user.Role = registerDto.Role!;
            }

            if (!_roleManager.RoleExistsAsync(RoleConstants.Role_Admin).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(RoleConstants.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(RoleConstants.Role_User_Indi)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(RoleConstants.Role_User_Comp)).GetAwaiter().GetResult();
            }
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, user.Role);

                return Ok(result);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            //var users = _context.ApplicationUsers.ToList();
            var admin = await _userRepository.GetFirstOrDefault(x => x.Role == RoleConstants.Role_Admin);
            //var userAdmin = _context.ApplicationUsers.Where();
            if (admin == null)
            {
                var adminUser = new ApplicationUser { UserName = "admin@gmail.com", Email = "admin@gmail.com", EmailConfirmed = true };

                adminUser.StreetAddres = "Admin";
                adminUser.City = "Admin";
                adminUser.State = "Admin";
                adminUser.PostalCode = "Admin";
                adminUser.Name = "Admin";
                adminUser.PhoneNumber = "Admin";
                adminUser.Role = "Admin";

                var result = await _userManager.CreateAsync(adminUser, "Admin.123");

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, adminUser.Role);
                }
            }

            var user = _context.ApplicationUsers.FirstOrDefault(x => x.Email.ToLower() == loginDto.Email.ToLower());
            if (user == null)
            {
                return BadRequest();
            }

            bool isValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if (isValid == false)
            {
                return BadRequest();
            }

            var token = _jwtTokenGenerator.GenerateToken(user);

            UserDto userDto = new()
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                StreetAddres = user.StreetAddres,
                City = user.City,
                State = user.State,
                PostalCode = user.PostalCode,
                Role = user.Role,
            };

            LoginResponseDto loginResponseDto = new LoginResponseDto()
            {
                User = userDto,
                Token = token
            };

            return Ok(loginResponseDto);
        }

        [HttpPut("updateUserData")]
        public async Task<IActionResult> UpdateUserData(RegisterDto registerDto)
        {
            if (registerDto == null)
            {
                return BadRequest();
            }
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);

            existingUser.Name = registerDto.Name;
            existingUser.Email = registerDto.Email;
            existingUser.UserName = registerDto.Email;
            existingUser.PhoneNumber = registerDto.PhoneNumber!;
            existingUser.StreetAddres = registerDto.StreetAddres!;
            existingUser.City = registerDto.City!;
            existingUser.State = registerDto.State!;
            existingUser.PostalCode = registerDto.PostalCode!;

            var updateUserDataResult = await _userManager.UpdateAsync(existingUser);
            if (updateUserDataResult.Succeeded)
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpPut("changePassword")]
        public async Task<IActionResult> ChangePassword(RegisterDto registerDto)
        {
            if (registerDto == null)
            {
                return BadRequest();
            }

            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser == null)
            {
                return BadRequest();
            }
            var isOldPassowrdCorrect = await _userManager.CheckPasswordAsync(existingUser, registerDto.OldPassword);
            if (!isOldPassowrdCorrect)
            {
                return BadRequest("Old password is incorrect");
            }
            var changePasswordResult = await _userManager.ChangePasswordAsync(existingUser, registerDto.OldPassword, registerDto.Password);
            return changePasswordResult.Succeeded ? Ok() : BadRequest();
        }
    }
}