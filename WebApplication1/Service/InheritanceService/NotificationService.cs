using TourishApi.Service.Interface;
using WebApplication1.Data;
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

        public List<Notification> getByTourRecentUpdate(Guid tourId, Guid modifiedId)
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

    }
}
