using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using bst.Model;
using System.IO;

namespace bst.Controllers
{
    [Route("FunctionalFile")]
    public class FunctionalFileController : BaseController
    {
        
        [HttpPost,Route("createChannel"),AuthFilter,PLockFilter]
        public async Task<object> createChannel([FromBody]ChannelData data)
        {
            var protocol = user.ProtocolUsers.FirstOrDefault(x => x.Protocol.Id.Equals(Request.Headers["protocolid"]));
            if (protocol == null) return Unauthorized("no participation found");
            var study = protocol.Protocol.Studies.FirstOrDefault(x => x.Id == data.studyID);
            var channel = data.toChannel(mapFile(protocol.Protocol.Id.ToString(),study.Id.ToString(),Guid.NewGuid().ToString()));
            context.Channels.Add(channel);
            context.FunctionalFiles.Add(channel.Parent);
            await context.SaveChangesAsync();
            FileStream fs = new FileStream(channel.Parent.FileName, FileMode.CreateNew);
            Guid uploadid = Guid.NewGuid();
            FileController.queue[uploadid] = fs;
            return uploadid;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="firstLayer"> protocolID</param>
        /// <param name="SecondLayer"> studyID</param>
        /// <param name="filename"> fileID</param>
        /// <returns></returns>
        public static string mapFile(string firstLayer,string SecondLayer,string filename)
        {
            if (string.IsNullOrEmpty(SecondLayer))
            {
                return $"./{firstLayer}/{filename}";
            }
            else
            {
                return $"./{firstLayer}/{SecondLayer}/{filename}";
            }
        }
        
    }

}