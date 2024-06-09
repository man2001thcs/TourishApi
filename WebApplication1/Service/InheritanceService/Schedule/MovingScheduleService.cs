using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;
using WebApplication1.Data;
using WebApplication1.Model;
using WebApplication1.Model.Schedule;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.InheritanceRepo;

namespace TourishApi.Service.InheritanceService.Schedule
{
    public class MovingScheduleService
    {
        private readonly TourishOuterScheduleRepository _entityRepository;
        private readonly NotificationService _notificationService;
        private readonly IDatabase _redisDatabase;

        public MovingScheduleService(
            TourishOuterScheduleRepository entityRepository,
            NotificationService notificationService,
            IConnectionMultiplexer connectionMultiplexer
        )
        {
            _entityRepository = entityRepository;
            _notificationService = notificationService;
            _redisDatabase = connectionMultiplexer.GetDatabase();
        }

        public async Task<Response> CreateNew(string userId, MovingScheduleModel entityModel)
        {
            try
            {
                var entityExist = _entityRepository.getByNameMovingSchedule(entityModel.BranchName);

                if (entityExist.Data == null)
                {
                    var response = await _entityRepository.AddMovingSchedule(userId, entityModel);

                    var notification = new NotificationModel
                    {
                        UserCreateId = new Guid(userId),
                        UserReceiveId = new Guid(userId),
                        MovingScheduleId = response.returnId,
                        IsGenerate = true,
                        Content = "",
                        ContentCode = "I431",
                        IsRead = false,
                        IsDeleted = false,
                        CreateDate = DateTime.UtcNow,
                        UpdateDate = DateTime.UtcNow
                    };

                    await _notificationService.CreateNewAsync(new Guid(userId), notification);

                    return (response);
                }
                else
                {
                    var response = new Response { resultCd = 1, MessageCode = "C431", };
                    return response;
                }
            }
            catch (Exception ex)
            {
                return new Response
                {
                    resultCd = 1,
                    MessageCode = "C434",
                    Error = ex.Message
                };
            }
        }

        public async Task<Response> DeleteById(Guid id)
        {
            try
            {
                await _entityRepository.DeleteMovingSchedule(id);
                var response = new Response { resultCd = 0, MessageCode = "I433" };
                return response;
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C434",
                    Error = ex.Message
                };
                return response;
            }
        }

        public async Task<Response> GetAll(
            string? search,
            int? type,
            double? priceFrom,
            double? priceTo,
            string? sortBy,
            string? sortDirection,
            string? userId,
            int page = 1,
            int pageSize = 5
        )
        {
            try
            {
                if (String.IsNullOrEmpty(userId))
                {
                    string cacheKey =
                        $"moving_service_list_{search ?? ""}_{type ?? 0}_{priceFrom ?? 0}_{priceTo ?? 0}_{sortBy ?? ""}_{sortDirection ?? ""}_page_{page}_pageSize_{pageSize}";
                    string cachedValue = await _redisDatabase.StringGetAsync(cacheKey);

                    if (!string.IsNullOrEmpty(cachedValue))
                    {
                        var resultCache =
                            JsonConvert.DeserializeObject<WebApplication1.Model.VirtualModel.Response>(
                                cachedValue
                            );
                        if (resultCache != null)
                        {
                            return resultCache;
                        }
                    }

                    var result = _entityRepository.GetAllMovingSchedule(
                        search,
                        type,
                        priceFrom,
                        priceTo,
                        sortBy,
                        sortDirection,
                        page,
                        pageSize
                    );
                    string resultJson = JsonConvert.SerializeObject(
                        result,
                        new JsonSerializerSettings
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                            ContractResolver = new CamelCasePropertyNamesContractResolver()
                        }
                    );

                    // Cache the result in Redis
                    await _redisDatabase.StringSetAsync(
                        cacheKey,
                        resultJson,
                        TimeSpan.FromMinutes(60)
                    ); // Cache for 10 minutes
                    return result;                 
                }
                else
                {
                    var entityList = _entityRepository.GetAllMovingScheduleWithAuthority(
                        search,
                        type,
                        sortBy,
                        sortDirection,
                        userId,
                        page,
                        pageSize
                    );
                    return entityList;
                }
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C434",
                    Error = ex.Message
                };
                return response;
            }
        }

        public Response GetById(Guid id)
        {
            try
            {
                var entity = _entityRepository.getByMovingScheduleId(id);
                if (entity.Data == null)
                {
                    var response = new Response { resultCd = 1, MessageCode = "C430", };
                    return response;
                }
                else
                {
                    return entity;
                }
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C434",
                    Error = ex.Message
                };
                return response;
            }
        }

        public async Task<Response> clientGetById(Guid id)
        {
            try
            {
                string cacheKey = $"moving_service_{id}";
                string cachedValue = await _redisDatabase.StringGetAsync(cacheKey);

                if (!string.IsNullOrEmpty(cachedValue))
                {
                    var resultCache =
                        JsonConvert.DeserializeObject<WebApplication1.Model.VirtualModel.Response>(
                            cachedValue
                        );
                    if (resultCache != null)
                    {
                        return resultCache;
                    }
                }

                var result = _entityRepository.clientGetByMovingScheduleId(id);
                if (result.Data == null)
                {
                    var response = new Response { resultCd = 1, MessageCode = "C430", };
                    return response;
                }

                string resultJson = JsonConvert.SerializeObject(
                    result,
                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    }
                );

                // Cache the result in Redis
                await _redisDatabase.StringSetAsync(cacheKey, resultJson, TimeSpan.FromMinutes(60)); // Cache for 10 minutes

                return result;
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C434",
                    Error = ex.Message
                };
                return response;
            }
        }

        public async Task<Response> UpdateEntityById(string userId, MovingScheduleModel entityModel)
        {
            try
            {
                var response = await _entityRepository.UpdateMovingSchedule(userId, entityModel);
                if (response.MessageCode == "I432")
                {
                    var interestList = await _entityRepository.getScheduleInterest(
                        entityModel.Id,
                        ScheduleType.MovingSchedule
                    );

                    foreach (var interest in interestList)
                    {
                        if (interest.User.Role == UserRole.User)
                        {
                            var isInNeedOfNotify = _entityRepository.checkArrangeScheduleFromUser(
                                interest.User.Email,
                                entityModel.Id,
                                ScheduleType.MovingSchedule
                            );
                            if (!isInNeedOfNotify)
                                continue;
                        }

                        var notification = new NotificationModel
                        {
                            UserCreateId = new Guid(userId),
                            UserReceiveId = interest.UserId,
                            MovingScheduleId = entityModel.Id,
                            Content = "",
                            IsGenerate = true,
                            ContentCode = "I432",
                            IsRead = false,
                            IsDeleted = false,
                            CreateDate = DateTime.UtcNow,
                            UpdateDate = DateTime.UtcNow
                        };

                        // _notificationService.CreateNew(notification);

                        await _notificationService.CreateNewAsync(interest.UserId, notification);
                    }
                    ;
                }

                return response;
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C434",
                    Error = ex.Message
                };
                return response;
            }
        }

        public Response getScheduleInterest(Guid scheduleId, Guid userId)
        {
            return _entityRepository.getScheduleInterest(
                scheduleId,
                ScheduleType.MovingSchedule,
                userId
            );
        }

        public Task<Response> setScheduleInterest(
            Guid scheduleId,
            Guid userId,
            InterestStatus interestStatus
        )
        {
            return _entityRepository.setScheduleInterest(
                scheduleId,
                ScheduleType.MovingSchedule,
                userId,
                interestStatus
            );
        }

        public async Task<Response> UpdateInstructionList(
            ScheduleInstructionModel scheduleInstructionModel
        )
        {
            return await _entityRepository.UpdateInstructionList(scheduleInstructionModel);
        }
    }
}
