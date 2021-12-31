using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Models;
using Couchbase;

namespace API.Services
{
	public interface IUserService
	{
		Task<bool> UserExists(string username);
		Task<User> CreateUser(string username, string password, uint expiry);
		Task<User> GetUser(string username);
		Task<IEnumerable<Todo>> GetUserTodos(string search);
		Task<User> GetAndAuthenticateUser(string username, string password);
		Task UpdateUser(User user);
	}
	public class UserService : IUserService
	{
		private readonly ICouchbaseService _couchbaseService;
		public UserService(ICouchbaseService couchbaseService)
		{
			_couchbaseService = couchbaseService;
		}

		public async Task<User> CreateUser(string useremail, string password, uint expiry)
		{
			var user = new User
			{
				UserEmail = useremail,
				Password = CalculateMd5Hash(password)
			};

			try
			{
				var userCollection = _couchbaseService.TodoCollection;
				await userCollection.InsertAsync(Guid.NewGuid().ToString(), user);
			}
			catch
			{
				return null;
			}

			return user;
		}

		public async Task UpdateUser(User user)
		{
			var userCollection = _couchbaseService.TodoCollection;
			await userCollection.ReplaceAsync(
				$"user::{user.UserEmail}",
				user,
				new Couchbase.KeyValue.ReplaceOptions());
		}

		private static string CalculateMd5Hash(string password)
		{
			using (var md5 = MD5.Create())
			{
				var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
				return string.Concat(bytes.Select(x => x.ToString("x2")));
			}
		}

		public async Task<User> GetUser(string useremail)
		{
			try
			{
				var queryResult = await _couchbaseService.Cluster.QueryAsync<dynamic>(
					"SELECT META().id  FROM `TodoAppBucket` WHERE userEmail=$1",
					options => options.Parameter(useremail));
				string queryString = "";
				await foreach (Object o in queryResult)
					queryString += o.ToString();
				string queryId = queryString.Substring(12, 36);

				var result = await _couchbaseService.TodoCollection.GetAsync(queryId);

				return result.ContentAs<User>();
			}
			catch
			{
				return null;
			}
		}

		public async Task<User> GetAndAuthenticateUser(string username, string password)
		{
			var user = await GetUser(username);
			if (user == null)
			{
				Console.WriteLine("User not found!");
				return null;
			}

			if (user.Password != CalculateMd5Hash(password))
			{
				Console.WriteLine("User password wrong");
				return null;
			}

			return user;
		}

		public async Task<IEnumerable<Todo>> GetUserTodos(string useremail)
		{
			try
			{
				var cluster = await Cluster.ConnectAsync("couchbase://localhost", "Administrator", "password");
				var queryResult = await cluster.QueryAsync<Todo>(
				"SELECT * FROM `TodoAppBucket` WHERE userEmail=$1 and type=$2",
				options => options.Parameter(useremail).Parameter("todo"));

				var todos = await queryResult.Rows.ToListAsync();
				return (todos);
			}
			catch
			{
				return null;
			}
		}

		public async Task<bool> UserExists(string useremail)
		{
			var userCollection = _couchbaseService.TodoCollection;
			var result = await userCollection.ExistsAsync(
				$"useremail::{useremail}",
				new Couchbase.KeyValue.ExistsOptions());
			return result.Exists;
		}
	}
}