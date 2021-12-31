using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace API.Models
{

	public class Result
	{
		public Result(dynamic data, params string[] contexts)
		{
			Data = data;
			Context = contexts ?? new string[] { };
		}

		[JsonPropertyName("data")]
		public dynamic Data { get; }

		[JsonPropertyName("context")]
		public IEnumerable<string> Context { get; }
	}
}