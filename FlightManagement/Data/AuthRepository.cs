using FlightManagement.Dtos;
using FlightManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace FlightManagement.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        //public static Dictionary<string, string> RefreshTokenStore = new Dictionary<string, string>();
        public AuthRepository(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            
        }
        public async Task<AuthenticationResponse> Login(string username, string password)
        {
            AuthenticationResponse response = new AuthenticationResponse();
            response.Username = username;

            User u = await _context.Users.FirstOrDefaultAsync(x => x.Username.ToLower().Equals(username.ToLower()));
            if (u == null)
            {
                //response = "-1";
                return response;
            }
            else if (!VerifyPasswordHash(password, u.PasswordHash, u.PasswordSalt))
            {
                //response = "-2";
                return response;
            }
            var jwtToken = GenerateJwtToken(u);
            response.AuthenticationToken = jwtToken.Token;
            response.RefreshToken = jwtToken.RefreshToken;
           
            
            
                //response = CreateToken(u);
                
                SaveOrUpdateUserRefreshToken(new UserRefreshToken
                {
                    RefreshToken = jwtToken.RefreshToken,
                    Username = u.Username,
                    UserId = u.Id
                });
            
            return response;
        }

        public async Task<int> Register(User user, string password)
        {
            if(await UserExists(user.Username))
            {
                return -1;
            }
            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user.Id;
        }

        public async Task<bool> UserExists(string username)
        {
            if(await _context.Users.AnyAsync(x => x.Username.ToLower() == username.ToLower()))
            {
                return true;
            }
            return false;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for(int i = 0; i < computedHash.Length; i++)
                {
                    if(computedHash[i] != passwordHash[i])
                    {
                        return false;
                    }
                }
                return true;
            }

        }


        private JwtToken GenerateJwtToken(User user)
        {
            Console.WriteLine(user.Username);
            Console.WriteLine(user.Id);
            Console.WriteLine(user.Email);
            var securityKey = Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]);

            var claims = new Claim[] {
                    new Claim(ClaimTypes.Name,user.Username),
                    new Claim(ClaimTypes.Email,user.Email),
                    new Claim("Id",user.Id.ToString())
                };

            var credentials = new SigningCredentials(new SymmetricSecurityKey(securityKey), SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
              _configuration["Jwt:Issuer"],
              claims,
              expires: DateTime.Now.AddDays(1),
              signingCredentials: credentials);


            var jwtToken = new JwtToken
            {
                RefreshToken = new RefreshTokenGenerator().GenerateRefreshToken(32),
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            };

            return jwtToken;
        }

        private TokenValidationParameters GetTokenValidationParameters()
        {
            var securityKey = Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]);

            return new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(securityKey),
                ValidIssuers = new string[] { _configuration["Jwt:Issuer"] },
                ValidAudiences = new string[] { _configuration["Jwt:Issuer"] },
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true
            };
        }

        public AuthenticationResponse RefreshToken(JwtToken jwtToken)
        {
            if (jwtToken == null)
            {
                return null;
            }

            var handler = new JwtSecurityTokenHandler();

            SecurityToken validatedToken;

            IPrincipal principal = handler.ValidateToken(jwtToken.Token, GetTokenValidationParameters(), out validatedToken);

            var username = principal.Identity.Name;
            Console.WriteLine("Refreshing token...");
            Console.WriteLine(username);
            if (CheckIfRefreshTokenIsValid(username, jwtToken.RefreshToken))
            {
                var user = _context.Users.Where(x => x.Username == username).FirstOrDefault();
                var newJwtToken = GenerateJwtToken(user);

                SaveOrUpdateUserRefreshToken(new UserRefreshToken
                {
                    Username = user.Username,
                    RefreshToken = newJwtToken.RefreshToken
                });
                AuthenticationResponse response = new AuthenticationResponse
                {
                    AuthenticationToken = newJwtToken.Token,
                    RefreshToken = newJwtToken.RefreshToken,
                    ExpiresAt = DateTime.Now.AddMinutes(10),
                    Username = username
                };

                return response;
            }

            return null;

        }

        public bool CheckIfRefreshTokenIsValid(string username, string refreshToken)
        {
            UserRefreshToken urt = _context.RefreshTokens.Where(x => x.Username == username && x.RefreshToken == refreshToken).SingleOrDefault();
            if(urt == null)
            {
                Console.WriteLine("===>>>>> FALSE  ---------");
                return false;
            }

            Console.WriteLine("===>>>>> T R U E  ---------");

            return true;
            /*string refToken = "";

            RefreshTokenStore.TryGetValue(username, out refToken);

            return refToken.Equals(refreshToken);*/
        }

        public void SaveOrUpdateUserRefreshToken(UserRefreshToken userRefreshToken)
        {
            UserRefreshToken urt = _context.RefreshTokens.FirstOrDefault(x => x.Username == userRefreshToken.Username);
            Console.WriteLine("==========================================");
            Console.WriteLine(urt);
            Console.WriteLine("==========================================");
            if (urt != null)
            {
                urt.RefreshToken = userRefreshToken.RefreshToken;
                _context.RefreshTokens.Update(urt);
                _context.SaveChanges();
            }
            else
            {
                Console.WriteLine("Not found, NULL");
                _context.RefreshTokens.Add(userRefreshToken);
                _context.SaveChanges();
            }

           /* if (RefreshTokenStore.ContainsKey(userRefreshToken.Username))
            {
                RefreshTokenStore[userRefreshToken.Username] = userRefreshToken.RefreshToken;
            }
            else
            {
                RefreshTokenStore.Add(userRefreshToken.Username, userRefreshToken.RefreshToken);
            }*/
        }

        public Task DeleteRefreshToken(int userId)
        {
            var rt = _context.RefreshTokens.Where(x => x.UserId == userId).FirstOrDefault();
            _context.RefreshTokens.Remove(rt);
            _context.SaveChanges();
            return Task.CompletedTask;
        }
    }
}
