using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AspNet.Identity3.MongoDB;
using Microsoft.AspNet.Identity;
using MongoDB.Driver;

namespace AspNet5.Identity.MongoDB
{
	public class RoleStore<TRole> : 
		RoleStore<TRole, string>
		where TRole : IdentityRole<string>
	{
		public RoleStore(string connectionString, string databaseName = null, string collectionName = null) : base(connectionString, databaseName, collectionName) { }

		public RoleStore(IMongoClient client, string databaseName = null, string collectionName = null) : base(client, databaseName, collectionName) { }

		public RoleStore(IMongoDatabase database, string collectionName = null) : base(database, collectionName) { }

		public RoleStore(IMongoCollection<TRole> collection) : base(collection) { }
	}

	public class RoleStore<TRole, TKey> :
		IQueryableRoleStore<TRole>,
		IRoleClaimStore<TRole>
		where TRole : IdentityRole<TKey>
		where TKey : IEquatable<TKey>
	{
		#region Constructor and MongoDB Connections
	
		protected string _databaseName = DefaultNames.Database;
		protected string _collectionName = DefaultNames.RoleCollection;
		protected IMongoClient _client;
		protected IMongoDatabase _database;
		protected IMongoCollection<TRole> _collection;

		public RoleStore(string connectionString, string databaseName = null, string collectionName = null)
		{
			SetProperties(databaseName, collectionName);
			SetDbConnection(connectionString);
		}

		public RoleStore(IMongoClient client, string databaseName = null, string collectionName = null)
		{
			SetProperties(databaseName, collectionName);
			SetDbConnection(client);
		}

		public RoleStore(IMongoDatabase database, string collectionName = null)
		{
			SetProperties(database.DatabaseNamespace.DatabaseName, collectionName);
			SetDbConnection(database);
		}

		public RoleStore(IMongoCollection<TRole> collection)
		{
			_collection = collection;
			_database = collection.Database;
			_client = _database.Client;

			SetProperties(_database.DatabaseNamespace.DatabaseName, _collection.CollectionNamespace.CollectionName);
		}

		protected void SetProperties(string databaseName, string collectionName)
		{
			if (!string.IsNullOrWhiteSpace(databaseName)) _databaseName =  databaseName;
			if (!string.IsNullOrWhiteSpace(collectionName)) _collectionName = collectionName;
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
			_collection = _database.GetCollection<TRole>(_collectionName);
		}

		#endregion
		
		#region IRoleStore<TRole> (base interface for both IQueryableRoleStore<TRole> and IRoleClaimStore<TRole>)

		/// <summary>
		/// Creates a new role in a store as an asynchronous operation.
		/// </summary>
		/// <param name="role">The role to create in the store.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>A <see cref="Task{TResult}"/> that represents the <see cref="IdentityResult"/> of the asynchronous query.</returns>
		public virtual Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Updates a role in a store as an asynchronous operation.
		/// </summary>
		/// <param name="role">The role to update in the store.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>A <see cref="Task{TResult}"/> that represents the <see cref="IdentityResult"/> of the asynchronous query.</returns>
		public virtual Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Deletes a role from the store as an asynchronous operation.
		/// </summary>
		/// <param name="role">The role to delete from the store.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>A <see cref="Task{TResult}"/> that represents the <see cref="IdentityResult"/> of the asynchronous query.</returns>
		public virtual Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets the ID for a role from the store as an asynchronous operation.
		/// </summary>
		/// <param name="role">The role whose ID should be returned.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>A <see cref="Task{TResult}"/> that contains the ID of the role.</returns>
		public virtual Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets the name of a role from the store as an asynchronous operation.
		/// </summary>
		/// <param name="role">The role whose name should be returned.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>A <see cref="Task{TResult}"/> that contains the name of the role.</returns>
		public virtual Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Get a role's normalized name as an asynchronous operation.
		/// </summary>
		/// <param name="role">The role whose normalized name should be retrieved.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>A <see cref="Task{TResult}"/> that contains the name of the role.</returns>
		public virtual Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Finds the role who has the specified ID as an asynchronous operation.
		/// </summary>
		/// <param name="roleId">The role ID to look for.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>A <see cref="Task{TResult}"/> that result of the look up.</returns>
		public virtual Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Finds the role who has the specified normalized name as an asynchronous operation.
		/// </summary>
		/// <param name="normalizedRoleName">The normalized role name to look for.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>A <see cref="Task{TResult}"/> that result of the look up.</returns>
		public virtual Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Sets the name of a role in the store as an asynchronous operation.
		/// </summary>
		/// <param name="role">The role whose name should be set.</param>
		/// <param name="roleName">The name of the role.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
		public virtual Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Set a role's normalized name as an asynchronous operation.
		/// </summary>
		/// <param name="role">The role whose normalized name should be set.</param>
		/// <param name="normalizedName">The normalized name to set</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
		public virtual Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IQueryableRoleStore<TRole>

		/// <summary>
		/// Returns an <see cref="IQueryable{T}"/> collection of roles.
		/// </summary>
		/// <value>An <see cref="IQueryable{T}"/> collection of roles.</value>
		public virtual IQueryable<TRole> Roles
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		#endregion

		#region IRoleClaimStore<TRole>

		/// <summary>
		///  Gets a list of <see cref="Claim"/>s to be belonging to the specified <paramref name="role"/> as an asynchronous operation.
		/// </summary>
		/// <param name="role">The role whose claims to retrieve.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>
		/// A <see cref="Task{TResult}"/> that represents the result of the asynchronous query, a list of <see cref="Claim"/>s.
		/// </returns>
		public virtual Task<IList<Claim>> GetClaimsAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
		{
			ThrowIfDisposed();
			if (role == null) throw new ArgumentNullException("role");

			IList<Claim> result = role.Claims.Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToList();
			return Task.FromResult(result);
		}

		/// <summary>
		/// Add a new claim to a role as an asynchronous operation.
		/// </summary>
		/// <param name="role">The role to add a claim to.</param>
		/// <param name="claim">The <see cref="Claim"/> to add.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		public virtual Task AddClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = default(CancellationToken))
		{
			ThrowIfDisposed();
			if (role == null) throw new ArgumentNullException("role");
			if (claim == null) throw new ArgumentNullException("claim");
			
			// new claim for the role
			if (!role.Claims.Any(x => x.ClaimType == claim.Type))
			{
				role.Claims.Add(new IdentityClaim
				{
					ClaimType = claim.Type,
					ClaimValue = claim.Value
				});
				// update role claims in the database
				DoClaimUpdate(role.Id, role.Claims, cancellationToken);
			}
			// new value for existing claim on the role
			else if (role.Claims.Any(x => x.ClaimType == claim.Type && x.ClaimValue != claim.Value))
			{
				var c = role.Claims.Single(x => x.ClaimType == claim.Type);
				c.ClaimValue = claim.Value;

				// update role claims in the database
				DoClaimUpdate(role.Id, role.Claims, cancellationToken);
			}

			return Task.FromResult(0);
		}

		/// <summary>
		/// Remove a claim from a role as an asynchronous operation.
		/// </summary>
		/// <param name="role">The role to remove the claim from.</param>
		/// <param name="claim">The <see cref="Claim"/> to remove.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		public virtual async Task RemoveClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = default(CancellationToken))
		{
			ThrowIfDisposed();
			if (role == null) throw new ArgumentNullException("role");
			if (claim == null) throw new ArgumentNullException("claim");

			if (role.Claims.Any(x => x.ClaimType == claim.Type))
			{
				var c = role.Claims.Single(x => x.ClaimType == claim.Type);
				role.Claims.Remove(c);
				await DoClaimUpdate(role.Id, role.Claims, cancellationToken);
			}
		}

		protected virtual Task<UpdateResult> DoClaimUpdate(TKey roleId, ICollection<IdentityClaim> claims, CancellationToken cancellationToken)
		{
			// update role claims in the database
			var filter = Builders<TRole>.Filter.Eq(x => x.Id, roleId);
			var update = Builders<TRole>.Update.Set(x => x.Claims, claims);
			return _collection.UpdateOneAsync(filter, update, null, cancellationToken);
		}

		#endregion

		#region IDisposable

		private bool _disposed = false; // To detect redundant calls


		public virtual void Dispose()
		{
			_disposed = true;
		}

		/// <summary>
		/// Throws if disposed.
		/// </summary>
		/// <exception cref="System.ObjectDisposedException"></exception>
		protected virtual void ThrowIfDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}
		}

		#endregion
	}
}
