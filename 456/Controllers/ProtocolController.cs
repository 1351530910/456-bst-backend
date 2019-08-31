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
    [AuthFilter]
    public class ProtocolController: BaseController
    {
        [HttpGet,Route("get/{protocolid}"),ProducesResponseType(typeof(ProtocolPreview),200)]
        public async Task<object> Getprotocol(Guid protocolid)
        {
            var user = (User)HttpContext.Items["user"];

            var participation = user.Protocols.Where(x => x.Protocol.Id.Equals(protocolid)).FirstOrDefault();
            if (participation!=null)
            {
                return new ProtocolPreview(participation.Protocol, participation.Privilege);
            }
            else
            {
                return NotFound();
            }
        }

#warning default values?
        [HttpPost, Route("create"), ProducesResponseType(typeof(Guid), 200)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<object> Create([FromBody]CreateProtocol data)
        {
            var user = (User)HttpContext.Items["user"];
            var group = user.Roles.Where(x => x.Group.Id.Equals(data.GroupId)).FirstOrDefault();
            if (group == null)
            {
                HttpContext.Response.StatusCode = 404;
                return "User doesn't belong to this group or the group doesn't exist";
            }
            var protocol = new Protocol
            {
                Id = Guid.NewGuid(),
                Name = data.Name,
                Isprivate = data.Isprivate,
                Comment = data.Comment,
                IStudy = data.Istudy ?? 0,
                UseDefaultAnat = data.Usedefaultanat ?? true,
                UseDefaultChannel = data.Usedefaultchannel ?? true,
                Group = group.Group
            };
            context.Protocols.Add(protocol);
            foreach (var u in group.Group.Users)
            {
                context.ParticipateProtocols.Add(new ParticipateProtocol
                {
                    Id = Guid.NewGuid(),
                    User = u.User,
                    Protocol = protocol,
                    Privilege = u.Privilege
                });
            }
            await context.SaveChangesAsync();
            return protocol.Id;
        }

    }
}
