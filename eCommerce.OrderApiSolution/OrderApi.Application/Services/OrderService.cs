using OrderApi.Application.DTOs;
using OrderApi.Application.Mappers;
using OrderApi.Application.Repository;
using Polly.Registry;
using System.Net.Http.Json;

namespace OrderApi.Application.Services
{
    /// <summary>
    /// Service is created to communicate to other API's
    /// </summary>
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly HttpClient _httpClient;
        private readonly ResiliencePipelineProvider<string> _resiliencePipelineProvider;

        public OrderService(IOrderRepository orderRepository, HttpClient httpClient, ResiliencePipelineProvider<string> resiliencePipelineProvider)
        {
            _orderRepository = orderRepository;
            _httpClient = httpClient;
            _resiliencePipelineProvider = resiliencePipelineProvider; // In order to support the retry policy
        }

        public async Task<ProductDTO> GetProduct(int productId)
        {
            // Call the products api here using http client. 
            // Redirect this call to the Api Gateway since product api will not respond to the outsiders
            // because of ListenToApiGatewayOnly middleware. 
            var getProduct = await _httpClient.GetAsync($"api/products/{productId}");
            if (!getProduct.IsSuccessStatusCode)
                return null;

            var product = await getProduct.Content.ReadFromJsonAsync<ProductDTO>();
            return product;
        }

        public async Task<AppUserDTO> GetUser(int userId)
        {
            // Call the products api here using http client. 
            // Redirect this call to the Api Gateway since product api will not respond to the outsiders
            // because of ListenToApiGatewayOnly middleware. 
            var getUser = await _httpClient.GetAsync($"api/products/{userId}");
            if (!getUser.IsSuccessStatusCode)
                return null;

            var user = await getUser.Content.ReadFromJsonAsync<AppUserDTO>();
            return user;
        }
        public async Task<OrderDetailsDTO> GetOrderDetails(int orderId)
        {
            var order = await _orderRepository.FindByIdAsync(orderId);
            if (order is null || order.Id <= 0)
                return null;

            // Get Retry Pipeline
            var retryPipeline = _resiliencePipelineProvider.GetPipeline("my-retry-pipeline");
            var productDTO = await retryPipeline.ExecuteAsync(async token => await GetProduct(order.ProductId));
            var appUserDTO = await retryPipeline.ExecuteAsync(async token => await GetUser(order.ClientId));

            // Populate OrderDetails
            return new OrderDetailsDTO(order.Id, productDTO.Id, appUserDTO.Id, appUserDTO.Name, appUserDTO.Email, appUserDTO.Address,
                appUserDTO.TelephoneNumber, productDTO.Name, order.PurchaseQuantity, productDTO.Price,
                productDTO.Quantity * productDTO.Price, order.OrderedDate
            );
        }

        public async Task<IEnumerable<OrderDTO>> GetOrdersByClientId(int clientId)
        {
            var orders = await _orderRepository.GetOrdersAsync(x => x.ClientId == clientId);
            if (!orders.Any())
                return null;

            var (_, ordersDto) = OrderMapper.FromEntity(null, orders);
            return ordersDto;
        }
    }
}
