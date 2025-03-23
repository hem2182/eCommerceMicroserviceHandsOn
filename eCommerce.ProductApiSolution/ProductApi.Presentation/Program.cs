using ProductApi.Infrastructure.ContainerExtensions;

internal class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddInfrastructureService(builder.Configuration);
        builder.Services.AddSwaggerGen();

        var app = builder.Build();
        app.UseInfrastructureService();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}