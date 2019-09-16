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
            // group name should be unique and is not allowed to be empty 
            if (data.Name == null
                //|| data.Name.IndexOf(' ') >= 0                
                || context.Group.FirstOrDefault(g => data.Name.Equals(g.Name)) != null)
                return BadRequest("Group name not valid.");
            var group = new Group
            {
                Name = data.Name.Trim()
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


        [HttpPost, Route("detail"), ProducesResponseType(typeof(GroupDetailOut), 200)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public object Detail([FromBody]GroupName groupname)
        {
            var u = (User)HttpContext.Items["user"];
            var group = u.GroupUsers.FirstOrDefault(x => x.Group.Name.Equals(groupname.Name.Trim()));
            if (group == null)            
                return NotFound("group not found");            

            return new GroupDetailOut(group.Group);
        }        


        [HttpPost, Route("changerole"), ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<object> ChangePrivilege([FromBody]EditGroupMemberIn data)
        {
            var user = (User)HttpContext.Items["user"];
            var group = user.GroupUsers.FirstOrDefault(x => x.Group.Name.Equals(data.GroupName));
            if (group == null)
                return NotFound("group not found");
            var userGroupRelation = user.GroupUsers.FirstOrDefault(r => r.Group.Name.Equals(data.GroupName));
            if (userGroupRelation == null || userGroupRelation.Role != 1)
                return Unauthorized("User must be group manager to change member role.");

            //change target user's role to group 
            var roletochange = context.GroupUsers.FirstOrDefault(r => r.Group.Name.Equals(data.GroupName) && r.User.Email.Equals(data.UserEmail));
            if (roletochange == null) return NotFound($"User {data.UserEmail} is not a group member.");
            if (data.Role != 1 && data.Role != 2)
                roletochange.Role = 2;
            else
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
            if (group.Role > 1) return Unauthorized("you are not group manager");
            var target = context.Users.FirstOrDefault(x => x.Email.Equals(data.UserEmail));
            if (target == null)
                return NotFound("user to add doesn't exist");

            var privilege = data.Privilege;
            if (data.Privilege != 1 && data.Privilege != 2)
                privilege = 2;
           
            context.GroupUsers.Add(new GroupUser
            {
                Id = Guid.NewGuid(),
                User = target,
                Group = group.Group,
                Role = privilege
            });
            await context.SaveChangesAsync();
            return Ok("Add user to group successfully!");
        }

        [HttpPost, Route("removeuser"), ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<object> RemoveUser([FromBody]RemoveGroupUserIn data)
        {
            var user = (User)HttpContext.Items["user"];
            var group = user.GroupUsers.FirstOrDefault(x=>x.Group.Name.Equals(data.GroupName));
            if (group == null)
                return NotFound("Group doesn't exist");
            var target = context.Users.FirstOrDefault(x => x.Email.Equals(data.UserEmail));
            if (target == null)
                return NotFound("user to remove doesn't exist");
            var userGroupRelation = user.GroupUsers.FirstOrDefault(r => r.Group.Name.Equals(data.GroupName));
            if (userGroupRelation == null || (userGroupRelation.Role != 1 && !user.Email.Equals(data.UserEmail)))
                return Unauthorized("User must be group manager or himself/herself to remove user.");
            var relationToRemove = target.GroupUsers.FirstOrDefault(r => r.Group.Name.Equals(data.GroupName));
            if (relationToRemove == null) return NotFound("User to remove does not belong to the group.");
            context.GroupUsers.Remove(relationToRemove);
            await context.SaveChangesAsync();
            return Ok("Remove user from group successfully!");       
        }
    }
}