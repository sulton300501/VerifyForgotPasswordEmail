using VerifyForgotPasswordEmail.Models;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using VerifyForgotPasswordEmail.Data;

namespace VerifyForgotPasswordEmail.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly DataContext _context;
        public UserController(DataContext context)
        {
            _context=context;
            
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegistrRequest request)
        {
            if (_context.Users.Any(u => u.Email == request.Email))
            {
                return BadRequest("User Already exists");
            }

            CreatePasswordHash(request.Password,
                out byte[] passwordHash,
                out byte[] passwordSalt);
          
            var user = new User
            {
                Email = request.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                VerificationToken = CreateRandomToken()
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync(); // Ma'lumotlar bazasiga yozish uchun
            return Ok("User Succesfully");

        }




        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

           

            if (user == null)
            {
                return BadRequest("User not found.");
            }else

            if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {

                return BadRequest("Password is incorrect.");
            }else
            if (user.VerifiedAt==null)
            {
                return BadRequest("Not verified!");
            }


            return Ok($"Welcome back, {user.Email}! :)");
        }





        [HttpPost("verify")]
        public async Task<IActionResult> Verify(string token)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.VerificationToken == token);
            if (user == null)
            {
                return BadRequest("Invalid Token");
            }

             
            user.VerifiedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();



            return Ok("User Verified");


        }



        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return BadRequest("User noot found");
            }


            user.PasswordResetToken=CreateRandomToken();
            user.ResetTokenExpires=DateTime.UtcNow.AddDays(1);
            await _context.SaveChangesAsync();



            return Ok("User Verified");


        }




        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(Models.ResetPasswordRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == request.Token);
            if (user == null||user.ResetTokenExpires<DateTime.Now)
            {
                return BadRequest("Invalid token");
            }



            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
        


            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.PasswordResetToken = null;
            user.PasswordResetToken = null;
            await _context.SaveChangesAsync();



            return Ok("You may noe");


        }






        private bool VerifyPasswordHash(string password, byte[] passwordHash,  byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
              
                var computed = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computed.SequenceEqual(passwordHash);
            }
        }



        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac
                    .ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }


        private string CreateRandomToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        }





    }
}
