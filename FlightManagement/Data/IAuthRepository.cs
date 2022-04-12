using FlightManagement.Dtos;
using FlightManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightManagement.Data
{
    public interface IAuthRepository
    {
        Task<int> Register(User user, string password);
        Task<AuthenticationResponse> Login(string username, string password);
        Task<bool> UserExists(string username);
        AuthenticationResponse RefreshToken(JwtToken jwtToken);
        Task DeleteRefreshToken(int userId);
    }
}
