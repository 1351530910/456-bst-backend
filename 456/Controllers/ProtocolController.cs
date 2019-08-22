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
    public class ProtocolController
    {
        [AuthFilter,HttpGet,Route("get/{protocolid}"),ProducesResponseType(typeof(ProtocolPreview),200)]
        public async Task<object> Getprotocol(Guid protocolid)
        {

            throw new NotImplementedException();
        }


        [HttpPost, Route("create"), ProducesResponseType(typeof(Guid), 200)]
        public async Task<object> Create([FromBody]CreateProtocol data)
        {

            throw new NotImplementedException();
        }


    }
}
