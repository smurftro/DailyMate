using Domain;
using Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Abstraction
{
    public interface ITaskService
    {
        public Task<ApiResponse> AddTasks(AddTaskDtos models);
        public Task<ApiResponse> IsCompleted();
        public Task<ApiResponse> UpdateTasks(UpdateTaskDtos models,Guid id);
        public Task<ApiResponse> DeleteTasks(Guid id);
        public Task<ApiResponse> GetTaskCaching();
    }
}
