using AuthenticationApi.Application.DTOs;
using AuthticationApi.Domain.Entities;
using eCommerce.SharedLibrary.Interface;
using eCommerce.SharedLibrary.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationApi.Application.Repository
{
    public interface IUserRepository
    {
        Task<Response> Register(AppUserDTO appUserDTO);
        Task<Response> Login(LoginDTO loginDTO);
        Task<GetUserDTO> GetUser(int userId);
    }
}
