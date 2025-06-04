using ApplicationCore.Abstraction;
using Domain;
using Domain.Dtos;
using Domain.Entites;
using Domain.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Concrete
{
    public class HabitService : BaseService.BaseService, IHabitService
    {
        private ApiResponse _apiResponse;
        private IWriteRepository<Habits> _writeHabitRepository;
        private readonly IReadRepository<Habits> _readHabitRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMemoryCache _memoryCache;
        public HabitService(IMemoryCache memoryCache,IHttpContextAccessor httpContextAccessor,ApiResponse apiResponse,IWriteRepository<Habits> writeRepository,IReadRepository<Habits> readRepository,UserManager<ApplicationUser> userManager) : base(httpContextAccessor)
        {
            _apiResponse = apiResponse;
            _writeHabitRepository = writeRepository;
            _readHabitRepository = readRepository;
            _userManager = userManager;
            _memoryCache = memoryCache;
        }

        public async Task<ApiResponse> AddHabits(AddHabitsDtos models)
        {
            if (GetUserId() == Guid.Empty)
            {
                _apiResponse.ErrorMessage.Add("Kullanıcı girişi yapılmamış");
                _apiResponse.HttpStatusCode=System.Net.HttpStatusCode.BadRequest;
                _apiResponse.IsSucces = false;
                return _apiResponse;
            }
            var habits = new Habits()
            {
                Name = models.Name,
                Description = models.Description,
                ActivityDates = new()
                {
                    ActivityType=Domain.Enum.ActivityType.Habit,
                    StartDate = models.StartDate,
                    EndDate = models.EndDate,
                },
                UserId = GetUserId()
            };
            await _writeHabitRepository.AddAsync(habits);
            if(await _writeHabitRepository.SaveAsync() > 0)
            {
                _apiResponse.IsSucces=true;
                _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.OK;
                _apiResponse.result = habits;
                return _apiResponse;
            }
            _apiResponse.ErrorMessage.Add("Ekleme işlemi başarısız olmuştur");
            _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.BadRequest;
            _apiResponse.IsSucces = false;
            return _apiResponse;
        }

        public async Task<ApiResponse> UpdateHabits(UpdateHabitsDtos models,Guid id)
        {
            if (GetUserId() == Guid.Empty)
            {
                _apiResponse.ErrorMessage.Add("Kullanıcı girişi yapılmamış");
                _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.BadRequest;
                _apiResponse.IsSucces = false;
                return _apiResponse;
            }
            var user=await _userManager.FindByIdAsync(GetUserId().ToString());
            var habits= _readHabitRepository.Table.Include(x=>x.ActivityDates).FirstOrDefault(x=>x.Id==id&&x.UserId==user.Id);
            habits.Name = models.Name;
            habits.Description = models.Description;
            habits.ActivityDates.StartDate=models.StartDate;
            habits.ActivityDates.EndDate=models.EndDate;
            await _writeHabitRepository.UpdateAsync(habits);
            if(await _writeHabitRepository.SaveAsync() > 0)
            {
                _apiResponse.IsSucces=true;
                _apiResponse.HttpStatusCode=System.Net.HttpStatusCode.OK;
                _apiResponse.result = habits;
                return _apiResponse;
            }
            _apiResponse.ErrorMessage.Add("Güncelleme işlemi başarısız olmuştur");
            _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.BadRequest;
            _apiResponse.IsSucces = false;
            return _apiResponse;
        }

        public async Task<ApiResponse> DeleteHabits(Guid id)
        {
            if (GetUserId() == Guid.Empty)
            {
                _apiResponse.ErrorMessage.Add("Kullanıcı girişi yapılmamış");
                _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.BadRequest;
                _apiResponse.IsSucces = false;
                return _apiResponse;
            }
            var user=await _userManager.FindByIdAsync(GetUserId().ToString());
            var habits= _readHabitRepository.Table.Where(x=>x.UserId==user.Id).FirstOrDefault(x=>x.Id==id);
             _writeHabitRepository.Delete(habits);
            if(await _writeHabitRepository.SaveAsync() > 0)
            {
                _apiResponse.IsSucces = true;
                _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.OK;
                return _apiResponse;
            }
            _apiResponse.ErrorMessage.Add("Silme işlemi başarısız olmuuştur");
            _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.BadRequest;
            _apiResponse.IsSucces = false;
            return _apiResponse;
        }

        public async Task<ApiResponse> GetHabitCaching()
        {
            if (GetUserId() == Guid.Empty)
            {
                _apiResponse.ErrorMessage.Add("Kullanıcı girişi yapılmamış");
                _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.BadRequest;
                _apiResponse.IsSucces = false;
                return _apiResponse;
            }
            var user = await _userManager.FindByIdAsync(GetUserId().ToString());
            var habits = await _readHabitRepository.Table.Where(x => x.UserId == user.Id).ToListAsync();
            if (habits.Count == null)
            {
                _apiResponse.ErrorMessage.Add("Kullanıcının herhangi bir hobisi yoktur");
                _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.BadRequest;
                _apiResponse.IsSucces = false;
                return _apiResponse;
            }
            var cacheKey = $"List_Habits_{user.Id}";
            if (!_memoryCache.TryGetValue(cacheKey, out List<Habits> cacheHabits))
            {
                _memoryCache.Set(cacheKey, habits, new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(5)
                });

            }
            _apiResponse.result = habits;
            _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.OK;
            _apiResponse.IsSucces = true;
            return _apiResponse;
        }
    }
}
