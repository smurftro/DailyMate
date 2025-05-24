using Domain.Enum;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entites
{
    public class ApplicationUser:IdentityUser<Guid>,IEntityBase
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Gender Gender = Gender.Bos;
        public ICollection<Tasks> tasks { get; set; }
        public ICollection<Habits> habits { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
