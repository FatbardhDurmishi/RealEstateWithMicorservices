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

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            ApplicationUser user = new()
            {
                Name = registerDto.Name,
                Email = registerDto.Email,
                UserName = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
                StreetAddres = registerDto.StreetAddres,
                City = registerDto.City,
                State = registerDto.State,
                PostalCode = registerDto.PostalCode,
            };
            if (registerDto.Role == "false")
            {
                user.Role = RoleConstants.Role_User_Indi;
            }
            else
            {
                user.Role = registerDto.Role;
            }

            try
            {
                if (!_roleManager.RoleExistsAsync(RoleConstants.Role_Admin).GetAwaiter().GetResult())
                {
                    _roleManager.CreateAsync(new IdentityRole(RoleConstants.Role_Admin)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new IdentityRole(RoleConstants.Role_User_Indi)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new IdentityRole(RoleConstants.Role_User_Comp)).GetAwaiter().GetResult();
                }
                var result = await _userManager.CreateAsync(user, registerDto.Password);
                await _userManager.AddToRoleAsync(user, user.Role);
                if (result.Succeeded)
                {
                    var userToReturn = _context.ApplicationUsers.First(x => x.Email == registerDto.Email);

                    UserDto userDto = new()
                    {
                        Name = userToReturn.Name,
                        Email = userToReturn.Email,
                        PhoneNumber = userToReturn.PhoneNumber,
                        StreetAddres = userToReturn.StreetAddres,
                        City = userToReturn.City,
                        State = userToReturn.State,
                        PostalCode = userToReturn.PostalCode,
                        Role = userToReturn.Role,
                    };

                    return Ok(result);
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
            catch (Exception ex)
            {
            }
            return BadRequest();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var users = _context.ApplicationUsers.ToList();
            if (users.Count() < 3)
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
                    await _userManager.AddToRoleAsync(adminUser, RoleConstants.Role_Admin);
                }
            }

            var user = _context.ApplicationUsers.FirstOrDefault(x => x.Email.ToLower() == loginDto.Email.ToLower());

            bool isValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if (user == null || isValid == false)
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
    }
}