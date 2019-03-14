using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;

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
                optionsBuilder.UseSqlServer("server=.;database=bstDB;Integrated Security=SSPI;user=sa;password=asd45214", null);
                //optionsBuilder.UseMySQL("server=localhost;database=bstdata;user=bst;password=asd45214", null);
            }
        }
        public DbSet<Protocol> protocols { get; set; }

    }
    public class Protocol
    {
        [Key]
        public Guid protocolID { get; set; }
        public string text { get; set; }
        public bool useDefaultAnat { get; set; }
        public bool useDefaultChannel { get; set; }

        public virtual ICollection<Subject> subjects { get; set; }
        //public virtual ICollection<Study> studies { get; set; }
    }
    public class Subject
    {
        [Key]
        public Guid id { get; set; }

        public virtual Protocol protocol { get; set; }
    }


}
