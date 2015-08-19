using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNet5.Identity.MongoDB;
using MongoDB.Driver;
using Xunit;

namespace AspNet.Identity3.MongoDB.Tests
{
	public class RoleStoreTests : IDisposable
	{
		protected DatabaseFixture DatabaseFixture;
		protected IMongoCollection<IdentityRole> roleCollection;
		//protected IMongoCollection<IdentityUser> userCollection;

		public RoleStoreTests()
		{
			DatabaseFixture = new DatabaseFixture<RoleStoreTests>();
			roleCollection = DatabaseFixture.GetCollection<IdentityRole>();
			//userCollection = DatabaseFixture.GetCollection<IdentityUser>();
		}

		public void Dispose()
		{
			DatabaseFixture.Dispose();
		}

		public class Constructors : RoleStoreTests
		{
			[Fact]
			public void Can_inisialise_from_connectionString()
			{
				// arrange
				string collectionName = "TestColName";
				var roleStore = new RoleStoreHelper(DatabaseFixture.ConnectionString, DatabaseFixture.DatabaseName, collectionName);

				// assert
				Assert.Equal(DatabaseFixture.DatabaseName, roleStore.DatabaseName);
				Assert.Equal(collectionName, roleStore.CollectionName);

				Assert.NotNull(roleStore.MongoClient);
				Assert.NotNull(roleStore.MongoDatabase);
				Assert.NotNull(roleStore.MongoCollection);

				Assert.Equal(DatabaseFixture.DatabaseName, roleStore.MongoDatabase.DatabaseNamespace.DatabaseName);
				Assert.Equal(DatabaseFixture.DatabaseName, roleStore.MongoCollection.CollectionNamespace.DatabaseNamespace.DatabaseName);
				Assert.Equal(collectionName, roleStore.MongoCollection.CollectionNamespace.CollectionName);
			}

			[Fact]
			public void Can_inisialise_from_connectionString_with_default_db_and_collection_names()
			{
				// arrange
				var roleStore = new RoleStoreHelper(DatabaseFixture.ConnectionString);

				// assert
				string defaultDatabaseName = "AspNetIdentity";
				string defaultCollectionName = "AspNetRoles";
				Assert.Equal(defaultDatabaseName, roleStore.DatabaseName);
				Assert.Equal(defaultCollectionName, roleStore.CollectionName);

				Assert.Equal(defaultDatabaseName, roleStore.MongoDatabase.DatabaseNamespace.DatabaseName);
				Assert.Equal(defaultDatabaseName, roleStore.MongoCollection.CollectionNamespace.DatabaseNamespace.DatabaseName);
				Assert.Equal(defaultCollectionName, roleStore.MongoCollection.CollectionNamespace.CollectionName);
			}

			[Fact]
			public void Can_inisialise_from_MongoClient()
			{
				// arrange
				string collectionName = "TestColName";
				var roleStore = new RoleStoreHelper(DatabaseFixture.GetMongoClient(), DatabaseFixture.DatabaseName, collectionName);

				// assert
				Assert.Equal(DatabaseFixture.DatabaseName, roleStore.DatabaseName);
				Assert.Equal(collectionName, roleStore.CollectionName);

				Assert.NotNull(roleStore.MongoClient);
				Assert.NotNull(roleStore.MongoDatabase);
				Assert.NotNull(roleStore.MongoCollection);

				Assert.Equal(DatabaseFixture.DatabaseName, roleStore.MongoDatabase.DatabaseNamespace.DatabaseName);
				Assert.Equal(DatabaseFixture.DatabaseName, roleStore.MongoCollection.CollectionNamespace.DatabaseNamespace.DatabaseName);
				Assert.Equal(collectionName, roleStore.MongoCollection.CollectionNamespace.CollectionName);
			}

			[Fact]
			public void Can_inisialise_from_MongoDatabase()
			{
				// arrange
				string collectionName = "TestColName";
				var roleStore = new RoleStoreHelper(DatabaseFixture.GetMongoDatabase(), collectionName);

				// assert
				Assert.Equal(DatabaseFixture.DatabaseName, roleStore.DatabaseName);
				Assert.Equal(collectionName, roleStore.CollectionName);

				Assert.NotNull(roleStore.MongoClient);
				Assert.NotNull(roleStore.MongoDatabase);
				Assert.NotNull(roleStore.MongoCollection);

				Assert.Equal(DatabaseFixture.DatabaseName, roleStore.MongoDatabase.DatabaseNamespace.DatabaseName);
				Assert.Equal(DatabaseFixture.DatabaseName, roleStore.MongoCollection.CollectionNamespace.DatabaseNamespace.DatabaseName);
				Assert.Equal(collectionName, roleStore.MongoCollection.CollectionNamespace.CollectionName);
			}

			[Fact]
			public void Can_inisialise_from_MongoCollection()
			{
				// arrange
				string collectionName = typeof(IdentityRole).Name;
				var roleStore = new RoleStoreHelper(roleCollection);

				// assert
				Assert.Equal(DatabaseFixture.DatabaseName, roleStore.DatabaseName);
				Assert.Equal(collectionName, roleStore.CollectionName);

				Assert.NotNull(roleStore.MongoClient);
				Assert.NotNull(roleStore.MongoDatabase);
				Assert.NotNull(roleStore.MongoCollection);

				Assert.Equal(DatabaseFixture.DatabaseName, roleStore.MongoDatabase.DatabaseNamespace.DatabaseName);
				Assert.Equal(DatabaseFixture.DatabaseName, roleStore.MongoCollection.CollectionNamespace.DatabaseNamespace.DatabaseName);
				Assert.Equal(collectionName, roleStore.MongoCollection.CollectionNamespace.CollectionName);
			}

		}

		public class CreateAsyncMethod : RoleStoreTests
		{
			
		}


		#region PROTECTED HELPER CLASS

		protected class RoleStoreHelper : RoleStore<IdentityRole>
		{
			public RoleStoreHelper(string connectionString, string databaseName = null, string collectionName = null) : base(connectionString, databaseName, collectionName) { }

			public RoleStoreHelper(IMongoClient client, string databaseName = null, string collectionName = null) : base(client, databaseName, collectionName) { }

			public RoleStoreHelper(IMongoDatabase database, string collectionName = null) : base(database, collectionName) { }

			public RoleStoreHelper(IMongoCollection<IdentityRole> collection) : base(collection) { }


			#region helper methods to get protected fields

			public string DatabaseName { get { return base._databaseName; } }
			public string CollectionName { get { return base._collectionName; } }
			public IMongoClient MongoClient { get { return base._client; } }
			public IMongoDatabase MongoDatabase { get { return base._database; } }
			public IMongoCollection<IdentityRole> MongoCollection { get { return base._collection; } }

			#endregion
		}

		#endregion
	}
}
