using System.ComponentModel.DataAnnotations;

namespace OrderApi.Application.DTOs
{
    // We could have created in the shared library. 
    // It is same as that of Product Api Solution ProductDTO.
    public record ProductDTO(
        int Id,
        [Required] string Name,
        [Required][DataType(DataType.Currency)] decimal Price,
        [Required][Range(1, int.MaxValue)] int Quantity
    );
}
