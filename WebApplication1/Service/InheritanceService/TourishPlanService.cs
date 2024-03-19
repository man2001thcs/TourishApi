using TourishApi.Service.Interface;
using WebApplication1.Model;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface;

namespace TourishApi.Service.InheritanceService
{
    public class TourishPlanService : ITourishPlanService
    {
        private readonly ITourishPlanRepository _entityRepository;

        public TourishPlanService(ITourishPlanRepository tourishPlanRepository)
        {
            _entityRepository = tourishPlanRepository;
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
}
