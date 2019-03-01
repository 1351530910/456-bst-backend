using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace bst.Model
{
    public class BstDB:DbContext
    {
        public BstDB()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            if (optionsBuilder != null)
            {
                optionsBuilder.UseMySQL("server=localhost;database=bstdata;user=bst;password=asd45214", null);
            }
        }
    }
}
