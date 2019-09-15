using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using bst.Model;

namespace bst.Controllers
{
    [Route("group")]
    [ApiController,AuthFilter]
    public class GroupController:BaseController
    {
        [HttpPost,Route("create"),ProducesResponseType(typeof(GroupPreview),200)]
        public async Task<object> CreateGroup([FromBody]CreateGroupIn data)
        {
            var user = (User)HttpContext.Items["user"];          
            var group = new Group
            {
                Name = data.Name
            };
            var groupUserRelation = new GroupUser
            {
                User = user,
                Group = group,
                //the user become group manager by default
                Role = 1
            };
            context.Group.Add(group);
            context.GroupUsers.Add(groupUserRelation);
            await context.SaveChangesAsync();
            return new GroupPreview(group);
        }

#warning what is going to be modified?
        [HttpPost, Route("modify"), ProducesResponseType(typeof(GroupPreview), 200)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<object> ModifyName([FromBody]ModifyGroupIn data)
        {
            var group = await context.Group.FindAsync(data.Name);
            if (group == null)
            {
                HttpContext.Response.StatusCode = 404;
                return "Group not found";
            }
            await context.SaveChangesAsync();
            return new GroupPreview(group);
        }

        [HttpPost, Route("detail/{groupname}"), ProducesResponseType(typeof(GroupDetailOut), 200)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<object> Detail(string groupname)
        {
            var u = (User)HttpContext.Items["user"];
            var group = u.GroupUsers.FirstOrDefault(x => x.Group.Name.Equals(groupname));
            if (group == null)
            {
                HttpContext.Response.StatusCode = 404;
                return "Group not found";
            }
            
            return new GroupDetailOut(group.Group);
        }        


        [HttpPost, Route("changerole"), ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<object> ChangePrivilege([FromBody]EditGroupMemberIn data)
        {
            var user = (User)HttpContext.Items["user"];
            var group = await context.Group.FindAsync(data.GroupName);
            if (group == null)
            {
                HttpContext.Response.StatusCode = 404;
                return "Group doesn't exist";
            }
            var userGroupRelation = user.GroupUsers.FirstOrDefault(r => r.Group.Name.Equals(data.GroupName));
            if (userGroupRelation == null || userGroupRelation.Role != 1)
                return Unauthorized("User must be group manager to change member role.");

            //change target user's role to group 
            var roletochange = context.GroupUsers.FirstOrDefault(r => r.Group.Name.Equals(data.GroupName) && r.User.Email.Equals(data.UserEmail));
            if (roletochange == null) return NotFound($"User {data.UserEmail} is not a group member.");
            roletochange.Role = data.Role;
            context.Entry(roletochange).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

            await context.SaveChangesAsync();
            return Ok("Change privilege successfully!");
        }

        [HttpPost, Route("adduser"), ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<object> AddUser([FromBody]AddGroupUserIn data)
        {
            var user = (User)HttpContext.Items["user"];
            var group = user.GroupUsers.FirstOrDefault(x => x.Group.Name.Equals(data.GroupName));
            if (group == null)
                return NotFound("group not found");
            var target = context.Users.Find(data.UserEmail);
            if (target == null)
                return NotFound("user not found");
            context.GroupUsers.Add(new GroupUser
            {
                Id = Guid.NewGuid(),
                User = user,
                Group = group.Group,
                Role = data.Privilege
            });
            await context.SaveChangesAsync();
            return Ok("Add user successfully!");
        }

        [HttpPost, Route("removeuser"), ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<object> RemoveUser([FromBody]RemoveGroupUserIn data)
        {
            var user = (User)HttpContext.Items["user"];
            var group = user.GroupUsers.FirstOrDefault(x=>x.Group.Name.Equals(data.GroupName));
            if (group == null)
            {
                HttpContext.Response.StatusCode = 404;
                return "Group doesn't exist";
            }
            var userGroupRelation = user.GroupUsers.FirstOrDefault(r => r.Group.Name.Equals(data.GroupName));
            if (userGroupRelation == null || (userGroupRelation.Role != 1 && !user.Email.Equals(data.UserEmail)))
                return Unauthorized("User must be group manager or himself/herself to remove user.");
            context.GroupUsers.Remove(userGroupRelation);
            var groupProtocolIds = group.Group.GroupProtocols.Select(p => p.Id);
            context.ProtocolUsers.RemoveRange(context.ProtocolUsers.Where(p => groupProtocolIds.Contains(p.Protocol.Id) && p.User.Email.Equals(data.UserEmail)));
            await context.SaveChangesAsync();
            return Ok("Remove user successfully!");       
        }
    }
}