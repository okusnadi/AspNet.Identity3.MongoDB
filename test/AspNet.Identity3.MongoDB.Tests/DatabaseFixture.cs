using System;
using System.Collections.Generic;
using Microsoft.Framework.Configuration;
using MongoDB.Driver;

namespace AspNet.Identity3.MongoDB.Tests
{
	public class DatabaseFixture<T> : DatabaseFixture
		where T : class
	{
		public DatabaseFixture() : this(typeof(T).Name) { }
		public DatabaseFixture(bool dropDatabaseOnInit, bool dropDatabaseOnDispose) : this(typeof(T).Name, dropDatabaseOnInit, dropDatabaseOnDispose) { }

		public DatabaseFixture(string databaseName = null) : base(databaseName) { }
		public DatabaseFixture(string databaseName, bool dropDatabaseOnInit, bool dropDatabaseOnDispose) : base (databaseName, dropDatabaseOnInit, dropDatabaseOnDispose) { }
	}

	public class DatabaseFixture : IDisposable
	{
		private IMongoClient _mongoClient;
		private IMongoDatabase _mongoDatabase;
		protected bool _dropDatabaseOnInit;
		protected bool _dropDatabaseOnDispose;
		
		public DatabaseFixture(string databaseName = null) : this(databaseName, true, false) { }
		public DatabaseFixture(string databaseName, bool dropDatabaseOnInit, bool dropDatabaseOnDispose)
		{
			

			//Below code demonstrates usage of multiple configuration sources. For instance a setting say 'setting1' is found in both the registered sources, 
			//then the later source will win. By this way a Local config can be overridden by a different setting while deployed remotely.
			var builder = new ConfigurationBuilder(".\\")
						.AddJsonFile("config.json");
			Configuration = builder.Build();

			ConnectionString = Configuration["Data:ConnectionString"];
			DatabaseName = string.IsNullOrWhiteSpace(databaseName) ? Configuration["Data:database"] : databaseName;

			_dropDatabaseOnInit = dropDatabaseOnInit;
			_dropDatabaseOnDispose = dropDatabaseOnDispose;

			if(_dropDatabaseOnInit) DropDatabase();
		}

		
		public IConfiguration Configuration { get; private set; }
		public string ConnectionString { get; private set; }
		public string DatabaseName { get; private set; }

		public void Dispose()
		{
			if(_dropDatabaseOnDispose) DropDatabase();
		}

		public void DropDatabase()
		{
			GetMongoClient().DropDatabaseAsync(DatabaseName);
		}

		public IMongoClient GetMongoClient()
		{
			if (_mongoClient == null) {
				_mongoClient = new MongoClient(ConnectionString);
			}
			return _mongoClient;
		}
		public IMongoDatabase GetMongoDatabase()
		{
			if (_mongoDatabase == null) {
				_mongoDatabase = GetMongoClient().GetDatabase(DatabaseName);
			}
			return _mongoDatabase;
		}

		public IMongoCollection<T> GetCollection<T>()
		{
			var collectionName = typeof(T).Name;
			_mongoCollectionNames.Add(collectionName);
			return GetMongoDatabase().GetCollection<T>(collectionName);
		}
		private IList<string> _mongoCollectionNames = new List<string>();
	}
}
