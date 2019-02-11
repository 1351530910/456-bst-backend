using System;
using Microsoft.EntityFrameworkCore;

namespace bst.Model
{
    public class UserDB:DbContext
    {
        public UserDB():base()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseMySQL("server=localhost;database=bstusers;user=bst;password=asd45214");
        }


        public DbSet<User> users { get; set; }
        public DbSet<Lab> labs { get; set; }

    }
}
