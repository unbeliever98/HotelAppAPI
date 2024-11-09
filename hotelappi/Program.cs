using DataAccessLibrary.Databases;
using DataAccessLibrary.Data;
using DataAccessLibrary.Models;
namespace hotelappi
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddHttpClient();
			builder.Services.AddControllers();
			builder.Services.AddCors(options =>
			{
				options.AddPolicy("AllowSpecificOrigins",
					policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
			});

			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			builder.Services.AddScoped<ISqlDataAccessAsync, SqlDataAccessAsync>();
			builder.Services.AddScoped<IDatabaseDataAsync, SqlDataAsync>();

			var app = builder.Build();
			
			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();

			app.UseCors("AllowSpecificOrigins");

			app.UseAuthorization();


			app.MapControllers();

			app.Run();
		}
	}
}
