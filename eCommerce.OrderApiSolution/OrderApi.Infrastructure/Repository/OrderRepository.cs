using eCommerce.SharedLibrary.Logging;
using eCommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using OrderApi.Application.Repository;
using OrderApi.Domain.Entities;
using OrderApi.Infrastructure.Data;
using System.Linq.Expressions;

namespace OrderApi.Infrastructure.Repository
{
    public class OrderRepository(OrderDbContext context) : IOrderRepository
    {
        public async Task<Response> CreateAsync(Order entity)
        {
            try
            {
                var order = context.Orders.Add(entity).Entity;
                await context.SaveChangesAsync();

                return order.Id > 0
                    ? new Response(true, "Order created successfully")
                    : new Response(false, "Order created failed");
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response(false, "Error Occurred while creating order.");
            }
        }

        public async Task<Response> DeleteAsync(Order entity)
        {
            try
            {
                var order = await FindByIdAsync(entity.Id);
                if (order is null)
                    return new Response(false, "Order is not found");

                context.Entry(order).State = EntityState.Detached;
                context.Remove(entity);
                await context.SaveChangesAsync();

                return new Response(true, "Order is deleted successfully");
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response(false, "Error Occurred while deleting order.");
            }
        }

        public async Task<Order> FindByIdAsync(int id)
        {
            try
            {
                return await context.Orders.FindAsync(id);
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new Exception("Error Occurred while creating order.");
            }
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            try
            {
                return await context.Orders.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new Exception("Error Occurred while creating order.");
            }
        }

        public async Task<Order> GetByAsync(Expression<Func<Order, bool>> predicate)
        {
            try
            {
                return await context.Orders.Where(predicate).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new Exception("Error Occurred while creating order.");
            }
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync(Expression<Func<Order, bool>> predicate)
        {
            try
            {
                return await context.Orders.Where(predicate).ToListAsync();
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new Exception("Error Occurred while creating order.");
            }
        }

        public async Task<Response> UpdateAsync(Order entity)
        {
            try
            {
                var order = await FindByIdAsync(entity.Id);
                if (order is null)
                    return new Response(false, "Order is not found");

                context.Entry(order).State = EntityState.Detached;
                context.Update(entity);
                await context.SaveChangesAsync();

                return new Response(true, "Order is updated successfully");
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response(false, "Error Occurred while updating order.");
            }
        }
    }
}
