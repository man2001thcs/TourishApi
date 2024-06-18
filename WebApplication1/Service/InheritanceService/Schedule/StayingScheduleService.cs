﻿using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using WebApplication1.Data;
using WebApplication1.Model;
using WebApplication1.Model.Schedule;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.InheritanceRepo;
using StackExchange.Redis;

namespace TourishApi.Service.InheritanceService.Schedule
{
    public class StayingScheduleService
    {
        private readonly TourishOuterScheduleRepository _entityRepository;
        private readonly NotificationService _notificationService;
        private readonly IDatabase _redisDatabase;

        public StayingScheduleService(TourishOuterScheduleRepository entityRepository, NotificationService notificationService, IConnectionMultiplexer connectionMultiplexer)
        {
            _entityRepository = entityRepository;
            _notificationService = notificationService;
            _redisDatabase = connectionMultiplexer.GetDatabase();
        }

        public async Task<Response> CreateNew(string userId, StayingScheduleModel entityModel)
        {
            try
            {
                var entityExist = _entityRepository.getByNameStayingSchedule(entityModel.PlaceName);

                if (entityExist.Data == null)
                {
                    var response = await _entityRepository.AddStayingSchedule(userId, entityModel);

                    var notification = new NotificationModel
                    {
                        UserCreateId = new Guid(userId),
                        UserReceiveId = new Guid(userId),
                        StayingScheduleId = response.returnId,
                        Content = "",
                        ContentCode = "I431",
                        IsGenerate = true,
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
                await _entityRepository.DeleteStayingSchedule(id);
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

        public async Task<Response> GetAll(string? search, int? type, double? priceFrom,
            double? priceTo, string? sortBy, string? sortDirection, string? userId, int page = 1, int pageSize = 5)
        {
            try
            {
                if (String.IsNullOrEmpty(userId))
                {
                    string cacheKey =
                        $"staying_service_list_{search ?? ""}_{type ?? 0}_{priceFrom ?? 0}_{priceTo ?? 0}_{sortBy ?? ""}_{sortDirection ?? ""}_page_{page}_pageSize_{pageSize}";
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

                    var result = _entityRepository.GetAllStayingSchedule(search, type, priceFrom, priceTo, sortBy, sortDirection, page, pageSize);
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
                    var entityList = _entityRepository.GetAllStayingScheduleWithAuthority(search, type, sortBy, sortDirection, userId, page, pageSize);
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
                var entity = _entityRepository.getByStayingScheduleId(id);
                if (entity.Data == null)
                {
                    var response = new Response
                    {
                        resultCd = 1,
                        MessageCode = "C430",
                    };
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
                string cacheKey = $"staying_service_{id}";
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

                var result = _entityRepository.clientGetByStayingScheduleId(id);
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


        public async Task<Response> UpdateEntityById(string userId, StayingScheduleModel entityModel)
        {
            try
            {
                var response = await _entityRepository.UpdateStayingSchedule(userId, entityModel);

                if (response.MessageCode == "I432")
                {
                    var interestList = await _entityRepository.getScheduleInterest(entityModel.Id, ScheduleType.StayingSchedule);

                    foreach (var interest in interestList)
                    {
                        if (interest.User.Role == UserRole.User)
                        {
                            var isInNeedOfNotify = await _entityRepository.checkArrangeScheduleFromUser(
                                interest.User.Email,
                                entityModel.Id,
                                ScheduleType.MovingSchedule,
                                new List<string>()
                            );
                            if (!isInNeedOfNotify)
                                continue;
                        }

                        if (response.Change != null)
                        {
                            // Create property change notification
                            if (response.Change.propertyChangeList.Count > 0)
                            {
                                await CreateNotification(userId, interest.UserId, null, entityModel.Id, "I432");
                            }

                            // Create schedule change notification
                            if (response.Change.scheduleChangeList.Count > 0 &&
                                interest.User.Role == UserRole.User && await _entityRepository.checkArrangeScheduleFromUser(
                                    interest.User.Email,
                                    entityModel.Id,
                                    ScheduleType.MovingSchedule,
                                    response.Change.scheduleChangeList
                                ))
                            {
                                await CreateNotification(userId, interest.UserId, null, entityModel.Id, "I432-schedule");
                            }

                            if (response.Change.scheduleChangeList.Count > 0 &&
                                (interest.User.Role == UserRole.Admin || interest.User.Role == UserRole.AdminManager))
                            {
                                await CreateNotification(userId, interest.UserId, null, entityModel.Id, "I432-schedule");
                            }

                            if (response.Change.isNewScheduleAdded &&
                               (interest.User.Role == UserRole.Admin || interest.User.Role == UserRole.AdminManager))
                            {
                                await CreateNotification(userId, interest.UserId, null, entityModel.Id, "I432-new-schedule");
                            }
                        }
                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C434",
                    Error = ex.Message,
                    Data = ex
                };
                return response;
            }
        }

        public async Task<int> CreateNotification(string userId, Guid userReceiveId, Guid? movingScheduleId, Guid? stayingScheduleId, string contentCode)
        {
            var notification = new NotificationModel
            {
                UserCreateId = new Guid(userId),
                UserReceiveId = userReceiveId,
                MovingScheduleId = movingScheduleId,
                StayingScheduleId = stayingScheduleId,
                IsGenerate = true,
                Content = "",
                ContentCode = contentCode,
                IsRead = false,
                IsDeleted = false,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow
            };

            await _notificationService.CreateNewAsync(userReceiveId, notification);

            return 1;
        }

        public Response getScheduleInterest(Guid scheduleId, Guid userId)
        {
            return _entityRepository.getScheduleInterest(scheduleId, ScheduleType.StayingSchedule, userId);
        }

        public Task<Response> setScheduleInterest(Guid scheduleId, Guid userId, InterestStatus interestStatus)
        {
            return _entityRepository.setScheduleInterest(scheduleId, ScheduleType.StayingSchedule, userId, interestStatus);
        }

        public async Task<Response> UpdateInstructionList(
            ScheduleInstructionModel scheduleInstructionModel
        )
        {
            return await _entityRepository.UpdateInstructionList(scheduleInstructionModel);
        }
    }
}