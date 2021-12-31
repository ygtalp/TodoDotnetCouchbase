using System;
using System.Threading.Tasks;
using API.Models;
using API.Services;
using Couchbase;
using Couchbase.Query;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers

{

	[ApiController]
	[Route("api/user")]
	public class UserController : ControllerBase
	{
		private readonly IUserService _userService;
		private readonly IAuthTokenService _authTokenService;
		private readonly ICouchbaseService _couchbaseService;
		public UserController(IUserService userService, IAuthTokenService authTokenService, ICouchbaseService couchbaseService)
		{
			_couchbaseService = couchbaseService;
			_userService = userService;
			_authTokenService = authTokenService;

		}

		[HttpPost("signup")]
		public async Task<ActionResult> SignUp(LoginModel model)
		{
			string useremail = model.UserEmail;

			// if (await _userService.UserExists(useremail))
			// {
			// 	return Conflict($"UserEmail '{useremail}' already exists");
			// }

			await _userService.CreateUser(useremail, model.Password, model.Expiry);

			var data = new
			{
				token = _authTokenService.CreateToken(useremail)
			};
			var context = new string[] {
				$"KV insert -users: document {useremail}"
			};
			return Created("", new Result(data, context));
		}

		[HttpPost("login")]
		public async Task<ActionResult> Login([FromBody] LoginModel model)
		{
			var queryResult = await _couchbaseService.Cluster.QueryAsync<dynamic>(
				"SELECT META().id FROM `TodoAppBucket` WHERE userEmail=$type",
				options => options.Parameter("type", model.UserEmail)
			);
			string queryString = "";
			await foreach (Object o in queryResult)
				queryString += o.ToString();
			string queryId = queryString.Substring(12, 36);

			Console.WriteLine(queryId);
			var result = await _couchbaseService.TodoCollection.GetAsync(queryId);
			var resultUser = result.ContentAs<ReturnModel>();
			if (resultUser == null)
			{
				return Unauthorized("Invalid username / password");
			}
			var data = new
			{
				token = _authTokenService.CreateToken(resultUser.UserEmail),
			};

			var context = new string[] {
				$"KV get - scoped to users: for password field in document {resultUser}"
			};
			return Ok(new Result(data, context));
		}

		[HttpGet("/usertodos")]
		public async Task<ActionResult> GetTodossForUser(string useremail)
		{
			var user = await _userService.GetUser(useremail);
			var email = user.UserEmail;

			var queryResult = await _couchbaseService.Cluster.QueryAsync<dynamic>(
				"SELECT * FROM `TodoAppBucket` WHERE userEmail=$1 and type=$2",
				options => options.Parameter(email).Parameter("todo")
			);
			//?????????????????????????????????????????????? cant get object?????????????????????????
			object s = null;
			string v = "";
			string res = "";
			string r ="";

			await foreach (object o in queryResult)
			{
				v = o.ToString();
				res = v.Replace("\r\n", string.Empty);
				r = res.Replace("\\", string.Empty);
				s += r;
			}
			///////////////////////////////////////////////////////////////////////////////////////////
			var context = new string[] {
				$"KV get - users: for  bookings in document {useremail}"
			};

			return Ok(new Result(s));
		}

	}
}