using System;
using System.Threading.Tasks;
using Couchbase;
using Couchbase.KeyValue;

namespace API.Services
{
	public interface ICouchbaseService
	{
		ICluster Cluster { get; }
		IBucket TodoAppBucket { get; }
		ICouchbaseCollection TodoCollection { get; }
	}

	public static class StringExtension
	{
		public static string DefaultIfEmpty(this string str, string defaultValue)
			=> string.IsNullOrWhiteSpace(str) ? defaultValue : str;
	}

	public class CouchbaseService : ICouchbaseService
	{
		public ICluster Cluster { get; private set; }
		public IBucket TodoAppBucket { get; private set; }
		public ICouchbaseCollection TodoCollection { get; private set; }

		public CouchbaseService()
		{
			// TODO: get these variables via DI, possibly overriding config in appsettings.json
			var CB_HOST = Environment.GetEnvironmentVariable("CB_HOST").DefaultIfEmpty("localhost");
			var CB_USER = Environment.GetEnvironmentVariable("CB_USER").DefaultIfEmpty("Administrator");
			var CB_PSWD = Environment.GetEnvironmentVariable("CB_PSWD").DefaultIfEmpty("qweasd1");

			Console.WriteLine(
				$"Connecting to couchbase://{CB_HOST} with {CB_USER} / {CB_PSWD}");

			try
			{
				var task = Task.Run(async () =>
				{
					var cluster = await Couchbase.Cluster.ConnectAsync(
						$"couchbase://{CB_HOST}",
						CB_USER,
						CB_PSWD);

					Cluster = cluster;
					var bucket = await cluster.BucketAsync("TodoAppBucket");
					var collection = await bucket.DefaultCollectionAsync();
					TodoCollection = collection;
				});
				task.Wait();
			}
			catch (AggregateException ae)
			{
				ae.Handle((x) => throw x);
			}
		}
	}
}