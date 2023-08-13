using WebApplication1.Data.DbContextFile;
using WebApplication1.Data.RelationData;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface;
using WebApplication1.Repository.Interface.Relation;

namespace WebApplication1.Repository.Relation.InheritanceRepo
{
    public class BookAuthorRepository : IBookAuthorRepository
    {
        private readonly MyDbContext _context;
        public BookAuthorRepository(MyDbContext _context)
        {
            this._context = _context;
        }

        public Response Add(Guid BookId, Guid RelationId)
        {

            var bookRelation = new BookAuthor
            {
                BookId = BookId,
                AuthorId = RelationId
            };
            _context.Add(bookRelation);
            _context.SaveChanges();

            return new Response
            {
                resultCd = 0,
                MessageCode = "I1101",
                // Create type success               
            };

        }

        public Response FindById(Guid BookId, Guid RelationId)
        {
            var bookRelation = _context.BookAuthorList.FirstOrDefault((bookRelation
              => bookRelation.BookId == BookId && bookRelation.AuthorId == RelationId));

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
            var bookRelation = _context.BookAuthorList.Where((bookRelation
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
                MessageCode = "I1102",
                // Delete type success               
            };
        }
    }
}
