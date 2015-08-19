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
		public RoleStore(string connectionString, string databaseName = null, string collectionName = null, MongoCollectionSettings collectionSettings = null, IdentityErrorDescriber describer = null) : 
			base(connectionString, databaseName, collectionName, collectionSettings, describer) { }

		public RoleStore(IMongoClient client, string databaseName = null, string collectionName = null, MongoCollectionSettings collectionSettings = null, IdentityErrorDescriber describer = null) : 
			base(client, databaseName, collectionName, collectionSettings, describer) { }

		public RoleStore(IMongoDatabase database, string collectionName = null, MongoCollectionSettings collectionSettings = null, IdentityErrorDescriber describer = null) : 
			base(database, collectionName, collectionSettings, describer) { }

		public RoleStore(IMongoCollection<TRole> collection, IdentityErrorDescriber describer = null) : 
			base(collection, describer) { }
	}

	public class RoleStore<TRole, TKey> :
		IQueryableRoleStore<TRole>,
		IRoleClaimStore<TRole>
		where TRole : IdentityRole<TKey>
		where TKey : IEquatable<TKey>
	{
		#region Constructor and MongoDB Connections
	
		protected string _databaseName;
		protected string _collectionName;
		protected IMongoClient _client;
		protected IMongoDatabase _database;
		protected IMongoCollection<TRole> _collection;

		public RoleStore(string connectionString, string databaseName = null, string collectionName = null, MongoCollectionSettings collectionSettings = null, IdentityErrorDescriber describer = null)
		{
			if(string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentNullException("connectionString");

			SetProperties(databaseName, collectionName, describer);
			SetDbConnection(connectionString, collectionSettings);
		}

		public RoleStore(IMongoClient client, string databaseName = null, string collectionName = null, MongoCollectionSettings collectionSettings = null, IdentityErrorDescriber describer = null)
		{
			if(client == null) throw new ArgumentNullException("client");

			SetProperties(databaseName, collectionName, describer);
			SetDbConnection(client, collectionSettings);
		}

		public RoleStore(IMongoDatabase database, string collectionName = null, MongoCollectionSettings collectionSettings = null, IdentityErrorDescriber describer = null)
		{
			if(database == null) throw new ArgumentNullException("database");

			SetProperties(database.DatabaseNamespace.DatabaseName, collectionName, describer);
			SetDbConnection(database, collectionSettings);
		}

		public RoleStore(IMongoCollection<TRole> collection, IdentityErrorDescriber describer = null)
		{
			if(collection == null) throw new ArgumentNullException("collection");

			_collection = collection;
			_database = collection.Database;
			_client = _database.Client;

			SetProperties(_database.DatabaseNamespace.DatabaseName, _collection.CollectionNamespace.CollectionName, describer);
		}

		protected void SetProperties(string databaseName, string collectionName, IdentityErrorDescriber describer)
		{
			_databaseName = string.IsNullOrWhiteSpace(databaseName) ? DefaultSettings.DatabaseName : databaseName;
			_collectionName = string.IsNullOrWhiteSpace(collectionName) ? DefaultSettings.RoleCollectionName : collectionName;
			ErrorDescriber = describer ?? new IdentityErrorDescriber();
		}

		/// <summary>
		/// IMPORTANT: ensure _databaseName and _collectionName are set (if needed) before calling this
		/// </summary>
		/// <param name="connectionString"></param>
		protected void SetDbConnection(string connectionString, MongoCollectionSettings collectionSettings)
		{
			SetDbConnection(new MongoClient(connectionString), collectionSettings);
		}

		/// <summary>
		/// IMPORTANT: ensure _databaseName and _collectionName are set (if needed) before calling this
		/// </summary>
		/// <param name="client"></param>
		protected void SetDbConnection(IMongoClient client, MongoCollectionSettings collectionSettings)
		{
			SetDbConnection(client.GetDatabase(_databaseName), collectionSettings);
		}

		/// <summary>
		/// IMPORTANT: ensure _collectionName is set (if needed) before calling this
		/// </summary>
		/// <param name="database"></param>
		protected void SetDbConnection(IMongoDatabase database, MongoCollectionSettings collectionSettings)
		{
			_database = database;
			_client = _database.Client;
			collectionSettings = collectionSettings ?? DefaultSettings.CollectionSettings();

			_collection = _database.GetCollection<TRole>(_collectionName, collectionSettings);
		}

		#endregion

		/// <summary>
		/// Used to generate public API error messages
		/// </summary>
		public virtual IdentityErrorDescriber ErrorDescriber { get; set; }

		#region IRoleStore<TRole> (base interface for both IQueryableRoleStore<TRole> and IRoleClaimStore<TRole>)

		/// <summary>
		/// Creates a new role in a store as an asynchronous operation.
		/// </summary>
		/// <param name="role">The role to create in the store.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>A <see cref="Task{TResult}"/> that represents the <see cref="IdentityResult"/> of the asynchronous query.</returns>
		public virtual async Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
		{
			ThrowIfDisposed();
			if(role == null) throw new ArgumentNullException("role");
			if (await RoleDetailsAlreadyExists(role, cancellationToken)) return IdentityResult.Failed(ErrorDescriber.DuplicateRoleName(role.ToString()));

			try
			{
				await _collection.InsertOneAsync(role, cancellationToken);
			}
			catch(MongoWriteException)
			{
				return IdentityResult.Failed(ErrorDescriber.DuplicateRoleName(role.ToString()));
			}

			return IdentityResult.Success;
		}

		/// <summary>
		/// Updates a role in a store as an asynchronous operation.
		/// </summary>
		/// <param name="role">The role to update in the store.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>A <see cref="Task{TResult}"/> that represents the <see cref="IdentityResult"/> of the asynchronous query.</returns>
		public virtual async Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
		{
			ThrowIfDisposed();
			if(role == null) throw new ArgumentNullException("role");
			if(await RoleDetailsAlreadyExists(role, cancellationToken)) return IdentityResult.Failed(ErrorDescriber.DuplicateRoleName(role.ToString()));


			try
			{
				var filter = Builders<TRole>.Filter.Eq(x => x.Id, role.Id);
				await _collection.ReplaceOneAsync(filter, role, null, cancellationToken);
			}
			catch(MongoWriteException)
			{
				return IdentityResult.Failed(ErrorDescriber.DuplicateRoleName(role.ToString()));
			}

			return IdentityResult.Success;
		}

		/// <summary>
		/// Deletes a role from the store as an asynchronous operation.
		/// </summary>
		/// <param name="role">The role to delete from the store.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>A <see cref="Task{TResult}"/> that represents the <see cref="IdentityResult"/> of the asynchronous query.</returns>
		public virtual Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets the ID for a role from the store as an asynchronous operation.
		/// </summary>
		/// <param name="role">The role whose ID should be returned.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>A <see cref="Task{TResult}"/> that contains the ID of the role.</returns>
		public virtual Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets the name of a role from the store as an asynchronous operation.
		/// </summary>
		/// <param name="role">The role whose name should be returned.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>A <see cref="Task{TResult}"/> that contains the name of the role.</returns>
		public virtual Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Get a role's normalized name as an asynchronous operation.
		/// </summary>
		/// <param name="role">The role whose normalized name should be retrieved.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>A <see cref="Task{TResult}"/> that contains the name of the role.</returns>
		public virtual Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Finds the role who has the specified ID as an asynchronous operation.
		/// </summary>
		/// <param name="roleId">The role ID to look for.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>A <see cref="Task{TResult}"/> that result of the look up.</returns>
		public virtual Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken = default(CancellationToken))
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Finds the role who has the specified normalized name as an asynchronous operation.
		/// </summary>
		/// <param name="normalizedRoleName">The normalized role name to look for.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
		/// <returns>A <see cref="Task{TResult}"/> that result of the look up.</returns>
		public virtual Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken = default(CancellationToken))
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
		public virtual Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken = default(CancellationToken))
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
		public virtual Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken = default(CancellationToken))
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

		protected virtual Task<UpdateResult> DoClaimUpdate(TKey roleId, ICollection<IdentityClaim> claims, CancellationToken cancellationToken = default(CancellationToken))
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

		#region PROTECTED HELPER METHODS

		/// <summary>
		/// Role names are distinct, and should never have two roles with the same name
		/// </summary>
		/// <remarks>
		/// Can override to have different "distinct role details" implementation if necessary.
		/// </remarks>
		/// <param name="role"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		protected virtual async Task<bool> RoleDetailsAlreadyExists(TRole role, CancellationToken cancellationToken = default(CancellationToken))
		{
			// if the result does exist, make sure its not for the same role object (ie same name, but different Ids)
			var fBuilder = Builders<TRole>.Filter;
			var filter = fBuilder.Ne(x => x.Id, role.Id) & fBuilder.Eq(x => x.Name, role.Name);

			var result = await _collection.Find(filter).FirstOrDefaultAsync();
			return result != null;
		}

		#endregion
	}
}
