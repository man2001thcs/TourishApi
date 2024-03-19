using TourishApi.Service.Interface;
using WebApplication1.Model.RestHouse;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.InheritanceRepo.RestHoouse;

namespace TourishApi.Service.InheritanceService
{
    public class HotelService : IBaseService<HotelRepository, HotelModel>
    {
        private readonly HotelRepository _entityRepository;

        public HotelService(HotelRepository airPlaneRepository)
        {
            _entityRepository = airPlaneRepository;
        }

        public Response CreateNew(HotelModel entityModel)
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
                    var response = new Response { resultCd = 1, MessageCode = "C211", };
                    return response;
                }
            }
            catch (Exception ex)
            {
                return new Response
                {
                    resultCd = 1,
                    MessageCode = "C214",
                    Error = ex.Message
                };
            }
        }

        public Response DeleteById(Guid id)
        {
            try
            {
                _entityRepository.Delete(id);
                var response = new Response { resultCd = 0, MessageCode = "I213" };
                return response;
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C214",
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
                    MessageCode = "C214",
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
                        MessageCode = "C210",
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
                    MessageCode = "C214",
                    Error = ex.Message
                };
                return response;
            }
        }

        public Response UpdateEntityById(Guid id, HotelModel HotelModel)
        {

            try
            {
                var response = _entityRepository.Update(HotelModel);

                return response;
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C214",
                    Error = ex.Message
                };
                return response;
            }



        }
    }
}
