using AutoMapper;
using FlightManagement.Data;
using FlightManagement.Dtos;
using FlightManagement.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FlightManagement.Services
{
    public class UserService
    {
        private readonly IAuthRepository _authRepository;
        private DataContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserService(IAuthRepository authRepository, DataContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _authRepository = authRepository;
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public int GetUserId() => int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue("Id"));
        
        public async Task UpdateUser(UserDetailsDto userDetailsDto)
        {
            var user = await _context.Users.FindAsync(userDetailsDto.Id);
            user.Email = userDetailsDto.Email;
            user.FirstName = userDetailsDto.FirstName;
            user.LastName = userDetailsDto.LastName;
            user.Phone = userDetailsDto.Phone;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAddress(UserDetailsDto userDetailsDto)
        {
            var user = await _context.Users.Include(x => x.Address).Where(x => x.Id == userDetailsDto.Id).FirstOrDefaultAsync();
            
            if(user.Address == null)
            {
                Address address = new Address()
                {
                    Street = userDetailsDto.Street,
                    City = userDetailsDto.City,
                    StateProvince = userDetailsDto.StateProvince,
                    ZipCode = userDetailsDto.ZipCode,
                    Country = userDetailsDto.Country
                };
                user.Address = address;
            }
            else
            {
                var a = await _context.Addresses.FindAsync(user.Address.AddressId);
                a.City = userDetailsDto.City;
                a.Country = userDetailsDto.Country;
                a.Street = userDetailsDto.Street;
                a.StateProvince = userDetailsDto.StateProvince;
                a.ZipCode = userDetailsDto.ZipCode;
            }
            
            await _context.SaveChangesAsync();
        }
        public async Task<UserDetailsDto> GetUserById(int id)
        {
            UserDetailsDto userDetailsDto;
            var user = await _context.Users.Include(x => x.Address).Where(x => x.Id == id).FirstOrDefaultAsync();
            if (user.Address == null)
            {
                userDetailsDto = new UserDetailsDto()
                {
                    Id = user.Id,
                    City = "",
                    Country = "",
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Username = user.Username,
                    Phone = user.Phone,
                    StateProvince = "",
                    Street = "",
                    ZipCode = -1
                };
            }
            else
            {
                userDetailsDto = new UserDetailsDto()
                {
                    Id = user.Id,
                    City = user.Address.City,
                    Country = user.Address.Country,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Username = user.Username,
                    Phone = user.Phone,
                    StateProvince = user.Address.StateProvince,
                    Street = user.Address.Street,
                    ZipCode = user.Address.ZipCode
                };
            }

            return userDetailsDto;
        }
            public async Task<UserDetailsDto> GetUser()
        {
            UserDetailsDto userDetailsDto;
            var user = await _context.Users.Include(x => x.Address).Where(x => x.Id == GetUserId()).FirstOrDefaultAsync();
            if(user.Address == null)
            {
                userDetailsDto = new UserDetailsDto()
                {
                    Id = user.Id,
                    City = "",
                    Country = "",
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Username = user.Username,
                    Phone = user.Phone,
                    StateProvince = "",
                    Street = "",
                    ZipCode = -1
                };
            }
            else
            {
                userDetailsDto = new UserDetailsDto()
                {
                    Id = user.Id,
                    City = user.Address.City,
                    Country = user.Address.Country,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Username = user.Username,
                    Phone = user.Phone,
                    StateProvince = user.Address.StateProvince,
                    Street = user.Address.Street,
                    ZipCode = user.Address.ZipCode
                };
            }
            
            return userDetailsDto;
        }
        public async Task<int> SignUp(UserDto userDto)
        {
            var _user = new User()
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                Phone = userDto.Phone,
                Username = userDto.Username,
                Role = Role.USER
            };
            return await _authRepository.Register(_user, userDto.Password);
            //await _context.Users.AddAsync(_mapper.Map<User>(userDto));
            //await _context.SaveChangesAsync();
        }

        public async Task AddAddress(AddressDto addressView)
        {
            User _user = _context.Users.Find(addressView.UserId);
            var _address = new Address()
            {
                City = addressView.City,
                Street = addressView.Street,
                StateProvince = addressView.StateProvince,
                Country = addressView.Country,
                ZipCode = addressView.ZipCode,
                User = _user
            };
            
            await _context.Addresses.AddAsync(_address);
            await _context.SaveChangesAsync();

            /*_user.Address = _address;
            _context.Users.Update(_user);
            _context.SaveChangesAsync();*/
        }

        public async Task<List<User>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }
        public async Task<List<Address>> GetAddresses()
        {
            return await _context.Addresses.ToListAsync();
        }
    }
}
