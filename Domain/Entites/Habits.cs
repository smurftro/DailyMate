using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entites
{
    public class Habits:IEntityBase
    {

        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ActivityDates ActivityDates { get; set; }
        public Guid Id { get; set; }
        public DateTime? CreatedAt { get ;set; }=DateTime.Now;
    }
}
