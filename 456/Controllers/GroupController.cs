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
            
            // group name should be unique and is not allowed to be empty 
            if (data.Name == null || context.Group.FirstOrDefault(g => data.Name.Equals(g.Name)) != null)
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


        [HttpPost, Route("detail"), ProducesResponseType(typeof(GroupPreview), 200)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public object Detail([FromBody]GroupName groupname)
        {
            var group = user.GroupUsers.FirstOrDefault(x => x.Group.Name.Equals(groupname.Name.Trim()));
            if (group == null)
                return NotFound($"You do not have a group named {groupname.Name}.");

            return new GroupPreview(group.Group);
        }        


        [HttpPost, Route("changerole"), ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<object> ChangePrivilege([FromBody]EditGroupMemberIn data)
        {
            //check user's participation
            var group = user.GroupUsers.FirstOrDefault(x => x.Group.Name.Equals(data.GroupName));
            if (group == null)
                return NotFound($"You do not have a group named {data.GroupName}.");

            //check if user actually has the right 
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

            //save to database
            context.Entry(roletochange).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await context.SaveChangesAsync();

            return Ok("Change privilege successfully!");
        }

        [HttpPost, Route("adduser"), ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<object> AddUser([FromBody]AddGroupUserIn data)
        {
            //find the target group
            var group = user.GroupUsers.FirstOrDefault(x => x.Group.Name.Equals(data.GroupName));
            if (group == null)
                return NotFound($"You do not have a group named {data.GroupName}.");

            //check user's priviledge
            if (group.Role > 1) return Unauthorized("You are not group manager.");

            //check if target user exists
            var target = context.Users.FirstOrDefault(x => x.Email.Equals(data.UserEmail));
            if (target == null)
                return NotFound("Target user doesn't exist.");

            //check if target already exists in group
            if (group.Group.Members.FirstOrDefault(m => data.UserEmail.Equals(m.User.Email)) != null)
                return Ok("user already in group");

            //add user to the group
            var privilege = data.Privilege;
            if (data.Privilege != 1 && data.Privilege != 2)
                privilege = 2;
            
            //save to database
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
            //check group exists
            var group = user.GroupUsers.FirstOrDefault(x=>x.Group.Name.Equals(data.GroupName));
            if (group == null)
                return NotFound($"You do not have a group named {data.GroupName}.");

            //check target is in group
            var target = context.Users.FirstOrDefault(x => x.Email.Equals(data.UserEmail));
            if (target == null)
                return NotFound("user to remove doesn't exist");

            //check user's priviledge
            var userGroupRelation = user.GroupUsers.FirstOrDefault(r => r.Group.Name.Equals(data.GroupName));
            if (userGroupRelation == null || (userGroupRelation.Role != 1 && !user.Email.Equals(data.UserEmail)))
                return Unauthorized("User must be group manager or himself/herself to remove user.");

            //find the target to remove
            var relationToRemove = target.GroupUsers.FirstOrDefault(r => r.Group.Name.Equals(data.GroupName));
            if (relationToRemove == null) return NotFound("User to remove does not belong to the group.");

            //remove target and save changes
            context.GroupUsers.Remove(relationToRemove);
            await context.SaveChangesAsync();


            return Ok("Remove user from group successfully!");       
        }
    }
}