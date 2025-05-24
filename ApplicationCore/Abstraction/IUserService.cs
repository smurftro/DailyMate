using Domain;
using Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Abstraction
{
    public interface IUserService
    {
       public Task<ApiResponse> Register(RegisterUserDTOS models);
        public Task<ApiResponse> Login(LoginUserDtos models);
    }
}
