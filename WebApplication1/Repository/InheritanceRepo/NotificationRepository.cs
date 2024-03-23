﻿using Microsoft.EntityFrameworkCore;
using SignalR.Hub;
using TourishApi.Repository.Interface;
using WebApplication1.Controllers.Notification;
using WebApplication1.Data;
using WebApplication1.Data.Connection;
using WebApplication1.Data.DbContextFile;
using WebApplication1.Model;
using WebApplication1.Model.VirtualModel;

namespace WebApplication1.Repository.InheritanceRepo
{
    public class NotificationRepository : IBaseRepository<NotificationModel>
    {
        private readonly MyDbContext _context;
        public static int PAGE_SIZE { get; set; } = 5;
        public NotificationRepository(MyDbContext _context)
        {
            this._context = _context;
        }

        public Response Add(NotificationModel addModel)
        {

            var addValue = new Notification
            {
                Content = addModel.Content,
                ContentCode = addModel.ContentCode,
                UserCreateId = addModel.UserCreateId,
                UserReceiveId = addModel.UserReceiveId,
                TourishPlanId = addModel.TourishPlanId,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
            };
            _context.Add(addValue);
            _context.SaveChanges();

            return new Response
            {
                resultCd = 0,
                MessageCode = "I701",
                // Create type success               
            };

        }

        public Response Delete(Guid id)
        {
            var deleteEntity = _context.Notifications.FirstOrDefault((entity
               => entity.Id == id));
            if (deleteEntity != null)
            {
                _context.Remove(deleteEntity);
                _context.SaveChanges();
            }

            return new Response
            {
                resultCd = 0,
                MessageCode = "I703",
                // Delete type success               
            };
        }

        public Response GetAll(string? search, string? sortBy, int page = 1, int pageSize = 5)
        {
            var entityQuery = _context.Notifications.AsQueryable();

            #region Filtering
            if (!string.IsNullOrEmpty(search))
            {
                entityQuery = entityQuery.Where(entity => entity.Content.Contains(search));
            }
            #endregion

            #region Sorting
            entityQuery = entityQuery.OrderByDescending(entity => entity.UpdateDate);

            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy)
                {
                    case "name_desc":
                        entityQuery = entityQuery.OrderByDescending(entity => entity.Content);
                        break;
                    case "updateDate_asc":
                        entityQuery = entityQuery.OrderBy(entity => entity.UpdateDate);
                        break;
                    case "updateDate_desc":
                        entityQuery = entityQuery.OrderByDescending(entity => entity.UpdateDate);
                        break;

                }
            }
            #endregion

            #region Paging
            var result = PaginatorModel<Notification>.Create(entityQuery, page, pageSize);
            #endregion

            var entityVM = new Response
            {
                resultCd = 0,
                Data = result.ToList(),
                count = result.TotalCount,
            };
            return entityVM;

        }

        public Response GetAllForReceiver(string? userId, string? sortBy, int page = 1, int pageSize = 5)
        {
            var entityQuery = _context.Notifications.Include(entity => entity.UserCreator)
                .Include(entity => entity.UserReceiver).Include(entity => entity.TourishPlan).AsQueryable();

            #region Filtering
            if (!string.IsNullOrEmpty(userId))
            {
                entityQuery = entityQuery.Where(entity => entity.UserReceiver.Id.ToString().Contains(userId));
            }
            #endregion

            #region Sorting
            entityQuery = entityQuery.OrderByDescending(entity => entity.UpdateDate);

            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy)
                {
                    case "name_desc":
                        entityQuery = entityQuery.OrderByDescending(entity => entity.Content);
                        break;
                    case "updateDate_asc":
                        entityQuery = entityQuery.OrderBy(entity => entity.UpdateDate);
                        break;
                    case "updateDate_desc":
                        entityQuery = entityQuery.OrderByDescending(entity => entity.UpdateDate);
                        break;
                }
            }
            #endregion

            #region Paging
            var result = PaginatorModel<Notification>.Create(entityQuery, page, pageSize);
            #endregion

            var entityVM = new Response
            {
                resultCd = 0,
                Data = result.ToList(),
                count = result.TotalCount,
            };
            return entityVM;

        }

        public Response GetAllForCreator(string? userId, string? sortBy, int page = 1, int pageSize = 5)
        {
            var entityQuery = _context.Notifications.Include(entity => entity.UserCreator)
                .Include(entity => entity.UserReceiver).Include(entity => entity.TourishPlan).
                AsQueryable();

            #region Filtering
            if (!string.IsNullOrEmpty(userId))
            {
                entityQuery = entityQuery.Where(entity => entity.UserCreator.Id.ToString().Contains(userId));
            }
            #endregion

            #region Sorting
            entityQuery = entityQuery.OrderByDescending(entity => entity.UpdateDate);

            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy)
                {
                    case "name_desc":
                        entityQuery = entityQuery.OrderByDescending(entity => entity.Content);
                        break;
                    case "updateDate_asc":
                        entityQuery = entityQuery.OrderBy(entity => entity.UpdateDate);
                        break;
                    case "updateDate_desc":
                        entityQuery = entityQuery.OrderByDescending(entity => entity.UpdateDate);
                        break;
                }
            }
            #endregion

            #region Paging
            var result = PaginatorModel<Notification>.Create(entityQuery, page, pageSize);
            #endregion

            var entityVM = new Response
            {
                resultCd = 0,
                Data = result.ToList(),
                count = result.TotalCount,
            };
            return entityVM;

        }

        public Response getById(Guid id)
        {
            var entity = _context.Notifications.Include(entity => entity.UserCreator)
                .Include(entity => entity.UserReceiver).Include(entity => entity.TourishPlan).FirstOrDefault((entity
                => entity.Id == id));

            return new Response
            {
                resultCd = 0,
                Data = entity
            };
        }

        public Response getByName(String name)
        {
            var entity = _context.Notifications.Include(entity => entity.UserCreator)
                .Include(entity => entity.UserReceiver).Include(entity => entity.TourishPlan).FirstOrDefault((entity
                => entity.Content == name));

            return new Response
            {
                resultCd = 0,
                Data = entity
            };
        }

        public NotificationCon getNotificationCon(Guid userReceiveId)
        {
            var connection = _context.NotificationConList.OrderByDescending(connection => connection.CreateDate).FirstOrDefault(u => u.UserId == userReceiveId && u.Connected);
            return connection;
        }

        public List<Notification> getByTourRecentUpdate(Guid tourId, Guid modifiedId)
        {
            var compareTime = DateTime.UtcNow.AddMinutes(-30);
            var entityList = _context.Notifications
                .Include(entity => entity.UserCreator)
                .Include(entity => entity.UserReceiver)
                .Include(entity => entity.TourishPlan)
                .Where(entity => entity.TourishPlan.Id == tourId && entity.UserCreator.Id == modifiedId
                && (entity.UpdateDate.Value > compareTime)).ToList();

            return entityList;
        }

        public Response Update(NotificationModel entityModel)
        {
            var entity = _context.Notifications.FirstOrDefault((entity
                => entity.Id == entityModel.Id));
            if (entity != null)
            {
                entity.UpdateDate = DateTime.UtcNow;
                entity.TourishPlanId = entityModel.TourishPlanId;
                entity.Content = entityModel.Content;
                entity.ContentCode = entityModel.ContentCode;
                _context.SaveChanges();
            }

            return new Response
            {
                resultCd = 0,
                MessageCode = "I702",
                // Update type success               
            };
        }
    }
}
