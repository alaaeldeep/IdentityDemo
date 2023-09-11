using Microsoft.AspNetCore.Identity;

namespace Identity.Data.Models
{
	public class Employee :IdentityUser
	{
        public string depatment { get; set; } = string.Empty;
    }
}
