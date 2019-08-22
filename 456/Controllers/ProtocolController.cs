using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc.Filters;
using bst.Model;
using Microsoft.EntityFrameworkCore;


namespace bst.Controllers
{
    [Route("protocol")]
    public class ProtocolController : AuthController
    {
        [AuthFilter,HttpGet,Route("get/{protocolid}"),ProducesResponseType(typeof(ProtocolPreview),200)]
        public async Task<object> getprotocol(Guid protocolid)
        {
            var p = await context.Protocols.FindAsync(protocolid);
            if (p.LockedUser==null)
            {
                p.LockedUser = u;
                context.Entry(p).State = EntityState.Modified;
            }
            if (p.LockedUser!=u)
            {
                return Unauthorized("resource locked by other user");
            }

            throw new NotImplementedException();
        }


        [HttpPost, Route("create"), ProducesResponseType(typeof(Guid), 200)]
        public async Task<object> Create([FromBody]CreateProtocol data)
        {

            throw new NotImplementedException();
        }


    }
}
