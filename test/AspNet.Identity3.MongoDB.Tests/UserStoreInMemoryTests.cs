using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using Xunit;

namespace AspNet.Identity3.MongoDB.Tests
{
	public class UserStoreInMemoryTests
	{
		private readonly UserStore<IdentityUser, IdentityRole> _userStore;

		public UserStoreInMemoryTests()
		{
			_userStore = new UserStore<IdentityUser, IdentityRole>("mongodb://localhost:27017");
		}

		public class Misc : UserStoreInMemoryTests
		{
			[Fact]
			public void Constructors_throw_ArgumentNullException_with_null()
			{
				Assert.Throws<ArgumentNullException>("connectionString", () => new UserStore<IdentityUser, IdentityRole>((string)null));
				Assert.Throws<ArgumentNullException>("connectionString", () => new UserStore<IdentityUser, IdentityRole>(""));
				Assert.Throws<ArgumentNullException>("connectionString", () => new UserStore<IdentityUser, IdentityRole>(String.Empty));

				Assert.Throws<ArgumentNullException>("client", () => new UserStore<IdentityUser, IdentityRole>((IMongoClient)null));
				Assert.Throws<ArgumentNullException>("database", () => new UserStore<IdentityUser, IdentityRole>((IMongoDatabase)null));
				Assert.Throws<ArgumentNullException>("collection", () => new UserStore<IdentityUser, IdentityRole>((IMongoCollection<IdentityUser>)null));
			}

			[Fact]
			public async Task Methods_throw_ObjectDisposedException_when_disposed()
			{
				_userStore.Dispose();
				await Assert.ThrowsAsync<ObjectDisposedException>(async () => await _userStore.GetUserIdAsync(null));
				await Assert.ThrowsAsync<ObjectDisposedException>(async () => await _userStore.GetUserNameAsync(null));
				await Assert.ThrowsAsync<ObjectDisposedException>(async () => await _userStore.SetUserNameAsync(null, null));
				await Assert.ThrowsAsync<ObjectDisposedException>(async () => await _userStore.GetNormalizedUserNameAsync(null));
				await Assert.ThrowsAsync<ObjectDisposedException>(async () => await _userStore.SetNormalizedUserNameAsync(null, null));

				await Assert.ThrowsAsync<ObjectDisposedException>(async () => await _userStore.FindByIdAsync(null));
				await Assert.ThrowsAsync<ObjectDisposedException>(async () => await _userStore.FindByNameAsync(null));
				await Assert.ThrowsAsync<ObjectDisposedException>(async () => await _userStore.CreateAsync(null));
				await Assert.ThrowsAsync<ObjectDisposedException>(async () => await _userStore.UpdateAsync(null));
				await Assert.ThrowsAsync<ObjectDisposedException>(async () => await _userStore.DeleteAsync(null));

				await Assert.ThrowsAsync<ObjectDisposedException>(async () => await _userStore.GetClaimsAsync(null));

			}

			[Fact]
			public async Task Methods_throw_ArgumentNullException_when_null_arguments()
			{
				await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await _userStore.GetUserIdAsync(null));
				await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await _userStore.GetUserNameAsync(null));
				await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await _userStore.SetUserNameAsync(null, null));
				await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await _userStore.GetNormalizedUserNameAsync(null));
				await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await _userStore.SetNormalizedUserNameAsync(null, null));
				
				await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await _userStore.CreateAsync(null));
				await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await _userStore.UpdateAsync(null));
				await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await _userStore.DeleteAsync(null));

				await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await _userStore.GetClaimsAsync(null));
			}
		}

		public class GetUserIdAsyncMethod : UserStoreInMemoryTests
		{
			[Fact]
			public async Task Returns_Id_from_User_as_string()
			{
				// arrange
				var user = new IdentityUser("Returns_Id_from_User_as_string");

				// act
				var result = await _userStore.GetUserIdAsync(user);

				// assert
				Assert.Equal(user.Id, result);
			}

			[Fact]
			public async Task Null_id_on_user_returns_Null()
			{
				// arrange
				var user = new IdentityUser("Null_id_on_user_returns_Null");
				user.Id = null;

				// act
				var result = await _userStore.GetUserIdAsync(user);

				// assert
				Assert.Null(result);
			}
		}

		public class GetUserNameAsyncMethod : UserStoreInMemoryTests
		{
			[Theory]
			[InlineData(null)]
			[InlineData("")]
			[InlineData("Returns_UserName_from_user")]
			public async Task Returns_UserName_from_user(string userName)
			{
				// arrange
				var user = new IdentityUser(userName);

				// act
				var result = await _userStore.GetUserNameAsync(user);

				// assert
				Assert.Equal(user.UserName, result);
				Assert.Equal(userName, result);
			}
		}

		public class SetUserNameAsyncMethod : UserStoreInMemoryTests
		{
			[Theory]
			[InlineData(null)]
			[InlineData("")]
			[InlineData("Sets_UserName_to_supplied_value")]
			public async Task Sets_UserName_to_supplied_value(string userName)
			{
				// arrange
				var user = new IdentityUser();

				// act
				await _userStore.SetUserNameAsync(user, userName);

				// assert
				Assert.Equal(userName, user.UserName);
			}
		}
		
		public class GetNormalizedUserNameAsyncMethod : UserStoreInMemoryTests
		{
			[Theory]
			[InlineData(null)]
			[InlineData("")]
			[InlineData("Returns_NormalizedUserName_from_user")]
			public async Task Returns_NormalizedUserName_from_user(string normalizedUuserName)
			{
				// arrange
				var user = new IdentityUser();
				user.NormalizedUserName = normalizedUuserName;

				// act
				var result = await _userStore.GetNormalizedUserNameAsync(user);

				// assert
				Assert.Equal(user.NormalizedUserName, result);
				Assert.Equal(normalizedUuserName, result);
			}
		}

		public class SetNormalizedUserNameAsyncMethod : UserStoreInMemoryTests
		{
			[Theory]
			[InlineData(null)]
			[InlineData("")]
			[InlineData("Sets_UserName_to_supplied_value")]
			public async Task Sets_UserName_to_supplied_value(string normalizedUuserName)
			{
				// arrange
				var user = new IdentityUser();

				// act
				await _userStore.SetNormalizedUserNameAsync(user, normalizedUuserName);

				// assert
				Assert.Equal(normalizedUuserName, user.NormalizedUserName);
			}
		}

	}
}
