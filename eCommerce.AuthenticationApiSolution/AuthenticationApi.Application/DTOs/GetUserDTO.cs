namespace AuthenticationApi.Application.DTOs
{
    public record GetUserDTO(
        int Id,
        string Name,
        string TelephoneNumber,
        string Address,
        string Email,
        string Role
    );
}
