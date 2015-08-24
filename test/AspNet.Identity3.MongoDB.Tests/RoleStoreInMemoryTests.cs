using System;
using System.Linq;
using System.Threading.Tasks;
using AspNet.Identity3.MongoDB;
using MongoDB.Driver;
using Xunit;

namespace AspNet.Identity3.MongoDB.Tests
{
	public class RoleStoreInMemoryTests
	{
		protected RoleStore<IdentityRole> RoleStore;

		public RoleStoreInMemoryTests()
		{
			RoleStore = new RoleStore<IdentityRole>("mongodb://localhost:27017");
		}

		public class GeneralChecks : RoleStoreInMemoryTests
		{
			[Fact]
			public async Task Methods_throw_ObjectDisposedException_when_disposed()
			{
				RoleStore.Dispose();
				await Assert.ThrowsAsync<ObjectDisposedException>(async () => await RoleStore.FindByIdAsync(null));
				await Assert.ThrowsAsync<ObjectDisposedException>(async () => await RoleStore.FindByNameAsync(null));
				await Assert.ThrowsAsync<ObjectDisposedException>(async () => await RoleStore.GetRoleIdAsync(null));
				await Assert.ThrowsAsync<ObjectDisposedException>(async () => await RoleStore.GetRoleNameAsync(null));
				await Assert.ThrowsAsync<ObjectDisposedException>(async () => await RoleStore.SetRoleNameAsync(null, null));
				await Assert.ThrowsAsync<ObjectDisposedException>(async () => await RoleStore.CreateAsync(null));
				await Assert.ThrowsAsync<ObjectDisposedException>(async () => await RoleStore.UpdateAsync(null));
				await Assert.ThrowsAsync<ObjectDisposedException>(async () => await RoleStore.DeleteAsync(null));
				
				await Assert.ThrowsAsync<ObjectDisposedException>(async () => await RoleStore.GetClaimsAsync(null));
				
			}

			[Fact]
			public void Constructors_throw_ArgumentNullException_with_null()
			{
				Assert.Throws<ArgumentNullException>("connectionString", () => new RoleStore<IdentityRole>((string)null));
				Assert.Throws<ArgumentNullException>("connectionString", () => new RoleStore<IdentityRole>(""));
				Assert.Throws<ArgumentNullException>("connectionString", () => new RoleStore<IdentityRole>(String.Empty));

				Assert.Throws<ArgumentNullException>("client", () => new RoleStore<IdentityRole>((IMongoClient)null));
				Assert.Throws<ArgumentNullException>("database", () => new RoleStore<IdentityRole>((IMongoDatabase)null));
				Assert.Throws<ArgumentNullException>("collection", () => new RoleStore<IdentityRole>((IMongoCollection<IdentityRole>)null));
			}

			[Fact]
			public async Task Methods_throw_ArgumentNullException_when_null_arguments()
			{
				await Assert.ThrowsAsync<ArgumentNullException>("role", async () => await RoleStore.GetRoleIdAsync(null));
				await Assert.ThrowsAsync<ArgumentNullException>("role", async () => await RoleStore.GetRoleNameAsync(null));
				await Assert.ThrowsAsync<ArgumentNullException>("role", async () => await RoleStore.SetRoleNameAsync(null, null));
				await Assert.ThrowsAsync<ArgumentNullException>("role", async () => await RoleStore.CreateAsync(null));
				await Assert.ThrowsAsync<ArgumentNullException>("role", async () => await RoleStore.UpdateAsync(null));
				await Assert.ThrowsAsync<ArgumentNullException>("role", async () => await RoleStore.DeleteAsync(null));

				await Assert.ThrowsAsync<ArgumentNullException>("role", async () => await RoleStore.GetClaimsAsync(null));
			}
		}

		public class GetRoleIdAsyncMethod : RoleStoreInMemoryTests
		{
			[Fact]
			public async Task Returns_Id_from_Role_as_string()
			{
				// arrange
				var role = new IdentityRole("Returns_Id_from_Role_as_string");

				// act
				var result = await RoleStore.GetRoleIdAsync(role);

				// assert
				Assert.Equal(role.Id, result);
			}

			[Fact]
			public async Task Null_id_on_role_returns_Null()
			{
				// arrange
				var role = new IdentityRole("Null_id_on_role_returns_Null");
				role.Id = null;

				// act
				var result = await RoleStore.GetRoleIdAsync(role);

				// assert
				Assert.Null(result);
			}
		}

		public class GetRoleNameAsyncMethod : RoleStoreInMemoryTests
		{
			[Theory]
			[InlineData(null)]
			[InlineData("")]
			[InlineData("Returns_name_from_Role")]
			public async Task Returns_name_from_Role(string roleName)
			{
				// arrange
				var role = new IdentityRole(roleName);

				// act
				var result = await RoleStore.GetRoleNameAsync(role);

				// assert
				Assert.Equal(role.Name, result);
				Assert.Equal(roleName, result);
			}
		}

		public class GetNormalizedRoleNameAsyncMethod : RoleStoreInMemoryTests
		{
			[Theory]
			[InlineData(null)]
			[InlineData("")]
			[InlineData("Returns_normalized_name_from_Role")]
			public async Task Returns_normalized_name_from_Role(string roleName)
			{
				// arrange
				var role = new IdentityRole(roleName);
				role.NormalizedName = roleName;

				// act
				var result = await RoleStore.GetNormalizedRoleNameAsync(role);

				// assert
				Assert.Equal(role.Name, result);
				Assert.Equal(roleName, result);
			}
		}

		public class FindByIdAsyncMethod : RoleStoreInMemoryTests
		{
			[Theory]
			[InlineData(null)]
			[InlineData("")]
			public async Task Find_with_blank_roleId_returns_null(string roleId)
			{
				// act
				var result = await RoleStore.FindByIdAsync(roleId);

				// assert
				Assert.Null(result);
			}
		}

		public class FindByNameAsyncMethod : RoleStoreInMemoryTests
		{
			[Theory]
			[InlineData(null)]
			[InlineData("")]
			public async Task Find_with_blank_normalisedName_returns_null(string normalisedName)
			{
				// act
				var result = await RoleStore.FindByNameAsync(normalisedName);

				// assert
				Assert.Null(result);
			}
		}

		public class SetRoleNameAsyncMethod : RoleStoreInMemoryTests
		{
			[Theory]
			[InlineData(null)]
			[InlineData("")]
			[InlineData("Sets_role_name_to_provided_value")]
			public async Task Sets_role_name_to_supplied_value(string roleName)
			{
				// arrange
				var role = new IdentityRole();

				// act
				await RoleStore.SetRoleNameAsync(role, roleName);

				// assert
				Assert.Equal(roleName, role.Name);
			}
		}

		public class SetNormalizedRoleNameAsyncMethod : RoleStoreInMemoryTests
		{
			[Theory]
			[InlineData(null)]
			[InlineData("")]
			[InlineData("normalised name value")]
			public async Task Sets_role_normalisedName_to_supplied_value(string normalisedName)
			{
				// arrange
				var role = new IdentityRole("Sets_role_normalisedName_to_supplied_value");

				// act
				await RoleStore.SetNormalizedRoleNameAsync(role, normalisedName);

				// assert
				Assert.Equal(normalisedName, role.NormalizedName);
			}
		}

		public class GetClaimsAsyncMethod : RoleStoreInMemoryTests
		{
			[Fact]
			public async Task Returns_empty_list_when_claims_on_role_not_set()
			{
				// arrange
				var role = new IdentityRole();

				// act
				var result = await RoleStore.GetClaimsAsync(role);

				// assert
				Assert.Empty(result);
			}

			[Fact]
			public async Task Returns_clist_of_claims_from_role()
			{
				// arrange
				var role = new IdentityRole();
				var claim1 = new IdentityClaim { ClaimType = "ClaimType1", ClaimValue = "some value" };
				var claim2 = new IdentityClaim { ClaimType = "ClaimType2", ClaimValue = "some other value" };
				role.Claims.Add(claim1);
				role.Claims.Add(claim2);


				// act
				var result = await RoleStore.GetClaimsAsync(role);

				// assert
				Assert.Equal(role.Claims.Count, result.Count);
				Assert.True(result.Single(c => c.Type == claim1.ClaimType && c.Value == claim1.ClaimValue) != null);
				Assert.True(result.Single(c => c.Type == claim2.ClaimType && c.Value == claim2.ClaimValue) != null);

			}
		}
	}
}
