using DataAccessLibrary.Databases;
using DataAccessLibrary.Data;
using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
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

			builder.Services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
				{
					ValidateIssuer=true,
					ValidateAudience=true,
					ValidateLifetime=true,
					ValidateIssuerSigningKey=true,
					ValidIssuer = builder.Configuration["Jwt:Issuer"],
					ValidAudience = builder.Configuration["Jwt:Audience"],
					IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
				};
			});

			builder.Services.AddScoped<ISqlDataAccessAsync, SqlDataAccessAsync>();
			builder.Services.AddScoped<IDatabaseDataAsync, SqlDataAsync>();

			var app = builder.Build();

			app.UseAuthentication();
			
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
