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

    public async Task<Response> AddTourReceipt(FullReceiptInsertModel receiptModel)
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
                if (receiptModel.MovingScheduleId != null)
                {
                    var schedule = _context.MovingSchedules.FirstOrDefault(e =>
                        e.Id == receiptModel.MovingScheduleId
                    );
                    originalPrice = schedule.SinglePrice ?? (double)0;
                }

                if (receiptModel.StayingScheduleId != null)
                {
                    var schedule = _context.StayingSchedules.FirstOrDefault(e =>
                        e.Id == receiptModel.StayingScheduleId
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
        else
        {
            return new Response { resultCd = 0, MessageCode = "C414" };
        }
    }

    public async Task<Response> AddServiceReceipt(FullReceiptInsertModel receiptModel)
    {
        var totalReceipt = _context.TotalScheduleReceiptList.FirstOrDefault(entity =>
            entity.MovingScheduleId == receiptModel.MovingScheduleId
            && entity.StayingScheduleId == receiptModel.StayingScheduleId
        );

        if (totalReceipt == null)
        {
            totalReceipt = new TotalScheduleReceipt
            {
                Status = ReceiptStatus.Created,
                Description = receiptModel.Description,

                MovingScheduleId = receiptModel.MovingScheduleId,
                StayingScheduleId = receiptModel.StayingScheduleId,

                CreatedDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow
            };

            await _context.AddAsync(totalReceipt);
            await _context.SaveChangesAsync();
        }

        var existFullReceipt = _context.FullScheduleReceiptList.FirstOrDefault(entity =>
            entity.Email == receiptModel.Email
            && entity.TotalReceiptId == totalReceipt.TotalReceiptId
        );

        var originalPrice = (double)0;

        if (receiptModel.MovingScheduleId != null)
        {
            var schedule = _context.MovingSchedules.FirstOrDefault(e =>
                e.Id == receiptModel.MovingScheduleId
            );
            originalPrice = schedule.SinglePrice ?? (double)0;
        }

        if (receiptModel.StayingScheduleId != null)
        {
            var schedule = _context.StayingSchedules.FirstOrDefault(e =>
                e.Id == receiptModel.StayingScheduleId
            );
            originalPrice = schedule.SinglePrice ?? (double)0;
        }

        if (existFullReceipt == null)
        {
            var fullReceipt = new FullScheduleReceipt
            {
                TotalReceiptId = totalReceipt.TotalReceiptId,
                ServiceScheduleId = receiptModel.ServiceScheduleId,
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

            await _context.AddAsync(fullReceipt);
            await _context.SaveChangesAsync();

            return new Response
            {
                resultCd = 0,
                MessageCode = "I512",
                returnId = fullReceipt.FullReceiptId,
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
                MessageCode = "I511",
                returnId = existFullReceipt.FullReceiptId,
                // Create type success
            };
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
            var user = _context.Users.FirstOrDefault(u => u.Email == receiptModel.Email);

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

            existFullReceipt.TourishScheduleId = receiptModel.TourishScheduleId;
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
        var totalReceipt = _context.TotalScheduleReceiptList.FirstOrDefault(entity =>
            entity.MovingScheduleId == receiptModel.MovingScheduleId
            && entity.StayingScheduleId == receiptModel.StayingScheduleId
        );

        if (totalReceipt == null)
        {
            totalReceipt = new TotalScheduleReceipt
            {
                MovingScheduleId = receiptModel.MovingScheduleId,
                StayingScheduleId = receiptModel.StayingScheduleId,
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
            var user = _context.Users.FirstOrDefault(u => u.Email == receiptModel.Email);

            if (receiptModel.MovingScheduleId != null)
            {
                var schedule = _context.MovingSchedules.FirstOrDefault(entity =>
                    entity.Id == receiptModel.MovingScheduleId
                );

                if (schedule == null)
                    return new Response { resultCd = 1, MessageCode = "I434", };

                var existScheduleInterest = _context.ScheduleInterests.FirstOrDefault(u =>
                    u.UserId == user.Id && u.MovingScheduleId == receiptModel.MovingScheduleId
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
            else if (receiptModel.StayingScheduleId != null)
            {
                var schedule = _context.StayingSchedules.FirstOrDefault(entity =>
                    entity.Id == receiptModel.StayingScheduleId
                );

                if (schedule == null)
                    return new Response { resultCd = 1, MessageCode = "I434", };

                var existScheduleInterest = _context.ScheduleInterests.FirstOrDefault(u =>
                    u.UserId == user.Id && u.StayingScheduleId == receiptModel.StayingScheduleId
                );
                if (existScheduleInterest == null)
                {
                    var scheduleInterest = new ScheduleInterest
                    {
                        InterestStatus = InterestStatus.User,
                        User = user,
                        StayingScheduleId = schedule.Id,
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

        if (receiptModel.MovingScheduleId != null)
        {
            var schedule = _context.MovingSchedules.FirstOrDefault(e =>
                e.Id == receiptModel.MovingScheduleId
            );
            originalPrice = schedule.SinglePrice ?? (double)0;
        }
        if (receiptModel.StayingScheduleId != null)
        {
            var schedule = _context.StayingSchedules.FirstOrDefault(e =>
                e.Id == receiptModel.StayingScheduleId
            );
            originalPrice = schedule.SinglePrice ?? (double)0;
        }

        var existFullReceipt = _context.FullScheduleReceiptList.FirstOrDefault(entity =>
            entity.Email == receiptModel.Email
            && entity.TotalReceiptId == totalReceipt.TotalReceiptId
        );

        if (existFullReceipt == null)
        {
            var fullReceipt = new FullScheduleReceipt
            {
                TotalReceiptId = totalReceipt.TotalReceiptId,
                ServiceScheduleId = receiptModel.ServiceScheduleId,
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

            await _context.AddAsync(fullReceipt);
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

    public Response DeleteTourReceipt(Guid id)
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

    public Response DeleteScheduleReceipt(Guid id)
    {
        var receipt = _context
            .FullScheduleReceiptList.Include(entity => entity.TotalReceipt)
            .FirstOrDefault((receipt => receipt.FullReceiptId == id));
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

    public Response DeleteAllByTourishPlanId(Guid tourishPlanId)
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

    public Response DeleteAllByMovingScheduleId(Guid scheduleId)
    {
        var receipt = _context.TotalScheduleReceiptList.FirstOrDefault(
            (receipt => receipt.MovingScheduleId == scheduleId)
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

    public Response DeleteAllByStayingScheduleId(Guid scheduleId)
    {
        var receipt = _context.TotalScheduleReceiptList.FirstOrDefault(
            (receipt => receipt.StayingScheduleId == scheduleId)
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

    public Response GetAllTourReceipt(
        string? tourishPlanId,
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
            .Include(receipt => receipt.FullReceiptList)
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

        if (status != null)
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
            Status = entity.Status,
            Description = entity.Description,
            CompleteDate = entity.CompleteDate,
            FullReceiptList = entity.FullReceiptList,
            TourishPlan = entity.TourishPlan,
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

    public Response GetAllScheduleReceipt(
        string? movingScheduleId,
        string? stayingScheduleId,
        ScheduleType? scheduleType,
        ReceiptStatus? status,
        string? sortBy,
        string? sortDirection,
        int page = 1,
        int pageSize = 5
    )
    {
        var receiptQuery = _context
            .TotalScheduleReceiptList.Include(receipt => receipt.FullReceiptList)
            .ThenInclude(receipt => receipt.ServiceSchedule)
            .Include(receipt => receipt.MovingSchedule)
            .Include(receipt => receipt.StayingSchedule)
            .AsQueryable();

        #region Filtering
        if (scheduleType != null)
        {
            if (scheduleType == ScheduleType.MovingSchedule)
                receiptQuery = receiptQuery.Where(receipt => receipt.MovingScheduleId != null);

            if (scheduleType == ScheduleType.StayingSchedule)
                receiptQuery = receiptQuery.Where(receipt => receipt.StayingScheduleId != null);
        }

        if (!string.IsNullOrEmpty(movingScheduleId))
        {
            receiptQuery = receiptQuery.Where(receipt =>
                receipt.MovingScheduleId == new Guid(movingScheduleId)
            );
        }

        if (!string.IsNullOrEmpty(stayingScheduleId))
        {
            receiptQuery = receiptQuery.Where(receipt =>
                receipt.StayingScheduleId == new Guid(stayingScheduleId)
            );
        }

        if (status != null)
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
        var result = PaginatorModel<TotalScheduleReceipt>.Create(receiptQuery, page, PAGE_SIZE);
        #endregion

        var resultDto = result.Select(entity => new
        {
            TotalReceiptId = entity.TotalReceiptId,
            MovingScheduleId = entity.MovingScheduleId,
            StayingScheduleId = entity.StayingScheduleId,
            Status = entity.Status,
            Description = entity.Description,
            CompleteDate = entity.CompleteDate,
            FullReceiptList = entity.FullReceiptList,
            StayingSchedule = entity.StayingSchedule,
            MovingSchedule = entity.MovingSchedule,
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

    public Response GetAllTourReceiptForUser(
        string? email,
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
            .Include(receipt => receipt.FullReceiptList)
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
            Status = entity.Status,
            Description = entity.Description,
            CompleteDate = entity.CompleteDate,

            FullReceiptList = entity
                .FullReceiptList.Where(entity => entity.Email == email)
                .ToList(),

            TourishPlan = entity.TourishPlan,
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

    public Response GetAllScheduleReceiptForUser(
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
            .TotalScheduleReceiptList.Include(receipt => receipt.FullReceiptList)
            .ThenInclude(receipt => receipt.ServiceSchedule)
            .Include(receipt => receipt.MovingSchedule)
            .Include(receipt => receipt.StayingSchedule)
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
            if (scheduleType == ScheduleType.MovingSchedule)
                receiptQuery = receiptQuery.Where(receipt => receipt.MovingScheduleId != null);

            if (scheduleType == ScheduleType.StayingSchedule)
                receiptQuery = receiptQuery.Where(receipt => receipt.StayingScheduleId != null);
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
        var result = PaginatorModel<TotalScheduleReceipt>.Create(receiptQuery, page, PAGE_SIZE);
        #endregion

        var resultDto = result.Select(entity => new
        {
            TotalReceiptId = entity.TotalReceiptId,
            MovingScheduleId = entity.MovingScheduleId,
            StayingScheduleId = entity.StayingScheduleId,

            Status = entity.Status,
            Description = entity.Description,
            CompleteDate = entity.CompleteDate,

            FullReceiptList = entity
                .FullReceiptList.Where(entity => entity.Email == email)
                .ToList(),

            StayingSchedule = entity.StayingSchedule,
            MovingSchedule = entity.MovingSchedule,

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

    public Response getTotalTourReceiptById(Guid id)
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

    public Response getTotalScheduleReceiptById(Guid id)
    {
        var receipt = _context
            .TotalScheduleReceiptList.Where(receipt => receipt.TotalReceiptId == id)
            .Include(receipt => receipt.FullReceiptList)
            .FirstOrDefault();
        if (receipt == null)
        {
            return null;
        }

        return new Response { resultCd = 0, Data = receipt };
    }

    public Response getFullTourReceiptById(Guid id)
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

    public Response getFullScheduleReceiptById(Guid id)
    {
        var receipt = _context
            .FullScheduleReceiptList.Where(receipt => receipt.FullReceiptId == id)
            .Include(entity => entity.TotalReceipt)
            .Include(entity => entity.ServiceSchedule)
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
                && receiptModel.Status != FullReceiptStatus.Completed
            )
            {
                await _context.SaveChangesAsync();
            }

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
            .FullScheduleReceiptList.Include(entity => entity.TotalReceipt)
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
            receipt.ServiceScheduleId = receiptModel.ServiceScheduleId;
            receipt.Description = receiptModel.Description;
            receipt.Status = receiptModel.Status;

            receipt.UpdateDate = DateTime.UtcNow;
            if (receiptModel.Status == FullReceiptStatus.Completed)
                receipt.CompleteDate = DateTime.UtcNow;
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

            var scheduleExist = _context.TourishScheduleList.FirstOrDefault(
                (plan => plan.Id == receipt.TourishScheduleId)
            );

            if (scheduleExist.PlanStatus == PlanStatus.OnGoing)
            {
                return new Response
                {
                    resultCd = 0,
                    MessageCode = "C517",
                    returnId = receipt.TotalReceiptId,
                };
            }
            else if (scheduleExist.PlanStatus == PlanStatus.Complete)
            {
                return new Response
                {
                    resultCd = 0,
                    MessageCode = "C518",
                    returnId = receipt.TotalReceiptId,
                };
            }
            else if (scheduleExist.PlanStatus == PlanStatus.Cancel)
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
            .FullScheduleReceiptList.Include(entity => entity.TotalReceipt)
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

            if (receiptModel.MovingScheduleId != null)
            {
                var scheduleExist = _context
                    .ServiceSchedule.Include(entity => entity.Id == receiptModel.ServiceScheduleId)
                    .FirstOrDefault();

                if (scheduleExist.Status == ScheduleStatus.OnGoing)
                {
                    return new Response
                    {
                        resultCd = 0,
                        MessageCode = "C517",
                        returnId = receipt.TotalReceiptId,
                    };
                }
                else if (scheduleExist.Status == ScheduleStatus.Completed)
                {
                    return new Response
                    {
                        resultCd = 0,
                        MessageCode = "C518",
                        returnId = receipt.TotalReceiptId,
                    };
                }
                else if (scheduleExist.Status == ScheduleStatus.Cancelled)
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
            else if (receiptModel.StayingScheduleId != null)
            {
                var scheduleExist = _context
                    .ServiceSchedule.Include(entity => entity.Id == receiptModel.ServiceScheduleId)
                    .FirstOrDefault();

                if (scheduleExist.Status == ScheduleStatus.OnGoing)
                {
                    return new Response
                    {
                        resultCd = 0,
                        MessageCode = "C517",
                        returnId = receipt.TotalReceiptId,
                    };
                }
                else if (scheduleExist.Status == ScheduleStatus.Completed)
                {
                    return new Response
                    {
                        resultCd = 0,
                        MessageCode = "C518",
                        returnId = receipt.TotalReceiptId,
                    };
                }
                else if (scheduleExist.Status == ScheduleStatus.Cancelled)
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
            receipt.ServiceScheduleId = receiptModel.ServiceScheduleId;
            receipt.UpdateDate = DateTime.UtcNow;

            if (receiptModel.Status == FullReceiptStatus.Completed)
                receipt.CompleteDate = DateTime.UtcNow;
        }

        return new Response
        {
            resultCd = 0,
            MessageCode = "I512",
            // Update type success
        };
    }

    public Response getUnpaidTourClient()
    {
        var receiptList = _context
            .FullReceiptList.Include(entity => entity.TotalReceipt)
            .Include(entity => entity.TotalReceipt)
            .Include(entity => entity.TourishSchedule)
            .ThenInclude(entity => entity.TourishPlan)
            .Where(entity => (int)entity.Status < 2)
            .GroupBy(entity => entity.TotalReceipt.TourishPlan.TourName)
            .Select(group => new
            {
                Name = group.Key,
                GuestList = group.Select(entity => new
                {
                    GuestName = entity.GuestName,
                    OriginalPrice = entity.OriginalPrice,
                    TotalTicket = entity.TotalTicket,
                    TotalChildTicket = entity.TotalChildTicket,
                    DiscountFloat = entity.DiscountFloat,
                    DiscountAmount = entity.DiscountAmount
                }).ToList()
            })
            .ToList();

        if (receiptList == null)
        {
            return new Response { resultCd = 1, Data = new List<FullReceipt>() };
        }

        return new Response { resultCd = 0, Data = receiptList };
    }

    public Response getUnpaidMovingScheduleClient()
    {
        var receiptList = _context
            .FullScheduleReceiptList.Include(entity => entity.TotalReceipt)
            .Include(entity => entity.TotalReceipt)
            .ThenInclude(entity => entity.MovingSchedule)
            .Include(entity => entity.ServiceSchedule)           
            .Where(entity => (int)entity.Status < 2 && entity.TotalReceipt.MovingScheduleId.HasValue)
            .GroupBy(entity => entity.TotalReceipt.MovingSchedule.Name)
            .Select(group => new
            {
                Name = group.Key,
                GuestList = group.Select(entity => new
                {
                    GuestName = entity.GuestName,
                    OriginalPrice = entity.OriginalPrice,
                    TotalTicket = entity.TotalTicket,
                    TotalChildTicket = entity.TotalChildTicket,
                    DiscountFloat = entity.DiscountFloat,
                    DiscountAmount = entity.DiscountAmount

                }).ToList()
            })
            .ToList();

        if (receiptList == null)
        {
            return new Response { resultCd = 1, Data = new List<FullReceipt>() };
        }

        return new Response { resultCd = 0, Data = receiptList };
    }

    public Response getUnpaidStayingScheduleClient()
    {
        var receiptList = _context
            .FullScheduleReceiptList
            .Include(entity => entity.TotalReceipt)
            .ThenInclude(entity => entity.StayingSchedule)
            .Include(entity => entity.ServiceSchedule)          
            .Where(entity => (int)entity.Status < 2 && entity.TotalReceipt.StayingScheduleId.HasValue)
            .GroupBy(entity => entity.TotalReceipt.StayingSchedule.Name)
            .Select(group => new
            {
                Name = group.Key,
                GuestList = group.Select(entity => new
                {
                    GuestName = entity.GuestName,
                    OriginalPrice = entity.OriginalPrice,
                    TotalTicket = entity.TotalTicket,
                    TotalChildTicket = entity.TotalChildTicket,
                    DiscountFloat = entity.DiscountFloat,
                    DiscountAmount = entity.DiscountAmount

                }).ToList()
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
            .GroupBy(entity => entity.TotalReceipt.TourishPlan.TourName)
            .Select(group => new
            {
                name = group.Key,
                gross = group.Sum(entity =>
                    (
                        entity.OriginalPrice * entity.TotalTicket
                        + entity.OriginalPrice * entity.TotalChildTicket
                    ) * (1 - entity.DiscountFloat)
                    - entity.DiscountAmount
                )
            })
            .OrderByDescending(group => group.gross)
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
            .GroupBy(entity => entity.TotalReceipt.TourishPlan.TourName)
            .Select(group => new
            {
                name = group.Key,
                totalTicket = group.Sum(entity => entity.TotalTicket + entity.TotalChildTicket)
            })
            .OrderByDescending(group => group.totalTicket)
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
            .FullScheduleReceiptList.Include(entity => entity.TotalReceipt)
            .ThenInclude(entity => entity.MovingSchedule)
            .Include(entity => entity.TotalReceipt)
            .Where(entity => (int)entity.Status < 3)
            .Where(entity => entity.TotalReceipt.MovingScheduleId != null)
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
            .GroupBy(entity => entity.TotalReceipt.MovingSchedule.Name)
            .Select(group => new
            {
                name = group.Key,
                gross = group.Sum(entity =>
                    (
                        entity.OriginalPrice * entity.TotalTicket
                        + entity.OriginalPrice * entity.TotalChildTicket
                    ) * (1 - entity.DiscountFloat)
                    - entity.DiscountAmount
                )
            })
            .OrderByDescending(group => group.gross)
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
            .FullScheduleReceiptList.Include(entity => entity.TotalReceipt)
            .Include(entity => entity.TotalReceipt)
            .ThenInclude(entity => entity.StayingSchedule)
            .Where(entity => (int)entity.Status < 3)
            .Where(entity => entity.TotalReceipt.StayingScheduleId != null)
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
            .GroupBy(entity => entity.TotalReceipt.StayingSchedule.Name)
            .Select(group => new
            {
                name = group.Key,
                gross = group.Sum(entity =>
                    (
                        entity.OriginalPrice * entity.TotalTicket
                        + entity.OriginalPrice * entity.TotalChildTicket
                    ) * (1 - entity.DiscountFloat)
                    - entity.DiscountAmount
                )
            })
            .OrderByDescending(group => group.gross)
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
            .FullScheduleReceiptList.Include(entity => entity.TotalReceipt)
            .Include(entity => entity.TotalReceipt)
            .ThenInclude(entity => entity.MovingSchedule)
            .Where(entity => (int)entity.Status < 3)
            .Where(entity => entity.TotalReceipt.MovingScheduleId.HasValue)
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
            .GroupBy(entity => entity.TotalReceipt.MovingSchedule.Name)
            .Select(group => new
            {
                name = group.Key,
                totalTicket = group.Sum(entity => entity.TotalTicket + entity.TotalChildTicket)
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
            .FullScheduleReceiptList.Include(entity => entity.TotalReceipt)
            .Include(entity => entity.TotalReceipt)
            .ThenInclude(entity => entity.StayingSchedule)
            .Where(entity => (int)entity.Status < 3)
            .Where(entity => entity.TotalReceipt.StayingScheduleId.HasValue)
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
            .GroupBy(entity => entity.TotalReceipt.StayingSchedule.Name)
            .Select(group => new
            {
                name = group.Key,
                totalTicket = group.Sum(entity => entity.TotalTicket + entity.TotalChildTicket)
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
            .FullScheduleReceiptList.Include(entity => entity.TotalReceipt)
            .Include(entity => entity.TotalReceipt)
            .Where(entity => (int)entity.Status < 3)
            .Where(entity => entity.TotalReceipt.StayingScheduleId != null)
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
                        + entity.OriginalPrice * entity.TotalChildTicket
                    ) * (1 - entity.DiscountFloat)
                    - entity.DiscountAmount
                )
            })
            .ToList();

        if (receiptGrossByMonth == null)
        {
            return new Response { resultCd = 1, Data = new List<FullScheduleReceipt>() };
        }

        return new Response { resultCd = 0, Data = receiptGrossByMonth };
    }

    public Response getGrossMovingScheduleInYear()
    {
        var currentDate = DateTime.UtcNow;
        var startOfYear = new DateTime(currentDate.Year, 1, 1);

        var receiptGrossByMonth = _context
            .FullScheduleReceiptList.Include(entity => entity.TotalReceipt)
            .Include(entity => entity.TotalReceipt)
            .Where(entity => (int)entity.Status < 3)
            .Where(entity => entity.TotalReceipt.MovingScheduleId != null)
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
                        + entity.OriginalPrice * entity.TotalChildTicket
                    ) * (1 - entity.DiscountFloat)
                    - entity.DiscountAmount
                )
            })
            .ToList();

        if (receiptGrossByMonth == null)
        {
            return new Response { resultCd = 1, Data = new List<FullScheduleReceipt>() };
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
                        + entity.OriginalPrice * entity.TotalChildTicket
                    ) * (1 - entity.DiscountFloat)
                    - entity.DiscountAmount
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
