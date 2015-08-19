using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNet5.Identity.MongoDB;
using MongoDB.Driver;
using Xunit;

namespace AspNet.Identity3.MongoDB.Tests
{
	public class UserStoreTests : IDisposable
	{
		protected DatabaseFixture DatabaseFixture;
		protected IMongoCollection<IdentityUser> userCollection;

		public UserStoreTests()
		{
			DatabaseFixture = new DatabaseFixture<UserStoreTests>();
			userCollection = DatabaseFixture.GetCollection<IdentityUser>(); 
		}

		public void Dispose()
		{
			DatabaseFixture.Dispose();
		}

		public class Constructors : UserStoreTests
		{
			[Fact]
			public void Can_inisialise_from_connectionString()
			{
				// arrange
				string collectionName = "TestColName";
				var UserStore = new UserStoreHelper(DatabaseFixture.ConnectionString, DatabaseFixture.DatabaseName, collectionName);

				// assert
				Assert.Equal(DatabaseFixture.DatabaseName, UserStore.DatabaseName);
				Assert.Equal(collectionName, UserStore.CollectionName);

				Assert.NotNull(UserStore.MongoClient);
				Assert.NotNull(UserStore.MongoDatabase);
				Assert.NotNull(UserStore.MongoCollection);

				Assert.Equal(DatabaseFixture.DatabaseName, UserStore.MongoDatabase.DatabaseNamespace.DatabaseName);
				Assert.Equal(DatabaseFixture.DatabaseName, UserStore.MongoCollection.CollectionNamespace.DatabaseNamespace.DatabaseName);
				Assert.Equal(collectionName, UserStore.MongoCollection.CollectionNamespace.CollectionName);
			}

			[Fact]
			public void Can_inisialise_from_connectionString_with_default_db_and_collection_names()
			{
				// arrange
				var UserStore = new UserStoreHelper(DatabaseFixture.ConnectionString);

				// assert
				string defaultDatabaseName = "AspNetIdentity";
				string defaultCollectionName = "AspNetUsers";
				Assert.Equal(defaultDatabaseName, UserStore.DatabaseName);
				Assert.Equal(defaultCollectionName, UserStore.CollectionName);

				Assert.Equal(defaultDatabaseName, UserStore.MongoDatabase.DatabaseNamespace.DatabaseName);
				Assert.Equal(defaultDatabaseName, UserStore.MongoCollection.CollectionNamespace.DatabaseNamespace.DatabaseName);
				Assert.Equal(defaultCollectionName, UserStore.MongoCollection.CollectionNamespace.CollectionName);
			}

			[Fact]
			public void Can_inisialise_from_MongoClient()
			{
				// arrange
				string collectionName = "TestColName";
				var UserStore = new UserStoreHelper(DatabaseFixture.GetMongoClient(), DatabaseFixture.DatabaseName, collectionName);

				// assert
				Assert.Equal(DatabaseFixture.DatabaseName, UserStore.DatabaseName);
				Assert.Equal(collectionName, UserStore.CollectionName);

				Assert.NotNull(UserStore.MongoClient);
				Assert.NotNull(UserStore.MongoDatabase);
				Assert.NotNull(UserStore.MongoCollection);

				Assert.Equal(DatabaseFixture.DatabaseName, UserStore.MongoDatabase.DatabaseNamespace.DatabaseName);
				Assert.Equal(DatabaseFixture.DatabaseName, UserStore.MongoCollection.CollectionNamespace.DatabaseNamespace.DatabaseName);
				Assert.Equal(collectionName, UserStore.MongoCollection.CollectionNamespace.CollectionName);
			}

			[Fact]
			public void Can_inisialise_from_MongoDatabase()
			{
				// arrange
				string collectionName = "TestColName";
				var UserStore = new UserStoreHelper(DatabaseFixture.GetMongoDatabase(), collectionName);

				// assert
				Assert.Equal(DatabaseFixture.DatabaseName, UserStore.DatabaseName);
				Assert.Equal(collectionName, UserStore.CollectionName);

				Assert.NotNull(UserStore.MongoClient);
				Assert.NotNull(UserStore.MongoDatabase);
				Assert.NotNull(UserStore.MongoCollection);

				Assert.Equal(DatabaseFixture.DatabaseName, UserStore.MongoDatabase.DatabaseNamespace.DatabaseName);
				Assert.Equal(DatabaseFixture.DatabaseName, UserStore.MongoCollection.CollectionNamespace.DatabaseNamespace.DatabaseName);
				Assert.Equal(collectionName, UserStore.MongoCollection.CollectionNamespace.CollectionName);
			}

			[Fact]
			public void Can_inisialise_from_MongoCollection()
			{
				// arrange
				string collectionName = typeof(IdentityUser).Name;
				var UserStore = new UserStoreHelper(userCollection);

				// assert
				Assert.Equal(DatabaseFixture.DatabaseName, UserStore.DatabaseName);
				Assert.Equal(collectionName, UserStore.CollectionName);

				Assert.NotNull(UserStore.MongoClient);
				Assert.NotNull(UserStore.MongoDatabase);
				Assert.NotNull(UserStore.MongoCollection);

				Assert.Equal(DatabaseFixture.DatabaseName, UserStore.MongoDatabase.DatabaseNamespace.DatabaseName);
				Assert.Equal(DatabaseFixture.DatabaseName, UserStore.MongoCollection.CollectionNamespace.DatabaseNamespace.DatabaseName);
				Assert.Equal(collectionName, UserStore.MongoCollection.CollectionNamespace.CollectionName);
			}

		}


		#region PROTECTED HELPER CLASS

		protected class UserStoreHelper : UserStore<IdentityUser, IdentityRole>
		{
			public UserStoreHelper(string connectionString, string databaseName = null, string collectionName = null) : base(connectionString, databaseName, collectionName) { }

			public UserStoreHelper(IMongoClient client, string databaseName = null, string collectionName = null) : base(client, databaseName, collectionName) { }

			public UserStoreHelper(IMongoDatabase database, string collectionName = null) : base(database, collectionName) { }

			public UserStoreHelper(IMongoCollection<IdentityUser> collection) : base(collection) { }


			#region helper methods to get protected fields

			public string DatabaseName { get { return base._databaseName; } }
			public string CollectionName { get { return base._collectionName; } }
			public IMongoClient MongoClient { get { return base._client; } }
			public IMongoDatabase MongoDatabase { get { return base._database; } }
			public IMongoCollection<IdentityUser> MongoCollection { get { return base._collection; } }

			#endregion
		}

		#endregion
	}
}
