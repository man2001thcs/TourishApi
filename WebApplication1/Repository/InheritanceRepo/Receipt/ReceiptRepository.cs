using Google.Api.Gax;
using Microsoft.EntityFrameworkCore;
using TourishApi.Extension;
using WebApplication1.Data;
using WebApplication1.Data.DbContextFile;
using WebApplication1.Data.Receipt;
using WebApplication1.Data.Schedule;
using WebApplication1.Model;
using WebApplication1.Model.Receipt;
using WebApplication1.Model.VirtualModel;

namespace WebApplication1.Repository.InheritanceRepo.Receipt;

public class ReceiptRepository
{
    private readonly MyDbContext _context;
    public static int PAGE_SIZE { get; set; } = 5;
    private readonly char[] delimiter = new char[] { ';' };

    public ReceiptRepository(MyDbContext _context)
    {
        this._context = _context;
    }

    public async Task<Response> Add(FullReceiptInsertModel receiptModel)
    {
        var totalReceipt = _context.TotalReceiptList.FirstOrDefault(entity =>
            entity.TourishPlanId == receiptModel.TourishPlanId
        );
        var planExist = _context.TourishPlan.FirstOrDefault(
            (plan => plan.Id == totalReceipt.TourishPlanId)
        );

        if (totalReceipt == null)
        {
            totalReceipt = new TotalReceipt
            {
                TourishPlanId = receiptModel.TourishPlanId,
                Status = ReceiptStatus.Created,
                Description = receiptModel.Description,

                ScheduleId = receiptModel.ScheduleId,
                ScheduleType = receiptModel.ScheduleType,

                CreatedDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow
            };

            await _context.AddAsync(totalReceipt);
            await _context.SaveChangesAsync();
        }

        var existFullReceipt = _context.FullReceiptList.FirstOrDefault(entity =>
            entity.Email == receiptModel.Email
        );

        if (planExist != null)
        {
            var originalPrice = (double)0;
            if (planExist != null)
            {
                originalPrice = GetTotalPrice(planExist);
            }
            else
            {
                if (receiptModel.ScheduleType == ScheduleType.EatSchedule)
                {
                    var schedule = _context.EatSchedules.FirstOrDefault(e =>
                        e.Id == receiptModel.ScheduleId
                    );
                    originalPrice = schedule.SinglePrice ?? (double)0;
                }
                else if (receiptModel.ScheduleType == ScheduleType.MovingSchedule)
                {
                    var schedule = _context.MovingSchedules.FirstOrDefault(e =>
                        e.Id == receiptModel.ScheduleId
                    );
                    originalPrice = schedule.SinglePrice ?? (double)0;
                }
                if (receiptModel.ScheduleType == ScheduleType.StayingSchedule)
                {
                    var schedule = _context.StayingSchedules.FirstOrDefault(e =>
                        e.Id == receiptModel.ScheduleId
                    );
                    originalPrice = schedule.SinglePrice ?? (double)0;
                }
            }

            if (existFullReceipt == null)
            {
                var fullReceipt = new FullReceipt
                {
                    TotalReceiptId = totalReceipt.TotalReceiptId,
                    TourishScheduleId = receiptModel.TourishScheduleId,
                    OriginalPrice = originalPrice,
                    GuestName = receiptModel.GuestName,
                    Description = receiptModel.Description,
                    PhoneNumber = receiptModel.PhoneNumber,
                    Email = receiptModel.Email,
                    TotalTicket = receiptModel.TotalTicket,
                    TotalChildTicket = receiptModel.TotalChildTicket,
                    DiscountAmount = receiptModel.DiscountAmount,
                    DiscountFloat = receiptModel.DiscountFloat,
                    Status = FullReceiptStatus.Created,
                    CreatedDate = DateTime.UtcNow,
                    UpdateDate = DateTime.UtcNow
                };

                if (planExist.RemainTicket >= receiptModel.TotalTicket)
                {
                    await _context.FullReceiptList.AddAsync(fullReceipt);
                    await _context.SaveChangesAsync();

                    return new Response
                    {
                        resultCd = 0,
                        MessageCode = "I511",
                        returnId = fullReceipt.FullReceiptId,
                        // Create type success
                    };
                }
                else
                {
                    return new Response
                    {
                        resultCd = 0,
                        MessageCode = "C515",
                        returnId = fullReceipt.FullReceiptId,
                        // Out of ticket
                    };
                }
            }
            else
            {
                existFullReceipt.GuestName = receiptModel.GuestName;
                existFullReceipt.PhoneNumber = receiptModel.PhoneNumber;
                existFullReceipt.TotalTicket = receiptModel.TotalTicket;
                existFullReceipt.TotalChildTicket = receiptModel.TotalChildTicket;
                existFullReceipt.OriginalPrice = originalPrice;
                existFullReceipt.UpdateDate = DateTime.UtcNow;

                if (planExist.RemainTicket >= receiptModel.TotalTicket)
                {
                    await _context.SaveChangesAsync();

                    return new Response
                    {
                        resultCd = 0,
                        MessageCode = "I511",
                        returnId = existFullReceipt.FullReceiptId,
                        // Create type success
                    };
                }
                else
                {
                    return new Response
                    {
                        resultCd = 0,
                        MessageCode = "C515",
                        returnId = existFullReceipt.FullReceiptId,
                        // Out of ticket
                    };
                }
            }
        }
        else
        {
            return new Response { resultCd = 0, MessageCode = "C414" };
        }
    }

    public async Task<Response> AddTourReceiptForClient(FullReceiptClientInsertModel receiptModel)
    {
        var totalReceipt = _context.TotalReceiptList.FirstOrDefault(entity =>
            entity.TourishPlanId == receiptModel.TourishPlanId
        );

        if (totalReceipt == null)
        {
            totalReceipt = new TotalReceipt
            {
                TourishPlanId = receiptModel.TourishPlanId,
                ScheduleId = receiptModel.ScheduleId,
                ScheduleType = receiptModel.ScheduleType,
                Status = ReceiptStatus.Created,
                Description = "",
                CreatedDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow
            };

            await _context.AddAsync(totalReceipt);
            await _context.SaveChangesAsync();
        }

        if (receiptModel.Email != null)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email.ToString() == receiptModel.Email);

            if (receiptModel.TourishPlanId != null)
            {
                var tourishPlan = _context.TourishPlan.FirstOrDefault(entity =>
                    entity.Id == receiptModel.TourishPlanId
                );

                if (tourishPlan == null)
                    return new Response { resultCd = 1, MessageCode = "I410", };

                var existInterest = _context.TourishInterests.FirstOrDefault(u =>
                    u.UserId == user.Id && u.TourishPlanId == receiptModel.TourishPlanId
                );

                if (existInterest == null)
                {
                    var tourishInterest = new TourishInterest
                    {
                        InterestStatus = InterestStatus.User,
                        User = user,
                        TourishPlanId = tourishPlan.Id,
                        UpdateDate = DateTime.UtcNow
                    };

                    await _context.AddAsync(tourishInterest);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    existInterest.InterestStatus = InterestStatus.User;
                    existInterest.UpdateDate = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }
            }
        }

        var originalPrice = (double)0;

        if (receiptModel.TourishPlanId != null)
        {
            var tourishPlan = _context.TourishPlan.FirstOrDefault(entity =>
                entity.Id == receiptModel.TourishPlanId
            );
            originalPrice = GetTotalPrice(tourishPlan);
        }

        var existFullReceipt = _context.FullReceiptList.FirstOrDefault(entity =>
            entity.Email == receiptModel.Email
        );

        if (existFullReceipt == null)
        {
            var fullReceipt = new FullReceipt
            {
                TotalReceiptId = totalReceipt.TotalReceiptId,
                TourishScheduleId = receiptModel.TourishScheduleId,
                GuestName = receiptModel.GuestName,
                Description = "",
                PhoneNumber = receiptModel.PhoneNumber,
                Email = receiptModel.Email,
                TotalTicket = receiptModel.TotalTicket,
                TotalChildTicket = receiptModel.TotalChildTicket,
                OriginalPrice = originalPrice,
                DiscountAmount = (double)0,
                DiscountFloat = (float)0,
                Status = FullReceiptStatus.Created,
                CreatedDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow
            };

            var planExist = _context.TourishPlan.FirstOrDefault(
                (plan => plan.Id == totalReceipt.TourishPlanId)
            );

            if (planExist.RemainTicket >= receiptModel.TotalTicket)
            {
                await _context.FullReceiptList.AddAsync(fullReceipt);
                await _context.SaveChangesAsync();

                return new Response
                {
                    resultCd = 0,
                    MessageCode = "I511",
                    returnId = fullReceipt.FullReceiptId,
                    Data = fullReceipt
                    // Create type success
                };
            }
            else
            {
                return new Response
                {
                    resultCd = 0,
                    MessageCode = "C515",
                    returnId = existFullReceipt.FullReceiptId,
                    // Out of ticket
                };
            }
        }
        else
        {
            var planExist = _context.TourishPlan.FirstOrDefault(
                (plan => plan.Id == totalReceipt.TourishPlanId)
            );

            existFullReceipt.GuestName = receiptModel.GuestName;
            existFullReceipt.PhoneNumber = receiptModel.PhoneNumber;
            existFullReceipt.TotalTicket = receiptModel.TotalTicket;
            existFullReceipt.TotalChildTicket = receiptModel.TotalChildTicket;
            existFullReceipt.OriginalPrice = originalPrice;
            existFullReceipt.UpdateDate = DateTime.UtcNow;

            if (planExist.RemainTicket >= receiptModel.TotalTicket)
            {
                await _context.SaveChangesAsync();

                return new Response
                {
                    resultCd = 0,
                    MessageCode = "I512",
                    returnId = existFullReceipt.FullReceiptId,
                    // Create type success
                };
            }
            else
            {
                return new Response
                {
                    resultCd = 0,
                    MessageCode = "C515",
                    returnId = existFullReceipt.FullReceiptId,
                    // Out of ticket
                };
            }
        }
    }

    public async Task<Response> AddScheduleReceiptForClient(
        FullReceiptClientInsertModel receiptModel
    )
    {
        var totalReceipt = _context.TotalReceiptList.FirstOrDefault(entity =>
            entity.ScheduleId == receiptModel.ScheduleId
            && entity.ScheduleType == receiptModel.ScheduleType
        );

        if (totalReceipt == null)
        {
            totalReceipt = new TotalReceipt
            {
                TourishPlanId = receiptModel.TourishPlanId,
                ScheduleId = receiptModel.ScheduleId,
                ScheduleType = receiptModel.ScheduleType,
                Status = ReceiptStatus.Created,
                Description = "",
                CreatedDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow
            };

            await _context.AddAsync(totalReceipt);
            await _context.SaveChangesAsync();
        }

        if (receiptModel.Email != null)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email.ToString() == receiptModel.Email);

            if (receiptModel.ScheduleType == ScheduleType.StayingSchedule)
            {
                var schedule = _context.MovingSchedules.FirstOrDefault(entity =>
                    entity.Id == receiptModel.ScheduleId
                );

                if (schedule == null)
                    return new Response { resultCd = 1, MessageCode = "I434", };

                var existScheduleInterest = _context.ScheduleInterests.FirstOrDefault(u =>
                    u.UserId == user.Id && u.MovingScheduleId == receiptModel.ScheduleId
                );
                if (existScheduleInterest == null)
                {
                    var scheduleInterest = new ScheduleInterest
                    {
                        InterestStatus = InterestStatus.User,
                        User = user,
                        MovingScheduleId = schedule.Id,
                        UpdateDate = DateTime.UtcNow
                    };

                    await _context.AddAsync(scheduleInterest);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    existScheduleInterest.InterestStatus = InterestStatus.User;
                    existScheduleInterest.UpdateDate = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }
            }
            else if (receiptModel.ScheduleType == ScheduleType.MovingSchedule)
            {
                var schedule = _context.MovingSchedules.FirstOrDefault(entity =>
                    entity.Id == receiptModel.ScheduleId
                );

                if (schedule == null)
                    return new Response { resultCd = 1, MessageCode = "I434", };

                var existScheduleInterest = _context.ScheduleInterests.FirstOrDefault(u =>
                    u.UserId == user.Id && u.MovingScheduleId == receiptModel.ScheduleId
                );
                if (existScheduleInterest == null)
                {
                    var scheduleInterest = new ScheduleInterest
                    {
                        InterestStatus = InterestStatus.User,
                        User = user,
                        MovingScheduleId = schedule.Id,
                        UpdateDate = DateTime.UtcNow
                    };

                    await _context.AddAsync(scheduleInterest);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    existScheduleInterest.InterestStatus = InterestStatus.User;
                    existScheduleInterest.UpdateDate = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }
            }
        }

        var originalPrice = (double)0;

        if (receiptModel.ScheduleType == ScheduleType.EatSchedule)
        {
            var schedule = _context.EatSchedules.FirstOrDefault(e =>
                e.Id == receiptModel.ScheduleId
            );
            originalPrice = schedule.SinglePrice ?? (double)0;
        }
        else if (receiptModel.ScheduleType == ScheduleType.MovingSchedule)
        {
            var schedule = _context.MovingSchedules.FirstOrDefault(e =>
                e.Id == receiptModel.ScheduleId
            );
            originalPrice = schedule.SinglePrice ?? (double)0;
        }
        if (receiptModel.ScheduleType == ScheduleType.StayingSchedule)
        {
            var schedule = _context.StayingSchedules.FirstOrDefault(e =>
                e.Id == receiptModel.ScheduleId
            );
            originalPrice = schedule.SinglePrice ?? (double)0;
        }

        var existFullReceipt = _context.FullReceiptList.FirstOrDefault(entity =>
            entity.Email == receiptModel.Email
        );

        if (existFullReceipt == null)
        {
            var fullReceipt = new FullReceipt
            {
                TotalReceiptId = totalReceipt.TotalReceiptId,
                TourishScheduleId = receiptModel.TourishScheduleId,
                GuestName = receiptModel.GuestName,
                Description = "",
                PhoneNumber = receiptModel.PhoneNumber,
                Email = receiptModel.Email,
                TotalTicket = receiptModel.TotalTicket,
                TotalChildTicket = receiptModel.TotalChildTicket,
                OriginalPrice = originalPrice,
                DiscountAmount = (double)0,
                DiscountFloat = (float)0,
                Status = FullReceiptStatus.Created,
                CreatedDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow
            };

            await _context.FullReceiptList.AddAsync(fullReceipt);
            await _context.SaveChangesAsync();

            return new Response
            {
                resultCd = 0,
                MessageCode = "I511",
                returnId = fullReceipt.FullReceiptId,
                Data = fullReceipt
                // Create type success
            };
        }
        else
        {
            existFullReceipt.GuestName = receiptModel.GuestName;
            existFullReceipt.PhoneNumber = receiptModel.PhoneNumber;
            existFullReceipt.TotalTicket = receiptModel.TotalTicket;
            existFullReceipt.TotalChildTicket = receiptModel.TotalChildTicket;
            existFullReceipt.OriginalPrice = originalPrice;
            existFullReceipt.UpdateDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new Response
            {
                resultCd = 0,
                MessageCode = "I512",
                returnId = existFullReceipt.FullReceiptId,
            };
        }
    }

    public double GetTotalPrice(TourishPlan tourishPlan)
    {
        double totalPrice = 0;

        if (tourishPlan.StayingSchedules != null)
        {
            foreach (var entity in tourishPlan.StayingSchedules)
            {
                totalPrice += entity.SinglePrice ?? 0;
            }
        }

        if (tourishPlan.EatSchedules != null)
        {
            foreach (var entity in tourishPlan.EatSchedules)
            {
                totalPrice += entity.SinglePrice ?? 0;
            }
        }

        if (tourishPlan.MovingSchedules != null)
        {
            foreach (var entity in tourishPlan.MovingSchedules)
            {
                totalPrice += entity.SinglePrice ?? 0;
            }
        }

        return totalPrice;
    }

    public Response Delete(Guid id)
    {
        var receipt = _context
            .FullReceiptList.Include(entity => entity.TotalReceipt)
            .FirstOrDefault((receipt => receipt.FullReceiptId == id));
        if (receipt != null)
        {
            var plan = _context.TourishPlan.FirstOrDefault(
                (plan => plan.Id == receipt.TotalReceipt.TourishPlanId)
            );
            if (plan != null)
            {
                plan.RemainTicket = plan.RemainTicket + receipt.TotalTicket;
            }
            _context.Remove(receipt);
            _context.SaveChanges();
        }

        return new Response
        {
            resultCd = 0,
            MessageCode = "I513",
            // Delete type success
        };
    }

    public Response DeleteAllBýTourishPlanId(Guid tourishPlanId)
    {
        var receipt = _context.TotalReceiptList.FirstOrDefault(
            (receipt => receipt.TourishPlanId == tourishPlanId)
        );
        if (receipt != null)
        {
            _context.Remove(receipt);

            var plan = _context.TourishPlan.FirstOrDefault((plan => plan.Id == tourishPlanId));
            if (plan != null)
            {
                plan.RemainTicket = plan.TotalTicket;
            }

            _context.SaveChanges();
        }

        return new Response
        {
            resultCd = 0,
            MessageCode = "I513",
            // Delete type success
        };
    }

    public Response DeleteAllBýScheduleId(Guid scheduleId, ScheduleType scheduleType)
    {
        var receipt = _context.TotalReceiptList.FirstOrDefault(
            (receipt => receipt.ScheduleId == scheduleId && receipt.ScheduleType == scheduleType)
        );
        if (receipt != null)
        {
            _context.Remove(receipt);
            _context.SaveChanges();
        }

        return new Response
        {
            resultCd = 0,
            MessageCode = "I513",
            // Delete type success
        };
    }

    public Response GetAll(
        string? tourishPlanId,
        string? scheduleId,
        ScheduleType? scheduleType,
        ReceiptStatus? status,
        string? sortBy,
        string? sortDirection,
        int page = 1,
        int pageSize = 5
    )
    {
        var receiptQuery = _context
            .TotalReceiptList.Include(receipt => receipt.FullReceiptList)
            .ThenInclude(receipt => receipt.TourishSchedule)
            .Include(receipt => receipt.TourishPlan)
            .Include(receipt => receipt.TourishPlan.MovingSchedules)
            .Include(receipt => receipt.TourishPlan.EatSchedules)
            .Include(receipt => receipt.TourishPlan.StayingSchedules)
            .AsQueryable();

        #region Filtering
        if (!string.IsNullOrEmpty(tourishPlanId))
        {
            receiptQuery = receiptQuery.Where(receipt =>
                receipt.TourishPlanId == new Guid(tourishPlanId)
            );
        }

        if (!string.IsNullOrEmpty(scheduleId))
        {
            receiptQuery = receiptQuery.Where(receipt =>
                receipt.ScheduleId == new Guid(scheduleId) && receipt.ScheduleType == scheduleType
            );
        }

        if (!string.IsNullOrEmpty(status.ToString()))
        {
            receiptQuery = receiptQuery.Where(receipt => receipt.Status == status);
        }
        #endregion

        #region Sorting
        if (!string.IsNullOrEmpty(sortBy))
        {
            receiptQuery = receiptQuery.OrderByColumn(sortBy);
            if (sortDirection == "desc")
            {
                receiptQuery = receiptQuery.OrderByColumnDescending(sortBy);
            }
        }
        #endregion

        #region Paging
        var result = PaginatorModel<TotalReceipt>.Create(receiptQuery, page, PAGE_SIZE);
        #endregion

        var resultDto = result.Select(entity => new
        {
            TotalReceiptId = entity.TotalReceiptId,
            TourishPlanId = entity.TourishPlanId,
            ScheduleId = entity.ScheduleId,
            ScheduleType = entity.ScheduleType,
            Status = entity.Status,
            Description = entity.Description,
            CompleteDate = entity.CompleteDate,
            FullReceiptList = entity.FullReceiptList,
            TourishPlan = entity.TourishPlan,
            StayingSchedule = scheduleType == ScheduleType.StayingSchedule
                ? _context.StayingSchedules.FirstOrDefault(e => e.Id == entity.ScheduleId)
                : null,
            MovingSchedule = scheduleType == ScheduleType.MovingSchedule
                ? _context.MovingSchedules.FirstOrDefault(e => e.Id == entity.ScheduleId)
                : null,
            CreatedDate = entity.CreatedDate,
            UpdateDate = entity.UpdateDate,
        });

        var receiptVM = new Response
        {
            resultCd = 0,
            Data = result.ToList(),
            count = result.TotalCount,
        };

        return receiptVM;
    }

    public Response GetAllForUser(
        string? email,
        ScheduleType? scheduleType,
        ReceiptStatus? status,
        string? sortBy,
        string? sortDirection,
        int page = 1,
        int pageSize = 5
    )
    {
        var receiptQuery = _context
            .TotalReceiptList.Include(receipt => receipt.FullReceiptList)
            .ThenInclude(receipt => receipt.TourishSchedule)
            .Include(receipt => receipt.TourishPlan)
            .Include(receipt => receipt.TourishPlan.MovingSchedules)
            .Include(receipt => receipt.TourishPlan.EatSchedules)
            .Include(receipt => receipt.TourishPlan.StayingSchedules)
            .AsQueryable();

        #region Filtering
        if (!string.IsNullOrEmpty(email))
        {
            receiptQuery = receiptQuery.Where(receipt =>
                receipt.FullReceiptList.Count(entity => entity.Email == email) >= 1
            );
        }

        if (!string.IsNullOrEmpty(status.ToString()))
        {
            receiptQuery = receiptQuery.Where(receipt => receipt.Status == status);
        }

        if (scheduleType != null)
        {
            receiptQuery = receiptQuery.Where(receipt => receipt.ScheduleType == scheduleType);
        }
        #endregion

        #region Sorting
        if (!string.IsNullOrEmpty(sortBy))
        {
            receiptQuery = receiptQuery.OrderByColumn(sortBy);
            if (sortDirection == "desc")
            {
                receiptQuery = receiptQuery.OrderByColumnDescending(sortBy);
            }
        }
        #endregion

        #region Paging
        var result = PaginatorModel<TotalReceipt>.Create(receiptQuery, page, PAGE_SIZE);
        #endregion

        var resultDto = result.Select(entity => new
        {
            TotalReceiptId = entity.TotalReceiptId,
            TourishPlanId = entity.TourishPlanId,
            ScheduleId = entity.ScheduleId,
            ScheduleType = entity.ScheduleType,

            Status = entity.Status,
            Description = entity.Description,
            CompleteDate = entity.CompleteDate,

            FullReceiptList = entity
                .FullReceiptList.Where(entity => entity.Email == email)
                .ToList(),
            TourishPlan = entity.TourishPlan,

            StayingSchedule = scheduleType == ScheduleType.StayingSchedule
                ? _context.StayingSchedules.FirstOrDefault(e => e.Id == entity.ScheduleId)
                : null,
            MovingSchedule = scheduleType == ScheduleType.MovingSchedule
                ? _context.MovingSchedules.FirstOrDefault(e => e.Id == entity.ScheduleId)
                : null,

            CreatedDate = entity.CreatedDate,
            UpdateDate = entity.UpdateDate,
        });

        var receiptVM = new Response
        {
            resultCd = 0,
            Data = resultDto.ToList(),
            count = result.TotalCount,
        };

        return receiptVM;
    }

    public Response getById(Guid id)
    {
        var receipt = _context
            .TotalReceiptList.Where(receipt => receipt.TotalReceiptId == id)
            .Include(receipt => receipt.FullReceiptList)
            .FirstOrDefault();
        if (receipt == null)
        {
            return null;
        }

        return new Response { resultCd = 0, Data = receipt };
    }

    public Response getFullReceiptById(Guid id)
    {
        var receipt = _context
            .FullReceiptList.Where(receipt => receipt.FullReceiptId == id)
            .Include(entity => entity.TotalReceipt)
            .Include(entity => entity.TourishSchedule)
            //.ThenInclude(entity => entity.TourishPlan)
            .FirstOrDefault();
        if (receipt == null)
        {
            return null;
        }

        return new Response { resultCd = 0, Data = receipt };
    }

    public async Task<Response> UpdateTourReceipt(FullReceiptUpdateModel receiptModel)
    {
        var receipt = _context
            .FullReceiptList.Include(entity => entity.TotalReceipt)
            .FirstOrDefault((receipt => receipt.FullReceiptId == receiptModel.FullReceiptId));
        if (receipt != null)
        {
            var oldTotalTicket = receipt.TotalTicket;

            receipt.GuestName = receiptModel.GuestName;
            receipt.Status = receiptModel.Status;
            receipt.DiscountAmount = receiptModel.DiscountAmount;
            receipt.DiscountFloat = receiptModel.DiscountFloat;
            receipt.OriginalPrice = receiptModel.OriginalPrice;
            receipt.TotalTicket = receiptModel.TotalTicket;
            receipt.TotalChildTicket = receiptModel.TotalChildTicket;
            receipt.TourishScheduleId = receiptModel.TourishScheduleId;
            receipt.Description = receiptModel.Description;
            receipt.Status = receiptModel.Status;

            receipt.UpdateDate = DateTime.UtcNow;
            if (receiptModel.Status == FullReceiptStatus.Completed)
                receipt.CompleteDate = DateTime.UtcNow;

            var planExist = _context.TourishPlan.FirstOrDefault(
                (plan => plan.Id == receipt.TotalReceipt.TourishPlanId)
            );

            if (
                receipt.Status != FullReceiptStatus.Completed
                && receiptModel.Status == FullReceiptStatus.Completed
            )
            {
                if (planExist != null)
                {
                    if (planExist != null && planExist.RemainTicket >= receiptModel.TotalTicket)
                    {
                        planExist.RemainTicket = planExist.RemainTicket - receiptModel.TotalTicket;

                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        return new Response
                        {
                            resultCd = 0,
                            MessageCode = "C515",
                            returnId = receipt.TotalReceiptId,
                            // Out of ticket
                        };
                    }
                }
            }

            if (
                receipt.Status == FullReceiptStatus.Completed
                && receiptModel.Status == FullReceiptStatus.Completed
            )
            {
                if (planExist != null)
                {
                    if (
                        planExist != null
                        && planExist.RemainTicket + oldTotalTicket >= receiptModel.TotalTicket
                    )
                    {
                        planExist.RemainTicket =
                            planExist.RemainTicket - receiptModel.TotalTicket + oldTotalTicket;

                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        return new Response
                        {
                            resultCd = 0,
                            MessageCode = "C515",
                            returnId = receipt.TotalReceiptId,
                            // Out of ticket
                        };
                    }
                }
            }

            if (
                receipt.Status == FullReceiptStatus.Completed
                && receiptModel.Status == FullReceiptStatus.Cancelled
            )
            {
                if (planExist != null)
                {
                    if (planExist.RemainTicket + oldTotalTicket <= planExist.TotalTicket)
                    {
                        planExist.RemainTicket = planExist.RemainTicket + oldTotalTicket;
                    }
                    else
                    {
                        planExist.RemainTicket = planExist.TotalTicket;
                    }

                    await _context.SaveChangesAsync();
                }
            }

            var totalReceiptComplete = await _context
                .TotalReceiptList.Where(receipt =>
                    receipt.TotalReceiptId == receiptModel.TotalReceiptId
                )
                .Include(entity => entity.FullReceiptList)
                .FirstOrDefaultAsync();

            var totalCount = totalReceiptComplete.FullReceiptList.Count();

            var createCount = totalReceiptComplete.FullReceiptList.Count(fullReceipt =>
                fullReceipt.Status == FullReceiptStatus.Created
            );
            var onGoingCount = totalReceiptComplete.FullReceiptList.Count(fullReceipt =>
                fullReceipt.Status == FullReceiptStatus.AwaitPayment
            );
            var complteteCount = totalReceiptComplete.FullReceiptList.Count(fullReceipt =>
                fullReceipt.Status == FullReceiptStatus.Completed
            );
            var cancelCount = totalReceiptComplete.FullReceiptList.Count(fullReceipt =>
                fullReceipt.Status == FullReceiptStatus.Cancelled
            );

            if (totalCount == createCount)
            {
                totalReceiptComplete.Status = ReceiptStatus.Created;
            }
            else if (onGoingCount < totalCount && onGoingCount > 1)
            {
                totalReceiptComplete.Status = ReceiptStatus.OnGoing;
            }
            else if (complteteCount == totalCount)
            {
                totalReceiptComplete.Status = ReceiptStatus.Completed;
            }
            else if (cancelCount == totalCount)
            {
                totalReceiptComplete.Status = ReceiptStatus.Cancelled;
            }

            _context.SaveChanges();
        }

        return new Response
        {
            resultCd = 0,
            MessageCode = "I512",
            // Update type success
        };
    }

    public async Task<Response> UpdateScheduleReceipt(FullReceiptUpdateModel receiptModel)
    {
        var receipt = _context
            .FullReceiptList.Include(entity => entity.TotalReceipt)
            .FirstOrDefault((receipt => receipt.FullReceiptId == receiptModel.FullReceiptId));
        if (receipt != null)
        {
            var oldTotalTicket = receipt.TotalTicket;

            receipt.GuestName = receiptModel.GuestName;
            receipt.Status = receiptModel.Status;
            receipt.DiscountAmount = receiptModel.DiscountAmount;
            receipt.DiscountFloat = receiptModel.DiscountFloat;
            receipt.OriginalPrice = receiptModel.OriginalPrice;
            receipt.TotalTicket = receiptModel.TotalTicket;
            receipt.TotalChildTicket = receiptModel.TotalChildTicket;
            receipt.TourishScheduleId = receiptModel.TourishScheduleId;
            receipt.Description = receiptModel.Description;
            receipt.Status = receiptModel.Status;

            receipt.UpdateDate = DateTime.UtcNow;
            if (receiptModel.Status == FullReceiptStatus.Completed)
                receipt.CompleteDate = DateTime.UtcNow;

            var planExist = _context.TourishPlan.FirstOrDefault(
                (plan => plan.Id == receipt.TotalReceipt.TourishPlanId)
            );

            var totalReceiptComplete = await _context
                .TotalReceiptList.Where(receipt =>
                    receipt.TotalReceiptId == receiptModel.TotalReceiptId
                )
                .Include(entity => entity.FullReceiptList)
                .FirstOrDefaultAsync();

            var totalCount = totalReceiptComplete.FullReceiptList.Count();

            var createCount = totalReceiptComplete.FullReceiptList.Count(fullReceipt =>
                fullReceipt.Status == FullReceiptStatus.Created
            );
            var onGoingCount = totalReceiptComplete.FullReceiptList.Count(fullReceipt =>
                fullReceipt.Status == FullReceiptStatus.AwaitPayment
            );
            var complteteCount = totalReceiptComplete.FullReceiptList.Count(fullReceipt =>
                fullReceipt.Status == FullReceiptStatus.Completed
            );
            var cancelCount = totalReceiptComplete.FullReceiptList.Count(fullReceipt =>
                fullReceipt.Status == FullReceiptStatus.Cancelled
            );

            if (totalCount == createCount)
            {
                totalReceiptComplete.Status = ReceiptStatus.Created;
            }
            else if (onGoingCount < totalCount && onGoingCount > 1)
            {
                totalReceiptComplete.Status = ReceiptStatus.OnGoing;
            }
            else if (complteteCount == totalCount)
            {
                totalReceiptComplete.Status = ReceiptStatus.Completed;
            }
            else if (cancelCount == totalCount)
            {
                totalReceiptComplete.Status = ReceiptStatus.Cancelled;
            }

            _context.SaveChanges();
        }

        return new Response
        {
            resultCd = 0,
            MessageCode = "I512",
            // Update type success
        };
    }

    public async Task<Response> UpdateTourReceiptForUser(FullReceiptUpdateModel receiptModel)
    {
        var receipt = _context
            .FullReceiptList.Include(entity => entity.TotalReceipt)
            .FirstOrDefault((receipt => receipt.FullReceiptId == receiptModel.FullReceiptId));

        if (receipt != null)
        {
            if (receiptModel.Status == FullReceiptStatus.Cancelled)
            {
                return new Response
                {
                    resultCd = 0,
                    MessageCode = "C516",
                    returnId = receipt.TotalReceiptId,
                };
            }

            var planExist = _context.TourishPlan.FirstOrDefault(
                (plan => plan.Id == receipt.TotalReceipt.TourishPlanId)
            );

            if (planExist.PlanStatus == PlanStatus.OnGoing)
            {
                return new Response
                {
                    resultCd = 0,
                    MessageCode = "C517",
                    returnId = receipt.TotalReceiptId,
                };
            }
            else if (planExist.PlanStatus == PlanStatus.Complete)
            {
                return new Response
                {
                    resultCd = 0,
                    MessageCode = "C518",
                    returnId = receipt.TotalReceiptId,
                };
            }
            else if (planExist.PlanStatus == PlanStatus.Cancel)
            {
                return new Response
                {
                    resultCd = 0,
                    MessageCode = "C519",
                    returnId = receipt.TotalReceiptId,
                };
            }

            var oldTotalTicket = receipt.TotalTicket;

            receipt.GuestName = receiptModel.GuestName;
            if (receipt.Status == FullReceiptStatus.Completed)
            {
                return new Response
                {
                    resultCd = 0,
                    MessageCode = "C520",
                    returnId = receipt.TotalReceiptId,
                };
            }
            receipt.Status = receiptModel.Status;
            receipt.TotalTicket = receiptModel.TotalTicket;
            receipt.TotalChildTicket = receiptModel.TotalChildTicket;
            receipt.TourishScheduleId = receiptModel.TourishScheduleId;

            receipt.UpdateDate = DateTime.UtcNow;
            if (receiptModel.Status == FullReceiptStatus.Completed)
                receipt.CompleteDate = DateTime.UtcNow;

            if (
                receipt.Status != FullReceiptStatus.Completed
                && receiptModel.Status == FullReceiptStatus.Completed
            )
            {
                if (planExist != null)
                {
                    if (planExist != null && planExist.RemainTicket >= receiptModel.TotalTicket)
                    {
                        planExist.RemainTicket = planExist.RemainTicket - receiptModel.TotalTicket;

                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        return new Response
                        {
                            resultCd = 0,
                            MessageCode = "C515",
                            returnId = receipt.TotalReceiptId,
                            // Out of ticket
                        };
                    }
                }
            }

            if (
                receipt.Status == FullReceiptStatus.Completed
                && receiptModel.Status == FullReceiptStatus.Completed
            )
            {
                if (planExist != null)
                {
                    if (
                        planExist != null
                        && planExist.RemainTicket + oldTotalTicket >= receiptModel.TotalTicket
                    )
                    {
                        planExist.RemainTicket =
                            planExist.RemainTicket - receiptModel.TotalTicket + oldTotalTicket;

                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        return new Response
                        {
                            resultCd = 0,
                            MessageCode = "C515",
                            returnId = receipt.TotalReceiptId,
                            // Out of ticket
                        };
                    }
                }
            }

            if (
                receipt.Status == FullReceiptStatus.Completed
                && receiptModel.Status == FullReceiptStatus.Cancelled
            )
            {
                if (planExist != null)
                {
                    if (planExist.RemainTicket + oldTotalTicket <= planExist.TotalTicket)
                    {
                        planExist.RemainTicket = planExist.RemainTicket + oldTotalTicket;
                    }
                    else
                    {
                        planExist.RemainTicket = planExist.TotalTicket;
                    }

                    await _context.SaveChangesAsync();
                }
            }

            var totalReceiptComplete = await _context
                .TotalReceiptList.Where(receipt =>
                    receipt.TotalReceiptId == receiptModel.TotalReceiptId
                )
                .Include(entity => entity.FullReceiptList)
                .FirstOrDefaultAsync();

            var totalCount = totalReceiptComplete.FullReceiptList.Count();

            var createCount = totalReceiptComplete.FullReceiptList.Count(fullReceipt =>
                fullReceipt.Status == FullReceiptStatus.Created
            );
            var onGoingCount = totalReceiptComplete.FullReceiptList.Count(fullReceipt =>
                fullReceipt.Status == FullReceiptStatus.AwaitPayment
            );
            var complteteCount = totalReceiptComplete.FullReceiptList.Count(fullReceipt =>
                fullReceipt.Status == FullReceiptStatus.Completed
            );
            var cancelCount = totalReceiptComplete.FullReceiptList.Count(fullReceipt =>
                fullReceipt.Status == FullReceiptStatus.Cancelled
            );

            if (totalCount == createCount)
            {
                totalReceiptComplete.Status = ReceiptStatus.Created;
            }
            else if (onGoingCount < totalCount && onGoingCount > 1)
            {
                totalReceiptComplete.Status = ReceiptStatus.OnGoing;
            }
            else if (complteteCount == totalCount)
            {
                totalReceiptComplete.Status = ReceiptStatus.Completed;
            }
            else if (cancelCount == totalCount)
            {
                totalReceiptComplete.Status = ReceiptStatus.Cancelled;
            }

            _context.SaveChanges();
        }

        return new Response
        {
            resultCd = 0,
            MessageCode = "I512",
            // Update type success
        };
    }

    public async Task<Response> UpdateScheduleReceiptForUser(FullReceiptUpdateModel receiptModel)
    {
        var receipt = _context
            .FullReceiptList.Include(entity => entity.TotalReceipt)
            .FirstOrDefault((receipt => receipt.FullReceiptId == receiptModel.FullReceiptId));

        if (receipt != null)
        {
            if (receiptModel.Status == FullReceiptStatus.Cancelled)
            {
                return new Response
                {
                    resultCd = 0,
                    MessageCode = "C516",
                    returnId = receipt.TotalReceiptId,
                };
            }

            if (receiptModel.ScheduleType == ScheduleType.MovingSchedule)
            {
                var scheduleExist = _context.MovingSchedules.FirstOrDefault(
                    (plan => plan.Id == receipt.TotalReceipt.ScheduleId)
                );

                if (scheduleExist.Status == MovingScheduleStatus.OnGoing)
                {
                    return new Response
                    {
                        resultCd = 0,
                        MessageCode = "C517",
                        returnId = receipt.TotalReceiptId,
                    };
                }
                else if (scheduleExist.Status == MovingScheduleStatus.Completed)
                {
                    return new Response
                    {
                        resultCd = 0,
                        MessageCode = "C518",
                        returnId = receipt.TotalReceiptId,
                    };
                }
                else if (scheduleExist.Status == MovingScheduleStatus.Cancelled)
                {
                    return new Response
                    {
                        resultCd = 0,
                        MessageCode = "C519",
                        returnId = receipt.TotalReceiptId,
                    };
                }

                if (receipt.Status == FullReceiptStatus.Completed)
                {
                    return new Response
                    {
                        resultCd = 0,
                        MessageCode = "C520",
                        returnId = receipt.TotalReceiptId,
                    };
                }
            } else if (receiptModel.ScheduleType == ScheduleType.StayingSchedule)
            {
                var scheduleExist = _context.StayingSchedules.FirstOrDefault(
                    (plan => plan.Id == receipt.TotalReceipt.ScheduleId)
                );

                if (scheduleExist.Status == StayingScheduleStatus.OnGoing)
                {
                    return new Response
                    {
                        resultCd = 0,
                        MessageCode = "C517",
                        returnId = receipt.TotalReceiptId,
                    };
                }
                else if (scheduleExist.Status == StayingScheduleStatus.Completed)
                {
                    return new Response
                    {
                        resultCd = 0,
                        MessageCode = "C518",
                        returnId = receipt.TotalReceiptId,
                    };
                }
                else if (scheduleExist.Status == StayingScheduleStatus.Cancelled)
                {
                    return new Response
                    {
                        resultCd = 0,
                        MessageCode = "C519",
                        returnId = receipt.TotalReceiptId,
                    };
                }

                if (receipt.Status == FullReceiptStatus.Completed)
                {
                    return new Response
                    {
                        resultCd = 0,
                        MessageCode = "C520",
                        returnId = receipt.TotalReceiptId,
                    };
                }
            }

            receipt.GuestName = receiptModel.GuestName;
            receipt.Status = receiptModel.Status;
            receipt.TotalTicket = receiptModel.TotalTicket;
            receipt.TotalChildTicket = receiptModel.TotalChildTicket;
            receipt.TourishScheduleId = receiptModel.TourishScheduleId;
            receipt.UpdateDate = DateTime.UtcNow;

            if (receiptModel.Status == FullReceiptStatus.Completed)
                receipt.CompleteDate = DateTime.UtcNow;

            var totalReceiptComplete = await _context
                .TotalReceiptList.Where(receipt =>
                    receipt.TotalReceiptId == receiptModel.TotalReceiptId
                )
                .Include(entity => entity.FullReceiptList)
                .FirstOrDefaultAsync();

            var totalCount = totalReceiptComplete.FullReceiptList.Count();

            var createCount = totalReceiptComplete.FullReceiptList.Count(fullReceipt =>
                fullReceipt.Status == FullReceiptStatus.Created
            );
            var onGoingCount = totalReceiptComplete.FullReceiptList.Count(fullReceipt =>
                fullReceipt.Status == FullReceiptStatus.AwaitPayment
            );
            var complteteCount = totalReceiptComplete.FullReceiptList.Count(fullReceipt =>
                fullReceipt.Status == FullReceiptStatus.Completed
            );
            var cancelCount = totalReceiptComplete.FullReceiptList.Count(fullReceipt =>
                fullReceipt.Status == FullReceiptStatus.Cancelled
            );

            if (totalCount == createCount)
            {
                totalReceiptComplete.Status = ReceiptStatus.Created;
            }
            else if (onGoingCount < totalCount && onGoingCount > 1)
            {
                totalReceiptComplete.Status = ReceiptStatus.OnGoing;
            }
            else if (complteteCount == totalCount)
            {
                totalReceiptComplete.Status = ReceiptStatus.Completed;
            }
            else if (cancelCount == totalCount)
            {
                totalReceiptComplete.Status = ReceiptStatus.Cancelled;
            }

            _context.SaveChanges();
        }

        return new Response
        {
            resultCd = 0,
            MessageCode = "I512",
            // Update type success
        };
    }

    public Response getUnpaidClient()
    {
        var receiptList = _context
            .FullReceiptList.Include(entity => entity.TotalReceipt)
            .Include(entity => entity.TotalReceipt)
            .Include(entity => entity.TourishSchedule)
            .ThenInclude(entity => entity.TourishPlan)
            .Where(entity => (int)entity.Status < 2)
            .Select(entity => new
            {
                GuestName = entity.GuestName,
                OriginalPrice = entity.OriginalPrice,
                TotalTicket = entity.TotalTicket,
                TotalChildTicket = entity.TotalChildTicket,
                TourishPlanId = entity.TotalReceipt.TourishPlanId,
                ScheduleId = entity.TotalReceipt.ScheduleId,
                ScheduleType = entity.TotalReceipt.ScheduleType
            })
            .ToList();

        if (receiptList == null)
        {
            return new Response { resultCd = 1, Data = new List<FullReceipt>() };
        }

        return new Response { resultCd = 0, Data = receiptList };
    }

    public Response getTopGrossTourInMonth()
    {
        var receiptList = _context
            .FullReceiptList.Include(entity => entity.TotalReceipt)
            .Include(entity => entity.TotalReceipt)
            .Include(entity => entity.TourishSchedule)
            .ThenInclude(entity => entity.TourishPlan)
            .Where(entity => entity.TotalReceipt.TourishPlanId.HasValue)
            .Where(entity => (int)entity.Status < 3)
            .Where(entity =>
                (
                    entity.CreatedDate.Month == DateTime.UtcNow.Month
                    && entity.CreatedDate.Year == DateTime.UtcNow.Year
                )
                || (
                    entity.CompleteDate.Value.Month == DateTime.UtcNow.Month
                    && entity.CompleteDate.Value.Year == DateTime.UtcNow.Year
                )
            )
            .OrderByDescending(entity =>
                (
                    entity.OriginalPrice * entity.TotalTicket
                    + entity.TotalChildTicket * entity.TotalTicket
                    - entity.DiscountAmount
                ) * (1 - entity.DiscountFloat)
            )
            .Select(entity => new
            {
                GuestName = entity.GuestName,
                TotalPrice = (
                    entity.OriginalPrice * entity.TotalTicket
                    + entity.TotalChildTicket * entity.TotalTicket
                    - entity.DiscountAmount
                ) * (1 - entity.DiscountFloat),
                OriginalPrice = entity.OriginalPrice,
                TotalTicket = entity.TotalTicket,
                TotalChildTicket = entity.TotalChildTicket,
                TourishPlanId = entity.TotalReceipt.TourishPlanId,
                ScheduleId = entity.TotalReceipt.ScheduleId,
                ScheduleType = entity.TotalReceipt.ScheduleType
            })
            .ToList();

        if (receiptList == null)
        {
            return new Response { resultCd = 1, Data = new List<FullReceipt>() };
        }

        return new Response { resultCd = 0, Data = receiptList };
    }

    public Response getTopTicketTourInMonth()
    {
        var receiptList = _context
            .FullReceiptList.Include(entity => entity.TotalReceipt)
            .Include(entity => entity.TotalReceipt)
            .Include(entity => entity.TourishSchedule)
            .ThenInclude(entity => entity.TourishPlan)
            .Where(entity => entity.TotalReceipt.TourishPlanId.HasValue)
            .Where(entity => (int)entity.Status < 3)
            .Where(entity =>
                (
                    entity.CreatedDate.Month == DateTime.UtcNow.Month
                    && entity.CreatedDate.Year == DateTime.UtcNow.Year
                )
                || (
                    entity.CompleteDate.Value.Month == DateTime.UtcNow.Month
                    && entity.CompleteDate.Value.Year == DateTime.UtcNow.Year
                )
            )
            .OrderByDescending(entity => entity.TotalTicket + entity.TotalChildTicket)
            .Select(entity => new
            {
                GuestName = entity.GuestName,
                TotalPrice = (
                    entity.OriginalPrice * entity.TotalTicket
                    + entity.TotalChildTicket * entity.TotalTicket
                    - entity.DiscountAmount
                ) * (1 - entity.DiscountFloat),
                OriginalPrice = entity.OriginalPrice,
                TotalTicket = entity.TotalTicket,
                TotalChildTicket = entity.TotalChildTicket,
                TourishPlanId = entity.TotalReceipt.TourishPlanId,
                ScheduleId = entity.TotalReceipt.ScheduleId,
                ScheduleType = entity.TotalReceipt.ScheduleType
            })
            .ToList();

        if (receiptList == null)
        {
            return new Response { resultCd = 1, Data = new List<FullReceipt>() };
        }

        return new Response { resultCd = 0, Data = receiptList };
    }

    public Response getTopGrossMovingScheduleInMonth()
    {
        var receiptList = _context
            .FullReceiptList.Include(entity => entity.TotalReceipt)
            .Include(entity => entity.TotalReceipt)
            .Where(entity => (int)entity.Status < 3)
            .Where(entity => !entity.TotalReceipt.TourishPlanId.HasValue)
            .Where(entity => entity.TotalReceipt.ScheduleType == ScheduleType.MovingSchedule)
            .Where(entity =>
                (
                    entity.CreatedDate.Month == DateTime.UtcNow.Month
                    && entity.CreatedDate.Year == DateTime.UtcNow.Year
                )
                || (
                    entity.CompleteDate.Value.Month == DateTime.UtcNow.Month
                    && entity.CompleteDate.Value.Year == DateTime.UtcNow.Year
                )
            )
            .OrderByDescending(entity =>
                (
                    entity.OriginalPrice * entity.TotalTicket
                    + entity.TotalChildTicket * entity.TotalTicket
                    - entity.DiscountAmount
                ) * (1 - entity.DiscountFloat)
            )
            .Select(entity => new
            {
                GuestName = entity.GuestName,
                TotalPrice = (
                    entity.OriginalPrice * entity.TotalTicket
                    + entity.TotalChildTicket * entity.TotalTicket
                    - entity.DiscountAmount
                ) * (1 - entity.DiscountFloat),
                OriginalPrice = entity.OriginalPrice,
                TotalTicket = entity.TotalTicket,
                TotalChildTicket = entity.TotalChildTicket,
                TourishPlanId = entity.TotalReceipt.TourishPlanId,
                ScheduleId = entity.TotalReceipt.ScheduleId,
                ScheduleType = entity.TotalReceipt.ScheduleType
            })
            .ToList();

        if (receiptList == null)
        {
            return new Response { resultCd = 1, Data = new List<FullReceipt>() };
        }

        return new Response { resultCd = 0, Data = receiptList };
    }

    public Response getTopGrossStayingScheduleInMonth()
    {
        var receiptList = _context
            .FullReceiptList.Include(entity => entity.TotalReceipt)
            .Include(entity => entity.TotalReceipt)
            .Where(entity => (int)entity.Status < 3)
            .Where(entity => !entity.TotalReceipt.TourishPlanId.HasValue)
            .Where(entity => entity.TotalReceipt.ScheduleType == ScheduleType.StayingSchedule)
            .Where(entity =>
                (
                    entity.CreatedDate.Month == DateTime.UtcNow.Month
                    && entity.CreatedDate.Year == DateTime.UtcNow.Year
                )
                || (
                    entity.CompleteDate.Value.Month == DateTime.UtcNow.Month
                    && entity.CompleteDate.Value.Year == DateTime.UtcNow.Year
                )
            )
            .OrderByDescending(entity =>
                (
                    entity.OriginalPrice * entity.TotalTicket
                    + entity.TotalChildTicket * entity.TotalTicket
                    - entity.DiscountAmount
                ) * (1 - entity.DiscountFloat)
            )
            .Select(entity => new
            {
                GuestName = entity.GuestName,
                TotalPrice = (
                    entity.OriginalPrice * entity.TotalTicket
                    + entity.TotalChildTicket * entity.TotalTicket
                    - entity.DiscountAmount
                ) * (1 - entity.DiscountFloat),
                OriginalPrice = entity.OriginalPrice,
                TotalTicket = entity.TotalTicket,
                TotalChildTicket = entity.TotalChildTicket,
                TourishPlanId = entity.TotalReceipt.TourishPlanId,
                ScheduleId = entity.TotalReceipt.ScheduleId,
                ScheduleType = entity.TotalReceipt.ScheduleType
            })
            .ToList();

        if (receiptList == null)
        {
            return new Response { resultCd = 1, Data = new List<FullReceipt>() };
        }

        return new Response { resultCd = 0, Data = receiptList };
    }

    public Response getTopTicketMovingScheduleInMonth()
    {
        var receiptList = _context
            .FullReceiptList.Include(entity => entity.TotalReceipt)
            .Include(entity => entity.TotalReceipt)
            .Where(entity => (int)entity.Status < 3)
            .Where(entity => !entity.TotalReceipt.TourishPlanId.HasValue)
            .Where(entity => entity.TotalReceipt.ScheduleType == ScheduleType.MovingSchedule)
            .Where(entity =>
                (
                    entity.CreatedDate.Month == DateTime.UtcNow.Month
                    && entity.CreatedDate.Year == DateTime.UtcNow.Year
                )
                || (
                    entity.CompleteDate.Value.Month == DateTime.UtcNow.Month
                    && entity.CompleteDate.Value.Year == DateTime.UtcNow.Year
                )
            )
            .OrderByDescending(entity => entity.TotalTicket + entity.TotalChildTicket)
            .Select(entity => new
            {
                GuestName = entity.GuestName,
                TotalPrice = (
                    entity.OriginalPrice * entity.TotalTicket
                    + entity.TotalChildTicket * entity.TotalTicket
                    - entity.DiscountAmount
                ) * (1 - entity.DiscountFloat),
                OriginalPrice = entity.OriginalPrice,
                TotalTicket = entity.TotalTicket,
                TotalChildTicket = entity.TotalChildTicket,
                TourishPlanId = entity.TotalReceipt.TourishPlanId,
                ScheduleId = entity.TotalReceipt.ScheduleId,
                ScheduleType = entity.TotalReceipt.ScheduleType
            })
            .ToList();

        if (receiptList == null)
        {
            return new Response { resultCd = 1, Data = new List<FullReceipt>() };
        }

        return new Response { resultCd = 0, Data = receiptList };
    }

    public Response getTopTicketStayingScheduleInMonth()
    {
        var receiptList = _context
            .FullReceiptList.Include(entity => entity.TotalReceipt)
            .Include(entity => entity.TotalReceipt)
            .Where(entity => (int)entity.Status < 3)
            .Where(entity => !entity.TotalReceipt.TourishPlanId.HasValue)
            .Where(entity => entity.TotalReceipt.ScheduleType == ScheduleType.StayingSchedule)
            .Where(entity =>
                (
                    entity.CreatedDate.Month == DateTime.UtcNow.Month
                    && entity.CreatedDate.Year == DateTime.UtcNow.Year
                )
                || (
                    entity.CompleteDate.Value.Month == DateTime.UtcNow.Month
                    && entity.CompleteDate.Value.Year == DateTime.UtcNow.Year
                )
            )
            .OrderByDescending(entity => entity.TotalTicket + entity.TotalChildTicket)
            .Select(entity => new
            {
                GuestName = entity.GuestName,
                TotalPrice = (
                    entity.OriginalPrice * entity.TotalTicket
                    + entity.TotalChildTicket * entity.TotalTicket
                    - entity.DiscountAmount
                ) * (1 - entity.DiscountFloat),
                OriginalPrice = entity.OriginalPrice,
                TotalTicket = entity.TotalTicket,
                TotalChildTicket = entity.TotalChildTicket,
                TourishPlanId = entity.TotalReceipt.TourishPlanId,
                ScheduleId = entity.TotalReceipt.ScheduleId,
                ScheduleType = entity.TotalReceipt.ScheduleType
            })
            .ToList();

        if (receiptList == null)
        {
            return new Response { resultCd = 1, Data = new List<FullReceipt>() };
        }

        return new Response { resultCd = 0, Data = receiptList };
    }

    public Response getGrossStayingScheduleInYear()
    {
        var currentDate = DateTime.UtcNow;
        var startOfYear = new DateTime(currentDate.Year, 1, 1);

        var receiptGrossByMonth = _context
            .FullReceiptList.Include(entity => entity.TotalReceipt)
            .Include(entity => entity.TotalReceipt)
            .Where(entity => (int)entity.Status < 3)
            .Where(entity => !entity.TotalReceipt.TourishPlanId.HasValue)
            .Where(entity => entity.TotalReceipt.ScheduleType == ScheduleType.StayingSchedule)
            .Where(entity =>
                (entity.CreatedDate >= startOfYear && entity.CreatedDate <= currentDate)
                || (
                    entity.CompleteDate.HasValue
                    && entity.CompleteDate.Value >= startOfYear
                    && entity.CompleteDate.Value <= currentDate
                )
            )
            .GroupBy(entity => new
            {
                Month = entity.CreatedDate.Month,
                Year = entity.CreatedDate.Year
            })
            .Select(group => new
            {
                Month = group.Key.Month,
                Year = group.Key.Year,
                Gross = group.Sum(entity =>
                    (
                        entity.OriginalPrice * entity.TotalTicket
                        + entity.TotalChildTicket * entity.TotalTicket
                        - entity.DiscountAmount
                    ) * (1 - entity.DiscountFloat)
                )
            })
            .ToList();

        if (receiptGrossByMonth == null)
        {
            return new Response { resultCd = 1, Data = new List<FullReceipt>() };
        }

        return new Response { resultCd = 0, Data = receiptGrossByMonth };
    }

    public Response getGrossMovingScheduleInYear()
    {
        var currentDate = DateTime.UtcNow;
        var startOfYear = new DateTime(currentDate.Year, 1, 1);

        var receiptGrossByMonth = _context
            .FullReceiptList.Include(entity => entity.TotalReceipt)
            .Include(entity => entity.TotalReceipt)
            .Where(entity => (int)entity.Status < 3)
            .Where(entity => !entity.TotalReceipt.TourishPlanId.HasValue)
            .Where(entity => entity.TotalReceipt.ScheduleType == ScheduleType.MovingSchedule)
            .Where(entity =>
                (entity.CreatedDate >= startOfYear && entity.CreatedDate <= currentDate)
                || (
                    entity.CompleteDate.HasValue
                    && entity.CompleteDate.Value >= startOfYear
                    && entity.CompleteDate.Value <= currentDate
                )
            )
            .GroupBy(entity => new
            {
                Month = entity.CreatedDate.Month,
                Year = entity.CreatedDate.Year
            })
            .Select(group => new
            {
                Month = group.Key.Month,
                Year = group.Key.Year,
                Gross = group.Sum(entity =>
                    (
                        entity.OriginalPrice * entity.TotalTicket
                        + entity.TotalChildTicket * entity.TotalTicket
                        - entity.DiscountAmount
                    ) * (1 - entity.DiscountFloat)
                )
            })
            .ToList();

        if (receiptGrossByMonth == null)
        {
            return new Response { resultCd = 1, Data = new List<FullReceipt>() };
        }

        return new Response { resultCd = 0, Data = receiptGrossByMonth };
    }

    public Response getGrossTourishPlanInYear()
    {
        var currentDate = DateTime.UtcNow;
        var startOfYear = new DateTime(currentDate.Year, 1, 1);

        var receiptGrossByMonth = _context
            .FullReceiptList.Include(entity => entity.TotalReceipt)
            .Include(entity => entity.TotalReceipt)
            .Where(entity => (int)entity.Status < 3)
            .Where(entity => entity.TotalReceipt.TourishPlanId.HasValue)
            .Where(entity =>
                (entity.CreatedDate >= startOfYear && entity.CreatedDate <= currentDate)
                || (
                    entity.CompleteDate.HasValue
                    && entity.CompleteDate.Value >= startOfYear
                    && entity.CompleteDate.Value <= currentDate
                )
            )
            .GroupBy(entity => new
            {
                Month = entity.CreatedDate.Month,
                Year = entity.CreatedDate.Year
            })
            .Select(group => new
            {
                Month = group.Key.Month,
                Year = group.Key.Year,
                Gross = group.Sum(entity =>
                    (
                        entity.OriginalPrice * entity.TotalTicket
                        + entity.TotalChildTicket * entity.TotalTicket
                        - entity.DiscountAmount
                    ) * (1 - entity.DiscountFloat)
                )
            })
            .ToList();

        if (receiptGrossByMonth == null)
        {
            return new Response { resultCd = 1, Data = new List<FullReceipt>() };
        }

        return new Response { resultCd = 0, Data = receiptGrossByMonth };
    }
}
