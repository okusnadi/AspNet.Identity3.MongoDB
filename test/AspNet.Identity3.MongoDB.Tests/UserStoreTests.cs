using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using MongoDB.Driver;
using Xunit;

namespace AspNet.Identity3.MongoDB.Tests
{
	public class UserStoreTests : IDisposable
	{
		private readonly DatabaseFixture _databaseFixture;
		private readonly IMongoCollection<IdentityUser> _userCollection;
		private readonly string _collectionPrefix;

		private readonly UserStore<IdentityUser, IdentityRole> _userStore;
		private readonly IdentityErrorDescriber _errorDescriber;

		public UserStoreTests(string collectionPrefix)
		{
			_databaseFixture = new DatabaseFixture(collectionPrefix);
			_userCollection = _databaseFixture.GetCollection<IdentityUser>();
			_collectionPrefix = collectionPrefix;

			_errorDescriber = new IdentityErrorDescriber();
			_userStore = new UserStore<IdentityUser, IdentityRole>(_userCollection, _errorDescriber);
		}

		public void Dispose()
		{
			_databaseFixture.Dispose();
		}

		public class Constructors : UserStoreTests
		{
			public Constructors() : base(typeof(Constructors).Name) { }

			[Fact]
			public void Can_inisialise_from_connectionString()
			{
				// arrange
				string collectionName = "TestColName";
				var UserStore = new UserStoreHelper(_databaseFixture.ConnectionString, _databaseFixture.DatabaseName, collectionName);

				// assert
				Assert.Equal(_databaseFixture.DatabaseName, UserStore.DatabaseName);
				Assert.Equal(collectionName, UserStore.CollectionName);

				Assert.NotNull(UserStore.MongoClient);
				Assert.NotNull(UserStore.MongoDatabase);
				Assert.NotNull(UserStore.MongoCollection);

				Assert.Equal(_databaseFixture.DatabaseName, UserStore.MongoDatabase.DatabaseNamespace.DatabaseName);
				Assert.Equal(_databaseFixture.DatabaseName, UserStore.MongoCollection.CollectionNamespace.DatabaseNamespace.DatabaseName);
				Assert.Equal(collectionName, UserStore.MongoCollection.CollectionNamespace.CollectionName);
			}

			[Fact]
			public void Can_inisialise_from_connectionString_with_default_db_and_collection_names()
			{
				// arrange
				var UserStore = new UserStoreHelper(_databaseFixture.ConnectionString);

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
				var UserStore = new UserStoreHelper(_databaseFixture.GetMongoClient(), _databaseFixture.DatabaseName, collectionName);

				// assert
				Assert.Equal(_databaseFixture.DatabaseName, UserStore.DatabaseName);
				Assert.Equal(collectionName, UserStore.CollectionName);

				Assert.NotNull(UserStore.MongoClient);
				Assert.NotNull(UserStore.MongoDatabase);
				Assert.NotNull(UserStore.MongoCollection);

				Assert.Equal(_databaseFixture.DatabaseName, UserStore.MongoDatabase.DatabaseNamespace.DatabaseName);
				Assert.Equal(_databaseFixture.DatabaseName, UserStore.MongoCollection.CollectionNamespace.DatabaseNamespace.DatabaseName);
				Assert.Equal(collectionName, UserStore.MongoCollection.CollectionNamespace.CollectionName);
			}

			[Fact]
			public void Can_inisialise_from_MongoDatabase()
			{
				// arrange
				string collectionName = "TestColName";
				var UserStore = new UserStoreHelper(_databaseFixture.GetMongoDatabase(), collectionName);

				// assert
				Assert.Equal(_databaseFixture.DatabaseName, UserStore.DatabaseName);
				Assert.Equal(collectionName, UserStore.CollectionName);

				Assert.NotNull(UserStore.MongoClient);
				Assert.NotNull(UserStore.MongoDatabase);
				Assert.NotNull(UserStore.MongoCollection);

				Assert.Equal(_databaseFixture.DatabaseName, UserStore.MongoDatabase.DatabaseNamespace.DatabaseName);
				Assert.Equal(_databaseFixture.DatabaseName, UserStore.MongoCollection.CollectionNamespace.DatabaseNamespace.DatabaseName);
				Assert.Equal(collectionName, UserStore.MongoCollection.CollectionNamespace.CollectionName);
			}

			[Fact]
			public void Can_inisialise_from_MongoCollection()
			{
				// arrange
				string collectionName = _userCollection.CollectionNamespace.CollectionName;
				var UserStore = new UserStoreHelper(_userCollection);

				// assert
				Assert.Equal(_databaseFixture.DatabaseName, UserStore.DatabaseName);
				Assert.Equal(collectionName, UserStore.CollectionName);

				Assert.NotNull(UserStore.MongoClient);
				Assert.NotNull(UserStore.MongoDatabase);
				Assert.NotNull(UserStore.MongoCollection);

				Assert.Equal(_databaseFixture.DatabaseName, UserStore.MongoDatabase.DatabaseNamespace.DatabaseName);
				Assert.Equal(_databaseFixture.DatabaseName, UserStore.MongoCollection.CollectionNamespace.DatabaseNamespace.DatabaseName);
				Assert.Equal(collectionName, UserStore.MongoCollection.CollectionNamespace.CollectionName);
			}

		}

		public class CreateAsyncMethod : UserStoreTests
		{
			public CreateAsyncMethod() : base(typeof(CreateAsyncMethod).Name) { }

			[Fact]
			public async Task Create_user_returns_Success()
			{
				// arrange
				var user = new IdentityUser("Create_user_returns_Success");

				// act
				var result = await _userStore.CreateAsync(user);

				// assert
				IdentityResultAssert.IsSuccess(result);

				var userFromDb = await _userCollection.Find(x => x.Id == user.Id).SingleOrDefaultAsync();
				IdentityUserAssert.Equal(user, userFromDb);
			}


			[Fact]
			public async Task Creating_same_user_twice_returns_DuplicateUserName_error()
			{
				// arrange
				var user = new IdentityUser("Creating_same_user_twice_returns_DuplicateUserName_error");

				// act
				var result1 = await _userStore.CreateAsync(user);

				user.UserName = "a different name, but same Id";
				var result2 = await _userStore.CreateAsync(user);

				// assert
				IdentityResultAssert.IsSuccess(result1);

				var expectedError = IdentityResult.Failed(_errorDescriber.DuplicateUserName(user.ToString()));
				IdentityResultAssert.IsFailure(result2, expectedError.Errors.FirstOrDefault());
			}

			[Fact]
			public async Task Creating_two_different_users_but_same_UserName_returns_DuplicateUserName_error()
			{
				// arrange
				var user1 = new IdentityUser("Creating_two_different_users_but_same_UserName_returns_DuplicateUserName_error");
				var user2 = new IdentityUser(user1.UserName);

				// act
				var result1 = await _userStore.CreateAsync(user1);
				var result2 = await _userStore.CreateAsync(user2);

				// assert
				IdentityResultAssert.IsSuccess(result1);

				var expectedError = IdentityResult.Failed(_errorDescriber.DuplicateUserName(user2.ToString()));
				IdentityResultAssert.IsFailure(result2, expectedError.Errors.FirstOrDefault());
			}
		}

		#region PROTECTED HELPER CLASS

		class UserStoreHelper : UserStore<IdentityUser, IdentityRole>
		{
			public UserStoreHelper(string connectionString, string databaseName = null, string collectionName = null) : base(connectionString, databaseName, collectionName) { }

			public UserStoreHelper(IMongoClient client, string databaseName = null, string collectionName = null) : base(client, databaseName, collectionName) { }

			public UserStoreHelper(IMongoDatabase database, string collectionName = null) : base(database, collectionName) { }

			public UserStoreHelper(IMongoCollection<IdentityUser> collection) : base(collection) { }
			
			#region helper methods to get protected fields

			public string DatabaseName => base._databaseName;
			public string CollectionName => base._collectionName;
			public IMongoClient MongoClient => base._client;
			public IMongoDatabase MongoDatabase => base._database;
			public IMongoCollection<IdentityUser> MongoCollection => base._collection;

			#endregion
		}

		#endregion
	}
}
