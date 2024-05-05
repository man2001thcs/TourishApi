using WebApplication1.Data;
using WebApplication1.Model;
using WebApplication1.Model.Schedule;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.InheritanceRepo;

namespace TourishApi.Service.InheritanceService.Schedule
{
    public class StayingScheduleService
    {
        private readonly TourishOuterScheduleRepository _entityRepository;
        private readonly NotificationService _notificationService;

        public StayingScheduleService(TourishOuterScheduleRepository entityRepository, NotificationService notificationService)
        {
            _entityRepository = entityRepository;
            _notificationService = notificationService;
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

        public Response GetAll(string? search, int? type, string? sortBy, string? sortDirection, string? userId, int page = 1, int pageSize = 5)
        {
            try
            {
                if (String.IsNullOrEmpty(userId))
                {
                    var entityList = _entityRepository.GetAllStayingSchedule(search, type, sortBy, sortDirection, page, pageSize);
                    return entityList;
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
                        var notification = new NotificationModel
                        {
                            UserCreateId = new Guid(userId),
                            UserReceiveId = interest.UserId,
                            StayingScheduleId = entityModel.Id,
                            Content = "",
                            ContentCode = "I432",
                            IsRead = false,
                            IsDeleted = false,
                            CreateDate = DateTime.UtcNow,
                            UpdateDate = DateTime.UtcNow
                        };

                        await _notificationService.CreateNewAsync(interest.UserId, notification);
                    };
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