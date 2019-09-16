using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc.Filters;
using bst.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace bst.Controllers
{
    [Route("protocol")]
    public class ProtocolController : BaseController
    {
        [HttpGet, Route("get/{protocolid}"), ProducesResponseType(typeof(ProtocolData), 200),AuthFilter]
        public async Task<object> Getprotocol(Guid protocolid)
        {
            var user = (User)HttpContext.Items["user"];

            var userProtocolRelation = user.ProtocolUsers.FirstOrDefault(x => x.Protocol.Id.Equals(protocolid));
            if (userProtocolRelation != null)
            {
                return new ProtocolData(userProtocolRelation.Protocol, userProtocolRelation.Privilege);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost,Route("lock/{protocolid}"), ProducesResponseType(typeof(string),200),AuthFilter]
        public object LockProtocol(Guid protocolid)
        {
            var session = (Session)HttpContext.Items["session"];
            session.Protocol = protocolid;
            return Ok();
        }
        [HttpPost, Route("unlock/{protocolid}"), ProducesResponseType(typeof(string), 200), AuthFilter]
        public object UnlockProtocol(Guid protocolid)
        {
            var session = (Session)HttpContext.Items["session"];
            session.Protocol = Guid.Empty;
            return Ok();
        }


        [HttpGet, Route("detail/{protocolid}"), ProducesResponseType(typeof(ProtocolGroupManagementOut), 200),AuthFilter]
        public async Task<object> GetProtocolUsers(Guid protocolid)
        {
            var protocol = context.Protocols.Find(protocolid);
            if (protocol == null) return NotFound($"Protocol {protocolid} doesn't exist.");
            List<GroupManagement> groups = protocol.ProtocolGroups.Select(x => new GroupManagement(x.Group, x.GroupPrivilege)).ToList();
            ProtocolGroupManagementOut result = new ProtocolGroupManagementOut
            {
                Groups = groups
            };
            List<string> internalusers = groups.SelectMany(g => g.Members.Select(m => m.Email)).ToList();
            var protocolusers = protocol.ProtocolUsers.Select(x => new ProtocolMember(x)).ToList();
            var externalusers = protocolusers.Where(u => !internalusers.Contains(u.Email)).ToList();
            result.ExternelUsers = externalusers;
            
           
            return result;
        }



        [HttpPost, Route("share"), ProducesResponseType(typeof(Protocolid), 200), AuthFilter]
        public async Task<object> ShareProtocol([FromBody]CreateProtocol data)
        {
            var user = (User)HttpContext.Items["user"];
            var session = (Session)HttpContext.Items["session"];
            Guid procotolid;
            Protocol protocol = null;
            if(Guid.TryParse(data.Id, out procotolid)){
                protocol = context.Protocols.Find(procotolid);
            }
            if (protocol == null)            
            {
                protocol = new Protocol
                {
                    Id = Guid.NewGuid(),
                    Name = data.Name,
                    Isprivate = data.Isprivate,
                    Comment = data.Comment,
                    IStudy = data.Istudy ?? 0,
                    UseDefaultAnat = data.Usedefaultanat ?? true,
                    UseDefaultChannel = data.Usedefaultchannel ?? true,
                    LastUpdate = System.DateTime.Now
                };
                context.Protocols.Add(protocol);
                var protocoluser = new ProtocolUser
                {
                    Id = Guid.NewGuid(),
                    User = user,
                    Protocol = protocol,
                    //become protocol admin by default
                    Privilege = 1
                };
                context.ProtocolUsers.Add(protocoluser);
                await context.SaveChangesAsync();
            }
            session.Protocol = protocol.Id;
            return new Protocolid
            {
                Id = protocol.Id
            };
        }


        [HttpGet, Route("groups/{protocolid}"), ProducesResponseType(typeof(List<ShareProtocolGroup>), 200), AuthFilter]
        public object ShowProtocolGroups(Guid protocolid)
        {
            var user = (User)HttpContext.Items["user"];
            var session = (Session)HttpContext.Items["session"];
            var protocol = context.Protocols.Find(protocolid);
            if (protocol == null) return NotFound();
            var protocolGroups = protocol.ProtocolGroups.Select(g => new ShareProtocolGroup
            {
                Name = g.Group.Name,
                Access = g.GroupPrivilege == 1 ? "write" : "read"
            }).ToList();
            var userGroupsWithNoAccess = user.GroupUsers.Where(x => !protocolGroups.Exists(g => x.Group.Name.Equals(g.Name)));
            protocolGroups.AddRange(userGroupsWithNoAccess.Select(g => new ShareProtocolGroup
            {
                Name = g.Group.Name,
                Access = "no access"
            }).ToList());
            return protocolGroups;
        }


        [HttpGet, Route("members/{protocolid}"), ProducesResponseType(typeof(List<ShareProtocolExternalMember>), 200), AuthFilter]
        public object ShowProtocolMembers(Guid protocolid)
        {
            var user = (User)HttpContext.Items["user"];
            var session = (Session)HttpContext.Items["session"];
            var protocol = context.Protocols.Find(protocolid);
            if (protocol == null) return NotFound();
            var externalMembers = protocol.ProtocolUsers.Select(x => new ShareProtocolExternalMember
            {
                Email = x.User.Email,
                Access = x.Privilege == 1 ? "admin" : x.Privilege == 2 ? "write" : "read"
            }).ToList();
            return externalMembers;
        }




        [HttpPost, Route("editgroup"), ProducesResponseType(typeof(Guid), 200),AuthFilter]
        public async Task<object> AddOrEditGroup([FromBody]EditGroupProtocolRelationIn data)
        {
            var group = await context.Group.FindAsync(data.Groupid);
            if (group == null) return NotFound("Group not found.");
            var protocol = await context.Protocols.FindAsync(data.Protocolid);
            if (protocol == null) return NotFound("Protocol not found.");
            //check if user is protocol admin
            var user = (User)HttpContext.Items["user"];
            var userProtocolRelation = user.ProtocolUsers.FirstOrDefault(x => x.Protocol.Id.Equals(data.Protocolid));
            if (userProtocolRelation == null || userProtocolRelation.Privilege > 1) Unauthorized("You are not protocol admin.");

            var protocolgroup = await context.ProtocolGroups
                .FirstOrDefaultAsync(p => p.Group.Name.Equals(data.Groupid) && p.Protocol.Id.Equals(data.Protocolid));
            if (protocolgroup == null)
            {
                //add group to protocol
                ProtocolGroup newProtocolGroup = new ProtocolGroup
                {
                    Id = Guid.NewGuid(),
                    Protocol = protocol,
                    Group = group,
                    GroupPrivilege = data.GroupPrivilege
                };
                context.ProtocolGroups.Add(newProtocolGroup);
                await context.SaveChangesAsync();
                return newProtocolGroup.Id;
            }
            else
            {
                //relation exists, edit
                protocolgroup.GroupPrivilege = data.GroupPrivilege;
                await context.SaveChangesAsync();
                return protocolgroup.Id;
            }
        }

        [HttpPost, Route("removegroup"), ProducesResponseType(200),AuthFilter]
        public async Task<object> RemoveGroup([FromBody]RemoveGroupProtocolRelationIn data)
        {
            var protocolgroup = await context.ProtocolGroups
                .FirstOrDefaultAsync(p => p.Group.Name.Equals(data.Groupid) && p.Protocol.Id.Equals(data.Protocolid));
            if (protocolgroup == null) NotFound("There's no relation between the protocol and the group");
            //check if user is protocol admin
            var user = (User)HttpContext.Items["user"];
            var userProtocolRelation = user.ProtocolUsers.FirstOrDefault(x => x.Protocol.Id.Equals(data.Protocolid));
            if (userProtocolRelation == null || userProtocolRelation.Privilege > 1) Unauthorized("You are not protocol admin.");
            //remove group
            context.ProtocolGroups.Remove(protocolgroup);
            return Ok();           
        }

        [HttpPost, Route("edituser"), ProducesResponseType(typeof(Guid), 200),AuthFilter]
        public async Task<object> AddOrEditUser([FromBody] EditUserProtocolRelationIn data)
        {
            //check if user is protocol admin
            var user = (User)HttpContext.Items["user"];
            var userProtocolRelation = user.ProtocolUsers.FirstOrDefault(x => x.Protocol.Id.Equals(data.Protocolid));
            if (userProtocolRelation == null || userProtocolRelation.Privilege > 1) Unauthorized("You are not protocol admin.");
            //find target user protocol relation
            var targetUserProtocolRelation = context.ProtocolUsers
                .FirstOrDefault(x => x.Protocol.Id.Equals(data.Protocolid) && x.User.Email.Equals(data.Userid));
            if(targetUserProtocolRelation == null)
            {
                //create relation 
                var newTargetUserProtocolRelation = new ProtocolUser
                {
                    Id = Guid.NewGuid(),
                    User = context.Users.Find(data.Userid),
                    Protocol = context.Protocols.Find(data.Protocolid),
                    Privilege = data.Privilege
                };
                context.ProtocolUsers.Add(newTargetUserProtocolRelation);
                await context.SaveChangesAsync();
                return newTargetUserProtocolRelation.Id;
            }
            else
            {
                //edit relation
                targetUserProtocolRelation.Privilege = data.Privilege;
                await context.SaveChangesAsync();
                return targetUserProtocolRelation.Id;
            }
        }
        [HttpPost, Route("adduser/{userdid}/{priviledge}"), ProducesResponseType(200), AuthFilter]
        public async Task<object> AddUser([FromBody] AddUserProtocolRelationIn data)
        {
            //check if user is protocol admin
            var user = (User)HttpContext.Items["user"];
            var userProtocolRelation = user.ProtocolUsers.FirstOrDefault(x => x.Protocol.Id.Equals(data.Protocolid));
            if (userProtocolRelation == null || userProtocolRelation.Privilege > 1) Unauthorized("You are not protocol admin.");

            var target = context.Users.Find(data.Userid);
            if (target==null)
            {
                return NotFound("user not found");
            }
            //find target user 
            context.Add(new ProtocolUser
            {
                Id = Guid.NewGuid(),
                User = target,
                Protocol = userProtocolRelation.Protocol,
                Privilege = data.Priviledge
            });
            await context.SaveChangesAsync();
            return Ok();
        }
        [HttpPost, Route("removeuser"), ProducesResponseType(200),AuthFilter]
        public async Task<object> RemoveUser([FromBody] RemoveUserProtocolRelationIn data)
        {
            //check if user is protocol admin
            var user = (User)HttpContext.Items["user"];
            var userProtocolRelation = user.ProtocolUsers.FirstOrDefault(x => x.Protocol.Id.Equals(data.Protocolid));
            if (userProtocolRelation == null || userProtocolRelation.Privilege > 1) Unauthorized("You are not protocol admin.");
            //find target user protocol relation
            var targetUserProtocolRelation = context.ProtocolUsers
                .FirstOrDefault(x => x.Protocol.Id.Equals(data.Protocolid) && x.User.Email.Equals(data.Userid));
            if (targetUserProtocolRelation == null) return NotFound("User protocol relation not found.");
            //remove
            context.ProtocolUsers.Remove(targetUserProtocolRelation);
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
