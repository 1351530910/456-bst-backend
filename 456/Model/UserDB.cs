using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace bst.Model
{
    public class UserDB:DbContext
    {
        public UserDB():base()
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>()
                .HasIndex(x => x.email)
                .IsUnique();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            if (optionsBuilder != null)
            {
                optionsBuilder.UseSqlServer("server=.;database=bstusers;Integrated Security=SSPI;user=sa;password=asd45214", null);
                //optionsBuilder.UseMySQL("server=localhost;database=bstusers;user=bst;password=asd45214", null);
            }
        }

        public DbSet<User> users { get; set; }
        public DbSet<ParticipateProject> participateProjects { get; set; }
        public DbSet<Role> roles { get; set; }
        public DbSet<Group> group { get; set; }
        public DbSet<Project> projects { get; set; }
        public DbSet<Invitation> invitations { get; set; }
    }
    public partial class User
    {
        [Key]
        public Guid id { get; set; }
        [EmailAddress]
        public string email { get; set; }
        [MinLength(8), MaxLength(15), Required]
        public string password { get; set; }
        [MaxLength(30)]
        public string FirstName { get; set; }
        [MaxLength(30)]
        public string LastName { get; set; }
    }

    public partial class ParticipateProject
    {
        [Key]
        public Guid id { get; set; }
        [Required]
        public virtual User user { get; set; }
        [Required]
        public virtual Project project { get; set; }
        [Required]
        public int priviledge { get; set; }
    }

    public partial class Role
    {
        [Key]
        public Guid id { get; set; }
        [Required]
        public virtual User user { get; set; }
        [Required]
        public virtual Group group { get; set; }
        [Required]
        public int priviledge { get; set; }
    }

    public partial class Group
    {
        [Key]
        public Guid id { get; set; }
        [MaxLength(100),Required]
        public string name { get; set; }
        public string description { get; set; }
    }

    public partial class Project
    {
        [Key]
        public Guid id { get; set; }
        [MaxLength(100),Required]
        public string name { get; set; }
        public bool isprivate { get; set; }

        [Required]
        public virtual Group group { get; set; }
    }

    public partial class Invitation
    {
        [Key]
        public Guid id { get; set; }
        [Required]
        public int type { get; set; }
        [Required]
        public Guid otherid { get; set; }
        [Required]
        public DateTime expiration { get; set; }

        public string message { get; set; }
    }
}
