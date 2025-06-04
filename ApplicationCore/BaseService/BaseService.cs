using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ApplicationCore.BaseService
{
    public abstract class BaseService//Sadece miras alınmalı tek başına kullanılmamalı getuseridnin kullanımını kolaylaştırmalı.
    {   
        protected readonly IHttpContextAccessor _contextAccessor;
        protected BaseService(IHttpContextAccessor httpContextAccessor) 
        { 
            _contextAccessor = httpContextAccessor;
        }
        protected Guid GetUserId()
        {
            var IdStr=_contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(IdStr,out var id)? id:Guid.Empty;
        }
    }
}
