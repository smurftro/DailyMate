using Domain;
using Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Abstraction
{
    public interface IHabitService
    {
        public Task<ApiResponse> AddHabits(AddHabitsDtos models);
        public Task<ApiResponse> UpdateHabits(UpdateHabitsDtos models,Guid id);
        public Task<ApiResponse> DeleteHabits(Guid id);
        public Task<ApiResponse> GetHabitCaching();
    }
}
