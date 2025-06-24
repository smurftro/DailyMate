using ApplicationCore.Abstraction;
using ApplicationCore.BaseService;
using Domain;
using Domain.Dtos;
using Domain.Entites;
using Domain.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Concrete
{
    public class TaskService: BaseService.BaseService,ITaskService
    {
        private readonly IWriteRepository<Tasks> _writeRepository;
        private readonly IReadRepository<ApplicationUser> _UserReadRepository;
        private readonly IReadRepository<Tasks> _TasksReadRepository;
        private readonly ApiResponse _apiResponse;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMemoryCache _memoryCache;
        public TaskService(IMemoryCache memoryCache,IReadRepository<Tasks> TasksReadRepository,IReadRepository<ApplicationUser> UserreadRepository,IWriteRepository<Tasks> writeRepository,ApiResponse apiResponse,UserManager<ApplicationUser> userManager,IHttpContextAccessor httpContextAccessor) :base(httpContextAccessor)
        { 
            _writeRepository = writeRepository;
            _apiResponse = apiResponse;
            _userManager = userManager;
            _UserReadRepository = UserreadRepository;
            _TasksReadRepository = TasksReadRepository;
            _memoryCache = memoryCache;
        }

        public async Task<ApiResponse> AddTasks(AddTaskDtos models)
        {
            if (GetUserId() == null)
            {
                _apiResponse.IsSucces = false;
                _apiResponse.ErrorMessage.Add("Kullanıcı girişi yapılmamıştır");
                _apiResponse.HttpStatusCode=System.Net.HttpStatusCode.BadRequest;
                return _apiResponse;

            }
            var tasks = new Tasks() {
                Title = models.Title,
                Description = models.Description,
                ActivityDates = new()
                {
                    ActivityType = Domain.Enum.ActivityType.Task,
                    StartDate = models.StartDate,
                    EndDate = models.EndDate,
                },
                UserId = GetUserId(),
            };
            var user=await _userManager.FindByIdAsync(GetUserId().ToString());
            var result=await _writeRepository.AddAsync(tasks);
            if(await _writeRepository.SaveAsync() > 0)
            {
                var cacheKey = $"List_Tasks_{user.Id}";
                _memoryCache.Remove(cacheKey);
                _apiResponse.IsSucces=true;
                _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.OK;
                _apiResponse.result = result;   
                return _apiResponse;
            }
            _apiResponse.IsSucces = false;
            _apiResponse.ErrorMessage.Add("Eklerken bir hata oluştu");
            _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.BadRequest;
            return _apiResponse;
        }
        
        public async Task<ApiResponse> IsCompleted()
        {
            if (GetUserId() == null)
            {
                _apiResponse.IsSucces = false;
                _apiResponse.ErrorMessage.Add("Kullanıcı girişi yapılmamıştır");
                _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.BadRequest;
                return _apiResponse;

            }
            var userId =await _UserReadRepository.GetByIdAsync(GetUserId());
            var task = await _TasksReadRepository.Table.
                Include(x=>x.ActivityDates).Where(x => x.UserId == userId.Id).ToListAsync();
            var today = DateOnly.FromDateTime(DateTime.Now);
            bool anyUpdated = false;
            foreach (var tasks in task)
            {
               
                if (today> tasks.ActivityDates.EndDate&&tasks.IsCompleted==false)
                {
                    tasks.IsCompleted = true;
                    anyUpdated = true;
                }
            }
            if (anyUpdated)
            {
                if (await _writeRepository.SaveAsync() > 0)
                {
                    _apiResponse.IsSucces = true;
                    _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.OK;
                    return _apiResponse;
                }
            }
            
            _apiResponse.IsSucces = false;
            _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.BadRequest;
            _apiResponse.ErrorMessage.Add("Zaman daha geçmedi");
            return _apiResponse;

        }
        public async Task<ApiResponse> UpdateTasks(UpdateTaskDtos models,Guid id)
        {
            if (GetUserId() == Guid.Empty)
            {
                _apiResponse.IsSucces = false;
                _apiResponse.ErrorMessage.Add("Kullanıcı girişi yapılmamıştır");
                _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.BadRequest;
                return _apiResponse;
            }
            var user = await _UserReadRepository.GetByIdAsync(GetUserId());
            var task =await _TasksReadRepository.Table.FirstOrDefaultAsync(x=>x.Id == id && x.UserId==user.Id);
            if (task!=null)
            {
                task.Title = models.Title;
                task.Description = models.Description;
                if (task.ActivityDates == null)
                {
                    task.ActivityDates = new ActivityDates();
                }
                task.ActivityDates.StartDate = models.StartDate;
                task.ActivityDates.EndDate = models.EndDate;
               
                await _writeRepository.UpdateAsync(task);
                if(await _writeRepository.SaveAsync() > 0)
                {
                    var cacheKey = $"List_Tasks_{user.Id}";
                    _memoryCache.Remove(cacheKey);
                    _apiResponse.IsSucces = true;
                    _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.OK;
                    return _apiResponse;
                }
                _apiResponse.IsSucces = false;
                _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessage.Add("Ekleme işlemi başarılı değil");
                return _apiResponse;
            }
            _apiResponse.IsSucces = false;
            _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.NotFound;
            _apiResponse.ErrorMessage.Add("Task bulunamadı");
            return _apiResponse;

        }
        public async Task<ApiResponse> DeleteTasks(Guid id)
        {
            if (GetUserId() == Guid.Empty)
            {
                _apiResponse.IsSucces = false;
                _apiResponse.ErrorMessage.Add("Kullanıcı girişi yapılmamıştır");
                _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.BadRequest;
                return _apiResponse;
            }
            var user=await _userManager.FindByIdAsync(GetUserId().ToString()); 
            if (await _writeRepository.Table.FirstOrDefaultAsync(x => x.Id == id)!=null)
            {
                var tasks = _writeRepository.Delete(id);
                if(await _writeRepository.SaveAsync() > 0)
                {
                    var cacheKey = $"List_Tasks_{user.Id}";
                    _memoryCache.Remove(cacheKey);
                    _apiResponse.IsSucces = true;
                    _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.OK;
                    return _apiResponse;
                }
            }
            _apiResponse.IsSucces = false;
            _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.NotFound;
            _apiResponse.ErrorMessage.Add("Task bulunamadı");
            return _apiResponse;
        }
        public async Task<ApiResponse> GetTaskCaching()
        {
            if (GetUserId() == Guid.Empty)
            {
                _apiResponse.IsSucces = false;
                _apiResponse.ErrorMessage.Add("Kullanıcı girişi yapılmamıştır");
                _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.BadRequest;
                return _apiResponse;
            }
            var user=await _UserReadRepository.GetByIdAsync(GetUserId());
            var cacheKey = $"List_Tasks_{user.Id}";
            if (!_memoryCache.TryGetValue(cacheKey, out List<Tasks> tasks)) 
            { 
                tasks=await _writeRepository.Table.Where(x=>x.UserId==user.Id).ToListAsync();
                _memoryCache.Set(cacheKey, tasks, new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(5)
                });
               
            }
            _apiResponse.result = tasks;
            _apiResponse.HttpStatusCode = System.Net.HttpStatusCode.OK;
            _apiResponse.IsSucces = true;
            return _apiResponse;
        }
    }

}
