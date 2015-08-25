using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Framework.Configuration;
using MongoDB.Driver;

namespace AspNet.Identity3.MongoDB.Tests
{
	public class DatabaseFixture<T> : DatabaseFixture
		where T : class
	{
		public DatabaseFixture() : base(typeof(T).Name) { }
		public DatabaseFixture(bool dropDatabaseOnInit, bool dropDatabaseOnDispose) : base(typeof(T).Name, null, dropDatabaseOnInit, dropDatabaseOnDispose) { }

		public DatabaseFixture(string collectionPrefix, string databaseName = null) : base(collectionPrefix, databaseName) { }
		public DatabaseFixture(string collectionPrefix, string databaseName, bool dropDatabaseOnInit, bool dropDatabaseOnDispose) : base(collectionPrefix, databaseName, dropDatabaseOnInit, dropDatabaseOnDispose) { }
	}

	public class DatabaseFixture : IDisposable
	{
		private IMongoClient _mongoClient;
		private IMongoDatabase _mongoDatabase;
		protected bool _dropDatabaseOnInit;
		protected bool _dropDatabaseOnDispose;

		public DatabaseFixture(string collectionPrefix, string databaseName = null) : this(collectionPrefix, databaseName, true, false) { }
		public DatabaseFixture(string collectionPrefix, string databaseName, bool dropDatabaseOnInit, bool dropDatabaseOnDispose)
		{
			RegisterClassMap<IdentityUser, IdentityRole, string>.Init();

			//Below code demonstrates usage of multiple configuration sources. For instance a setting say 'setting1' is found in both the registered sources, 
			//then the later source will win. By this way a Local config can be overridden by a different setting while deployed remotely.
			var builder = new ConfigurationBuilder(".\\")
						.AddJsonFile("config.json");
			Configuration = builder.Build();

			ConnectionString = Configuration["Data:ConnectionString"];
			CollectionPrefix = string.IsNullOrWhiteSpace(collectionPrefix) ? DateTime.UtcNow.ToShortTimeString() : collectionPrefix;
			DatabaseName = !string.IsNullOrWhiteSpace(databaseName) ? databaseName :
									!string.IsNullOrWhiteSpace(Configuration["Data:database"]) ? Configuration["Data:database"] : "Testing";

			_dropDatabaseOnInit = dropDatabaseOnInit;
			_dropDatabaseOnDispose = dropDatabaseOnDispose;

			if(_dropDatabaseOnInit) DropDatabase();
		}


		public IConfiguration Configuration { get; private set; }
		public string ConnectionString { get; private set; }
		public string DatabaseName { get; private set; }
		public string CollectionPrefix { get; private set; }

		public void Dispose()
		{
			if(_dropDatabaseOnDispose) DropDatabase();
		}

		public void DropDatabase()
		{
			foreach(var c in _mongoCollectionNames)
			{
				GetMongoDatabase().DropCollectionAsync(c);
			}
			var cursorTask = GetMongoDatabase().ListCollectionsAsync();
			// as its async function - sleep for  a bit just to give it a chance to run and finish
			Thread.Sleep(50);

			var cursor = cursorTask.Result;
			cursor.ForEachAsync(c =>
			{
				var collectionName = c["name"].ToString();
				GetMongoDatabase().DropCollectionAsync(collectionName);
			});

			Thread.Sleep(50);
		}

		public IMongoClient GetMongoClient()
		{
			if(_mongoClient == null)
			{
				_mongoClient = new MongoClient(ConnectionString);
			}
			return _mongoClient;
		}
		public IMongoDatabase GetMongoDatabase()
		{
			if(_mongoDatabase == null)
			{
				_mongoDatabase = GetMongoClient().GetDatabase(DatabaseName);
			}
			return _mongoDatabase;
		}

		public IMongoCollection<T> GetCollection<T>()
		{
			var collectionName = string.Format("{0}_{1}", CollectionPrefix, typeof(T).Name);
			var collectionSettings = new MongoCollectionSettings { WriteConcern = WriteConcern.WMajority };
			_mongoCollectionNames.Add(collectionName);

			return GetMongoDatabase().GetCollection<T>(collectionName, collectionSettings);
		}
		private IList<string> _mongoCollectionNames = new List<string>();
	}
}
