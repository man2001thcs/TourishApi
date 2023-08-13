using WebApplication1.Data.DbContextFile;
using WebApplication1.Data.RelationData;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface;
using WebApplication1.Repository.Interface.Relation;

namespace WebApplication1.Repository.Relation.InheritanceRepo
{
    public class BookCategoryRepository : IBookCategoryRepository
    {
        private readonly MyDbContext _context;
        public BookCategoryRepository(MyDbContext _context)
        {
            this._context = _context;
        }

        public Response Add(Guid BookId, Guid RelationId)
        {

            var bookRelation = new BookCategory
            {
                BookId = BookId,
                CategoryId = RelationId
            };
            _context.Add(bookRelation);
            _context.SaveChanges();

            return new Response
            {
                resultCd = 0,
                MessageCode = "I1201",
                // Create type success               
            };
        }

        public Response FindById(Guid BookId, Guid RelationId)
        {
            var bookRelation = _context.BookCategoryList.FirstOrDefault((bookRelation
              => bookRelation.BookId == BookId && bookRelation.CategoryId == RelationId));

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
            var bookRelation = _context.BookCategoryList.Where((bookRelation
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
                MessageCode = "I1202",
                // Delete type success               
            };
        }
    }
}
