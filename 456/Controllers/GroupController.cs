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
    public class GroupController:Controller
    {
        public UserDB context = new UserDB();
        [HttpPost,Route("create"),ProducesResponseType(typeof(GroupPreview),200)]
        public async Task<object> CreateGroup([FromBody]CreateGroupIn data)
        {
            var user = await context.users.FindAsync(HttpContext.Items["user"]);
            var group = new Group
            {
                id = Guid.NewGuid(),
                name = data.name,
                description = data.description,

            };
            var role = new Role
            {
                id = Guid.NewGuid(),
                user = user,
                group = group,
                //add administrator
                privilege = 1
            };
            context.group.Add(group);
            context.roles.Add(role);
            await context.SaveChangesAsync();
            return new GroupPreview(group);
        }


        [HttpPost, Route("modify"), ProducesResponseType(typeof(GroupPreview), 200)]
        public async Task<object> Modify([FromBody]ModifyGroupIn data)
        {
            var group = await context.group.FindAsync(data.id);
            group.name = data.name;
            group.description = data.description;
            await context.SaveChangesAsync();
            return new GroupPreview(group);
        }

        [HttpPost, Route("detail"), ProducesResponseType(typeof(Group), 200)]
        public async Task<object> Detail([FromBody]GroupDetailIn data)
        {
            var group = await context.group.FindAsync(data.groupid);
            return group;
        }        

        [HttpPost, Route("listGroup"), ProducesResponseType(typeof(IEnumerable<GroupPreview>), 200)]
        public async Task<object> ListGroup([FromBody]ListCount data)
        {
            var user = await context.users.FindAsync(HttpContext.Items["user"]);
            var groups = user.roles.Select(r => r.group);
            return groups.Select(g => new GroupPreview(g));
        }

        [HttpPost, Route("invite"), ProducesResponseType(200)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<object> Invite([FromBody]GroupInviteIn data)
        {
            var user = await context.users.FindAsync(HttpContext.Items["user"]);
            var userPrivilege = user.roles.FirstOrDefault(r => r.group.id.Equals(data.groupid));
            if (userPrivilege == null || userPrivilege.privilege != 1)
                return BadRequest("User must be group admin to add people.");
            var group = context.group.FirstOrDefault(g => g.id.Equals(data.groupid));
            var addeduser = context.users.FirstOrDefault(u => u.id.Equals(data.userid));
            if (group == null || addeduser == null) return NotFound("Group or added user not found.");
            var newrole = new Role
            {
                id = Guid.NewGuid(),
                user = addeduser,
                group = group,
                //add administrator
                privilege = data.permission
            };
            context.roles.Add(newrole);
            await context.SaveChangesAsync();
            return Ok("Add member successfully!");
        }


        [HttpPost, Route("changePriviledge"), ProducesResponseType(typeof(string), 200)]
        public async Task<object> ChangePrivilege([FromBody]GroupInviteIn data)
        {
            var user = await context.users.FindAsync(HttpContext.Items["user"]);
            var userPrivilege = user.roles.FirstOrDefault(r => r.group.id.Equals(data.groupid));
            if (userPrivilege == null || userPrivilege.privilege != 1)
                return BadRequest("User must be group admin to change privilege.");
            var roletochange = context.roles.FirstOrDefault(r => r.group.id.Equals(data.groupid) && r.user.id.Equals(data.userid));
            if (roletochange == null) return NotFound($"User {data.userid} is not a group member.");
            roletochange.privilege = data.permission;
            return Ok("Change privilege successfully!");
        }

        [HttpPost, Route("removeUser"), ProducesResponseType(typeof(string), 200)]
        public async Task<object> RemoveUser([FromBody]RemoveUserIn data)
        {
            var user = await context.users.FindAsync(HttpContext.Items["user"]);
            var userPrivilege = user.roles.FirstOrDefault(r => r.group.id.Equals(data.groupid));
            if (userPrivilege == null || (userPrivilege.privilege != 1 && !user.id.Equals(data.userid)))
                return BadRequest("User must be group admin or himself/herself to remove user.");
            context.roles.Remove(context.roles.FirstOrDefault(r => r.group.id.Equals(data.groupid) && r.user.id.Equals(data.userid)));
            await context.SaveChangesAsync();
            return Ok("Remove user successfully!");       
        }

        
    }
}