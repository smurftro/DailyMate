using ApplicationCore.Abstraction;
using Domain.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace DailyMate.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HabitController : ControllerBase
    {

       private readonly IHabitService _habitService;
       
        public HabitController(IHabitService habitService)
        {
            _habitService = habitService;
        }
        [Authorize]
        [HttpGet("GetHabits")]
        public async Task<IActionResult> GetHabits()
        {
            var result = await _habitService.GetHabitCaching();
            if (result == null)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [Authorize]
        [HttpPost("AddHabits")]
        public async Task<IActionResult> AddHabits(AddHabitsDtos models)
        {
            var result=await _habitService.AddHabits(models);
            if (result == null)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [Authorize,HttpPut("UpdateHabits")]
        public async Task<IActionResult> UpdateHabits(UpdateHabitsDtos models,Guid id)
        {
            var result = await _habitService.UpdateHabits(models,id);
            if(result == null)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [Authorize,HttpDelete("DeleteHabits")]
        public async Task<IActionResult> DeleteHabits(Guid id)
        {
            var result=await _habitService.DeleteHabits(id);
            if(result == null)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
