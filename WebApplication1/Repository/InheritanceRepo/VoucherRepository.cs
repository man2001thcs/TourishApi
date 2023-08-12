using WebApplication1.Data;
using WebApplication1.Data.DbContextFile;
using WebApplication1.Model;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface;

namespace WebApplication1.Repository.InheritanceRepo
{
    public class VoucherRepository : IVoucherRepository
    {
        private readonly MyDbContext _context;
        public static int PAGE_SIZE { get; set; } = 5;
        public VoucherRepository(MyDbContext _context)
        {
            this._context = _context;
        }

        public Response Add(VoucherModel voucherModel)
        {

            var voucher = new Voucher
            {
                Name = voucherModel.Name,
                DiscountFloat = voucherModel.DiscountFloat,
                DiscountAmount = voucherModel.DiscountAmount,
                Description = voucherModel.Description,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
            };
            _context.Add(voucher);
            _context.SaveChanges();

            return new Response
            {
                resultCd = 0,
                MessageCode = "I301",
                // Create type success               
            };

        }

        public Response Delete(Guid id)
        {
            var voucher = _context.Categories.FirstOrDefault((voucher
               => voucher.Id == id));
            if (voucher != null)
            {
                _context.Remove(voucher);
                _context.SaveChanges();
            }

            return new Response
            {
                resultCd = 0,
                MessageCode = "I303",
                // Delete type success               
            };
        }

        public Response GetAll(string? search, string? sortBy, int page = 1, int pageSize = 5)
        {
            var voucherQuery = _context.Vouchers.AsQueryable();

            #region Filtering
            if (!string.IsNullOrEmpty(search))
            {
                voucherQuery = voucherQuery.Where(voucher => voucher.Name.Contains(search));
            }
            #endregion

            #region Sorting
            //Default sort by Name (TenHh)
            voucherQuery = voucherQuery.OrderBy(voucher => voucher.Name);

            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy)
                {
                    case "name_desc":
                        voucherQuery = voucherQuery.OrderByDescending(voucher => voucher.Name);
                        break;
                    case "float_asc":
                        voucherQuery = voucherQuery.OrderBy(voucher => voucher.DiscountFloat);
                        break;
                    case "float_desc":
                        voucherQuery = voucherQuery.OrderByDescending(voucher => voucher.DiscountFloat);
                        break;
                    case "amount_asc":
                        voucherQuery = voucherQuery.OrderBy(voucher => voucher.DiscountAmount);
                        break;
                    case "amount_desc":
                        voucherQuery = voucherQuery.OrderByDescending(voucher => voucher.DiscountAmount);
                        break;
                    case "updateDate_asc":
                        voucherQuery = voucherQuery.OrderBy(voucher => voucher.UpdateDate);
                        break;
                    case "updateDate_desc":
                        voucherQuery = voucherQuery.OrderByDescending(voucher => voucher.UpdateDate);
                        break;
                }
            }
            #endregion

            #region Paging
            var result = PaginatorModel<Voucher>.Create(voucherQuery, page, pageSize);
            #endregion

            var voucherVM = new Response
            {
                resultCd = 0,
                Data = result.ToList(),
                count = result.TotalCount,
            };
            return voucherVM;

        }

        public Response getById(Guid id)
        {
            var voucher = _context.Vouchers.FirstOrDefault((voucher
                => voucher.Id == id));

            return new Response
            {
                resultCd = 0,
                Data = voucher
            };
        }

        public Response getByName(String name)
        {
            var voucher = _context.Vouchers.FirstOrDefault((voucher
                => voucher.Name == name));

            return new Response
            {
                resultCd = 0,
                Data = voucher
            };
        }

        public Response Update(VoucherModel voucherModel)
        {
            var voucher = _context.Vouchers.FirstOrDefault((voucher
                => voucher.Id == voucherModel.Id));
            if (voucher != null)
            {
                voucher.UpdateDate = DateTime.UtcNow;
                voucher.Name = voucherModel.Name;
                voucher.DiscountFloat = voucherModel.DiscountFloat;
                voucher.DiscountAmount = voucherModel.DiscountAmount;
                voucher.Description = voucherModel.Description;
                _context.SaveChanges();
            }

            return new Response
            {
                resultCd = 0,
                MessageCode = "I302",
                // Update type success               
            };
        }
    }
}
