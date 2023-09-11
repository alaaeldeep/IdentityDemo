
using Identity.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static Identity.DTOs.Dtos;

namespace Identity.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserController : ControllerBase
	{
		private readonly IConfiguration _configuration;
		private readonly UserManager<Employee> _userManager;

		public UserController(IConfiguration configuration, UserManager<Employee> userManager)
        {
			_configuration= configuration;
			_userManager = userManager;
		}

		[HttpPost("test")]
		public ActionResult<TokenDto> LoggingTest(LoginDto credentials )
		{
			//1- check user credinatials => return 401 if not correct
			//if( credentials == null || credentials.password!="abc"|| credentials.userName!="abc")
			//{
			//	return Unauthorized();
			//}

			//2- generate Claims
			var claimsList = new List<Claim>
			{
				new Claim("key","value"),
				new Claim("key2","value2")
			};

			//3- get secret key from appSetting.json by reding it from configration
			var SecretKeyString = _configuration.GetValue<string>("SecretKey");

			// ==> 4, 5 from [ Microsoft.IdentityModel.Tokens] helping in hashing ;
			//4- generate secret key 
			var SecretKey = new SymmetricSecurityKey( Encoding.ASCII.GetBytes(SecretKeyString));
		
            // 5- create a combination of a secretKey, Algorithm
            var siningCredinatils = new SigningCredentials(SecretKey, SecurityAlgorithms.HmacSha256Signature);

			// ==> 6,7  from [ System.IdentityModel.Tokens.Jwt ] helping in combining (hsiningCredinatils + SecretKey) 
			// 6- compine all together and generate the token  as a Object 
			var token = new JwtSecurityToken(
				claims: claimsList,
				expires:DateTime.Now.AddDays(5),
				signingCredentials: siningCredinatils
				);

			// 7- token handler convert token object to string
			var tokenHandler = new JwtSecurityTokenHandler();
			var tokenString = tokenHandler.WriteToken(token);

			return new TokenDto(tokenString);
		}

		[HttpPost("Register")]
		public async Task<ActionResult> Register(RegisterDto registerDto)
		{
			var employee = new Employee()
			{
				UserName = registerDto.UserName,
				Email = registerDto.Email,
				depatment = registerDto.Email

			};
			var result= await	_userManager.CreateAsync(employee, registerDto.Password);
			if(!result.Succeeded) 
			{
				return BadRequest(result.Errors);
			}

			var claims = new List<Claim>
			{ 
				new  (ClaimTypes.NameIdentifier,employee.Id),
				new  (ClaimTypes.Role, "User"),
				new  (ClaimTypes.Name, employee.UserName)
			};

			
			await	_userManager.AddClaimsAsync(employee, claims);

			return Ok();
		}

		[HttpPost]
		public async Task<ActionResult<TokenDto>> Logging(LoginDto credentials)
		{
			//1 - check user credinatials => return 401 if not correct
			var user = await _userManager.FindByNameAsync(credentials.userName);

			if (user == null)
			{
				return Unauthorized(); 
			}

			var isAuthenticated = await _userManager.CheckPasswordAsync(user, credentials.password);
			if(!isAuthenticated)
			{
				return Unauthorized();
			}

			//2- get  Claims for this user from database
			var claimsList = await _userManager.GetClaimsAsync(user); 

			//3- get secret key from appSetting.json by reding it from configration
			var SecretKeyString = _configuration.GetValue<string>("SecretKey");

			// ==> 4, 5 from [ Microsoft.IdentityModel.Tokens] helping in hashing ;
			//4- generate secret key 
			var SecretKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKeyString));

			// 5- create a combination of a secretKey, Algorithm
			var siningCredinatils = new SigningCredentials(SecretKey, SecurityAlgorithms.HmacSha256Signature);

			// ==> 6,7  from [ System.IdentityModel.Tokens.Jwt ] helping in combining (hsiningCredinatils + SecretKey) 
			// 6- compine all together and generate the token  as a Object 
			var token = new JwtSecurityToken(
				claims: claimsList,
				expires: DateTime.Now.AddDays(5),
				signingCredentials: siningCredinatils
				);

			// 7- token handler convert token object to string
			var tokenHandler = new JwtSecurityTokenHandler();
			var tokenString = tokenHandler.WriteToken(token);

			return new TokenDto(tokenString);
		}

	} 
}
