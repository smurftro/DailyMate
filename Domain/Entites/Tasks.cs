using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entites
{
    public class Tasks:IEntityBase
    {
        public Guid Id { get; set; }
        public DateTime? CreatedAt { get; set; }=DateTime.Now;
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }=false;
        public ActivityDates ActivityDates { get; set; }
    }
}
