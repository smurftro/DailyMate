using ApplicationCore.Abstraction;
using Domain.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DailyMate.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _TaskService;
        public TaskController(ITaskService taskService)
        {
            _TaskService = taskService;
        }
        [Authorize]
        [HttpPost("AddTask")]
        public async Task<IActionResult> AddTask(AddTaskDtos models)
        {
            var result=await _TaskService.AddTasks(models);
            if (result.IsSucces == true)
            {
                return Ok(result);
            }
            return BadRequest();
        }
        [Authorize]
        [HttpPut("IsCompleted")]
       public async Task<IActionResult> IsCompleteds()
        {
            var result = await _TaskService.IsCompleted();
            if(result.IsSucces == true)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [Authorize]
        [HttpPut("UpdateTask")]
        public async Task<IActionResult> UpdateTask([FromBody] UpdateTaskDtos models, [FromQuery] Guid id)
        {
            var result = await _TaskService.UpdateTasks(models,id);
            if (result.IsSucces == true)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [Authorize]
        [HttpDelete("DeleteTasks")]
        public async Task<IActionResult> DeleteTasks(Guid id)
        {
            var result=await _TaskService.DeleteTasks(id);
            if(result.IsSucces == true)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [Authorize]
        [HttpGet("GetTasks")]
        public async Task<IActionResult> GetTasks()
        {
            var result=await _TaskService.GetTaskCaching();
            if(result.IsSucces == true)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}
