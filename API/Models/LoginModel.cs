using System.Text.Json.Serialization;

namespace API.Models
{
	public class LoginModel
	{
		[JsonPropertyName("useremail")]
		public string UserEmail { get; set; }

		[JsonPropertyName("password")]
		public string Password { get; set; }

		[JsonPropertyName("expiry")]
		public uint Expiry { get; set; }
	}
}