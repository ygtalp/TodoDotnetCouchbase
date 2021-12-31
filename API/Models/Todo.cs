using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API.Models
{
	public class Todo
	{
		[Required]
		[JsonPropertyName("UserEmail")]
		public string UserEmail { get; set; }
		
		[JsonPropertyName("TodoDate")]
		public DateTimeOffset TodoDate { get; set; } = DateTimeOffset.Now;
		
		
		[Required]
		[JsonPropertyName("Message")]
		public string Message { get; set; }


		[JsonPropertyName("IsCompleted")]
		public bool IsCompleted { get; set; }
		

		[JsonPropertyName("type")]
		public string Type => "todo";
	}
}