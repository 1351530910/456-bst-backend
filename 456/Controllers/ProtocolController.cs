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
using bst.Logic;

namespace bst.Controllers
{
    [Route("protocol")]
    [AuthFilter]
    public class ProtocolController : BaseController
    {
        [HttpGet, Route("get/{protocolid}"), ProducesResponseType(typeof(ProtocolData), 200)]
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
        
        [HttpGet, Route("detail/{protocolid}"), ProducesResponseType(typeof(ProtocolGroupManagementOut), 200)]
        public async Task<object> GetProtocolDetail(Guid protocolid)
        {
            var protocol = context.Protocols.Find(protocolid);
            if (protocol == null) return NotFound($"Protocol {protocolid} doesn't exist.");
            List<GroupManagement> groups = protocol.ProtocolGroups.Select(x => ConfigureData.ToGroupManagement(x.Group, protocolid)).ToList();
            List<Guid> internalusers = groups.SelectMany(g => g.Members.Select(m => m.Id)).ToList();
            List<ProtocolMember> externalUsers = protocol.ProtocolUsers
                .Select(x => ConfigureData.ToProtocolMember(x.User, protocolid))
                .Where(x => !internalusers.Contains(x.Id))
                .ToList();

            ProtocolGroupManagementOut result = new ProtocolGroupManagementOut
            {
                Groups = groups,
                ExternelUsers = externalUsers
            };
            return result;
        }

#warning default values?
        [HttpPost, Route("create"), ProducesResponseType(typeof(Guid), 200)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<object> CreateProtocol([FromBody]CreateProtocol data)
        {
            var user = (User)HttpContext.Items["user"];
            var protocol = new Protocol
            {
                Id = Guid.NewGuid(),
                Name = data.Name,
                Isprivate = data.Isprivate,
                Comment = data.Comment,
                IStudy = data.Istudy ?? 0,
                UseDefaultAnat = data.Usedefaultanat ?? true,
                UseDefaultChannel = data.Usedefaultchannel ?? true,
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
            return protocol.Id;
        }


        [HttpPost, Route("editgroup"), ProducesResponseType(typeof(Guid), 200)]
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
                .FirstOrDefaultAsync(p => p.Group.Id.Equals(data.Groupid) && p.Protocol.Id.Equals(data.Protocolid));
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

        [HttpPost, Route("removegroup"), ProducesResponseType(200)]
        public async Task<object> RemoveGroup([FromBody]RemoveGroupProtocolRelationIn data)
        {
            var protocolgroup = await context.ProtocolGroups
                .FirstOrDefaultAsync(p => p.Group.Id.Equals(data.Groupid) && p.Protocol.Id.Equals(data.Protocolid));
            if (protocolgroup == null) NotFound("There's no relation between the protocol and the group");
            //check if user is protocol admin
            var user = (User)HttpContext.Items["user"];
            var userProtocolRelation = user.ProtocolUsers.FirstOrDefault(x => x.Protocol.Id.Equals(data.Protocolid));
            if (userProtocolRelation == null || userProtocolRelation.Privilege > 1) Unauthorized("You are not protocol admin.");
            //remove group
            context.ProtocolGroups.Remove(protocolgroup);
            return Ok();           
        }

        [HttpPost, Route("edituser"), ProducesResponseType(typeof(Guid), 200)]
        public async Task<object> AddOrEditUser([FromBody] EditUserProtocolRelationIn data)
        {
            //check if user is protocol admin
            var user = (User)HttpContext.Items["user"];
            var userProtocolRelation = user.ProtocolUsers.FirstOrDefault(x => x.Protocol.Id.Equals(data.Protocolid));
            if (userProtocolRelation == null || userProtocolRelation.Privilege > 1) Unauthorized("You are not protocol admin.");
            //find target user protocol relation
            var targetUserProtocolRelation = context.ProtocolUsers
                .FirstOrDefault(x => x.Protocol.Id.Equals(data.Protocolid) && x.User.Id.Equals(data.Userid));
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

        [HttpPost, Route("removeuser"), ProducesResponseType(200)]
        public async Task<object> RemoveUser([FromBody] RemoveUserProtocolRelationIn data)
        {
            //check if user is protocol admin
            var user = (User)HttpContext.Items["user"];
            var userProtocolRelation = user.ProtocolUsers.FirstOrDefault(x => x.Protocol.Id.Equals(data.Protocolid));
            if (userProtocolRelation == null || userProtocolRelation.Privilege > 1) Unauthorized("You are not protocol admin.");
            //find target user protocol relation
            var targetUserProtocolRelation = context.ProtocolUsers
                .FirstOrDefault(x => x.Protocol.Id.Equals(data.Protocolid) && x.User.Id.Equals(data.Userid));
            if (targetUserProtocolRelation == null) return NotFound("User protocol relation not found.");
            //remove
            context.ProtocolUsers.Remove(targetUserProtocolRelation);
            await context.SaveChangesAsync();
            return Ok();
        }


    }
}
