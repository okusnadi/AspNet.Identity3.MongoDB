using System;
using System.Linq;
using System.Threading.Tasks;
using AspNet5.Identity.MongoDB;
using Microsoft.AspNet.Identity;
using MongoDB.Driver;
using Xunit;

namespace AspNet.Identity3.MongoDB.Tests
{
	public class RoleStoreTests : IDisposable
	{
		protected DatabaseFixture DatabaseFixture;
		protected IMongoCollection<IdentityRole> roleCollection;
		//protected IMongoCollection<IdentityUser> userCollection;

		protected RoleStore<IdentityRole> roleStore;
		protected IdentityErrorDescriber ErrorDescriber;

		public RoleStoreTests()
		{
			DatabaseFixture = new DatabaseFixture<RoleStoreTests>();
			roleCollection = DatabaseFixture.GetCollection<IdentityRole>();
			//userCollection = DatabaseFixture.GetCollection<IdentityUser>();

			roleStore = new RoleStore<IdentityRole>(roleCollection);
			ErrorDescriber = new IdentityErrorDescriber();
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
			[Fact]
			public async Task Create_role_returns_Success()
			{
				// arrange
				var claim1 = new IdentityClaim { ClaimType = "ClaimType1", ClaimValue = "some value" };
				var claim2 = new IdentityClaim { ClaimType = "ClaimType2", ClaimValue = "some other value" };
				var role = new IdentityRole("some role");
				role.Claims.Add(claim1);
				role.Claims.Add(claim2);

				// act
				var result = await roleStore.CreateAsync(role);

				// assert
				IdentityResultAssert.IsSuccess(result);

				var roleFromDb = await roleCollection.Find(x => x.Id == role.Id).SingleOrDefaultAsync();
				IdentityRoleAssert.Equal(role, roleFromDb);
			}


			[Fact]
			public async Task Creating_same_role_twice_returns_DuplicateRoleName_error()
			{
				// arrange
				var role = new IdentityRole("some role");

				// act
				var result1 = await roleStore.CreateAsync(role);

				role.Name = "a different name, but same Id";
				var result2 = await roleStore.CreateAsync(role);

				// assert
				IdentityResultAssert.IsSuccess(result1);

				var expectedError = IdentityResult.Failed(ErrorDescriber.DuplicateRoleName(role.ToString()));
				IdentityResultAssert.IsFailure(result2, expectedError.Errors.FirstOrDefault());
			}

			[Fact]
			public async Task Creating_two_different_roles_but_same_Name_returns_DuplicateRoleName_error()
			{
				// arrange
				var role1 = new IdentityRole("some role name");
				var role2 = new IdentityRole(role1.Name);

				// act
				var result1 = await roleStore.CreateAsync(role1);
				var result2 = await roleStore.CreateAsync(role2);

				// assert
				IdentityResultAssert.IsSuccess(result1);

				var expectedError = IdentityResult.Failed(ErrorDescriber.DuplicateRoleName(role2.ToString()));
				IdentityResultAssert.IsFailure(result2, expectedError.Errors.FirstOrDefault());
			}
		}

		public class UpdateAsyncMethod : RoleStoreTests
		{
			[Fact]
			public async Task Update_role_returns_Success()
			{
				// arrange
				var claim1 = new IdentityClaim { ClaimType = "ClaimType1", ClaimValue = "some value" };
				var claim2 = new IdentityClaim { ClaimType = "ClaimType2", ClaimValue = "some other value" };
				var role = new IdentityRole("some role");
				role.Claims.Add(claim1);

				// initial role creation
				await roleStore.CreateAsync(role);
				role.Name = "a new name";
				role.Claims.Add(claim2);


				// act
				var result = await roleStore.UpdateAsync(role);

				// assert
				IdentityResultAssert.IsSuccess(result);

				var roleFromDb = await roleCollection.Find(x => x.Id == role.Id).SingleOrDefaultAsync();
				IdentityRoleAssert.Equal(role, roleFromDb);
			}


			[Fact]
			public async Task Can_update_role_multiple_times()
			{
				// arrange
				var role = new IdentityRole("some role");
				await roleStore.CreateAsync(role);

				// act
				role.Claims.Add(new IdentityClaim { ClaimType = "ClaimType1", ClaimValue = "claim value" });
				var result1 = await roleStore.UpdateAsync(role);

				role.Name = "a different name";
				var result2 = await roleStore.UpdateAsync(role);

				// assert
				IdentityResultAssert.IsSuccess(result1);
				IdentityResultAssert.IsSuccess(result2);

				var roleFromDb = await roleCollection.Find(x => x.Id == role.Id).SingleOrDefaultAsync();
				IdentityRoleAssert.Equal(role, roleFromDb);
			}

			[Fact]
			public async Task Updating_role_name_to_existing_name_returns_DuplicateRoleName_error()
			{
				// arrange
				var role1 = new IdentityRole("some role");
				var role2 = new IdentityRole("another role");

				var result1 = await roleStore.CreateAsync(role1);
				var result2 = await roleStore.CreateAsync(role2);

				// act
				role2.Name = role1.Name;
				var result = await roleStore.UpdateAsync(role2);

				// assert
				var expectedError = IdentityResult.Failed(ErrorDescriber.DuplicateRoleName(role2.ToString()));
				IdentityResultAssert.IsFailure(result2, expectedError.Errors.FirstOrDefault());
			}
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
