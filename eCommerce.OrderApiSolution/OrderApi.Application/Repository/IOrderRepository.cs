using eCommerce.SharedLibrary.Interface;
using OrderApi.Domain.Entities;
using System.Linq.Expressions;

namespace OrderApi.Application.Repository
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<IEnumerable<Order>> GetOrdersAsync(Expression<Func<Order, bool>> predicate);
    }
}
