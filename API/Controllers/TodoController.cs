using System;
using System.Threading.Tasks;
using API.Models;
using API.Services;
using Couchbase.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	[ApiController]
	[Route("api/todos")]
	public class TodoController : ControllerBase
	{
		private readonly IBucketProvider _provider;
		private readonly ICouchbaseService _couchbaseService;

		public TodoController(IBucketProvider provider, ICouchbaseService couchbaseService)
		{
			_couchbaseService = couchbaseService;

			_provider = provider;
		}

		[HttpGet]
		[Route("{Id}")]
		public async Task<Todo> GetTodoItem(string Id)
		{

			var getResult = await _couchbaseService.TodoCollection.GetAsync(Id);
			return getResult.ContentAs<Todo>();
		}

		[HttpPost]
		public async Task<ActionResult> CreateTodoItem([FromBody] Todo todo)
		{
			if (todo.UserEmail.Length < 0)
			{
				throw new Exception("Error in input data, Id should not be set!");
			}
			try
			{
				await _couchbaseService.TodoCollection.InsertAsync<Todo>(Guid.NewGuid().ToString(), todo);
				return Ok("Created..");
			}
			catch (Exception ex)
			{
				return BadRequest(("Something went wrong..", ex));
			}

		}

		[HttpPut]
		public async Task Update(string _id, Todo todo)
		{

			if (_id == "")
			{
				throw new Exception("Error in input data, Id is required!");
			}

			var getTodo = await _couchbaseService.TodoCollection.GetAsync(_id);
			var oldTodo = getTodo.ContentAs<Todo>();
			var isComp = false;

			if (oldTodo.IsCompleted == todo.IsCompleted)
			{
				isComp = oldTodo.IsCompleted;
			}
			else
			{
				isComp = todo.IsCompleted;
			}
			var upsertResult = await _couchbaseService.TodoCollection.UpsertAsync(_id, new { userEmail = todo.UserEmail, message = todo.Message, todoDate = oldTodo.TodoDate, isCompleted = isComp });
			var getResult = await _couchbaseService.TodoCollection.GetAsync(_id);
			Console.WriteLine(getResult.ContentAs<dynamic>());
		}

		[HttpDelete]
		[Route("{Id}")]
		public async Task DeleteTodoItem(string Id)
		{
			if (string.IsNullOrEmpty(Id))
			{
				throw new Exception("Error in input data, Id is required!");
			}

			await _couchbaseService.TodoCollection.RemoveAsync(Id);
		}
	}
}