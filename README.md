# AspNet.Identity3.MongoDB
An implementation for MongoDB.Driver (>= v2.0.0) with ASP.NET 5 Identity (>= v3) framework


## Default details
AspNet.Identity3.MongoDB follows the EntityFramework naming defaults where possible.

The default Mongo details are:

* Database Name: AspNetIdentity
* Role Collection Name: AspNetRoles
* User Collection Name: AspNetUsers
* Mongo Collection Settings: WriteConcern.WMajority


## WARNING
RoleStore.Roles and UserStore.Users are functions to return a IQueryable of roles/users.
However MongoDb have not yet implemented any AsQueryable functionality in MongoDB.Driver yet.
- <https://jira.mongodb.org/browse/CSHARP-935>
- <http://stackoverflow.com/questions/29124995/is-asqueryable-method-departed-in-new-mongodb-c-sharp-driver-2-0rc>

At the moment I have implemented the IQueryable from a ToList() the role/user collections.
This will not perform well.
Highly recommend you don't use it unless you are very very sure its a very small collection.