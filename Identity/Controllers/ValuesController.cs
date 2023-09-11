using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(Policy ="admin")]
	public class ValuesController : ControllerBase
	{

		[HttpGet]
		public IActionResult Get()
		{
			return Ok(new
			{
				name = "alaa"
			});
		}
	}
}
