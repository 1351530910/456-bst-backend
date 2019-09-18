# 456-bst-backend setup instruction for development

prerequisites software:
1.  Visual Studio 2019 (any edition, either mac or windows)
2.  .Net core sdk : https://dotnet.microsoft.com/download/dotnet-core/2.2
3.  Mysql or MSSQL(any edition) 
    https://www.microsoft.com/en-ca/sql-server/sql-server-downloads  
    https://www.mysql.com/downloads/


Hosting/publishing:
https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/?view=aspnetcore-2.2

development setup steps:
0. unstall the prerequisites and configure them to work in the environment
1. clone the repository
2. open '456.sln' file using visual studio
3. under Project-> bst Properties->Build , check 'Define DEBUG constant' if building for developement environment, uncheck for production environment
4. under Project-> bst Properties->Build , modify 'Launch' to 'Executable'
5. in Program.cs modify the app urls to match the target ports.
6. in ConnectionString.cs update the connection string to a database user who has create/drop database permissions. 
    (no need to create any database in advance since the software will generate a database it uses)
7. in Startup.cs, uncomment 'usercontext.Database.EnsureDeleted();' if need to delete an old database with the same name
8. run

