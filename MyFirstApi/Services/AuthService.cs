using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyFirstApi.Data;
using MyFirstApi.Dto;
using MyFirstApi.IService;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyFirstApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Tuple<int, TokenDto>> LoginUser(UserDto dto)
        {
            try
            {
                var tokenDto = new TokenDto();
                if (dto == null)
                {
                    tokenDto.Message = "Please Fill All the Details!";

                    return new Tuple<int, TokenDto>(0, tokenDto);
                }
                var existingUser = await _context.AccountUsers.FirstOrDefaultAsync(x => x.Email == dto.Email);

                if (existingUser == null)
                {

                    tokenDto.Message = "This User Not Exist, Please Login";

                    return new Tuple<int, TokenDto>(0, tokenDto);
                }

                //if (existingUser.Password != dto.Password)
                //{
                //    return new Tuple<int, string>(1, "Password Incorrect");
                //}

                var passwordHasher = new PasswordHasher<string>();

                var verifyPassword = passwordHasher.VerifyHashedPassword(dto.Email, existingUser.Password, dto.Password);

                if (verifyPassword == PasswordVerificationResult.Success)
                {
                    UserDto user = new();
                    user.Email = dto.Email;
                    user.Id = existingUser.Id;
                    user.Name = existingUser.Name;
                    var token = GetJwtToken(user);

                    tokenDto.Token = token;
                    tokenDto.Message = "Login Successfull";

                    return new Tuple<int, TokenDto>(2, tokenDto);
                }
                else if (verifyPassword == PasswordVerificationResult.SuccessRehashNeeded)
                {
                    UserDto user = new();
                    user.Email = existingUser.Email;
                    user.Id = existingUser.Id;
                    user.Name = existingUser.Name;
                    var token = GetJwtToken(user);

                    existingUser.Password = PasswordHashing(dto);

                    _context.AccountUsers.Update(existingUser);
                    _context.SaveChanges();


                    tokenDto.Token = token;
                    tokenDto.Message = "Login Successfull, New Hash Generated";

                    return new Tuple<int, TokenDto>(2, tokenDto);

                }
                else if (verifyPassword == PasswordVerificationResult.Failed)
                {
                    tokenDto.Message = "Password Incorrect";

                    return new Tuple<int, TokenDto>(1, tokenDto);

                }

                tokenDto.Message = "This User Not Exist, Please Login";


                return new Tuple<int, TokenDto>(0, tokenDto);


            }
            catch (Exception)
            {
                throw;
            }
        }

        private string GetJwtToken(UserDto dto)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, dto.Name),
                new Claim(ClaimTypes.Email , dto.Email),
                new Claim(ClaimTypes.NameIdentifier, dto.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("04b83dfe0e9dadbc32f4354525b521412f0527beff39812c0dcf602aa1b5a648"));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


            var token = new JwtSecurityToken(
                issuer: "rohan-client",
                audience: "rohan-backend",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(1),
                signingCredentials: creds

                );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        public async Task<Tuple<int, string>> RegisterUser(UserDto dto)
        {
            try
            {
                var existingUser = await _context.AccountUsers.AnyAsync(x => x.Email == dto.Email);

                if (existingUser)
                {
                    return new Tuple<int, string>(0, "This User is already Exist, Please Register With New User!");
                }

                _context.AccountUsers.Add(new Entities.User
                {
                    Id = Guid.NewGuid(),
                    Name = dto.Name,
                    Email = dto.Email,
                    Password = PasswordHashing(dto),
                });

                await _context.SaveChangesAsync();

                return new Tuple<int, string>(1, "User Registered Succesffully!");

            }
            catch (Exception ex)
            {

                throw;
            }
        }


        private string PasswordHashing(UserDto dto)
        {
            var passwordHasher = new PasswordHasher<string>();

            var hash = passwordHasher.HashPassword(dto.Email, dto.Password);

            return hash;

        }



    }
}
