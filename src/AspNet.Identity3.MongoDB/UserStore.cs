using Microsoft.AspNet.Identity;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace AspNet5.Identity.MongoDB
{
	public class UserStore<TUser, TRole, TKey> :
		IUserLoginStore<TUser>,
		IUserRoleStore<TUser>,
		IUserClaimStore<TUser>,
		IUserPasswordStore<TUser>,
		IUserSecurityStampStore<TUser>,
		IUserEmailStore<TUser>,
		IUserLockoutStore<TUser>,
		IUserPhoneNumberStore<TUser>,
		IQueryableUserStore<TUser>,
		IUserTwoFactorStore<TUser>
		where TUser : IdentityUser<TKey>
		where TRole : IdentityRole<TKey>
		where TKey : IEquatable<TKey>
	{
		#region Constructor and MongoDB Connections

		protected string _databaseName = "AspNetIdentity";
		protected string _collectionName = "AspNetUsers";
		protected IMongoClient _client;
		protected IMongoDatabase _database;
		protected IMongoCollection<TUser> _collection;

		public UserStore(string connectionString, string databaseName = null, string collectionName = null, IdentityErrorDescriber describer = null)
		{
			SetProperties(databaseName, collectionName, describer);
			SetDbConnection(connectionString);
		}

		public UserStore(IMongoClient client, string databaseName = null, string collectionName = null, IdentityErrorDescriber describer = null)
		{
			SetProperties(databaseName, collectionName, describer);
			SetDbConnection(client);
		}

		public UserStore(IMongoDatabase database, string collectionName = null, IdentityErrorDescriber describer = null)
		{
			SetProperties(database.DatabaseNamespace.DatabaseName, collectionName, describer);
			SetDbConnection(database);
		}

		public UserStore(IMongoCollection<TUser> collection, IdentityErrorDescriber describer = null)
		{
			_collection = collection;
			_database = collection.Database;
			_client = _database.Client;

			SetProperties(_database.DatabaseNamespace.DatabaseName, _collection.CollectionNamespace.CollectionName, describer);
		}

		protected void SetProperties(string databaseName, string collectionName, IdentityErrorDescriber describer)
		{
			if (!string.IsNullOrWhiteSpace(databaseName)) _databaseName = databaseName;
			if (!string.IsNullOrWhiteSpace(collectionName)) _collectionName = collectionName;
			ErrorDescriber = describer ?? new IdentityErrorDescriber();
		}

		/// <summary>
		/// IMPORTANT: ensure _databaseName and _collectionName are set (if needed) before calling this
		/// </summary>
		/// <param name="connectionString"></param>
		protected void SetDbConnection(string connectionString)
		{
			SetDbConnection(new MongoClient(connectionString));
		}

		/// <summary>
		/// IMPORTANT: ensure _databaseName and _collectionName are set (if needed) before calling this
		/// </summary>
		/// <param name="client"></param>
		protected void SetDbConnection(IMongoClient client)
		{
			SetDbConnection(client.GetDatabase(_databaseName));
		}

		/// <summary>
		/// IMPORTANT: ensure _collectionName is set (if needed) before calling this
		/// </summary>
		/// <param name="database"></param>
		protected void SetDbConnection(IMongoDatabase database)
		{
			_database = database;
			_client = _database.Client;
			_collection = _database.GetCollection<TUser>(_collectionName);
		}


		#endregion

		/// <summary>
		/// Used to generate public API error messages
		/// </summary>
		public IdentityErrorDescriber ErrorDescriber { get; set; }

		#region IUserStore<TUser> (base inteface for the other interfaces)
		/// <summary>
		/// Gets the user identifier for the specified <paramref name="user"/>, as an asynchronous operation.
		/// </summary>
		/// <param name="user">The user whose identifier should be retrieved.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the identifier for the specified <paramref name="user"/>.</returns>
		public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets the user name for the specified <paramref name="user"/>, as an asynchronous operation.
		/// </summary>
		/// <param name="user">The user whose name should be retrieved.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the name for the specified <paramref name="user"/>.</returns>
		public Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Sets the given <paramref name="userName" /> for the specified <paramref name="user"/>, as an asynchronous operation.
		/// </summary>
		/// <param name="user">The user whose name should be set.</param>
		/// <param name="userName">The user name to set.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
		public Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets the normalized user name for the specified <paramref name="user"/>, as an asynchronous operation.
		/// </summary>
		/// <param name="user">The user whose normalized name should be retrieved.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the normalized user name for the specified <paramref name="user"/>.</returns>
		public Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Sets the given normalized name for the specified <paramref name="user"/>, as an asynchronous operation.
		/// </summary>
		/// <param name="user">The user whose name should be set.</param>
		/// <param name="userName">The normalized name to set.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
		public Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Creates the specified <paramref name="user"/> in the user store, as an asynchronous operation.
		/// </summary>
		/// <param name="user">The user to create.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the creation operation.</returns>
		public Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Updates the specified <paramref name="user"/> in the user store, as an asynchronous operation.
		/// </summary>
		/// <param name="user">The user to update.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the update operation.</returns>
		public Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Deletes the specified <paramref name="user"/> from the user store, as an asynchronous operation.
		/// </summary>
		/// <param name="user">The user to delete.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the update operation.</returns>
		public Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Finds and returns a user, if any, who has the specified <paramref name="userId"/>.
		/// </summary>
		/// <param name="userId">The user ID to search for.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents the asynchronous operation, containing the user matching the specified <paramref name="userID"/> if it exists.
		/// </returns>
		public Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Finds and returns a user, if any, who has the specified normalized user name.
		/// </summary>
		/// <param name="normalizedUserName">The normalized user name to search for.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents the asynchronous operation, containing the user matching the specified <paramref name="userID"/> if it exists.
		/// </returns>
		public Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IUserLoginStore<TUser>

		/// <summary>
		/// Adds an external <see cref="UserLoginInfo"/> to the specified <paramref name="user"/>, as an asynchronous operation.
		/// </summary>
		/// <param name="user">The user to add the login to.</param>
		/// <param name="login">The external <see cref="UserLoginInfo"/> to add to the specified <paramref name="user"/>.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
		public Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Attempts to remove the provided login information from the specified <paramref name="user"/>, as an asynchronous operation.
		/// and returns a flag indicating whether the removal succeed or not.
		/// </summary>
		/// <param name="user">The user to remove the login information from.</param>
		/// <param name="loginProvider">The login provide whose information should be removed.</param>
		/// <param name="providerKey">The key given by the external login provider for the specified user.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>
		/// The <see cref="Task"/> that contains a flag the result of the asynchronous removing operation. The flag will be true if
		/// the login information was existed and removed, otherwise false.
		/// </returns>
		public Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Retrieves the associated logins for the specified <param ref="user"/>, as an asynchronous operation.
		/// </summary>
		/// <param name="user">The user whose associated logins to retrieve.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>
		/// The <see cref="Task"/> for the asynchronous operation, containing a list of <see cref="UserLoginInfo"/> for the specified <paramref name="user"/>, if any.
		/// </returns>
		public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Retrieves the user associated with the specified login provider and login provider key, as an asynchronous operation..
		/// </summary>
		/// <param name="loginProvider">The login provider who provided the <paramref name="providerKey"/>.</param>
		/// <param name="providerKey">The key provided by the <paramref name="loginProvider"/> to identify a user.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>
		/// The <see cref="Task"/> for the asynchronous operation, containing the user, if any which matched the specified login provider and key.
		/// </returns>
		public Task<TUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IUserRoleStore<TUser>

		/// <summary>
		/// Add a the specified <paramref name="user"/> to the named role, as an asynchronous operation.
		/// </summary>
		/// <param name="user">The user to add to the named role.</param>
		/// <param name="roleName">The name of the role to add the user to.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
		public Task AddToRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Add a the specified <paramref name="user"/> from the named role, as an asynchronous operation.
		/// </summary>
		/// <param name="user">The user to remove the named role from.</param>
		/// <param name="roleName">The name of the role to remove.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
		public Task RemoveFromRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets a list of role names the specified <paramref name="user"/> belongs to, as an asynchronous operation.
		/// </summary>
		/// <param name="user">The user whose role names to retrieve.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing a list of role names.</returns>
		public Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Returns a flag indicating whether the specified <paramref name="user"/> is a member of the give named role, as an asynchronous operation.
		/// </summary>
		/// <param name="user">The user whose role membership should be checked.</param>
		/// <param name="roleName">The name of the role to be checked.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents the asynchronous operation, containing a flag indicating whether the specified <see cref="user"/> is
		/// a member of the named role.
		/// </returns>
		public Task<bool> IsInRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Returns a list of Users who are members of the named role.
		/// </summary>
		/// <param name="roleName">The name of the role whose membership should be returned.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents the asynchronous operation, containing a list of users who are in the named role.
		/// </returns>
		public Task<IList<TUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IUserClaimStore<TUser>

		/// <summary>
		/// Gets a list of <see cref="Claim"/>s to be belonging to the specified <paramref name="user"/> as an asynchronous operation.
		/// </summary>
		/// <param name="user">The role whose claims to retrieve.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>
		/// A <see cref="Task{TResult}"/> that represents the result of the asynchronous query, a list of <see cref="Claim"/>s.
		/// </returns>
		public Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Add claims to a user as an asynchronous operation.
		/// </summary>
		/// <param name="user">The user to add the claim to.</param>
		/// <param name="claims">The collection of <see cref="Claim"/>s to add.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		public Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Replaces the given <paramref name="claim"/> on the specified <paramref name="user"/> with the <paramref name="newClaim"/>
		/// </summary>
		/// <param name="user">The user to replace the claim on.</param>
		/// <param name="claim">The claim to replace.</param>
		/// <param name="newClaim">The new claim to replace the existing <paramref name="claim"/> with.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		public Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Removes the specified <paramref name="claims"/> from the given <paramref name="user"/>.
		/// </summary>
		/// <param name="user">The user to remove the specified <paramref name="claims"/> from.</param>
		/// <param name="claims">A collection of <see cref="Claim"/>s to remove.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		public Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Returns a list of users who contain the specified <see cref="Claim"/>.
		/// </summary>
		/// <param name="claim">The claim to look for.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>
		/// A <see cref="Task{TResult}"/> that represents the result of the asynchronous query, a list of <typeparamref name="TUser"/> who
		/// contain the specified claim.
		/// </returns>
		public Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IUserPasswordStore<TUser>

		/// <summary>
		/// Sets the password hash for the specified <paramref name="user"/>, as an asynchronous operation.
		/// </summary>
		/// <param name="user">The user whose password hash to set.</param>
		/// <param name="passwordHash">The password hash to set.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
		public Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets the password hash for the specified <paramref name="user"/>, as an asynchronous operation.
		/// </summary>
		/// <param name="user">The user whose password hash to retrieve.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>The <see cref="Task"/> that represents the asynchronous operation, returning the password hash for the specified <paramref name="user"/>.</returns>
		public Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets a flag indicating whether the specified <paramref name="user"/> has a password, as an asynchronous operation.
		/// </summary>
		/// <param name="user">The user to return a flag for, indicating whether they have a password or not.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents the asynchronous operation, returning true if the specified <paramref name="user"/> has a password
		/// otherwise false.
		/// </returns>
		public Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IUserSecurityStampStore<TUser>

		/// <summary>
		/// Sets the provided security <paramref name="stamp"/> for the specified <paramref name="user"/>, as an asynchronous operation.
		/// </summary>
		/// <param name="user">The user whose security stamp should be set.</param>
		/// <param name="stamp">The security stamp to set.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
		public Task SetSecurityStampAsync(TUser user, string stamp, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Get the security stamp for the specified <paramref name="user" />, as an asynchronous operation.
		/// </summary>
		/// <param name="user">The user whose security stamp should be set.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the security stamp for the specified <paramref name="user"/>.</returns>
		public Task<string> GetSecurityStampAsync(TUser user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IUserEmailStore<TUser>

		/// <summary>
		/// Sets the <paramref name="email"/> address for a <paramref name="user"/>, as an asynchronous operation.
		/// </summary>
		/// <param name="user">The user whose email should be set.</param>
		/// <param name="email">The email to set.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		public Task SetEmailAsync(TUser user, string email, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets the email address for the specified <paramref name="user"/>, as an asynchronous operation.
		/// </summary>
		/// <param name="user">The user whose email should be returned.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>The task object containing the results of the asynchronous operation, the email address for the specified <paramref name="user"/>.</returns>
		public Task<string> GetEmailAsync(TUser user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets a flag indicating whether the email address for the specified <paramref name="user"/> has been verified, true if the email address is verified otherwise
		/// false, as an asynchronous operation.
		/// </summary>
		/// <param name="user">The user whose email confirmation status should be returned.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>
		/// The task object containing the results of the asynchronous operation, a flag indicating whether the email address for the specified <paramref name="user"/>
		/// has been confirmed or not.
		/// </returns>
		public Task<bool> GetEmailConfirmedAsync(TUser user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Sets the flag indicating whether the specified <paramref name="user"/>'s email address has been confirmed or not, as an asynchronous operation.
		/// </summary>
		/// <param name="user">The user whose email confirmation status should be set.</param>
		/// <param name="confirmed">A flag indicating if the email address has been confirmed, true if the address is confirmed otherwise false.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		public Task SetEmailConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets the user, if any, associated with the specified, normalized email address, as an asynchronous operation.
		/// </summary>
		/// <param name="normalizedEmail">The normalized email address to return the user for.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>
		/// The task object containing the results of the asynchronous lookup operation, the user if any associated with the specified normalized email address.
		/// </returns>
		public Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Returns the normalized email for the specified <paramref name="user"/>, as an asynchronous operation.
		/// </summary>
		/// <param name="user">The user whose email address to retrieve.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>
		/// The task object containing the results of the asynchronous lookup operation, the normalized email address if any associated with the specified user.
		/// </returns>
		public Task<string> GetNormalizedEmailAsync(TUser user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Sets the normalized email for the specified <paramref name="user"/>, as an asynchronous operation.
		/// </summary>
		/// <param name="user">The user whose email address to set.</param>
		/// <param name="normalizedEmail">The normalized email to set for the specified <paramref name="user"/>.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		public Task SetNormalizedEmailAsync(TUser user, string normalizedEmail, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IUserLockoutStore<TUser>

		/// <summary>
		/// Gets the last <see cref="DateTimeOffset"/> a user's last lockout expired, if any, as an asynchronous operation.
		/// Any time in the past should be indicates a user is not locked out.
		/// </summary>
		/// <param name="user">The user whose lockout date should be retrieved.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>
		/// A <see cref="Task{TResult}"/> that represents the result of the asynchronous query, a <see cref="DateTimeOffset"/> containing the last time
		/// a user's lockout expired, if any.
		/// </returns>
		public Task<DateTimeOffset?> GetLockoutEndDateAsync(TUser user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Locks out a user until the specified end date has passed, as an asynchronous operation. Setting a end date in the past immediately unlocks a user.
		/// </summary>
		/// <param name="user">The user whose lockout date should be set.</param>
		/// <param name="lockoutEnd">The <see cref="DateTimeOffset"/> after which the <paramref name="user"/>'s lockout should end.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
		public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Records that a failed access has occurred, incrementing the failed access count, as an asynchronous operation.
		/// </summary>
		/// <param name="user">The user whose cancellation count should be incremented.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the incremented failed access count.</returns>
		public Task<int> IncrementAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Resets a user's failed access count, as an asynchronous operation.
		/// </summary>
		/// <param name="user">The user whose failed access count should be reset.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
		/// <remarks>This is typically called after the account is successfully accessed.</remarks>
		public Task ResetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Retrieves the current failed access count for the specified <paramref name="user"/>, as an asynchronous operation..
		/// </summary>
		/// <param name="user">The user whose failed access count should be retrieved.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the failed access count.</returns>
		public Task<int> GetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Retrieves a flag indicating whether user lockout can enabled for the specified user, as an asynchronous operation.
		/// </summary>
		/// <param name="user">The user whose ability to be locked out should be returned.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents the asynchronous operation, true if a user can be locked out, otherwise false.
		/// </returns>
		public Task<bool> GetLockoutEnabledAsync(TUser user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Set the flag indicating if the specified <paramref name="user"/> can be locked out, as an asynchronous operation..
		/// </summary>
		/// <param name="user">The user whose ability to be locked out should be set.</param>
		/// <param name="enabled">A flag indicating if lock out can be enabled for the specified <paramref name="user"/>.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
		public Task SetLockoutEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IUserPhoneNumberStore<TUser>

		/// <summary>
		/// Sets the telephone number for the specified <paramref name="user"/>, as an asynchronous operation.
		/// </summary>
		/// <param name="user">The user whose telephone number should be set.</param>
		/// <param name="phoneNumber">The telephone number to set.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
		public Task SetPhoneNumberAsync(TUser user, string phoneNumber, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets the telephone number, if any, for the specified <paramref name="user"/>, as an asynchronous operation.
		/// </summary>
		/// <param name="user">The user whose telephone number should be retrieved.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the user's telephone number, if any.</returns>
		public Task<string> GetPhoneNumberAsync(TUser user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets a flag indicating whether the specified <paramref name="user"/>'s telephone number has been confirmed, as an asynchronous operation.
		/// </summary>
		/// <param name="user">The user to return a flag for, indicating whether their telephone number is confirmed.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents the asynchronous operation, returning true if the specified <paramref name="user"/> has a confirmed
		/// telephone number otherwise false.
		/// </returns>
		public Task<bool> GetPhoneNumberConfirmedAsync(TUser user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Sets a flag indicating if the specified <paramref name="user"/>'s phone number has been confirmed, as an asynchronous operation..
		/// </summary>
		/// <param name="user">The user whose telephone number confirmation status should be set.</param>
		/// <param name="confirmed">A flag indicating whether the user's telephone number has been confirmed.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
		public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IQueryableUserStore<TUser>

		/// <summary>
		/// Returns an <see cref="IQueryable{T}"/> collection of users.
		/// </summary>
		/// <value>An <see cref="IQueryable{T}"/> collection of users.</value>
		public IQueryable<TUser> Users
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		#endregion

		#region IUserTwoFactorStore<TUser>

		/// <summary>
		/// Sets a flag indicating whether the specified <paramref name="user "/>has two factor authentication enabled or not,
		/// as an asynchronous operation.
		/// </summary>
		/// <param name="user">The user whose two factor authentication enabled status should be set.</param>
		/// <param name="enabled">A flag indicating whether the specified <paramref name="user"/> has two factor authentication enabled.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
		public Task SetTwoFactorEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Returns a flag indicating whether the specified <paramref name="user "/>has two factor authentication enabled or not,
		/// as an asynchronous operation.
		/// </summary>
		/// <param name="user">The user whose two factor authentication enabled status should be set.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents the asynchronous operation, containing a flag indicating whether the specified 
		/// <paramref name="user "/>has two factor authentication enabled or not.
		/// </returns>
		public Task<bool> GetTwoFactorEnabledAsync(TUser user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IDisposable

		private bool _disposed = false; // To detect redundant calls


		public void Dispose()
		{
			_disposed = true;
		}

		/// <summary>
		/// Throws if disposed.
		/// </summary>
		/// <exception cref="System.ObjectDisposedException"></exception>
		protected void ThrowIfDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}
		}

		#endregion
	}
}
