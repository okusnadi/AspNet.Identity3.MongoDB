using Xunit;

namespace AspNet.Identity3.MongoDB.Tests
{
	public class IdentityRoleTests
	{
		#region Common/Shared

		protected IdentityRole Role;

		protected IdentityClaim Claim1;
		protected IdentityClaim Claim2;
		protected IdentityClaim Claim3;
		protected IdentityClaim Claim4;
		protected IdentityClaim Claim1Alt;

		public IdentityRoleTests()
		{
			Role = new IdentityRole { Name = "Role" };

			Claim1 = new IdentityClaim { ClaimType = "Claim1", ClaimValue = "Some value" };
			Claim2 = new IdentityClaim { ClaimType = "Claim2", ClaimValue = "Some other value" };
			Claim3 = new IdentityClaim { ClaimType = "Claim3", ClaimValue = "Yet another value" };
			Claim4 = new IdentityClaim { ClaimType = "Claim4", ClaimValue = "Many many claims" };

			Claim1Alt = new IdentityClaim { ClaimType = "Claim1", ClaimValue = "Some alternate value" };

		}

		#endregion

		public class ClaimsProperty : IdentityRoleTests
		{
			[Fact]
			public void When_try_to_set_claims_to_null_is_actually_set_to_empty_list()
			{
				// arrange
				Role.Claims.Add(Claim1);

				// act
				Role.Claims = null;

				// assert
				Assert.NotNull(Role.Claims);
				Assert.Empty(Role.Claims);
			}
		}
	}
}
