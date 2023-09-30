using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Data.DbContextFile;
using WebApplication1.Model;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface;

namespace WebApplication1.Repository.InheritanceRepo
{
    public class MessageRepository : IMessageRepository
    {
        private readonly MyDbContext _context;
        public static int PAGE_SIZE { get; set; } = 5;
        public MessageRepository(MyDbContext _context)
        {
            this._context = _context;
        }

        public Response Add(MessageModel messageModel)
        {

            var message = new Message
            {
                UserSentId = messageModel.UserSentId,
                UserReceiveId = messageModel.UserReceiveId,
                Content = messageModel.Content,
                IsRead = messageModel.IsRead,
                IsDeleted = messageModel.IsDeleted,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
            };
            _context.Add(message);
            _context.SaveChanges();

            return new Response
            {
                resultCd = 0,
                MessageCode = "I601",
                // Create type success               
            };

        }

        public Response Delete(Guid id)
        {
            var message = _context.Messages.FirstOrDefault((message
               => message.Id == id));
            if (message != null)
            {
                _context.Remove(message);
                _context.SaveChanges();
            }

            return new Response
            {
                resultCd = 0,
                MessageCode = "I603",
                // Delete type success               
            };
        }

        public Response GetAll(string? search, Guid UserId, string? sortBy, int page = 1, int pageSize = 5)
        {
            var messageQuery = _context.Messages.AsQueryable();

            #region Filtering
            if (!string.IsNullOrEmpty(search))
            {
                messageQuery = messageQuery.Where(message => message.Content.Contains(search));
            }

            messageQuery = messageQuery.Where(message => message.UserReceiveId == UserId || message.UserSentId == UserId);
            #endregion

            #region Sorting
            //Default sort by Name (TenHh)
            messageQuery = messageQuery.OrderBy(message => message.UpdateDate);

            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy)
                {
                    case "name_desc":
                        messageQuery = messageQuery.OrderByDescending(message => message.Content);
                        break;
                    case "updateDate_asc":
                        messageQuery = messageQuery.OrderBy(message => message.UpdateDate);
                        break;
                    case "updateDate_desc":
                        messageQuery = messageQuery.OrderByDescending(message => message.UpdateDate);
                        break;
                }
            }
            #endregion

            var messageReturn = messageQuery.Include(message => message.UserSent.FullName)
                .Include(message => message.UserReceive.FullName)
                .Select(x =>
                 new MessageReturnModel
                 {
                     UserSentId = x.UserSentId,
                     UserReceiveId = x.UserReceiveId,
                     IsDeleted = x.IsDeleted,
                     IsRead = x.IsRead,
                     Content = x.Content,
                     CreateDate = x.CreateDate,
                     UserSentName = x.UserSent.FullName,
                     UserReceiveName = x.UserReceive.FullName,
                 }
              );

            #region Paging
            var result = PaginatorModel<MessageReturnModel>.Create(messageReturn, page, pageSize);
            #endregion

            var messageVM = new Response
            {
                resultCd = 0,
                Data = result.ToList(),
                count = result.TotalCount,
            };
            return messageVM;

        }

        public Response getById(Guid id)
        {
            var message = _context.Categories.FirstOrDefault((message
                => message.Id == id));

            return new Response
            {
                resultCd = 0,
                Data = message
            };
        }

        public Response getByName(String name)
        {
            var message = _context.Categories.FirstOrDefault((message
                => message.Name == name));

            return new Response
            {
                resultCd = 0,
                Data = message
            };
        }

        public Response Update(MessageModel messageModel)
        {
            var message = _context.Messages.FirstOrDefault((message
                => message.Id == messageModel.Id));
            if (message != null)
            {
                message.UserSentId = messageModel.UserSentId;
                message.Content = messageModel.Content;
                message.UpdateDate = DateTime.UtcNow;
                message.IsRead = true;
                _context.SaveChanges();
            }

            return new Response
            {
                resultCd = 0,
                MessageCode = "I602",
                // Update type success               
            };
        }
    }
}
