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
                Id = Guid.NewGuid(),
                Name = data.Name,
                Description = data.Description,

            };
            var role = new Role
            {
                Id = Guid.NewGuid(),
                User = user,
                Group = group,
                //add administrator
                Privilege = 1
            };
            context.Group.Add(group);
            context.Roles.Add(role);
            await context.SaveChangesAsync();
            return new GroupPreview(group);
        }


        [HttpPost, Route("modify"), ProducesResponseType(typeof(GroupPreview), 200)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<object> Modify([FromBody]ModifyGroupIn data)
        {
            var group = await context.Group.FindAsync(data.Id);
            if (group == null)
            {
                HttpContext.Response.StatusCode = 404;
                return "Group not found";
            }
            group.Name = data.Name;
            group.Description = data.Description;
            await context.SaveChangesAsync();
            return new GroupPreview(group);
        }

        [HttpPost, Route("detail"), ProducesResponseType(typeof(GroupDetailOut), 200)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<object> Detail([FromBody]GroupDetailIn data)
        {
            var group = await context.Group.FindAsync(data.Groupid);
            if (group == null)
            {
                HttpContext.Response.StatusCode = 404;
                return "Group not found";
            }
            
            return new GroupDetailOut(group);
        }        

        [HttpPost, Route("listGroup"), ProducesResponseType(typeof(IEnumerable<GroupPreview>), 200)]
        public async Task<object> ListGroup([FromBody]ListCount data)
        {
            var user = (User)HttpContext.Items["user"];
            var result = user.Roles.Select(r => r.Group).Select(g => new GroupPreview(g));
            if (data.Order == 0) result.OrderBy(r => r.Name);
            else if (data.Order == 1) result.OrderByDescending(r => r.Name);
            result.Skip(data.Start).Take(data.Count);
            return result;           
        }
        /*
        [HttpPost, Route("invite"), ProducesResponseType(200)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<object> Invite([FromBody]GroupInviteIn data)
        {
            var user = (User)HttpContext.Items["user"];
            var userPrivilege = user.Roles.FirstOrDefault(r => r.Group.Id.Equals(data.Groupid));
            if (userPrivilege == null || userPrivilege.Privilege != 1)
                return BadRequest("User must be group admin to add people.");
            var group = context.Group.FirstOrDefault(g => g.Id.Equals(data.Groupid));
            var addeduser = context.Users.FirstOrDefault(u => u.Id.Equals(data.Userid));
            if (group == null || addeduser == null) return NotFound("Group or added user not found.");
            var newrole = new Role
            {
                Id = Guid.NewGuid(),
                User = addeduser,
                Group = group,
                //add administrator
                Privilege = data.Permission
            };
            context.Roles.Add(newrole);
            await context.SaveChangesAsync();
            return Ok("Add member successfully!");
        }
        */

        [HttpPost, Route("changePrivilege"), ProducesResponseType(typeof(string), 200)]
        public async Task<object> ChangePrivilege([FromBody]GroupInviteIn data)
        {
            var user = (User)HttpContext.Items["user"];
            var userPrivilege = user.Roles.FirstOrDefault(r => r.Group.Id.Equals(data.Groupid));
            if (userPrivilege == null || userPrivilege.Privilege != 1)
                return Unauthorized("User must be group admin to change privilege.");
            var roletochange = context.Roles.FirstOrDefault(r => r.Group.Id.Equals(data.Groupid) && r.User.Id.Equals(data.Userid));
            if (roletochange == null) return NotFound($"User {data.Userid} is not a group member.");
            roletochange.Privilege = data.Permission;
            context.Entry(roletochange).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await context.SaveChangesAsync();
            return Ok("Change privilege successfully!");
        }

        [HttpPost, Route("removeUser"), ProducesResponseType(typeof(string), 200)]
        public async Task<object> RemoveUser([FromBody]RemoveUserIn data)
        {
            var user = (User)HttpContext.Items["user"];
            var userPrivilege = user.Roles.FirstOrDefault(r => r.Group.Id.Equals(data.Groupid));
            if (userPrivilege == null || (userPrivilege.Privilege != 1 && !user.Id.Equals(data.Userid)))
                return Unauthorized("User must be group admin or himself/herself to remove user.");
            context.Roles.Remove(context.Roles.FirstOrDefault(r => r.Group.Id.Equals(data.Groupid) && r.User.Id.Equals(data.Userid)));
            await context.SaveChangesAsync();
            return Ok("Remove user successfully!");       
        }
    }
}