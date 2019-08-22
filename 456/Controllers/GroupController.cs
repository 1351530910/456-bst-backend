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
    public class GroupController : AuthController
    {
        [HttpPost,Route("create"),ProducesResponseType(typeof(GroupPreview),200)]
        public async Task<object> createGroup([FromBody]CreateGroupIn data)
        {
            var group = new Group
            {
                id = Guid.NewGuid(),
                name = data.name,
                description = data.description
            };
            var role = new Role
            {
                id = Guid.NewGuid(),
                user = u,
                group = group,
                priviledge = 0
            };
            context.group.Add(group);
            context.roles.Add(role);
            await context.SaveChangesAsync();
            return Ok(new GroupPreview(group));
        }


        [HttpPost, Route("modify"), ProducesResponseType(typeof(GroupPreview), 200)]
        public async Task<object> modify([FromBody]ModifyGroupIn data)
        {

            throw new NotImplementedException();
        }

        [HttpPost, Route("detail"), ProducesResponseType(typeof(ProtocolPreview), 200)]
        public async Task<object> detail([FromBody]GroupDetailIn data)
        {

            throw new NotImplementedException();
        }

        [HttpPost, Route("listGroup"), ProducesResponseType(typeof(IEnumerable<GroupPreview>), 200)]
        public async Task<object> listGroup([FromBody]ListCount data)
        {

            throw new NotImplementedException();
        }

        [HttpPost, Route("invite"), ProducesResponseType(200)]
        public async Task<object> invite([FromBody]GroupInviteIn data)
        {

            throw new NotImplementedException();
        }


        [HttpPost, Route("changePriviledge"), ProducesResponseType(typeof(string), 200)]
        public async Task<object> changePriviledge([FromBody]GroupInviteIn data)
        {

            throw new NotImplementedException();
        }

        [HttpPost, Route("removeUser"), ProducesResponseType(typeof(string), 200)]
        public async Task<object> removeUser([FromBody]RemoveUserIn data)
        {

            throw new NotImplementedException();
        }

        
    }
}