using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using API.Helpers;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
	public interface IAuthTokenService
	{
		string CreateToken(string username);
		bool VerifyToken(string encodedToken, string username);
	}

	public class AuthTokenService : IAuthTokenService
	{
		private readonly AppSettings _appSettings;


		public AuthTokenService(IOptions<AppSettings> appSettings)
		{
			_appSettings = appSettings.Value;
		}

		public string CreateToken(string username)
		{
			Random rand = new Random();
			Byte[] b = new Byte[16];
			rand.NextBytes(b);
			////////////////////Getting error. Get random byte and give as a key.
			// var key = Encoding.UTF8.GetBytes(_appSettings.Secret);
			var tokenDescriptor = new SecurityTokenDescriptor

			{
				Subject = new ClaimsIdentity(new[]
				{
					new Claim("user", username)
				}),
				Expires = DateTime.UtcNow.AddDays(7),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(b), SecurityAlgorithms.HmacSha256Signature)
			};

			var tokenHandler = new JwtSecurityTokenHandler();
			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}

		public bool VerifyToken(string encodedToken, string username)
		{
			if (encodedToken.StartsWith("Bearer "))
			{
				encodedToken = encodedToken.Substring(7);
			}

			var tokenHandler = new JwtSecurityTokenHandler();
			var token = tokenHandler.ReadJwtToken(encodedToken);

			return token.Claims.Any(x => x.Type == "user" && x.Value == username);
		}
	}
}