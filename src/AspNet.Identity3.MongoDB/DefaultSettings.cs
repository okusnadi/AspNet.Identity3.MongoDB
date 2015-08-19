using MongoDB.Driver;

namespace AspNet.Identity3.MongoDB
{
	public static class DefaultSettings
	{
		public const string DatabaseName = "AspNetIdentity";
		public const string RoleCollectionName = "AspNetRoles";
		public const string UserCollectionName = "AspNetUsers";


		public static MongoCollectionSettings CollectionSettings()
		{
			var collectionSettings = new MongoCollectionSettings();
			collectionSettings.WriteConcern = WriteConcern.WMajority;

			return collectionSettings;
		}
	}
}
