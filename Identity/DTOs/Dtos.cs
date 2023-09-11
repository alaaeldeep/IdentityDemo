namespace Identity.DTOs
{
	public class Dtos
	{
		public record LoginDto (string userName, string password);
		public record TokenDto (string token);
		public record RegisterDto (string UserName, string Password ,string Email, string Deparment);
	}
}
