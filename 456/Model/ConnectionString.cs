using System;

namespace bst.Model
{
    public class ConnectionString
    {
        public static string servertype = "MYSQL";
        public static string connectionstring = "server=localhost;database=bstusers;user=bst;password=asd45214";
        static ConnectionString()
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").Equals("Development"))
            {
                servertype = "MSSQL";
                connectionstring = "server=localhost;database=bstusers;user=sa;password=asd45214..";
            }
        }
    }
}
