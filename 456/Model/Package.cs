using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using bst.Model;

namespace bst.Controllers
{
    public class CreateUserIn
    {
        [EmailAddress,Required]
        public string Email { get; set; }
        [MinLength(8), MaxLength(15), Required]
        public string Password { get; set; }
        [MaxLength(30),Required]
        public string FirstName { get; set; }
        [MaxLength(30),Required]
        public string LastName { get; set; }
        [Required]
        public string Deviceid { get; set; }
    }
    public class CreateUserOut
    {
        public Guid Sessionid { get; set; }
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
    }

    public class LoginIn
    {
        [EmailAddress,Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Deviceid { get; set; }
    }
    public class LoginOut
    {
        public Guid Sessionid { get; set; }
    }
    public class CreateGroupIn
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
    }
    public class GroupPreview
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public IEnumerable<UserPreview> Users { get; set; }
        public IEnumerable<ProtocolPreview> Projects { get; set; }

        public GroupPreview(Group group)
        {
            Id = group.Id;
            Name = group.Name;
            Description = group.Description;
            
            //why protocol have privilege?
            //projects = group.protocols.Select(x => new ProtocolPreview(x));
            Users = group.Users.Select(x => new UserPreview(x.User, x.Privilege));
        }
    }
    public class UserPreview
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public int Privilege { get; set; }

        public UserPreview(User user,int privilege)
        {
            this.Privilege = privilege;
            Id = user.Id;
            Email = user.Email;
            Firstname = user.FirstName;
            Lastname = user.LastName;

        }
    }
    public class ProtocolPreview
    {
        [Key]
        public Guid Id { get; set; }
        [MaxLength(100), Required]
        public string Name { get; set; }
        public bool Isprivate { get; set; }
        //metadata
        public string Comment { get; set; }
        public int IStudy { get; set; }
        public bool UseDefaultAnat { get; set; }
        public bool UseDefaultChannel { get; set; }
        public int priviledge { get; set; }
        public ProtocolPreview(Protocol protocol,int priviledge)
        {
            Id = protocol.Id;
            Name = protocol.Name;
            Isprivate = protocol.Isprivate;
            Comment = protocol.Comment;
            IStudy = protocol.IStudy;
            UseDefaultAnat = protocol.UseDefaultAnat;
            UseDefaultChannel = protocol.UseDefaultChannel;
            this.priviledge = priviledge;
        }
    }
    public class GroupInviteIn
    {
        [Required]
        public Guid Groupid { get; set; }
        [Required]
        public Guid Userid { get; set; }
        [Required]
        //the privilege of the added person in the group
        public int Permission { get; set; }
    }
    public class RemoveUserIn
    {
        [Required]
        public Guid Groupid { get; set; }
        [Required]
        public Guid Userid { get; set; }
    }
    public class CreateProtocol
    {
        [Required]
        public Guid Group { get; set; }
        [MaxLength(100), Required]
        public string Name { get; set; }
        public bool Isprivate { get; set; }
        public string Comment { get; set; }
        public int? Istudy { get; set; }
        public bool? Usedefaultanat { get; set; }
        public bool? Usedefaultchannel { get; set; }
        
    }
    public class ListCount
    {
        /// <summary>
        /// start index
        /// </summary>
        [Required]
        public int Start { get; set; }
        /// <summary>
        /// how many results(max)
        /// </summary>
        [Required]
        public int Count { get; set; }
        /// <summary>
        /// ordering
        /// </summary>
        [Required]
        public int Order { get; set; }
    }
    public class GroupDetailIn
    {
        [Required]
        public Guid Groupid { get; set; }
    }
    public class ModifyGroupIn
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class FileTransferIn
    {
        public Guid Protocolid { get; set; }
        public string Filelocation { get; set; }
    }
}
