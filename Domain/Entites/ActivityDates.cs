using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Entites
{
    public class ActivityDates:IEntityBase
    {   public Guid Id {  get; set; }
        public ActivityType ActivityType { get; set; } = ActivityType.Bos;
        [JsonIgnore]
        public ICollection<Habits> habits { get; set; } 
        public ICollection<Tasks> tasks { get; set; }
        public Guid ActivityId { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        public bool  IsCompleted { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
    }
}
