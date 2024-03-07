﻿using WebApplication1.Model;
using WebApplication1.Model.VirtualModel;

namespace WebApplication1.Repository.Interface
{
    public interface ITourishPlanRepository
    {
        Response GetAll(string? search, string? sortBy, int page = 1, int pageSize = 5);
        Response getById(Guid id);
        Response getByName(String TourName);
        Task<Response> Add(TourishPlanInsertModel entityModel, String id);
        Task<Response> Update(TourishPlanUpdateModel entityModel, String id);
        Response Delete(Guid id);
    }
}
