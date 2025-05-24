using Domain;
using ApplicationCore.Abstraction;
using Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Domain.Entites;
using Domain.Repository;
using System.Runtime.Intrinsics.X86;
using System.Net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.Design;

namespace ApplicationCore.Concrete
{
    public class UserService : IUserService
    {
        private ApiResponse _apiresponse;
        private readonly IWriteRepository<ApplicationUser>  _UserWriteRepository;
        private readonly IReadRepository<ApplicationUser> _UserReadRepository;
        private readonly UserManager<ApplicationUser> _usermnagaer;
        private IConfiguration _configuration;
        public UserService(ApiResponse apiResponse,UserManager<ApplicationUser> userManager,IReadRepository<ApplicationUser> readRepository,IConfiguration configuration,IWriteRepository<ApplicationUser> writeRepository)
        {
            _apiresponse = apiResponse;
            _usermnagaer = userManager;
            _UserReadRepository = readRepository;
            _configuration = configuration;
            _UserWriteRepository = writeRepository;
          
        }

       public async Task<ApiResponse> Login(LoginUserDtos models)
        {
            var user=await _usermnagaer.FindByEmailAsync(models.email);
            if (user != null)
            {
               var IsValid=await _usermnagaer.CheckPasswordAsync(user,models.password);
                if (IsValid == true)
                {
                    JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                    var secretKey = _configuration.GetValue<string>("JwtKey");
                    byte[] key = Encoding.UTF8.GetBytes(secretKey);
                    SecurityTokenDescriptor securityTokenDescriptor = new SecurityTokenDescriptor()
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                        {
                            new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                            new Claim(ClaimTypes.Email,user.Email),
                            new Claim("Name",user.FirstName),
                            new Claim("UserName",user.UserName),
                        }),
                        Expires = DateTime.UtcNow.AddDays(1),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
                    };
                    SecurityToken token = tokenHandler.CreateToken(securityTokenDescriptor);
                    LoginResponseModel loginResponseModels = new()
                    {
                        Email = user.Email,
                        Token = tokenHandler.WriteToken(token),
                    };
                    

                    _apiresponse.result = loginResponseModels;
                    _apiresponse.IsSucces = true;
                    _apiresponse.HttpStatusCode = System.Net.HttpStatusCode.OK;
                    return _apiresponse;
                }
                else
                {
                    _apiresponse.IsSucces = false;
                    _apiresponse.ErrorMessage.Add("Şifre yanlış");
                    _apiresponse.HttpStatusCode = HttpStatusCode.BadRequest;
                    return _apiresponse;
                }
            }
            _apiresponse.IsSucces = false;
            _apiresponse.ErrorMessage.Add("Kullanıcı bulunamadı");
            _apiresponse.HttpStatusCode = HttpStatusCode.BadRequest;
            return _apiresponse;
        }

        public async Task<ApiResponse> Register(RegisterUserDTOS models)
        {
            var user = await _usermnagaer.FindByEmailAsync(models.Email);
            if (user != null)
            {
                _apiresponse.IsSucces = false;
                _apiresponse.ErrorMessage.Add("Bu maile sahip kullanıcı mevcut.");
                _apiresponse.HttpStatusCode = System.Net.HttpStatusCode.BadRequest;
                return _apiresponse;
            }


            ApplicationUser userr = new()
            {
                FirstName = models.FirstName,
                LastName = models.LastName,
                Email = models.Email,
                UserName = models.UserName,
            };
            var result = await _usermnagaer.CreateAsync(userr, models.Password);
            if (result.Succeeded)
            {
                _apiresponse.IsSucces = true;
                _apiresponse.HttpStatusCode = System.Net.HttpStatusCode.OK;
                _apiresponse.result = result;
                return _apiresponse;
            }

            return new ApiResponse()
            {
                IsSucces = false,
                ErrorMessage = result.Errors.Select(e => e.Description).ToList(),
                HttpStatusCode = HttpStatusCode.BadRequest

            };

        }
    }
}
