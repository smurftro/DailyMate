using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public interface IEntityBase
    {
        public  Guid Id { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
