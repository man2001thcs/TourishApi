using WebApplication1.Data.DbContextFile;
using WebApplication1.Data.RelationData;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface;
using WebApplication1.Repository.Interface.Relation;

namespace WebApplication1.Repository.Relation.InheritanceRepo
{
    public class BookPublisherRepository : IBookPublisherRepository
    {
        private readonly MyDbContext _context;
        public BookPublisherRepository(MyDbContext _context)
        {
            this._context = _context;
        }

        public Response Add(Guid BookId, Guid RelationId)
        {

            var bookRelation = new BookPublisher
            {
                BookId = BookId,
                PublisherId = RelationId
            };
            _context.Add(bookRelation);
            _context.SaveChanges();

            return new Response
            {
                resultCd = 0,
                MessageCode = "I1301",
                // Create type success               
            };

        }

        public Response FindById(Guid BookId, Guid RelationId)
        {
            var bookRelation = _context.BookPublisherList.FirstOrDefault((bookRelation
              => bookRelation.BookId == BookId && bookRelation.PublisherId == RelationId));

            if (bookRelation != null)
            {
                return new Response
                {
                    resultCd = 0,
                    Data = bookRelation
                };
            };

            return new Response
            {
                resultCd = 1,
                Data = null
            };
        }

        public Response Delete(Guid BookId)
        {
            var bookRelation = _context.BookPublisherList.Where((bookRelation
             => bookRelation.BookId == BookId)).ToList();

            foreach (var Relation in bookRelation)
            {
                if (Relation != null)
                {
                    _context.Remove(Relation);
                    _context.SaveChanges();
                }
            }

            return new Response
            {
                resultCd = 0,
                MessageCode = "I1302",
                // Delete type success               
            };
        }
    }
}
