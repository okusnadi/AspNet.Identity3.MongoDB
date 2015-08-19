using Xunit;
using AspNet5.Identity.MongoDB;

namespace AspNet.Identity3.MongoDB.Tests
{
	public class IdentityUserTests
	{
		#region Common/Shared

		protected IdentityUser User;

		protected IdentityRole Role1;
		protected IdentityRole Role2;

		protected IdentityClaim Claim1;
		protected IdentityClaim Claim2;
		protected IdentityClaim Claim3;
		protected IdentityClaim Claim4;
		protected IdentityClaim Claim1Alt;

		public IdentityUserTests()
		{
			User = new IdentityUser("John Doe");

			Role1 = new IdentityRole { Name = "Role1" };
			Role2 = new IdentityRole { Name = "Role2" };

			Claim1 = new IdentityClaim { ClaimType = "Claim1", ClaimValue = "Some value" };
			Claim2 = new IdentityClaim { ClaimType = "Claim2", ClaimValue = "Some other value" };
			Claim3 = new IdentityClaim { ClaimType = "Claim3", ClaimValue = "Yet another value" };
			Claim4 = new IdentityClaim { ClaimType = "Claim4", ClaimValue = "Many many claims" };


			Claim1Alt = new IdentityClaim { ClaimType = "Claim1", ClaimValue = "Some alternate value" };

		}

		#endregion
		
		public class AllClaimsMethod : IdentityUserTests
		{
			[Fact]
			public void Returns_empty_list_when_initialised()
			{
				// assert
				Assert.NotNull(User.AllClaims);
				Assert.Empty(User.AllClaims);
			}

			[Fact]
			public void Returns_all_from_users_claims()
			{
				// arrange
				User.Claims.Add(Claim1);
				User.Claims.Add(Claim2);

				// assert
				Assert.Contains(Claim1, User.AllClaims);
				Assert.Contains(Claim2, User.AllClaims);
				Assert.Equal(2, User.AllClaims.Count);
			}

			[Fact]
			public void Returns_distinct_list_of_all_claims_from_roles()
			{
				// arrange
				Role1.Claims.Add(Claim1);
				Role1.Claims.Add(Claim2);
				Role1.Claims.Add(Claim4);

				Role2.Claims.Add(Claim1Alt);
				Role2.Claims.Add(Claim2);
				Role2.Claims.Add(Claim4);

				User.Roles.Add(Role1);
				User.Roles.Add(Role2);

				// assert
				Assert.Contains(Claim1, User.AllClaims);
				Assert.Contains(Claim1Alt, User.AllClaims);
				Assert.Contains(Claim2, User.AllClaims);
				Assert.Contains(Claim4, User.AllClaims);
				Assert.DoesNotContain(Claim3, User.AllClaims);

				Assert.Equal(4, User.AllClaims.Count);
			}

			[Fact]
			public void Returns_distinct_list_of_all_from_user_claims_and_roles_claims()
			{
				// arrange
				Role1.Claims.Add(Claim1);
				Role1.Claims.Add(Claim2);
				Role1.Claims.Add(Claim4);
				
				Role2.Claims.Add(Claim2);
				Role2.Claims.Add(Claim4);

				User.Roles.Add(Role1);
				User.Roles.Add(Role2);
				User.Claims.Add(Claim1);
				User.Claims.Add(Claim1Alt);

				// assert
				Assert.Contains(Claim1, User.AllClaims);
				Assert.Contains(Claim1Alt, User.AllClaims);
				Assert.Contains(Claim2, User.AllClaims);
				Assert.Contains(Claim4, User.AllClaims);
				Assert.DoesNotContain(Claim3, User.AllClaims);

				Assert.Equal(4, User.AllClaims.Count);
			}
		}
	}
}
