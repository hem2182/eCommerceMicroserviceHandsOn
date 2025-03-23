using eCommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.Mappers;
using ProductApi.Application.Repository;

namespace ProductApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetAllProducts()
        {
            var products = await _productRepository.GetAllAsync();
            if (!products.Any())
                return NotFound("No products available.");

            var (_, productsDto) = ProductMapper.FromEntity(null, products);
            return Ok(productsDto);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            var product = await _productRepository.FindByIdAsync(id);
            if (product == null)
                return NotFound("Product is not found");

            var (productDto, _) = ProductMapper.FromEntity(product, null);
            return Ok(productDto);
        }

        [HttpPost]
        public async Task<ActionResult<Response>> CreateProduct(ProductDTO productDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = ProductMapper.ToEntity(productDTO);
            var response = await _productRepository.CreateAsync(product);
            return response.Flag ? Ok(response) : BadRequest(response);
        }

        [HttpPut]
        public async Task<ActionResult<Response>> UpdateProduct(ProductDTO productDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = ProductMapper.ToEntity(productDTO);
            var response = await _productRepository.UpdateAsync(product);
            return response.Flag ? Ok(response) : BadRequest(response);
        }

        [HttpDelete]
        public async Task<ActionResult<Response>> DeleteProduct(ProductDTO productDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = ProductMapper.ToEntity(productDTO);
            var response = await _productRepository.DeleteAsync(product);
            return response.Flag ? Ok(response) : BadRequest(response);
        }
    }
}
