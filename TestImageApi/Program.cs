
using TestImageApi.DB;
using TestImageApi.Services;

namespace TestImageApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // add entity framework core - ImageDB
            builder.Services.AddDbContext<ImageDB>();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSingleton<HostUrlService>();
            builder.Services.AddScoped<IPDBService>();
            builder.Services.AddSingleton<ClientIPService>();
            builder.Services.AddScoped<IPSimpleService>();
            builder.Services.AddScoped<WebsiteUrlService>();
            builder.Services.AddSingleton<S3Service>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
