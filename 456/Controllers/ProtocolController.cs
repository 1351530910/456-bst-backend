using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using bst.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace bst.Controllers
{
    [Route("protocol")]
    public class ProtocolController : BaseController
    {
        [HttpGet, Route("get/{protocolid}"), ProducesResponseType(typeof(ProtocolData), 200), AuthFilter]
        public async Task<object> Getprotocol(Guid protocolid)
        {
            var userProtocolRelation = user.ProtocolUsers.FirstOrDefault(x => x.Protocol.Id.Equals(protocolid));
            if (userProtocolRelation != null)
                return new ProtocolData(userProtocolRelation.Protocol, userProtocolRelation.Privilege);
            else
                return NotFound();
        }

        [HttpPost, Route("lock/{protocolid}"), ProducesResponseType(typeof(string), 200), AuthFilter]
        public object LockProtocol(Guid protocolid)
        {
            session.Protocolid = protocolid;
            return Ok();
        }
        [HttpPost, Route("unlock/{protocolid}"), ProducesResponseType(typeof(string), 200), AuthFilter]
        public object UnlockProtocol(Guid protocolid)
        {
            session.Protocolid = Guid.Empty;
            return Ok();
        }

        /// <summary>
        /// get users participating the protocol
        /// </summary>
        /// <param name="protocolid"></param>
        /// <returns></returns>
        [HttpGet, Route("detail/{protocolid}"), ProducesResponseType(typeof(ProtocolGroupManagementOut), 200), AuthFilter]
        public async Task<object> GetProtocolUsers(Guid protocolid)
        {
            //check if protocol actually exists
            var protocol = context.Protocols.Find(protocolid);
            if (protocol == null) return NotFound($"Protocol {protocolid} doesn't exist.");

            //find groups participations
            List<GroupManagement> groups = protocol.ProtocolGroups.Select(x => new GroupManagement(x.Group, x.GroupPrivilege)).ToList();
            ProtocolGroupManagementOut result = new ProtocolGroupManagementOut
            {
                Groups = groups
            };

            //find direct user participations
            List<string> internalusers = groups.SelectMany(g => g.Members.Select(m => m.Email)).ToList();


            var protocolusers = protocol.ProtocolUsers.Select(x => new ProtocolMember(x)).ToList();
            var externalusers = protocolusers.Where(u => !internalusers.Contains(u.Email)).ToList();
            result.ExternelUsers = externalusers;

            return result;
        }


        /// <summary>
        /// create a new protocol
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost, Route("share"), ProducesResponseType(typeof(ID), 200), AuthFilter]
        public async Task<object> CreateProtocol([FromBody]CreateProtocol data)
        {

            Guid procotolid;
            Protocol protocol = null;

            //check if this protocol has already been uploaded
            if (Guid.TryParse(data.Id, out procotolid))
            {
                protocol = user.ProtocolUsers.Select(x => x.Protocol).FirstOrDefault(p => p.Id.Equals(procotolid));
                protocol = user.GroupUsers.Select(x => x.Group).SelectMany(g => g.GroupProtocols)
                            .Select(x => x.Protocol).FirstOrDefault(p => p.Id.Equals(procotolid));
            }

            //if not present in user visible range then create a new one
            if (protocol == null)
            {
                //create the protocol
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
                //give user admin priviledge
                var protocoluser = new ProtocolUser
                {
                    Id = Guid.NewGuid(),
                    User = user,
                    Protocol = protocol,
                    //become protocol admin by default
                    Privilege = 1
                };

                //save changes to database
                context.Protocols.Add(protocol);
                context.ProtocolUsers.Add(protocoluser);
                await context.SaveChangesAsync();
            }

            //lock the protocol since creation is a write action
            session.Protocolid = protocol.Id;
            return new ID
            {
                Id = protocol.Id
            };
        }


        [HttpGet, Route("groups/{protocolid}"), ProducesResponseType(typeof(List<ShareProtocolGroup>), 200), AuthFilter]
        public object ShowProtocolGroups(Guid protocolid)
        {
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


        [HttpGet, Route("availablegroups/{protocolid}"), ProducesResponseType(typeof(List<String>), 200), AuthFilter]
        public object ShowAvailableGroupsThatCanBeAdded(Guid protocolid)
        {
            var protocol = context.Protocols.Find(protocolid);
            if (protocol == null) return NotFound();
            var protocolGroups = protocol.ProtocolGroups.Select(g => g.Group.Name).ToList();
            var userGroupsWithNoAccess = user.GroupUsers.Where(x => !protocolGroups.Contains(x.Group.Name));
            var result = userGroupsWithNoAccess.Select(g => g.Group.Name).ToList();
            return result;
        }


        [HttpGet, Route("members/{protocolid}"), ProducesResponseType(typeof(List<ShareProtocolExternalMember>), 200), AuthFilter]
        public object ShowProtocolMembers(Guid protocolid)
        {
            var protocol = context.Protocols.Find(protocolid);
            if (protocol == null) return NotFound();
            var externalMembers = protocol.ProtocolUsers.Select(x => new ShareProtocolExternalMember
            {
                Email = x.User.Email,
                Access = x.Privilege == 1 ? "admin" : x.Privilege == 2 ? "write" : "read"
            }).ToList();
            return externalMembers;
        }




        [HttpPost, Route("editgroup"), ProducesResponseType(typeof(Guid), 200), AuthFilter]
        public async Task<object> AddOrEditGroup([FromBody]EditGroupProtocolRelationIn data)
        {
            var group = await context.Group.FirstOrDefaultAsync(g => g.Name.Equals(data.Groupname));

            if (group == null) return NotFound("Group not found.");
            var protocol = await context.Protocols.FindAsync(data.Protocolid);
            if (protocol == null) return NotFound("Protocol not found.");
            //check if user is protocol admin

            var userProtocolRelation = user.ProtocolUsers.FirstOrDefault(x => x.Protocol.Id.Equals(data.Protocolid));
            if (userProtocolRelation == null || userProtocolRelation.Privilege > 1) Unauthorized("You are not protocol admin.");

            var protocolgroup = await context.ProtocolGroups
                .FirstOrDefaultAsync(p => p.Group.Name.Equals(data.Groupname) && p.Protocol.Id.Equals(data.Protocolid));
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

        [HttpPost, Route("removegroup"), ProducesResponseType(200), AuthFilter]
        public async Task<object> RemoveGroup([FromBody]RemoveGroupProtocolRelationIn data)
        {
            var protocolgroup = await context.ProtocolGroups
                .FirstOrDefaultAsync(p => p.Group.Name.Equals(data.Groupname) && p.Protocol.Id.Equals(data.Protocolid));
            if (protocolgroup == null) NotFound("There's no relation between the protocol and the group");
            //check if user is protocol admin

            var userProtocolRelation = user.ProtocolUsers.FirstOrDefault(x => x.Protocol.Id.Equals(data.Protocolid));
            if (userProtocolRelation == null || userProtocolRelation.Privilege > 1) Unauthorized("You are not protocol admin.");
            //remove group
            context.ProtocolGroups.Remove(protocolgroup);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost, Route("edituser"), ProducesResponseType(typeof(Guid), 200), AuthFilter]
        public async Task<object> AddOrEditUser([FromBody]EditUserProtocolRelationIn data)
        {
            //check if user is protocol admin

            var userProtocolRelation = user.ProtocolUsers.FirstOrDefault(x => x.Protocol.Id.Equals(data.Protocolid));
            if (userProtocolRelation == null || userProtocolRelation.Privilege > 1) Unauthorized("You are not protocol admin.");
            //find target user protocol relation
            var targetUserProtocolRelation = context.ProtocolUsers
                .FirstOrDefault(x => x.Protocol.Id.Equals(data.Protocolid) && x.User.Email.Equals(data.Useremail));
            var privildge = data.Privilege;
            if (privildge < 1 || privildge > 3) privildge = 3;
            if (targetUserProtocolRelation == null)
            {
                var targetuser = context.Users.FirstOrDefault(u => u.Email.Equals(data.Useremail));
                if (targetuser == null) return NotFound("The user you want to edit doesn't exist.");

                //create relation 
                var newTargetUserProtocolRelation = new ProtocolUser
                {
                    Id = Guid.NewGuid(),
                    User = targetuser,
                    Protocol = userProtocolRelation.Protocol,
                    Privilege = privildge
                };
                context.ProtocolUsers.Add(newTargetUserProtocolRelation);
                await context.SaveChangesAsync();
                return newTargetUserProtocolRelation.Id;
            }
            else
            {
                //edit relation
                targetUserProtocolRelation.Privilege = privildge;
                await context.SaveChangesAsync();
                return targetUserProtocolRelation.Id;
            }
        }

        [HttpPost, Route("removeuser"), ProducesResponseType(200), AuthFilter]
        public async Task<object> RemoveUser([FromBody] RemoveUserProtocolRelationIn data)
        {
            //check if user is protocol admin
            var userProtocolRelation = user.ProtocolUsers.FirstOrDefault(x => x.Protocol.Id.Equals(data.Protocolid));
            if (userProtocolRelation == null || userProtocolRelation.Privilege > 1) Unauthorized("You are not protocol admin.");
            //find target user protocol relation
            var targetUserProtocolRelation = context.ProtocolUsers
                .FirstOrDefault(x => x.Protocol.Id.Equals(data.Protocolid) && x.User.Email.Equals(data.Useremail));
            if (targetUserProtocolRelation == null) return NotFound("User protocol relation not found.");
            //remove
            context.ProtocolUsers.Remove(targetUserProtocolRelation);
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
