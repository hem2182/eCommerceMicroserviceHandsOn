using eCommerce.SharedLibrary.Logging;
using eCommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ProductApi.Application.Repository;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;
using System.Linq.Expressions;

namespace ProductApi.Infrastructure.Repository
{
    public class ProductRepository(ProductDbContext context) : IProductRepository
    {
        public async Task<Response> CreateAsync(Product entity)
        {
            try
            {
                var getProduct = await GetByAsync(x => x.Name.Equals(entity.Name));
                if (getProduct is not null && !string.IsNullOrEmpty(getProduct.Name))
                    return new Response(false, $"{entity.Name} Already added.");

                var currentEntity = context.Products.Add(entity).Entity;
                await context.SaveChangesAsync();

                if (currentEntity is not null && currentEntity.Id > 0)
                    return new Response(true, $"{entity.Name} added to the database successfully");
                else
                    return new Response(false, $"Error occurred while adding {entity.Name}");
            }
            catch (Exception ex)
            {
                // Log exceptions
                LogException.LogExceptions(ex);

                // display scare free response
                return new Response(false, "Error Occurred while adding new product");
            }
        }

        public async Task<Response> DeleteAsync(Product entity)
        {
            try
            {
                var product = await FindByIdAsync(entity.Id);
                if (product is null)
                    return new Response(false, $"{entity.Name} does not exists.");

                context.Entry(product).State = EntityState.Detached;
                context.Products.Remove(entity);
                await context.SaveChangesAsync();

                return new Response(true, $"{entity.Name} removed successfully.");
            }
            catch (Exception ex)
            {
                // Log exceptions
                LogException.LogExceptions(ex);

                // display scare free response
                return new Response(false, "Error Occurred while deleting new product");
            }
        }

        public async Task<Product> FindByIdAsync(int id)
        {
            try
            {
                var product = await context.Products.FindAsync(id);
                return product ?? null!;
            }
            catch (Exception ex)
            {
                // Log exceptions
                LogException.LogExceptions(ex);

                // display scare free response
                throw new Exception("Error Occurred while fetching product");
            }
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            try
            {
                return await context.Products.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                // Log exceptions
                LogException.LogExceptions(ex);

                // display scare free response
                throw new Exception("Error Occurred while fetching all product");
            }
        }

        public async Task<Product> GetByAsync(Expression<Func<Product, bool>> predicate)
        {
            try
            {
                return await context.Products.Where(predicate).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                // Log exceptions
                LogException.LogExceptions(ex);

                // display scare free response
                throw new InvalidOperationException("Error Occurred while fetching product");
            }
        }

        public async Task<Response> UpdateAsync(Product entity)
        {
            try
            {
                var product = await FindByIdAsync(entity.Id);
                if (product is null)
                    return new Response(false, $"{entity.Name} not found.");

                // Entity keeps tracking the recod from the database as soon as it get it.
                // FOr us to use the EF update key, we have to detatch it or untract it before we can update it else we cannot do that. 
                context.Entry(product).State = EntityState.Detached;
                context.Products.Update(entity);
                await context.SaveChangesAsync();

                return new Response(true, $"{entity.Name} updated Successfully");
            }
            catch (Exception ex)
            {
                // Log exceptions
                LogException.LogExceptions(ex);

                // display scare free response
                return new Response(false, "Error Occurred while updating new product");
            }
        }
    }
}
