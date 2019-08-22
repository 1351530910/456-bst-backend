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
                priviledge = 1
            };
            context.group.Add(group);
            context.roles.Add(role);
            await context.SaveChangesAsync();
            return new GroupPreview(group);
        }


        [HttpPost, Route("modify"), ProducesResponseType(typeof(GroupPreview), 200)]
        public async Task<object> Modify([FromBody]ModifyGroupIn data)
        {

            throw new NotImplementedException();
        }

        [HttpPost, Route("detail"), ProducesResponseType(typeof(ProtocolPreview), 200)]
        public async Task<object> Detail([FromBody]GroupDetailIn data)
        {

            throw new NotImplementedException();
        }

        [HttpPost, Route("listGroup"), ProducesResponseType(typeof(IEnumerable<GroupPreview>), 200)]
        public async Task<object> ListGroup([FromBody]ListCount data)
        {

            throw new NotImplementedException();
        }

        [HttpPost, Route("invite"), ProducesResponseType(200)]
        public async Task<object> Invite([FromBody]GroupInviteIn data)
        {

            throw new NotImplementedException();
        }


        [HttpPost, Route("changePriviledge"), ProducesResponseType(typeof(string), 200)]
        public async Task<object> ChangePriviledge([FromBody]GroupInviteIn data)
        {

            throw new NotImplementedException();
        }

        [HttpPost, Route("removeUser"), ProducesResponseType(typeof(string), 200)]
        public async Task<object> RemoveUser([FromBody]RemoveUserIn data)
        {

            throw new NotImplementedException();
        }

        
    }
}