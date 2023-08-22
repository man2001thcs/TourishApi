using WebApplication1.Data;
using WebApplication1.Data.DbContextFile;
using WebApplication1.Model;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface;

namespace WebApplication1.Repository.InheritanceRepo
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly MyDbContext _context;
        public static int PAGE_SIZE { get; set; } = 5;
        public NotificationRepository(MyDbContext _context)
        {
            this._context = _context;
        }

        public Response Add(NotificationModel notificationModel)
        {

            var notification = new Notification
            {
                UserId = notificationModel.UserId,
                Content = notificationModel.Content,
                IsRead = notificationModel.IsRead,
                IsDeleted = notificationModel.IsDeleted,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
            };
            _context.Add(notification);
            _context.SaveChanges();

            return new Response
            {
                resultCd = 0,
                MessageCode = "I201",
                // Create type success               
            };

        }

        public Response Delete(Guid id)
        {
            var notification = _context.Categories.FirstOrDefault((notification
               => notification.Id == id));
            if (notification != null)
            {
                _context.Remove(notification);
                _context.SaveChanges();
            }

            return new Response
            {
                resultCd = 0,
                MessageCode = "I203",
                // Delete type success               
            };
        }

        public Response GetAll(string? search, string? sortBy, int page = 1, int pageSize = 5)
        {
            var notificationQuery = _context.Notifications.AsQueryable();

            #region Filtering
            if (!string.IsNullOrEmpty(search))
            {
                notificationQuery = notificationQuery.Where(notification => notification.Content.Contains(search));
            }
            #endregion

            #region Sorting
            //Default sort by Name (TenHh)
            notificationQuery = notificationQuery.OrderBy(notification => notification.UpdateDate);

            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy)
                {
                    case "name_desc":
                        notificationQuery = notificationQuery.OrderByDescending(notification => notification.Content);
                        break;
                    case "updateDate_asc":
                        notificationQuery = notificationQuery.OrderBy(notification => notification.UpdateDate);
                        break;
                    case "updateDate_desc":
                        notificationQuery = notificationQuery.OrderByDescending(notification => notification.UpdateDate);
                        break;
                }
            }
            #endregion

            #region Paging
            var result = PaginatorModel<Notification>.Create(notificationQuery, page, pageSize);
            #endregion

            var notificationVM = new Response
            {
                resultCd = 0,
                Data = result.ToList(),
                count = result.TotalCount,
            };
            return notificationVM;

        }

        public Response getById(Guid id)
        {
            var notification = _context.Categories.FirstOrDefault((notification
                => notification.Id == id));

            return new Response
            {
                resultCd = 0,
                Data = notification
            };
        }

        public Response getByName(String name)
        {
            var notification = _context.Categories.FirstOrDefault((notification
                => notification.Name == name));

            return new Response
            {
                resultCd = 0,
                Data = notification
            };
        }

        public Response Update(NotificationModel notificationModel)
        {
            var notification = _context.Notifications.FirstOrDefault((notification
                => notification.Id == notificationModel.Id));
            if (notification != null)
            {
                notification.UserId = notificationModel.UserId;
                notification.Content = notificationModel.Content;
                notification.IsRead = notificationModel.IsRead;
                notification.IsDeleted = notificationModel.IsDeleted;
                _context.SaveChanges();
            }

            return new Response
            {
                resultCd = 0,
                MessageCode = "I202",
                // Update type success               
            };
        }
    }
}
