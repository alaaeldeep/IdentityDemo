
using Identity.Data.Context;
using Identity.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace Identity
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			#region default
			builder.Services.AddControllers();
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();
			#endregion
			// configure userManger Idendity
			builder.Services.AddIdentity<Employee, IdentityRole>(opt =>
			{
				opt.Password.RequireUppercase = true;
				opt.Password.RequiredLength = 8;
				opt.User.RequireUniqueEmail = true;


			})
				.AddEntityFrameworkStores<CompanyContext>();




			// 8- we need to tell asp.net we are useing JWT token,  and where the secret key to validate it 
			builder.Services.AddAuthentication(opt =>
			{
				// specify which schema should use in authntication
				opt.DefaultAuthenticateScheme = "bearerSchema";
				// specify which schema if not auth like redirect in mvc or return 401 || 403 
				opt.DefaultChallengeScheme = "bearerSchema";

				// add new schema with name = bearerSchema
			}).AddJwtBearer("bearerSchema", opt =>
			{
				// determine the secret key should use to validate the token
				var SecretKeyString = builder.Configuration.GetValue<string>("SecretKey");
				var SecretKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKeyString));

				// validate 
				opt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
				{
					IssuerSigningKey = SecretKey,
					ValidateAudience = false,
					ValidateIssuer = false,
				};
			});

			// authorization
			builder.Services.AddAuthorization(opt => opt.AddPolicy("admin", policy =>
			{
				policy.RequireClaim(ClaimTypes.Role, "Admin");
			}
				));
			// 
			builder.Services.AddDbContext<CompanyContext>(opt =>
			{
				opt.UseSqlServer(builder.Configuration.GetConnectionString("default"));
			});



			var app = builder.Build();


			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();

			app.UseAuthentication();
			app.UseAuthorization();


			app.MapControllers();

			app.Run();
		}
	}
}