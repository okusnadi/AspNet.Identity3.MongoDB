﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace AspNet.Identity3.MongoDB
{
	public class IdentityUser : IdentityUser<string>
	{
		public IdentityUser() : base()
		{
			Id = Guid.NewGuid().ToString();
		}

		public IdentityUser(string userName) : this()
		{
			UserName = userName;
		}
	}

	public class IdentityUser<TKey> where TKey : IEquatable<TKey>
	{
		public IdentityUser() { }

		public IdentityUser(string userName) : this()
		{
			UserName = userName;
		}

		public virtual TKey Id { get; set; }
		public virtual string UserName { get; set; }
		public virtual string NormalizedUserName { get; set; }

		/// <summary>
		/// Email
		/// </summary>
		public virtual string Email { get; set; }
		public virtual string NormalizedEmail { get; set; }

		/// <summary>
		/// True if the email is confirmed, default is false
		/// </summary>
		public virtual bool EmailConfirmed { get; set; }

		/// <summary>
		/// The salted/hashed form of the user password
		/// </summary>
		public virtual string PasswordHash { get; set; }

		/// <summary>
		/// A random value that should change whenever a user's credentials change (ie, password changed, login removed)
		/// </summary>
		public virtual string SecurityStamp { get; set; }

		/// <summary>
		/// PhoneNumber for the user
		/// </summary>
		public virtual string PhoneNumber { get; set; }

		/// <summary>
		/// True if the phone number is confirmed, default is false
		/// </summary>
		public virtual bool PhoneNumberConfirmed { get; set; }

		/// <summary>
		/// Is two factor enabled for the user
		/// </summary>
		public virtual bool TwoFactorEnabled { get; set; }

		/// <summary>
		/// DateTime in UTC when lockout ends, any time in the past is considered not locked out.
		/// </summary>
		public virtual DateTimeOffset? LockoutEnd { get; set; }

		/// <summary>
		/// Is lockout enabled for this user
		/// </summary>
		public virtual bool LockoutEnabled { get; set; }

		/// <summary>
		/// Used to record failures for the purposes of lockout
		/// </summary>
		public virtual int AccessFailedCount { get; set; }

		/// <summary>
		/// Navigation property for users in the role
		/// </summary>
		public virtual IList<IdentityRole<TKey>> Roles
		{
			get { return _roles; }
			set { _roles = value ?? new List<IdentityRole<TKey>>(); }
		} 
		private IList<IdentityRole<TKey>> _roles = new List<IdentityRole<TKey>>();

		/// <summary>
		/// Navigation property for users claims
		/// </summary>
		public virtual IList<IdentityClaim> Claims
		{
			get { return _claims; }
			set { _claims = value ?? new List<IdentityClaim>(); }
		}
		private IList<IdentityClaim> _claims = new List<IdentityClaim>();

		/// <summary>
		/// Get a list of all user's claims combined with claims from role
		/// </summary>
		public virtual IList<IdentityClaim> AllClaims
		{ 
			get
			{
				// as Claims and Roles are virtual and could be overridden with an implementation that allows nulls
				//	- make sure they aren't null just in case
				var clms = Claims ?? new List<IdentityClaim>();
				var rls = Roles ??  new List<IdentityRole<TKey>>();

				return clms.Concat(rls.Where(r => r.Claims != null).SelectMany(r => r.Claims)).Distinct().ToList();
			}
		}

		/// <summary>
		/// Navigation property for users logins
		/// </summary>
		public virtual IList<IdentityUserLogin> Logins
		{
			get { return _logins; }
			set { _logins = value ?? new List<IdentityUserLogin>(); }
		}
		private IList<IdentityUserLogin> _logins = new List<IdentityUserLogin>();

		/// <summary>
		/// Returns a friendly name
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return UserName;
		}
	}
}
