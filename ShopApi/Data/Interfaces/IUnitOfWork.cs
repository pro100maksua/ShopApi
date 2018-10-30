using System.Threading.Tasks;

namespace ShopApi.Data.Interfaces
{
    public interface IUnitOfWork
    {
        ICategoryRepository CategoryRepository { get; }
        IProductRepository ProductRepository { get; }
        ICartItemRepository CartItemRepository { get; }

        Task SaveAsync();
    }
}