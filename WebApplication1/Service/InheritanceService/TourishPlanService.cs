using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;
using TourishApi.Service.Interface;
using WebApplication1.Data;
using WebApplication1.Model;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface;

namespace TourishApi.Service.InheritanceService
{
    public class TourishPlanService : ITourishPlanService
    {
        private readonly ITourishPlanRepository _entityRepository;
        private readonly NotificationService _notificationService;
        private readonly IDatabase _redisDatabase;

        public TourishPlanService(
            ITourishPlanRepository tourishPlanRepository,
            NotificationService notificationService,
            IConnectionMultiplexer connectionMultiplexer
        )
        {
            _entityRepository = tourishPlanRepository;
            _notificationService = notificationService;
            _redisDatabase = connectionMultiplexer.GetDatabase();
        }

        public async Task<Response> CreateNew(string userId, TourishPlanInsertModel entityModel)
        {
            try
            {
                var entityExist = _entityRepository.getByName(entityModel.TourName);

                // Lấy ID từ token
                // Tiếp tục xử lý logic của bạn ở đây với userId đã lấy được
                if (entityExist.Data == null)
                {
                    var response = await _entityRepository.Add(entityModel, userId);

                    var notification = new NotificationModel
                    {
                        UserCreateId = new Guid(userId),
                        UserReceiveId = new Guid(userId),
                        TourishPlanId = response.returnId,
                        IsGenerate = true,
                        Content = "",
                        ContentCode = "I411",
                        IsRead = false,
                        IsDeleted = false,
                        CreateDate = DateTime.UtcNow,
                        UpdateDate = DateTime.UtcNow
                    };

                    await _notificationService.CreateNewAsync(new Guid(userId), notification);

                    return response;
                }
                else
                {
                    var response = new Response
                    {
                        resultCd = 1,
                        MessageCode = "C411",
                        Error = "Exist"
                    };
                    return response;
                }
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C414",
                    Error = ex.Message,
                    Data = ex
                };
                return response;
            }
        }

        public Response DeleteById(Guid id)
        {
            {
                try
                {
                    _entityRepository.Delete(id);
                    var response = new Response { resultCd = 0, MessageCode = "I413", };
                    return response;
                }
                catch (Exception ex)
                {
                    var response = new Response
                    {
                        resultCd = 1,
                        MessageCode = "C414",
                        Error = ex.Message
                    };
                    return response;
                }
            }
        }

        public async Task<Response> GetAll(
            string? search,
            string? category,
            string? categoryString,
            string? startingPoint,
            string? endPoint,
            string? startingDate,
            double? priceFrom,
            double? priceTo,
            string? sortBy,
            string? sortDirection,
            string? userId,
            int page,
            int pageSize
        )
        {
            try
            {
                if (String.IsNullOrEmpty(userId))
                {
                    string cacheKey =
                        $"tourish_plan_list_{search ?? ""}_{category ?? ""}_{categoryString ?? ""}_{startingPoint ?? ""}_{endPoint ?? ""}_{startingDate ?? ""}_{priceFrom ?? 0}_{priceTo ?? 0}_{sortBy ?? ""}_{sortDirection ?? ""}_page_{page}_pageSize_{pageSize}";
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

                    var result = _entityRepository.GetAll(
                        search,
                        category,
                        categoryString,
                        startingPoint,
                        endPoint,
                        startingDate,
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
                    var entityList = _entityRepository.GetAllWithAuthority(
                        search,
                        category,
                        categoryString,
                        startingPoint,
                        endPoint,
                        startingDate,
                        priceFrom,
                        priceTo,
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
                    MessageCode = "C414",
                    Error = ex.Message
                };
                return response;
            }
        }

        public async Task<Response> clientGetById(Guid id)
        {
            try
            {
                string cacheKey = $"tourish_plan_{id}";
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

                var result = _entityRepository.clientGetById(id);
                if (result.Data == null)
                {
                    var response = new Response { resultCd = 1, MessageCode = "C410", };
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
                    MessageCode = "C414",
                    Error = ex.Message
                };
                return response;
            }
        }

        public Response GetById(Guid id)
        {
            try
            {
                var entity = _entityRepository.getById(id);
                if (entity.Data == null)
                {
                    var response = new Response { resultCd = 1, MessageCode = "C410", };
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
                    MessageCode = "C414",
                    Error = ex.Message
                };
                return response;
            }
        }

        public async Task<Response> UpdateEntityById(
            string userId,
            TourishPlanUpdateModel entityModel
        )
        {
            try
            {
                var response = await _entityRepository.Update(entityModel, userId);
                if (response.MessageCode == "I412")
                {
                    var interestList = await _entityRepository.getTourInterest(entityModel.Id);

                    foreach (var interest in interestList)
                    {
                        if (interest.User.Role == UserRole.User)
                        {
                            var isInNeedOfNotify = _entityRepository.checkArrangeScheduleFromUser(
                                interest.User.Email,
                                entityModel.Id
                            );
                            if (!isInNeedOfNotify)
                                continue;
                        }

                        var notification = new NotificationModel
                        {
                            UserCreateId = new Guid(userId),
                            UserReceiveId = interest.UserId,
                            TourishPlanId = entityModel.Id,
                            IsGenerate = true,
                            Content = "",
                            ContentCode = "I412",
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
                    MessageCode = "C414",
                    Error = ex.Message,
                    Data = ex
                };
                return response;
            }
        }

        public async Task<Response> getDescription(string containerName, string blobName)
        {          
            var result = new Response
            {
                resultCd = 0,
                Data = await _entityRepository.getDescription(containerName, blobName)
            };

            return result;
        }

        public async Task<Response> clientGetDescription(string containerName, string blobName)
        {
            string cacheKey = $"tourish_plan_des_{containerName}_{blobName}";
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

            var result = new Response
            {
                resultCd = 0,
                Data = await _entityRepository.getDescription(containerName, blobName)
            };

            string resultJson = JsonConvert.SerializeObject(result, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            // Cache the result in Redis
            await _redisDatabase.StringSetAsync(cacheKey, resultJson, TimeSpan.FromMinutes(10));

            return result;
        }

        public Response getTourInterest(Guid tourId, Guid userId)
        {
            return _entityRepository.getTourInterest(tourId, userId);
        }

        public Task<Response> setTourInterest(
            Guid tourId,
            Guid userId,
            InterestStatus interestStatus
        )
        {
            return _entityRepository.setTourInterest(tourId, userId, interestStatus);
        }

        public Response getTopTourRating()
        {
            try
            {
                return _entityRepository.getTopTourRating();
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C414",
                    Error = ex.Message,
                    Data = ex
                };
                return response;
            }
        }
    }
}
