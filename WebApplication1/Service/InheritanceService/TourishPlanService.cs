using Microsoft.AspNetCore.SignalR;
using SignalR.Hub;
using SignalR.Hub.Client;
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
        private IHubContext<NotificationHub, INotificationHubClient> _notificationHub;

        public TourishPlanService(
            ITourishPlanRepository tourishPlanRepository,
            NotificationService notificationService,
            IHubContext<NotificationHub, INotificationHubClient> notificationHub
        )
        {
            _entityRepository = tourishPlanRepository;
            _notificationService = notificationService;
            _notificationHub = notificationHub;
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

                    var connection = _notificationService.getNotificationCon(new Guid(userId));

                    var notification = new NotificationModel
                    {
                        UserCreateId = new Guid(userId),
                        UserReceiveId = new Guid(userId),
                        TourishPlanId = response.returnId,
                        Content = "",
                        ContentCode = "I412",
                        IsRead = false,
                        IsDeleted = false,
                        CreateDate = DateTime.UtcNow,
                        UpdateDate = DateTime.UtcNow
                    };

                    var responseNotify = await _notificationService.CreateNewAsync(notification);

                    if (connection != null)
                    {
                        var fullDetailNotification = (Notification)
                            _notificationService
                                .GetById(responseNotify.returnId ?? new Guid())
                                .Data;
                        if (fullDetailNotification != null)
                        {
                            var notificationDTOUpdate = new NotificationDTOModel
                            {
                                Id = fullDetailNotification.Id,
                                UserCreateId = fullDetailNotification.UserCreateId,
                                UserReceiveId = fullDetailNotification.UserReceiveId,
                                TourName = fullDetailNotification.TourishPlan.TourName,
                                CreatorFullName = fullDetailNotification.UserCreator.FullName,
                                Content = fullDetailNotification.Content,
                                ContentCode = fullDetailNotification.ContentCode,
                                IsRead = fullDetailNotification.IsRead,
                                IsDeleted = fullDetailNotification.IsDeleted,
                                CreateDate = fullDetailNotification.CreateDate,
                                UpdateDate = fullDetailNotification.UpdateDate
                            };
                            _notificationHub
                                .Clients.Client(connection.ConnectionID)
                                .SendOffersToUser(new Guid(userId), notificationDTOUpdate);
                        }
                    }
                    return response;
                }
                else
                {
                    var response = new Response { resultCd = 1, MessageCode = "C411", };
                    return response;
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

        public Response GetAll(
            string? search,
            string? category,
            string? sortBy,
            int page = 1,
            int pageSize = 5
        )
        {
            try
            {
                var entityList = _entityRepository.GetAll(search, category, sortBy, page, pageSize);
                return entityList;
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
                var interestList = _entityRepository.getTourInterest(entityModel.Id);

                interestList.ForEach(async interest =>
                {
                    var recentNotification = _notificationService.getByTourRecentUpdate(
                        interest.TourishPlanId,
                        new Guid(userId)
                    );

                    if (recentNotification.Count > 0)
                    {
                        foreach (var notification in recentNotification)
                        {
                            var notificationUpdate = new NotificationModel
                            {
                                Id = notification.Id,
                                UserCreateId = notification.UserCreateId,
                                UserReceiveId = notification.UserReceiveId,
                                TourishPlanId = notification.TourishPlanId,
                                Content = notification.Content,
                                ContentCode = notification.ContentCode,
                                IsRead = notification.IsRead,
                                IsDeleted = notification.IsDeleted,
                                CreateDate = notification.CreateDate,
                                UpdateDate = DateTime.UtcNow
                            };

                            _notificationService.UpdateEntityById(
                                notification.Id,
                                notificationUpdate
                            );

                            var connection = _notificationService.getNotificationCon(
                                interest.UserId
                            );

                            if (connection != null)
                            {
                                var fullDetailNotification = (Notification)
                                    _notificationService.GetById(notification.Id).Data;
                                var notificationDTOUpdate = new NotificationDTOModel
                                {
                                    Id = fullDetailNotification.Id,
                                    UserCreateId = fullDetailNotification.UserCreateId,
                                    UserReceiveId = fullDetailNotification.UserReceiveId,
                                    TourName = fullDetailNotification.TourishPlan.TourName,
                                    CreatorFullName = fullDetailNotification.UserCreator.FullName,
                                    Content = fullDetailNotification.Content,
                                    ContentCode = fullDetailNotification.ContentCode,
                                    IsRead = fullDetailNotification.IsRead,
                                    IsDeleted = fullDetailNotification.IsDeleted,
                                    CreateDate = fullDetailNotification.CreateDate,
                                    UpdateDate = fullDetailNotification.UpdateDate
                                };
                                _notificationHub
                                    .Clients.Client(connection.ConnectionID)
                                    .SendOffersToUser(interest.UserId, notificationDTOUpdate);
                            }
                        }
                    }

                    if (recentNotification.Count == 0 || recentNotification == null)
                    {
                        var notification = new NotificationModel
                        {
                            UserCreateId = new Guid(userId),
                            UserReceiveId = interest.UserId,
                            TourishPlanId = interest.TourishPlanId,
                            Content = "",
                            ContentCode = "I412",
                            IsRead = false,
                            IsDeleted = false,
                            CreateDate = DateTime.UtcNow,
                            UpdateDate = DateTime.UtcNow
                        };

                        var responseNotify = await _notificationService.CreateNewAsync(
                            notification
                        );

                        var connection = _notificationService.getNotificationCon(interest.UserId);

                        if (connection != null)
                        {
                            var fullDetailNotification = (Notification)
                                _notificationService
                                    .GetById(responseNotify.returnId ?? new Guid())
                                    .Data;

                            if (fullDetailNotification != null)
                            {
                                var notificationDTOUpdate = new NotificationDTOModel
                                {
                                    Id = fullDetailNotification.Id,
                                    UserCreateId = fullDetailNotification.UserCreateId,
                                    UserReceiveId = fullDetailNotification.UserReceiveId,
                                    TourName = fullDetailNotification.TourishPlan.TourName,
                                    CreatorFullName = fullDetailNotification.UserCreator.FullName,
                                    Content = fullDetailNotification.Content,
                                    ContentCode = fullDetailNotification.ContentCode,
                                    IsRead = fullDetailNotification.IsRead,
                                    IsDeleted = fullDetailNotification.IsDeleted,
                                    CreateDate = fullDetailNotification.CreateDate,
                                    UpdateDate = fullDetailNotification.UpdateDate
                                };
                                _notificationHub
                                    .Clients.Client(connection.ConnectionID)
                                    .SendOffersToUser(interest.UserId, notificationDTOUpdate);
                            }
                        }
                    }
                });
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
    }
}