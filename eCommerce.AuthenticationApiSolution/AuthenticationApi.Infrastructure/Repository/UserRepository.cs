using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Repository;
using AuthenticationApi.Infrastructure.Data;
using AuthticationApi.Domain.Entities;
using BCrypt.Net;
using eCommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationApi.Infrastructure.Repository
{
    public class UserRepository(AuthenticationDbContext context, IConfiguration configuration) : IUserRepository
    {
        private async Task<AppUser> GetUserByEmail(string email) => await context.Users.FirstOrDefaultAsync(x => x.Email == email);

        public async Task<GetUserDTO> GetUser(int userId)
        {
            var user = await context.Users.FindAsync(userId);
            return user is not null
                ? new GetUserDTO(user.Id, user.Name, user.TelephoneNumber, user.Address, user.Email, user.Role)
                : null!;
        }

        public async Task<Response> Login(LoginDTO loginDTO)
        {
            var getUser = await GetUserByEmail(loginDTO.Email);
            if (getUser is null)
                return new Response(false, $"User: {loginDTO.Email} is not registered.");

            var verifyPassword = BCrypt.Net.BCrypt.Verify(loginDTO.Password, getUser.Password);
            if (!verifyPassword)
                return new Response(false, "Invalid Password");

            var token = GenerateToken(getUser);
            return new Response(true, $"{token}");
        }

        private string GenerateToken(AppUser getUser)
        {
            var key = Encoding.UTF8.GetBytes(configuration.GetSection("Authentication:Key").Value!);
            var securityKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, getUser.Name),
                new(ClaimTypes.Email, getUser.Email)
                //new(ClaimTypes.Role, getUser.Role)
            };

            if (!string.IsNullOrEmpty(getUser.Role) || !Equals("string", getUser.Role))
                claims.Add(new(ClaimTypes.Role, getUser.Role));

            var token = new JwtSecurityToken(
                issuer: configuration["Authentication:Issuer"],
                audience: configuration["Authentication:Audience"],
                claims: claims,
                expires: null, 
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<Response> Register(AppUserDTO appUserDTO)
        {
            var getUser = await GetUserByEmail(appUserDTO.Email);
            if (getUser is not null)
                return new Response(false, "You cannot user this email for registration. User is already registered with this email.");

            var result = context.Users.Add(new AppUser
            {
                Name = appUserDTO.Name,
                Address = appUserDTO.Address,
                Email = appUserDTO.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(appUserDTO.Password),
                TelephoneNumber = appUserDTO.TelephoneNumber,
                Role = appUserDTO.Role
            });

            await context.SaveChangesAsync();
            return result.Entity.Id > 0 
                ? new Response(true, "User is successfully registered.")
                : new Response(false, "Registration failed...");
        }
    }
}
