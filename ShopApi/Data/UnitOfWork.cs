using System.Threading.Tasks;
using ShopApi.Data.Interfaces;
using ShopApi.Data.Repositories;

namespace ShopApi.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private ICategoryRepository _categoryRepository;
        private IProductRepository _productRepository;
        private ICartItemRepository _cartItemRepository;

        public ICategoryRepository CategoryRepository =>
            _categoryRepository ?? (_categoryRepository = new CategoryRepository(_context));

        public IProductRepository ProductRepository =>
            _productRepository ?? (_productRepository = new ProductRepository(_context));

        public ICartItemRepository CartItemRepository =>
            _cartItemRepository ?? (_cartItemRepository = new CartItemRepository(_context));

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}