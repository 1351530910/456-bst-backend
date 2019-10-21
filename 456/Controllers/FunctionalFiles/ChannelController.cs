using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using bst.Model;
using System.IO;

namespace bst.Controllers.FunctionalFiles
{
    public class ChannelController:FunctionalFileController
    {
        [HttpPost, Route("createChannel"), AuthFilter, WriteLock]
        public async Task<object> createChannel([FromBody]ChannelData data)
        {
            var study = protocol.Studies.FirstOrDefault(x => x.Id == data.studyID);
            var channel = data.toChannel();
            channel.Study = study;
            
            channel.Parent.url = mapUrl(protocol.Id.ToString(), study.Id.ToString(), channel.Parent.Id.ToString());
            context.Channels.Add(channel);
            context.FunctionalFiles.Add(channel.Parent);
            await context.SaveChangesAsync();
            Directory.CreateDirectory(mapFile(protocol.Id.ToString(), study.Id.ToString(), ""));
            FileStream fs = new FileStream(mapFile(protocol.Id.ToString(), study.Id.ToString(), channel.Parent.Id.ToString()), FileMode.CreateNew);
            Guid uploadid = Guid.NewGuid();
            FileController.q.Add(new FileController.QueueItem
            {
                uploadid = uploadid,
                fs = fs,
                sessionid = session.Sessionid
            });
            return uploadid;
        }
    }
}
