using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ApiResponse
    {
        public ApiResponse() 
        {
            ErrorMessage = new List<string>();
        }

        public List<string> ErrorMessage { get; set; }
        public bool IsSucces { get; set; }
        public object result { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }
    }
}
