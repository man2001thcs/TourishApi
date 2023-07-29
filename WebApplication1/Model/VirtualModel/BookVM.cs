using WebApplication1.Data;

namespace WebApplication1.Model.VirtualModel
{
    public class BookVM : Response
    {
        public List<Book> Books { get; set; }
    }
}
