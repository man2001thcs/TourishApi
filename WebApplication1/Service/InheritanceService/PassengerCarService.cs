using TourishApi.Service.Interface;
using WebApplication1.Model.Transport;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.InheritanceRepo.Transport;

namespace TourishApi.Service.InheritanceService
{
    public class PassengerCarService : IBaseService<PassengerCarRepository, PassengerCarModel>
    {
        private readonly PassengerCarRepository _entityRepository;

        public PassengerCarService(PassengerCarRepository airPlaneRepository)
        {
            _entityRepository = airPlaneRepository;
        }

        public Response CreateNew(PassengerCarModel entityModel)
        {
            try
            {
                var entityExist = _entityRepository.getByName(entityModel.BranchName);

                if (entityExist.Data == null)
                {
                    var response = _entityRepository.Add(entityModel);

                    return (response);
                }
                else
                {
                    var response = new Response { resultCd = 1, MessageCode = "C311", };
                    return response;
                }
            }
            catch (Exception ex)
            {
                return new Response
                {
                    resultCd = 1,
                    MessageCode = "C124",
                    Error = ex.Message
                };
            }
        }

        public Response DeleteById(Guid id)
        {
            try
            {
                _entityRepository.Delete(id);
                var response = new Response { resultCd = 0, MessageCode = "I313" };
                return response;
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C124",
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
                    MessageCode = "C124",
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
                    var response = new Response { resultCd = 1, MessageCode = "C120", };
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
                    MessageCode = "C314",
                    Error = ex.Message
                };
                return response;
            }
        }

        public Response UpdateEntityById(Guid id, PassengerCarModel PassengerCarModel)
        {
            try
            {
                var response = _entityRepository.Update(PassengerCarModel);

                return response;
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C124",
                    Error = ex.Message
                };
                return response;
            }
        }
    }
}
