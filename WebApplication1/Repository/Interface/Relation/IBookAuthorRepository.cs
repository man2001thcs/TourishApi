using WebApplication1.Model.VirtualModel;

namespace WebApplication1.Repository.Interface.Relation
{
    public interface IBookAuthorRepository
    {
        Response Add(Guid BookId, Guid RelationId);

        Response FindById(Guid BookId, Guid RelationId);
        Response Delete(Guid BookId);
    }
}
