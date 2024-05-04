using Microsoft.EntityFrameworkCore;
using TourishApi.Extension;
using WebApplication1.Data;
using WebApplication1.Data.DbContextFile;
using WebApplication1.Data.Schedule;
using WebApplication1.Model;
using WebApplication1.Model.Schedule;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Service;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApplication1.Repository.InheritanceRepo
{
    public class TourishOuterScheduleRepository
    {
        private readonly MyDbContext _context;
        private readonly IBlobService _blobService;

        private readonly string[] priceInstructionLines = new string[]
        {
            "Hộ chiếu phải còn thời hạn sử dụng trên 6 tháng, Tính từ ngày khởi hành đi và về.",
            "Hành lý quá cước qui định. Xe vận chuyển ngoài chương trình + Các show về đêm.",
            "Điện thoại, giặt ủi, nước uống trong phòng khách sạn và các chi phí cá nhân khác.",
        };

        private readonly string[] cautionInstructionLines = new string[]
        {
            "Du khách Việt Kiều hoặc nước ngoài phải có visa tái nhập nhiều lần hoặc miễn thị thực 5 năm.",
            "Trẻ em dưới 16 tuổi phải có bố mẹ đi cùng hoặc người được uỷ quyền có giấy uỷ quyền từ bố mẹ.",
            "Hộ chiếu phải mang theo bản gốc hợp lệ không bị rạn, phai mờ, và còn thời hạn sử dụng trên 6 tháng (tính từ ngày khởi hành).",
            "Không sử dụng thẻ xanh. Nếu sử dụng Sổ Du lịch (yêu cầu visa nước nhập cảnh), vui lòng thông báo cho nhân viên nhận tour nếu Quý khách sử dụng các hồ sơ khác ngoài hộ chiếu.",
            "Công ty du lịch không chịu trách nhiệm nếu Quý khách bị từ chối nhập cảnh với bất kỳ lý do nào từ hải quan nước ngoài.",
        };

        public static int PAGE_SIZE { get; set; } = 5;

        public TourishOuterScheduleRepository(MyDbContext _context, IBlobService blobService)
        {
            this._context = _context;
            _blobService = blobService;
        }

        public async Task<Response> AddEatSchedule(EatScheduleModel addModel)
        {
            var addValue = new EatSchedule
            {
                Name = addModel.Name,
                PlaceName = addModel.PlaceName,
                RestaurantId = addModel.RestaurantId,
                SinglePrice = addModel.SinglePrice,
                Address = addModel.Address,
                SupportNumber = addModel.SupportNumber,
                TourishPlanId = null,
                Status = addModel.Status,
                StartDate = addModel.StartDate,
                EndDate = addModel.EndDate,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
            };

            await _context.AddAsync(addValue);
            await _context.SaveChangesAsync();

            await _blobService.UploadStringBlobAsync(
                "eatschedule-content-container",
                addModel.Description ?? "Không có thông tin",
                "text/plain",
                addValue.Id.ToString() ?? "" + ".txt"
            );

            return new Response
            {
                resultCd = 0,
                MessageCode = "I431",
                // Create type success
            };
        }

        public async Task<Response> AddMovingSchedule(string userId, MovingScheduleModel addModel)
        {
            var addValue = new MovingSchedule
            {
                Name = addModel.Name,
                BranchName = addModel.BranchName,
                HeadingPlace = addModel.HeadingPlace,
                StartingPlace = addModel.StartingPlace,
                TransportId = addModel.TransportId,
                VehicleType = addModel.VehicleType,
                SinglePrice = addModel.SinglePrice,
                DriverName = addModel.DriverName,
                VehiclePlate = addModel.VehiclePlate,
                PhoneNumber = addModel.PhoneNumber,
                TourishPlanId = null,
                Status = addModel.Status,
                StartDate = addModel.StartDate,
                EndDate = addModel.EndDate,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
            };

            var scheduleInterest = new ScheduleInterest();
            var scheduleInterestList = new List<ScheduleInterest>();

            if (userId != null)
            {
                var user = _context.Users.SingleOrDefault(u => u.Id.ToString() == userId);

                scheduleInterest = new ScheduleInterest
                {
                    InterestStatus = InterestStatus.Creator,
                    User = user,
                    MovingSchedule = addValue,
                    UpdateDate = DateTime.UtcNow
                };

                scheduleInterestList.Add(scheduleInterest);

                addValue.ScheduleInterestList = scheduleInterestList;
            }

            addValue.ScheduleInterestList = scheduleInterestList;
            addValue.InstructionList = initiateInstructionList();

            await _context.AddAsync(addValue);
            await _context.SaveChangesAsync();

            await _blobService.UploadStringBlobAsync(
                "movingschedule-content-container",
                addModel.Description ?? "Không có thông tin",
                "text/plain",
                addValue.Id.ToString() ?? "" + ".txt"
            );

            return new Response
            {
                resultCd = 0,
                MessageCode = "I431",
                // Create type success
            };
        }

        public async Task<Response> AddStayingSchedule(string userId, StayingScheduleModel addModel)
        {
            var addValue = new StayingSchedule
            {
                Name = addModel.Name,
                PlaceName = addModel.PlaceName,
                RestHouseBranchId = addModel.RestHouseBranchId,
                RestHouseType = addModel.RestHouseType,
                SinglePrice = addModel.SinglePrice,
                Address = addModel.Address,
                SupportNumber = addModel.SupportNumber,
                TourishPlanId = null,
                Status = addModel.Status,
                StartDate = addModel.StartDate,
                EndDate = addModel.EndDate,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
            };

            var scheduleInterest = new ScheduleInterest();
            var scheduleInterestList = new List<ScheduleInterest>();

            if (userId != null)
            {
                var user = _context.Users.SingleOrDefault(u => u.Id.ToString() == userId);

                scheduleInterest = new ScheduleInterest
                {
                    InterestStatus = InterestStatus.Creator,
                    User = user,
                    StayingSchedule = addValue,
                    UpdateDate = DateTime.UtcNow
                };

                scheduleInterestList.Add(scheduleInterest);

                addValue.ScheduleInterestList = scheduleInterestList;
            }

            addValue.ScheduleInterestList = scheduleInterestList;
            addValue.InstructionList = initiateInstructionList();

            await _context.AddAsync(addValue);
            await _context.SaveChangesAsync();

            await _blobService.UploadStringBlobAsync(
                "stayingschedule-content-container",
                addModel.Description ?? "Không có thông tin",
                "text/plain",
                addValue.Id.ToString() ?? "" + ".txt"
            );

            return new Response
            {
                resultCd = 0,
                MessageCode = "I431",
                // Create type success
            };
        }

        private List<Instruction> initiateInstructionList()
        {
            var instructionList = new List<Instruction>();

            foreach (var instruction in priceInstructionLines)
            {
                instructionList.Add(
                    new Instruction
                    {
                        InstructionType = InstructionType.Price,
                        Description = instruction,
                        CreateDate = DateTime.UtcNow,
                        UpdateDate = DateTime.UtcNow,
                    }
                );
            }

            foreach (var instruction in cautionInstructionLines)
            {
                instructionList.Add(
                    new Instruction
                    {
                        InstructionType = InstructionType.Caution,
                        Description = instruction,
                        CreateDate = DateTime.UtcNow,
                        UpdateDate = DateTime.UtcNow,
                    }
                );
            }

            return instructionList;
        }

        public async Task<Response> DeleteEatSchedule(Guid id)
        {
            var deleteEntity = _context.EatSchedules.FirstOrDefault((entity => entity.Id == id));
            if (deleteEntity != null)
            {
                await _blobService.DeleteFileBlobAsync(
                    "eatschedule-content-container",
                    deleteEntity.Id.ToString()
                );
                _context.Remove(deleteEntity);
                _context.SaveChanges();
            }

            return new Response
            {
                resultCd = 0,
                MessageCode = "I433",
                // Delete type success
            };
        }

        public async Task<Response> DeleteMovingSchedule(Guid id)
        {
            var deleteEntity = _context.MovingSchedules.FirstOrDefault((entity => entity.Id == id));
            if (deleteEntity != null)
            {
                await _blobService.DeleteFileBlobAsync(
                    "movingschedule-content-container",
                    deleteEntity.Id.ToString()
                );
                _context.Remove(deleteEntity);
                _context.SaveChanges();
            }

            return new Response
            {
                resultCd = 0,
                MessageCode = "I433",
                // Delete type success
            };
        }

        public async Task<Response> DeleteStayingSchedule(Guid id)
        {
            var deleteEntity = _context.StayingSchedules.FirstOrDefault(
                (entity => entity.Id == id)
            );
            if (deleteEntity != null)
            {
                await _blobService.DeleteFileBlobAsync(
                    "stayingschedule-content-container",
                    deleteEntity.Id.ToString()
                );
                _context.Remove(deleteEntity);
                _context.SaveChanges();
            }

            return new Response
            {
                resultCd = 0,
                MessageCode = "I433",
                // Delete type success
            };
        }

        public Response GetAllEatSchedule(
            string? search,
            int? type,
            string? sortBy,
            string? sortDirection,
            int page = 1,
            int pageSize = 5
        )
        {
            var entityQuery = _context.EatSchedules.AsQueryable();

            #region Filtering
            if (!string.IsNullOrEmpty(search))
            {
                entityQuery = entityQuery.Where(entity => entity.PlaceName.Contains(search));
            }
            #endregion

            #region Sorting
            if (!string.IsNullOrEmpty(sortBy))
            {
                entityQuery = entityQuery.OrderByColumn(sortBy);
                if (sortDirection == "desc")
                {
                    entityQuery = entityQuery.OrderByColumnDescending(sortBy);
                }
            }
            #endregion

            #region Paging
            var result = PaginatorModel<EatSchedule>.Create(entityQuery, page, pageSize);
            #endregion

            var entityVM = new Response
            {
                resultCd = 0,
                Data = result.ToList(),
                count = result.TotalCount,
            };
            return entityVM;
        }

        public Response GetAllMovingSchedule(
            string? search,
            int? type,
            string? sortBy,
            string? sortDirection,
            int page = 1,
            int pageSize = 5
        )
        {
            var entityQuery = _context.MovingSchedules.AsQueryable();

            #region Filtering
            entityQuery = entityQuery.Where(entity => entity.TourishPlan == null);
            if (!string.IsNullOrEmpty(search))
            {
                entityQuery = entityQuery.Where(entity => entity.BranchName.Contains(search));
            }
            #endregion

            #region Sorting
            if (!string.IsNullOrEmpty(sortBy))
            {
                entityQuery = entityQuery.OrderByColumn(sortBy);
                if (sortDirection == "desc")
                {
                    entityQuery = entityQuery.OrderByColumnDescending(sortBy);
                }
            }
            #endregion

            #region Paging
            var result = PaginatorModel<MovingSchedule>.Create(entityQuery, page, pageSize);
            #endregion

            var entityVM = new Response
            {
                resultCd = 0,
                Data = result.ToList(),
                count = result.TotalCount,
            };
            return entityVM;
        }

        public Response GetAllStayingSchedule(
            string? search,
            int? type,
            string? sortBy,
            string? sortDirection,
            int page = 1,
            int pageSize = 5
        )
        {
            var entityQuery = _context.StayingSchedules.AsQueryable();

            #region Filtering
            if (!string.IsNullOrEmpty(search))
            {
                entityQuery = entityQuery.Where(entity => entity.PlaceName.Contains(search));
            }
            #endregion

            #region Sorting
            if (!string.IsNullOrEmpty(sortBy))
            {
                entityQuery = entityQuery.OrderByColumn(sortBy);
                if (sortDirection == "desc")
                {
                    entityQuery = entityQuery.OrderByColumnDescending(sortBy);
                }
            }
            #endregion

            #region Paging
            var result = PaginatorModel<StayingSchedule>.Create(entityQuery, page, pageSize);
            #endregion

            var entityVM = new Response
            {
                resultCd = 0,
                Data = result.ToList(),
                count = result.TotalCount,
            };
            return entityVM;
        }

        public Response getByEatScheduleId(Guid id)
        {
            var entity = _context.EatSchedules.FirstOrDefault((entity => entity.Id == id));

            return new Response { resultCd = 0, Data = entity };
        }

        public Response getByNameEatSchedule(string name)
        {
            var entity = _context.EatSchedules.FirstOrDefault((entity => entity.PlaceName == name));

            return new Response { resultCd = 0, Data = entity };
        }

        public async Task<Response> UpdateEatSchedule(EatScheduleModel entityModel)
        {
            var entity = _context.EatSchedules.FirstOrDefault(
                (entity => entity.Id == entityModel.Id)
            );
            if (entity != null)
            {
                entity.Name = entityModel.Name;
                entity.UpdateDate = DateTime.UtcNow;
                entity.PlaceName = entityModel.PlaceName;
                entity.RestaurantId = entityModel.RestaurantId;
                entity.SinglePrice = entityModel.SinglePrice;
                entity.SupportNumber = entityModel.SupportNumber;
                entity.Address = entityModel.Address;
                entity.StartDate = entityModel.StartDate;
                entity.EndDate = entityModel.EndDate;
                entity.Status = entityModel.Status;

                _context.SaveChanges();
                await _blobService.UploadStringBlobAsync(
                    "eatschedule-content-container",
                    entityModel.Description ?? "Không có thông tin",
                    "text/plain",
                    entity.Id.ToString() ?? "" + ".txt"
                );
            }

            return new Response
            {
                resultCd = 0,
                MessageCode = "I432",
                // Update type success
            };
        }

        public Response getByStayingScheduleId(Guid id)
        {
            var entity = _context.StayingSchedules.FirstOrDefault((entity => entity.Id == id));

            return new Response { resultCd = 0, Data = entity };
        }

        public Response getByNameStayingSchedule(string name)
        {
            var entity = _context.StayingSchedules.FirstOrDefault(
                (entity => entity.PlaceName == name)
            );

            return new Response { resultCd = 0, Data = entity };
        }

        public async Task<Response> UpdateStayingSchedule(
            string userId,
            StayingScheduleModel entityModel
        )
        {
            var entity = _context.StayingSchedules.FirstOrDefault(
                (entity => entity.Id == entityModel.Id)
            );
            if (entity != null)
            {
                entity.Name = entityModel.Name;
                entity.UpdateDate = DateTime.UtcNow;
                entity.PlaceName = entityModel.PlaceName;
                entity.SupportNumber = entityModel.SupportNumber;
                entity.Address = entityModel.Address;
                entity.SinglePrice = entityModel.SinglePrice;
                entity.RestHouseBranchId = entityModel.RestHouseBranchId;
                entity.RestHouseType = entityModel.RestHouseType;
                entity.Status = entityModel.Status;
                entity.StartDate = entityModel.StartDate;
                entity.EndDate = entityModel.EndDate;

                var scheduleInterest = new ScheduleInterest();
                if (userId != null)
                {
                    var user = _context.Users.SingleOrDefault(u => u.Id.ToString() == userId);

                    if (user != null)
                    {
                        scheduleInterest = new ScheduleInterest
                        {
                            InterestStatus = InterestStatus.Modifier,
                            User = user,
                            StayingSchedule = entity,
                            UpdateDate = DateTime.UtcNow
                        };

                        if (entity.ScheduleInterestList == null)
                        {
                            entity.ScheduleInterestList = new List<ScheduleInterest>();
                        }

                        if (
                            entity.ScheduleInterestList.Count(interest =>
                                interest.UserId.ToString() == userId
                            ) <= 0
                        )
                        {
                            entity.ScheduleInterestList.Add(scheduleInterest);
                        }
                    }
                }

                await _context.SaveChangesAsync();
            }

            await _blobService.UploadStringBlobAsync(
                "stayingschedule-content-container",
                entityModel.Description ?? "Không có thông tin",
                "text/plain",
                entity.Id.ToString() ?? "" + ".txt"
            );

            return new Response
            {
                resultCd = 0,
                MessageCode = "I432",
                // Update type success
            };
        }

        public Response getByMovingScheduleId(Guid id)
        {
            var entity = _context.MovingSchedules.FirstOrDefault((entity => entity.Id == id));

            return new Response { resultCd = 0, Data = entity };
        }

        public Response getByNameMovingSchedule(string name)
        {
            var entity = _context.MovingSchedules.FirstOrDefault((entity => entity.Name == name));

            return new Response { resultCd = 0, Data = entity };
        }

        public async Task<Response> UpdateMovingSchedule(
            string userId,
            MovingScheduleModel entityModel
        )
        {
            var entity = _context.MovingSchedules.FirstOrDefault(
                (entity => entity.Id == entityModel.Id)
            );

            if (entity != null)
            {
                entity.UpdateDate = DateTime.UtcNow;
                entity.Name = entityModel.Name;
                entity.BranchName = entityModel.BranchName;
                entity.PhoneNumber = entityModel.PhoneNumber;
                entity.TransportId = entityModel.TransportId;
                entity.VehiclePlate = entityModel.VehiclePlate;
                entity.VehicleType = entityModel.VehicleType;
                entity.SinglePrice = entityModel.SinglePrice;
                entity.DriverName = entityModel.DriverName;
                entity.StartingPlace = entityModel.StartingPlace;
                entity.HeadingPlace = entityModel.HeadingPlace;
                entity.Status = entityModel.Status;
                entity.StartDate = entityModel.StartDate;
                entity.EndDate = entityModel.EndDate;

                var scheduleInterest = new ScheduleInterest();
                if (userId != null)
                {
                    var user = _context.Users.SingleOrDefault(u => u.Id.ToString() == userId);

                    if (user != null)
                    {
                        scheduleInterest = new ScheduleInterest
                        {
                            InterestStatus = InterestStatus.Modifier,
                            User = user,
                            MovingSchedule = entity,
                            UpdateDate = DateTime.UtcNow
                        };

                        if (entity.ScheduleInterestList == null)
                        {
                            entity.ScheduleInterestList = new List<ScheduleInterest>();
                        }

                        if (
                            entity.ScheduleInterestList.Count(interest =>
                                interest.UserId.ToString() == userId
                            ) <= 0
                        )
                        {
                            entity.ScheduleInterestList.Add(scheduleInterest);
                        }
                    }
                }

                await _context.SaveChangesAsync();

                await _blobService.UploadStringBlobAsync(
                    "movingschedule-content-container",
                    entityModel.Description ?? "Không có thông tin",
                    "text/plain",
                    entity.Id.ToString() ?? "" + ".txt"
                );
            }

            return new Response
            {
                resultCd = 0,
                MessageCode = "I432",
                // Update type success
            };
        }

        public async Task<List<ScheduleInterest>> getScheduleInterest(
            Guid id,
            ScheduleType scheduleType
        )
        {
            if (scheduleType == ScheduleType.MovingSchedule)
            {
                var entity = await _context
                    .MovingSchedules.Where(entity => entity.Id == id)
                    .Include(tour => tour.ScheduleInterestList)
                    .FirstOrDefaultAsync();
                if (entity == null)
                {
                    return null;
                }

                return entity.ScheduleInterestList.ToList();
            }
            else if (scheduleType == ScheduleType.StayingSchedule)
            {
                var entity = await _context
                    .StayingSchedules.Where(entity => entity.Id == id)
                    .Include(tour => tour.ScheduleInterestList)
                    .FirstOrDefaultAsync();
                if (entity == null)
                {
                    return null;
                }

                return entity.ScheduleInterestList.ToList();
            }

            return new List<ScheduleInterest>();
        }

        public Response getScheduleInterest(Guid scheduleId, ScheduleType scheduleType, Guid userId)
        {
            if (scheduleType == ScheduleType.MovingSchedule)
            {
                var data = _context.ScheduleInterests.FirstOrDefault(entity =>
                    entity.MovingScheduleId == scheduleId && entity.UserId == userId
                );
                return new Response
                {
                    resultCd = 0,
                    Data = data,
                    // Update type success
                };
            }
            else if (scheduleType == ScheduleType.StayingSchedule)
            {
                var data = _context.ScheduleInterests.FirstOrDefault(entity =>
                    entity.StayingScheduleId == scheduleId && entity.UserId == userId
                );
                return new Response
                {
                    resultCd = 0,
                    Data = data,
                    // Update type success
                };
            }

            return new Response
            {
                resultCd = 1,
                Data = null,
                MessageCode = "C430"
                // Update type success
            };
        }

        public async Task<Response> setScheduleInterest(
            Guid scheduleId,
            ScheduleType scheduleType,
            Guid userId,
            InterestStatus interestStatus
        )
        {
            if (scheduleType == ScheduleType.MovingSchedule)
            {
                var existInterest = _context.ScheduleInterests.FirstOrDefault(entity =>
                    entity.MovingScheduleId == scheduleId && entity.UserId == userId
                );

                if (existInterest != null)
                {
                    existInterest.InterestStatus = interestStatus;
                    existInterest.UpdateDate = DateTime.UtcNow;

                    await _context.SaveChangesAsync();

                    return new Response
                    {
                        resultCd = 0,
                        MessageCode = "I415",
                        // Update type success
                    };
                }
                else
                {
                    var insertValue = new TourishInterest
                    {
                        TourishPlanId = scheduleId,
                        UserId = userId,
                        InterestStatus = interestStatus,
                        CreateDate = DateTime.UtcNow,
                        UpdateDate = DateTime.UtcNow
                    };

                    _context.Add(insertValue);
                    await _context.SaveChangesAsync();

                    return new Response
                    {
                        resultCd = 0,
                        MessageCode = "I415",
                        // Update type success
                    };
                }
            }
            else if (scheduleType == ScheduleType.StayingSchedule)
            {
                var existInterest = _context.ScheduleInterests.FirstOrDefault(entity =>
                    entity.StayingScheduleId == scheduleId && entity.UserId == userId
                );

                if (existInterest != null)
                {
                    existInterest.InterestStatus = interestStatus;
                    existInterest.UpdateDate = DateTime.UtcNow;

                    await _context.SaveChangesAsync();

                    return new Response
                    {
                        resultCd = 0,
                        MessageCode = "I415",
                        // Update type success
                    };
                }
                else
                {
                    var insertValue = new TourishInterest
                    {
                        TourishPlanId = scheduleId,
                        UserId = userId,
                        InterestStatus = interestStatus,
                        CreateDate = DateTime.UtcNow,
                        UpdateDate = DateTime.UtcNow
                    };

                    _context.Add(insertValue);
                    await _context.SaveChangesAsync();

                    return new Response
                    {
                        resultCd = 0,
                        MessageCode = "I415",
                        // Update type success
                    };
                }
            }

            return new Response
            {
                resultCd = 1,
                MessageCode = "C434",
                // Update type success
            };
        }

        public async Task<Response> UpdateInstructionList(
            ScheduleInstructionModel scheduleInstructionModel
        )
        {
            if (scheduleInstructionModel.ScheduleType == ScheduleType.MovingSchedule)
            {
                var existSchedule = _context.MovingSchedules.FirstOrDefault(entity =>
                    entity.Id == scheduleInstructionModel.ScheduleId
                );

                if (existSchedule == null)
                    return new Response
                    {
                        resultCd = 1,
                        MessageCode = "C430",
                    };

                if (scheduleInstructionModel.InstructionList != null)
                {
                    var instructionList = new List<Instruction>();
                    foreach (var item in scheduleInstructionModel.InstructionList)
                    {
                        instructionList.Add(
                            new Instruction
                            {
                                Id = item.Id.Value,
                                TourishPlanId = item.TourishPlanId,
                                Description = item.Description,
                                InstructionType = item.InstructionType,
                                CreateDate = item.CreateDate,
                                UpdateDate = DateTime.UtcNow,
                            }
                        );
                    }
                    existSchedule.InstructionList = instructionList;
                    await _context.SaveChangesAsync();

                    return new Response
                    {
                        resultCd = 0,
                        MessageCode = "I432",
                        // Update type success
                    };
                }
            }
            else if (scheduleInstructionModel.ScheduleType == ScheduleType.StayingSchedule)
            {
                var existSchedule = _context.StayingSchedules.FirstOrDefault(entity =>
                    entity.Id == scheduleInstructionModel.ScheduleId
                );

                if (scheduleInstructionModel.InstructionList != null)
                {
                    var instructionList = new List<Instruction>();
                    foreach (var item in scheduleInstructionModel.InstructionList)
                    {
                        instructionList.Add(
                            new Instruction
                            {
                                Id = item.Id.Value,
                                TourishPlanId = item.TourishPlanId,
                                Description = item.Description,
                                InstructionType = item.InstructionType,
                                CreateDate = item.CreateDate,
                                UpdateDate = DateTime.UtcNow,
                            }
                        );
                    }
                    existSchedule.InstructionList = instructionList;
                    await _context.SaveChangesAsync();

                    return new Response
                    {
                        resultCd = 0,
                        MessageCode = "I432",
                        // Update type success
                    };
                }
            }

            return new Response
            {
                resultCd = 1,
                MessageCode = "C430",
            };
        }
    }
}
