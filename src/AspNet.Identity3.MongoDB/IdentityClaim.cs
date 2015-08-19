using System;

namespace AspNet5.Identity.MongoDB
{
	/// <summary>
	/// EntityType that represents one specific claim
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	public class IdentityClaim
	{
		/// <summary>
		///     Claim type
		/// </summary>
		public virtual string ClaimType { get; set; }

		/// <summary>
		///     Claim value
		/// </summary>
		public virtual string ClaimValue { get; set; }
	}
}
