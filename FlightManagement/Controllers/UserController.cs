using FlightManagement.Data;
using FlightManagement.Dtos;
using FlightManagement.Models;
using FlightManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace FlightManagement.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")] 
    public class UserController : Controller
    {
        private UserService _userService;
        private readonly IAuthRepository _authRepository;
        public UserController(IAuthRepository authRepository, UserService userService)
        {
            _userService = userService;
            _authRepository = authRepository;

        }

        [HttpGet("get-user")]
        public async Task<ActionResult<UserDetailsDto>> GetUser()
        {
            var user = await _userService.GetUser();
            return Ok(user);
        }

        [HttpGet("get-user-by-id/{id:int}")]
        public async Task<ActionResult<UserDetailsDto>> GetUserById(int id)
        {
            var user = await _userService.GetUserById(id);
            return Ok(user);
        }


        [AllowAnonymous]
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] UserDto userDto)
        {
            int response = await _userService.SignUp(userDto);
            if(response == -1)
            {
                return BadRequest("User Already Exists!");
            }
            return Ok();
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userLoginDto)
        {
            AuthenticationResponse response = await _authRepository.Login(userLoginDto.Username, userLoginDto.Password);

            return Ok(response);
        }


        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            Console.WriteLine(_userService.GetUserId());
            await _authRepository.DeleteRefreshToken(_userService.GetUserId());
            return Ok();
        }

        [HttpPost("add-address")]
        public async Task<IActionResult> AddAddress([FromBody] AddressDto addressDto)
        {
            await _userService.AddAddress(addressDto);
            return Ok();
        }
        
        [HttpGet("get-users")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var us = await _userService.GetUsers();
            return Ok(us);
        }
        [HttpPost("update-user")]
        public async Task<ActionResult> UpdateUser([FromBody] UserDetailsDto userDetailsDto)
        {
            await _userService.UpdateUser(userDetailsDto);
            return Ok();
        }
        [HttpPost("update-address")]
        public async Task<ActionResult> UpdateAddress([FromBody] UserDetailsDto userDetailsDto)
        {
            await _userService.UpdateAddress(userDetailsDto);
            return Ok();
        }

        [HttpGet("get-addresses")]
        public async Task<ActionResult<IEnumerable<Address>>> GetAddresses()
        {
            var ad = await _userService.GetAddresses();
            return Ok(ad);
        }

        //[AllowAnonymous]
        [HttpPost]
        [Route("refreshToken")]
        public IActionResult RefreshToken([FromBody] JwtToken jwtToken)
        {
            AuthenticationResponse response = _authRepository.RefreshToken(jwtToken);
            if(response == null)
            {
                return BadRequest("Invalid Request");
            }
            return Ok(response);

        }




    }

    
}
