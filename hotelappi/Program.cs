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
                var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY")
                ?? throw new ArgumentNullException("JWT_KEY", "JWT_KEY environment variable is missing.");

                var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER")
                                ?? throw new ArgumentNullException("JWT_ISSUER", "JWT_ISSUER environment variable is missing.");

                var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")
                                  ?? throw new ArgumentNullException("JWT_AUDIENCE", "JWT_AUDIENCE environment variable is missing.");

                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
				{
					ValidateIssuer=true,
					ValidateAudience=true,
					ValidateLifetime=true,
					ValidateIssuerSigningKey=true,
					ValidIssuer = jwtIssuer,
					ValidAudience = jwtAudience,
					IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
				};

				options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
				{
					OnAuthenticationFailed = context =>
					{
						context.Response.StatusCode = StatusCodes.Status401Unauthorized;
						context.Response.ContentType = "application/json";
						return context.Response.WriteAsync("{\"message\": \"Invalid token.\"}");
					}
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
