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

    public class CheckSessionIn
    {
        public string Deviceid { get; set; }
        public Guid Sessionid { get; set; }
    }
    public class LoginOut
    {
        public Guid Sessionid { get; set; }
    }
    public class CreateGroupIn
    {
        [Required]
        public string Name { get; set; }
    }
    public class GroupPreview
    {
        public string Name { get; set; }
        public IEnumerable<UserPreview> Users { get; set; }
        public IEnumerable<ProtocolData> Projects { get; set; }

        public GroupPreview(Group group)
        {
            Name = group.Name;            
            //why protocol have privilege?
            //projects = group.protocols.Select(x => new ProtocolPreview(x));
            Users = group.Members.Select(x => new UserPreview(x.User, x.Role));
        }
    }
    public class UserPreview
    {
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public int Privilege { get; set; }

        public UserPreview(User user,int privilege)
        {
            this.Privilege = privilege;
            Email = user.Email;
            Firstname = user.FirstName;
            Lastname = user.LastName;
        }
    }
    public class ProtocolData
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
        public int Privilege { get; set; }
        public ProtocolData(Protocol protocol,int privilege)
        {
            Id = protocol.Id;
            Name = protocol.Name;
            Isprivate = protocol.Isprivate;
            Comment = protocol.Comment;
            IStudy = protocol.IStudy;
            UseDefaultAnat = protocol.UseDefaultAnat;
            UseDefaultChannel = protocol.UseDefaultChannel;
            Privilege = privilege;
        }
    }
    public class EditGroupMemberIn
    {
        [Required]
        public string GroupName { get; set; }
        [Required]
        public string UserEmail { get; set; }
        [Required]
        //the role of the added person in the group
        public int Role { get; set; }
    }
    public class AddGroupUserIn
    {
        [Required]
        public string GroupName { get; set; }
        [Required]
        public string UserEmail { get; set; }
        [Required]
        public int Privilege { get; set; }
    }
    public class RemoveGroupUserIn
    {
        [Required]
        public string GroupName { get; set; }
        [Required]
        public string UserEmail { get; set; }
    }
    public class CreateProtocol
    {
        public string Id { get; set; }
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
        public string GroupName { get; set; }
    }
    public class ModifyGroupIn
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class FileTransferIn
    {
        public Guid Protocolid { get; set; }
        public string Filelocation { get; set; }
    }

    public class GroupMember
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Privilege { get; set; }
        public GroupMember(GroupUser role)
        {
            FirstName = role.User.FirstName;
            LastName = role.User.LastName;
            Privilege = role.Role;
        }
    }

    public class GroupProtocolPreview
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Isprivate { get; set; }
        //metadata
        public string Comment { get; set; }
        public int IStudy { get; set; }
        public bool UseDefaultAnat { get; set; }
        public bool UseDefaultChannel { get; set; }
        public GroupProtocolPreview(Protocol protocol)
        {
            Id = protocol.Id;
            Name = protocol.Name;
            Isprivate = protocol.Isprivate;
            Comment = protocol.Comment;
            IStudy = protocol.IStudy;
            UseDefaultAnat = protocol.UseDefaultAnat;
            UseDefaultChannel = protocol.UseDefaultChannel;
        }
    }

    public class GroupDetailOut
    {
        public string Name { get; set; }

        public List<GroupMember> GroupMembers { get; set; }
        public List<GroupProtocolPreview> GroupProtocols { get; set; }

        public GroupDetailOut(Group group)
        {
            Name = group.Name;
            GroupMembers = group.Members.Select(u => new GroupMember(u)).ToList();
            GroupProtocols = group.GroupProtocols.Select(p => new GroupProtocolPreview(p.Protocol)).ToList();
        }
    }

    public class EditGroupProtocolRelationIn
    {
        public Guid Groupid { get; set; }
        public Guid Protocolid { get; set; }
        public int GroupPrivilege { get; set; }
    }

    public class RemoveGroupProtocolRelationIn
    {
        public Guid Groupid { get; set; }
        public Guid Protocolid { get; set; }
    }

    public class EditUserProtocolRelationIn
    {
        public Guid Userid { get; set; }
        public Guid Protocolid { get; set; }
        public int Privilege { get; set; }
    }
    public class AddUserProtocolRelationIn
    {
        public Guid Userid { get; set; }
        public Guid Protocolid { get; set; }
        public int Priviledge { get; set; }
    }
    public class RemoveUserProtocolRelationIn
    {
        public Guid Userid { get; set; }
        public Guid Protocolid { get; set; }
    }

    public class ProtocolGroupManagementOut
    {
        public List<GroupManagement> Groups;
        public List<ProtocolMember> ExternelUsers;
    }

    public class GroupManagement
    {
        public string GroupName { get; set; }
        public int GroupPrivilege { get; set; }
        public List<ProtocolMember> Members { get; set; }
        public GroupManagement() { }
        public GroupManagement(Group group,int privilege)
        {
            GroupName = group.Name;
            Members = group.Members.Select(role => new ProtocolMember(role.User)).ToList();
            GroupPrivilege = privilege;
        }
    }

    public class ProtocolMember
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int ProtocolPrivilege { get; set; }
        public ProtocolMember() { }
        public ProtocolMember(User user)
        {
            Email = user.Email;
            FirstName = user.FirstName;
            LastName = user.LastName;
        }
        public ProtocolMember(ProtocolUser protocolUser)
        {
            Email = protocolUser.User.Email;
            FirstName = protocolUser.User.FirstName;
            LastName = protocolUser.User.LastName;
            ProtocolPrivilege = protocolUser.Privilege;
        }
    }






    public class ShareProtocolGroup
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Access { get; set; }
    }

    public class ShareProtocolExternalMember
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Access { get; set; }
    }

    public class Protocolid
    {
        public Guid Id { get; set; }
    }
}
