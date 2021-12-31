using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace API.Models
{
	public class User
	{
		[JsonPropertyName("useremail")]
		public string UserEmail { get; set; }

		[JsonPropertyName("password")]
		public string Password { get; set; }


		[JsonPropertyName("type")]
		public string Type => "user";

	}
}