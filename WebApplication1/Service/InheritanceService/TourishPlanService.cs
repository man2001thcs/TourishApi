﻿using TourishApi.Service.Interface;
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

        public TourishPlanService(
            ITourishPlanRepository tourishPlanRepository,
            NotificationService notificationService
        )
        {
            _entityRepository = tourishPlanRepository;
            _notificationService = notificationService;
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
                    var response = new Response { resultCd = 1, MessageCode = "C411", Error = "Exist" };
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

        public Response GetAll(
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
                    var entityList = _entityRepository.GetAll(
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
                    return entityList;
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
                            var isInNeedOfNotify = _entityRepository.checkArrangeScheduleFromUser(interest.User.Email, entityModel.Id);
                            if (!isInNeedOfNotify) continue;
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
            var response = new Response
            {
                resultCd = 0,
                Data = await _entityRepository.getDescription(containerName, blobName)
            };
            return response;
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
