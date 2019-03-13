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

        public class Protocol
        {
            [Key]
            public Guid ProtocolID { get; set; }

            public string Comment { get; set; }

            public int iStudy { get; set; }

            public bool UseDefaultAnat { get; set; }

            public bool UseDefaultChannel { get; set; }

            public boo isLocked { get; set; }

        }
    }
}
