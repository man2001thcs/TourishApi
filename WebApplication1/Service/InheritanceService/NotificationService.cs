using Azure.Core;
using FirebaseAdmin.Messaging;
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

        public NotificationService(NotificationRepository airPlaneRepository)
        {
            _entityRepository = airPlaneRepository;
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
        public async Task<Response> CreateNewAsync(NotificationModel entityModel)
        {
            try
            {
                var response = await _entityRepository.AddNotifyAsync(entityModel);

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

        public Response saveFcmToken(NotificationFcmTokenModel notificationFcmTokenModel)
        {
            return _entityRepository.saveFcmToken(notificationFcmTokenModel);
        }

        public async System.Threading.Tasks.Task sendFcmNotificationAsync(NotificationModel notificationModel)
        {
            var fcmToken = _entityRepository.GetFcmToken(notificationModel.UserReceiveId ?? new Guid());

            if (fcmToken != null)
            {
                var message = new FirebaseAdmin.Messaging.Message()
                {
                    Notification = new FirebaseAdmin.Messaging.Notification
                    {
                        Title = "Roxanne thông báo",
                        Body = getContent(notificationModel),
                    },
                    Token = fcmToken.DeviceToken
                };

                var messaging = FirebaseMessaging.DefaultInstance;
                var result = await messaging.SendAsync(message);
            }  
        }

        public string getContent(NotificationModel notification)
        {
            var notify = (WebApplication1.Data.Notification)_entityRepository.getById(notification.Id ?? new Guid()).Data;

            if (notify != null)
            {
                if (notification.Content != null) {
                    return notify.Content; 
                }

                if (notify.ContentCode != null)
                {
                   
                      return Constant.NotificationCode.NOTIFI_CODE_VI[notify.ContentCode] + notify.TourishPlan.TourName;
                }
            }

            return "";
        }
    }
}
