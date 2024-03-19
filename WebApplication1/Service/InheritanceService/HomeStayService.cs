using TourishApi.Service.Interface;
using WebApplication1.Model.RestHouse;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.InheritanceRepo.RestHoouse;

namespace TourishApi.Service.InheritanceService
{
    public class HomeStayService : IBaseService<HomeStayRepository, HomeStayModel>
    {
        private readonly HomeStayRepository _entityRepository;

        public HomeStayService(HomeStayRepository airPlaneRepository)
        {
            _entityRepository = airPlaneRepository;
        }

        public Response CreateNew(HomeStayModel entityModel)
        {
            try
            {
                var entityExist = _entityRepository.getByName(entityModel.PlaceBranch);

                if (entityExist.Data == null)
                {
                    var response = _entityRepository.Add(entityModel);

                    return (response);
                }
                else
                {
                    var response = new Response { resultCd = 1, MessageCode = "C221", };
                    return response;
                }
            }
            catch (Exception ex)
            {
                return new Response
                {
                    resultCd = 1,
                    MessageCode = "C224",
                    Error = ex.Message
                };
            }
        }

        public Response DeleteById(Guid id)
        {
            try
            {
                _entityRepository.Delete(id);
                var response = new Response { resultCd = 0, MessageCode = "I223" };
                return response;
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C224",
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
                    MessageCode = "C224",
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
                    var response = new Response
                    {
                        resultCd = 1,
                        MessageCode = "C220",
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
                    MessageCode = "C224",
                    Error = ex.Message
                };
                return response;
            }
        }

        public Response UpdateEntityById(Guid id, HomeStayModel HomeStayModel)
        {

            try
            {
                var response = _entityRepository.Update(HomeStayModel);

                return response;
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C224",
                    Error = ex.Message
                };
                return response;
            }



        }
    }
}
