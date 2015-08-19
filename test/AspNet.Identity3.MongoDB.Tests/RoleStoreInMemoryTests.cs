using System;
using System.Threading.Tasks;
using AspNet5.Identity.MongoDB;
using MongoDB.Driver;
using Xunit;

namespace AspNet.Identity3.MongoDB.Tests
{
	public class RoleStoreInMemoryTests
	{
		private RoleStore<IdentityRole> _roleStore;

		public RoleStoreInMemoryTests()
		{
			_roleStore = new RoleStore<IdentityRole>("mongodb://localhost:27017");
		}

		[Fact]
		public async Task RoleStoreMethodsThrowWhenDisposedTest()
		{
			_roleStore.Dispose();
			await Assert.ThrowsAsync<ObjectDisposedException>(async () => await _roleStore.FindByIdAsync(null));
			await Assert.ThrowsAsync<ObjectDisposedException>(async () => await _roleStore.FindByNameAsync(null));
			await Assert.ThrowsAsync<ObjectDisposedException>(async () => await _roleStore.GetRoleIdAsync(null));
			await Assert.ThrowsAsync<ObjectDisposedException>(async () => await _roleStore.GetRoleNameAsync(null));
			await Assert.ThrowsAsync<ObjectDisposedException>(async () => await _roleStore.SetRoleNameAsync(null, null));
			await Assert.ThrowsAsync<ObjectDisposedException>(async () => await _roleStore.CreateAsync(null));
			await Assert.ThrowsAsync<ObjectDisposedException>(async () => await _roleStore.UpdateAsync(null));
			await Assert.ThrowsAsync<ObjectDisposedException>(async () => await _roleStore.DeleteAsync(null));
		}

		[Fact]
		public void RoleStoreConstructorNullCheckTest()
		{
			Assert.Throws<ArgumentNullException>("connectionString", () => new RoleStore<IdentityRole>(""));
			Assert.Throws<ArgumentNullException>("client", () => new RoleStore<IdentityRole>((IMongoClient)null));
			Assert.Throws<ArgumentNullException>("database", () => new RoleStore<IdentityRole>((IMongoDatabase)null));
			Assert.Throws<ArgumentNullException>("collection", () => new RoleStore<IdentityRole>((IMongoCollection<IdentityRole>)null));
		}

		[Fact]
		public async Task RoleStorePublicNullCheckTest()
		{
			await Assert.ThrowsAsync<ArgumentNullException>("role", async () => await _roleStore.GetRoleIdAsync(null));
			await Assert.ThrowsAsync<ArgumentNullException>("role", async () => await _roleStore.GetRoleNameAsync(null));
			await Assert.ThrowsAsync<ArgumentNullException>("role", async () => await _roleStore.SetRoleNameAsync(null, null));
			await Assert.ThrowsAsync<ArgumentNullException>("role", async () => await _roleStore.CreateAsync(null));
			await Assert.ThrowsAsync<ArgumentNullException>("role", async () => await _roleStore.UpdateAsync(null));
			await Assert.ThrowsAsync<ArgumentNullException>("role", async () => await _roleStore.DeleteAsync(null));
		}
	}
}
