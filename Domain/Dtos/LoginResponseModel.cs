using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dtos
{
    public class LoginResponseModel
    {
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
