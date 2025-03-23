using ProductApi.Application.DTOs;
using ProductApi.Domain.Entities;

namespace ProductApi.Application.Mappers
{
    public static class ProductMapper
    {
        // We could have used AutoMapper here. 
        public static Product ToEntity(ProductDTO productDTO)
        {
            return new Product
            {
                Id = productDTO.Id,
                Name = productDTO.Name,
                Price = productDTO.Price,
                Quantity = productDTO.Quantity
            };
        }

        public static (ProductDTO?, IEnumerable<ProductDTO>?) FromEntity(Product product, IEnumerable<Product>? products)
        {
            // return single
            if (product is not null && products == null)
            {
                var singleProductDto = new ProductDTO(product.Id, product.Name, product.Price, product.Quantity);
                return (singleProductDto, null);
            }

            if (product == null && products is not null)
            {
                var productsDto = products.Select(p => new ProductDTO(p.Id, p.Name, p.Price, p.Quantity)).ToList();
                return (null, productsDto);
            }

            return (null, null);
        }
    }
}
