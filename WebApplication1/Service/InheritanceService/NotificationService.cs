using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.SignalR;
using SignalR.Hub;
using SignalR.Hub.Client;
using System.Diagnostics;
using TourishApi.Service.Interface;
using WebApplication1.Data.Connection;
using WebApplication1.Model;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.InheritanceRepo;


namespace TourishApi.Service.InheritanceService
{
    public class NotificationService : IBaseService<NotificationRepository, NotificationModel>
    {
        private readonly NotificationRepository _entityRepository;
        private IHubContext<NotificationHub, INotificationHubClient> _notificationHub;

        public NotificationService(NotificationRepository airPlaneRepository, IHubContext<NotificationHub, INotificationHubClient> notificationHub)
        {
            _entityRepository = airPlaneRepository;
            _notificationHub = notificationHub;
        }

        public Response CreateNew(NotificationModel entityModel)
        {
            try
            {
                var response = _entityRepository.Add(entityModel);

                return (response);
            }
            catch (Exception ex)
            {
                return new Response
                {
                    resultCd = 1,
                    MessageCode = "C704",
                    Error = ex.Message
                };
            }
        }
        public async Task<Response> CreateNewAsync(Guid userReceiveId,NotificationModel entityModel)
        {
            try
            {
                var response = await _entityRepository.AddNotifyAsync(entityModel);

                if (response.MessageCode == "I701")
                {
                    await sendNotify(userReceiveId, response.returnId ?? new Guid());
                }


                //var notificationDTOUpdate = new NotificationDTOModel
                //{
                //    UserCreateId = entityModel.UserCreateId,
                //    UserReceiveId = entityModel.UserReceiveId,
                //    Content = entityModel.Content,
                //    ContentCode = entityModel.ContentCode,
                //    IsRead = entityModel.IsRead,
                //    IsDeleted = entityModel.IsDeleted,
                //    CreateDate = entityModel.CreateDate,
                //    UpdateDate = entityModel.UpdateDate
                //};

                //Debug.WriteLine("Here");
                //var connection = await _entityRepository.getNotificationConAsync(userReceiveId);

                //await _notificationHub
                //    .Clients.Client(connection.ConnectionID)
                //    .SendOffersToUser(userReceiveId, notificationDTOUpdate);

                return (response);
            }
            catch (Exception ex)
            {
                return new Response
                {
                    resultCd = 1,
                    MessageCode = "C704",
                    Error = ex.Message
                };
            }
        }

        private async System.Threading.Tasks.Task sendNotify(Guid userReceiveId, Guid notifyId)
        {
            var connection = await _entityRepository.getNotificationConAsync(userReceiveId);
            var fullDetailNotification = await _entityRepository.getByIdAsync(notifyId);

            if (connection != null)
            {
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

                    Debug.WriteLine("Here");
                    Debug.WriteLine(fullDetailNotification.ToString());

                    await _notificationHub
                        .Clients.Client(connection.ConnectionID)
                        .SendOffersToUser(userReceiveId, notificationDTOUpdate);
                }
            }

            // await sendFcmNotificationAsync(fullDetailNotification);
 
        }

        public Response DeleteById(Guid id)
        {
            try
            {
                _entityRepository.Delete(id);
                var response = new Response { resultCd = 0, MessageCode = "I703" };
                return response;
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C704",
                    Error = ex.Message
                };
                return response;
            }
        }

        public Response GetAll(string? search, string? sortBy, int page = 1, int pageSize = 5)
        {
            try
            {
                var entityList = _entityRepository.GetAll(search, sortBy, page, pageSize);
                return entityList;
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C704",
                    Error = ex.Message
                };
                return response;
            }
        }


        public Response GetAllForReceiver(string? userId, string? sortBy, int page = 1, int pageSize = 5)
        {
            try
            {
                var entityList = _entityRepository.GetAllForReceiver(userId, sortBy, page, pageSize);
                return entityList;
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C314",
                    Error = ex.Message
                };
                return response;
            }
        }

        public Response GetAllForCreator(string? userId, string? sortBy, int page = 1, int pageSize = 5)
        {
            try
            {
                var entityList = _entityRepository.GetAllForCreator(userId, sortBy, page, pageSize);
                return entityList;
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C314",
                    Error = ex.Message
                };
                return response;
            }
        }

        public List<WebApplication1.Data.Notification> getByTourRecentUpdate(Guid tourId, Guid modifiedId)
        {
            var entity = _entityRepository.getByTourRecentUpdate(tourId, modifiedId);
            return entity;
        }

        public Response GetById(Guid id)
        {
            try
            {
                var entity = _entityRepository.getById(id);
                if (entity.Data == null)
                {
                    var response = new Response
                    {
                        resultCd = 1,
                        MessageCode = "C700",
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
                    MessageCode = "C704",
                    Error = ex.Message
                };
                return response;
            }
        }

        public NotificationCon getNotificationCon(Guid userReceiveId)
        {
            return _entityRepository.getNotificationCon(userReceiveId);
        }

        public Response UpdateEntityById(Guid id, NotificationModel NotificationModel)
        {
            try
            {
                var response = _entityRepository.Update(NotificationModel);

                return response;
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C704",
                    Error = ex.Message
                };
                return response;
            }
        }

        public async Task<Response> UpdateEntityByIdAsync(Guid id, NotificationModel NotificationModel)
        {
            try
            {
                var response = await _entityRepository.UpdateAsync(NotificationModel);

                var connection = getNotificationCon(
                                NotificationModel.UserReceiveId ?? new Guid()
                            );

                var fullDetailNotification = (WebApplication1.Data.Notification)
                       GetById(NotificationModel.Id ?? new Guid()).Data;

                if (connection != null)
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
                        .SendOffersToUser(NotificationModel.UserReceiveId ?? new Guid(), notificationDTOUpdate);
                }

                sendFcmNotificationAsync(fullDetailNotification);

                return response;
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C704",
                    Error = ex.Message
                };
                return response;
            }
        }

        public Response saveFcmToken(NotificationFcmTokenModel notificationFcmTokenModel)
        {
            return _entityRepository.saveFcmToken(notificationFcmTokenModel);
        }

        public async System.Threading.Tasks.Task sendFcmNotificationAsync(WebApplication1.Data.Notification notification)
        {
            var fcmToken = _entityRepository.GetFcmToken(notification.UserReceiveId ?? new Guid());

            if (fcmToken != null)
            {
                var message = new FirebaseAdmin.Messaging.Message()
                {
                    Notification = new FirebaseAdmin.Messaging.Notification
                    {
                        Title = "Roxanne thông báo",
                        Body = getContent(notification),
                    },
                    Token = fcmToken.DeviceToken
                };

                var messaging = FirebaseMessaging.DefaultInstance;
                var result = await messaging.SendAsync(message);
            }
        }

        public string getContent(WebApplication1.Data.Notification notification)
        {


            if (notification.Content != null)
            {
                return notification.Content;
            }

            if (notification.ContentCode != null)
            {

                return Constant.NotificationCode.NOTIFI_CODE_VI[notification.ContentCode] + notification.TourishPlan.TourName;
            }


            return "";
        }
    }
}
