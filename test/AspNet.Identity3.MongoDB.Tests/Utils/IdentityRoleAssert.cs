using AspNet5.Identity.MongoDB;
using Xunit;

namespace AspNet.Identity3.MongoDB.Tests
{
	public static class IdentityRoleAssert
	{
		public static void Equal(IdentityRole expected, IdentityRole actual)
		{
			Assert.True((expected == null && actual == null) || (expected != null && actual != null));

			Assert.Equal(expected.Id, actual.Id);
			Assert.Equal(expected.Name, actual.Name);
			Assert.Equal(expected.NormalizedName, actual.NormalizedName);

			IdentityClaimAssert.Equal(expected.Claims, actual.Claims);
		}
	}
}