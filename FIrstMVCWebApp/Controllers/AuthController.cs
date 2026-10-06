using FIrstMVCWebApp.Data;
using FIrstMVCWebApp.Dto;
using FIrstMVCWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace FIrstMVCWebApp.Controllers
{
    public class AuthController(AppDbContext _context, IConfiguration _configuration) : Controller // for API based controller it must extend the ControllerBase class...
        // Note: for MVC controller, use the same name of folder as controller in views folder ... ex: View -> Auth
    {

        // above is primary constructor we can make it here also...but primry constructor is easy to do..
        public IActionResult Login() // here Login beacuase, the conroller goes to Login.cshtml page and shows it...
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        //public async Task<IActionResult> CreateUser(UserDto dto)
        //{
        //    //return View(dto);
        //    var existringUser = await 
        //}

        public async Task<IActionResult> RegisterUser(UserDto dto)
        {
            if (dto == null)
            {
                ViewBag.Message = "Please provide email and password.";
                return View("Login");
            }
            if (dto.Email == null || dto.Password == null)
            {
                ViewBag.Message = "Email and password are required.";
                return View("Login");
            }

            var data = await _context.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);

            if (data != null)
            {
                ViewBag.Message = "Email already exists.";
                // return View("Registration");
                return View("Register");
            }

            _context.Users.Add(new User
            {
                Username = dto.Username,
                Password = dto.Password,
                Email = dto.Email
            });

            await _context.SaveChangesAsync();

            return RedirectToAction("Login");

        }

        // public IActionResult LoginUser(UserDto dto)
        public async Task<IActionResult> LoginUser(UserDto dto)
        {
           if (dto == null)
           {
               ViewBag.Message = "Please provide email and password.";
               return View("Login");
           }
           if (dto.Email == null || dto.Password == null)
           {
               ViewBag.Message = "Email and password are required.";
               return View("Login");
           }

           var isExist = await _context.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);

           if (isExist == null)
           {
               ViewBag.Message = "Email does not exist.";
               return View("Login");
           }

           if (isExist.Password != dto.Password)
           {
               ViewBag.Message = "Incorrect password.";
               return View("Login");
           }

           var token = GenerateJwtToken(dto);

           Response.Cookies.Append("jwt_Token", token, new CookieOptions
           {
               HttpOnly = true,
               Secure = true,
               SameSite = SameSiteMode.Strict,
               Expires = DateTime.UtcNow.AddHours(1)
           });

           return RedirectToAction("Index", "Dashboard");

        }

        // without JWT

        // public async Task<IActionResult> LoginUser(UserDto dto)
        // {
        //     if (dto == null)
        //     {
        //         ViewBag.Message = "Please provide email and password.";
        //         return View("Login");
        //     }

        //     if (dto.Email == null || dto.Password == null)
        //     {
        //         ViewBag.Message = "Email and password are required.";
        //         return View("Login");
        //     }

        //     var isExist = await _context.Users
        //         .FirstOrDefaultAsync(x => x.Email == dto.Email);

        //     if (isExist == null)
        //     {
        //         ViewBag.Message = "Email does not exist.";
        //         return View("Login");
        //     }

        //     if (isExist.Password != dto.Password)
        //     {
        //         ViewBag.Message = "Incorrect password.";
        //         return View("Login");
        //     }

        //     // Login successful
        //     return RedirectToAction("Index", "Dashboard");
        // }

        private string GenerateJwtToken(UserDto  dto)
        {
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            // var key = Encoding.UTF8.GetBytes("d2f1d58034bf9137e6e399385843aa23d7fa70970794d0db4f09ff19d249ae5b4b890341");

            // var key = Encoding.UTF8.GetBytes(
            //     _configuration["Jwt:Key"]!
            // );

            var key = Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"]!
            );

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, dto.Email)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);

        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt_Token");
            return RedirectToAction("Login");
        }
    }
}
