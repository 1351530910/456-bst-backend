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
        public string email { get; set; }
        [MinLength(8), MaxLength(15), Required]
        public string password { get; set; }
        [MaxLength(30),Required]
        public string FirstName { get; set; }
        [MaxLength(30),Required]
        public string LastName { get; set; }
        [Required]
        public string deviceid { get; set; }
    }
    public class CreateUserOut
    {
        public Guid sessionid { get; set; }
        public string email { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
    }

    public class LoginIn
    {
        [EmailAddress,Required]
        public string email { get; set; }
        [Required]
        public string password { get; set; }
        [Required]
        public string deviceid { get; set; }
    }
    public class LoginOut
    {
        public Guid sessionid { get; set; }
    }
    public class CreateGroupIn
    {
        [Required]
        public string name { get; set; }
        public string description { get; set; }
    }
    public class GroupPreview
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public IEnumerable<UserPreview> users { get; set; }
        public IEnumerable<ProtocolPreview> projects { get; set; }

        public GroupPreview(Group group)
        {
            id = group.id;
            name = group.name;
            description = group.description;
            
            projects = group.protocols.Select(x => new ProtocolPreview(x));
            users = group.users.Select(x => new UserPreview(x.user, x.priviledge));
        }
    }
    public class UserPreview
    {
        public Guid id { get; set; }
        public string email { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public int priviledge { get; set; }

        public UserPreview(User user,int priviledge)
        {
            this.priviledge = priviledge;
            id = user.id;
            email = user.email;
            firstname = user.FirstName;
            lastname = user.LastName;

        }
    }
    public class ProtocolPreview
    {
        [Key]
        public Guid id { get; set; }
        [MaxLength(100), Required]
        public string name { get; set; }
        public bool isprivate { get; set; }
        //metadata
        public string Comment { get; set; }
        public int IStudy { get; set; }
        public bool UseDefaultAnat { get; set; }
        public bool UseDefaultChannel { get; set; }
        public ProtocolPreview(Protocol protocol)
        {
            id = protocol.id;
            name = protocol.name;
            isprivate = protocol.isprivate;
            Comment = protocol.Comment;
            IStudy = protocol.IStudy;
            UseDefaultAnat = protocol.UseDefaultAnat;
            UseDefaultChannel = protocol.UseDefaultChannel;
        }
    }
    public class GroupInviteIn
    {
        [Required]
        public Guid groupid { get; set; }
        [Required]
        public Guid userid { get; set; }
        [Required]
        public int permission { get; set; }
    }
    public class RemoveUserIn
    {
        [Required]
        public Guid groupid { get; set; }
        [Required]
        public Guid userid { get; set; }
    }
    public class CreateProtocol
    {
        [Required]
        public Guid group { get; set; }
        [MaxLength(100), Required]
        public string name { get; set; }
        public bool isprivate { get; set; }
        public string comment { get; set; }
        public int? istudy { get; set; }
        public bool? usedefaultanat { get; set; }
        public bool? usedefaultchannel { get; set; }
        
    }
    public class ListCount
    {
        /// <summary>
        /// start index
        /// </summary>
        [Required]
        public int start { get; set; }
        /// <summary>
        /// how many results(max)
        /// </summary>
        [Required]
        public int count { get; set; }
        /// <summary>
        /// ordering
        /// </summary>
        [Required]
        public int order { get; set; }
    }
    public class GroupDetailIn
    {
        [Required]
        public Guid groupid { get; set; }
    }
    public class ModifyGroupIn
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }
}
