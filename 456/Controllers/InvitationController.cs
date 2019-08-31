using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using bst.Model;
using Microsoft.AspNetCore.Mvc.Filters;

namespace bst.Controllers
{
    [Route("invitation")]
    public class InvitationController : BaseController
    {

        [HttpPost,Route("invite"),AuthFilter]
        public async Task<object> Invite([FromBody]Invitation invitation)
        {
            if (invitation.Expiration <= System.DateTime.Now)
            {
                return BadRequest("expiration time invalid");
            }

            var user = (User)HttpContext.Items["user"];
            invitation.SentFrom = user;
            var group = await context.Group.FindAsync(invitation.GroupId);
            var protocol = await context.Protocols.FindAsync(invitation.ProtocolId);

            if (group!=null)
            {
                var role = user.Roles.FirstOrDefault(x => x.Group == group && x.Privilege==1);
                if (role==null)
                {
                    return Unauthorized();
                }
                invitation.Id = Guid.NewGuid();
                context.Invitations.Add(invitation);
                await context.SaveChangesAsync();
                return invitation.Id;
            }
            if (protocol!=null)
            {
                var groups = user.Roles.Where(x => x.Privilege == 1).Select(x=>x.Group);
                foreach (var g in groups)
                {
                    if (g.Protocols.Select(x=>x.Id).Contains(invitation.ProtocolId))
                    {
                        invitation.Id = Guid.NewGuid();
                        context.Invitations.Add(invitation);
                        await context.SaveChangesAsync();
                        return invitation.Id;
                    }
                }
                return Unauthorized();
            }
            return NotFound();
        }

        [HttpPost,Route("accept/{invitationid}"),AuthFilter]
        public async Task<object> Accept(Guid invitationid)
        {
            var invitation = await context.Invitations.FindAsync(invitationid);
            if (invitation.Expiration <= System.DateTime.Now)
            {
                return BadRequest("expiration time invalid");
            }

            var user = (User)HttpContext.Items["user"];
            var group = await context.Group.FindAsync(invitation.GroupId);
            var protocol = await context.Protocols.FindAsync(invitation.ProtocolId);

            if (group!=null)
            {
                context.Roles.Add(new Role
                {
                    Id = Guid.NewGuid(),
                    User = user,
                    Group = group,
                    Privilege = invitation.Privilege
                });
                foreach (var p in group.Protocols)
                {
                    context.ParticipateProtocols.Add(new ParticipateProtocol
                    {
                        Id = Guid.NewGuid(),
                        User = user,
                        Protocol = p,
                        Privilege = invitation.Privilege
                    });
                }
                context.Invitations.Remove(invitation);
                await context.SaveChangesAsync();
                return Accepted();
            }

            if (protocol!=null)
            {
                context.ParticipateProtocols.Add(new ParticipateProtocol
                {
                    Id = Guid.NewGuid(),
                    User = user,
                    Protocol = protocol,
                    Privilege = invitation.Privilege
                });
                context.Invitations.Remove(invitation);
                await context.SaveChangesAsync();
                return Accepted();
            }

            return BadRequest("no group or protocol");
        }


    }
}