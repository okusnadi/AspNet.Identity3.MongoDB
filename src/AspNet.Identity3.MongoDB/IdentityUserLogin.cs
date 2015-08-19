using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNet5.Identity.MongoDB
{
	/// <summary>
	///     Entity type for a user's login (i.e. facebook, google)
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	public class IdentityUserLogin<TKey> where TKey : IEquatable<TKey>
	{
		/// <summary>
		///     User Id for the user who owns this login
		/// </summary>
		public virtual TKey UserId { get; set; }

		/// <summary>
		///     The login provider for the login (i.e. facebook, google)
		/// </summary>
		public virtual string LoginProvider { get; set; }

		/// <summary>
		///     Key representing the login for the provider
		/// </summary>
		public virtual string ProviderKey { get; set; }

		/// <summary>
		///     Display name for the login
		/// </summary>
		public virtual string ProviderDisplayName { get; set; }
	}
}
