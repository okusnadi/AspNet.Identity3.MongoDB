using System;
using System.Collections.Generic;

namespace AspNet5.Identity.MongoDB
{
	/// <summary>
	/// Represents a Role entity
	/// </summary>
	public class IdentityRole : IdentityRole<string>
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public IdentityRole()
		{
			Id = Guid.NewGuid().ToString();
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="roleName"></param>
		public IdentityRole(string roleName) : this()
		{
			Name = roleName;
		}
	}

	/// <summary>
	/// Represents a Role entity
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	public class IdentityRole<TKey> where TKey : IEquatable<TKey>
	{
		public IdentityRole() { }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="roleName"></param>
		public IdentityRole(string roleName) : this()
		{
			Name = roleName;
		}

		/// <summary>
		/// Role id
		/// </summary>
		public virtual TKey Id { get; set; }

		/// <summary>
		/// Role name
		/// </summary>
		public virtual string Name { get; set; }
		
		public virtual string NormalizedName { get; set; }

		/// <summary>
		/// Navigation property for claims in the role
		/// </summary>
		public virtual IList<IdentityClaim> Claims { get; set; } = new List<IdentityClaim>();

		/// <summary>
		/// Returns a friendly name
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Name;
		}
	}
}
