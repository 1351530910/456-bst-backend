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
            //create channel data
            var study = protocol.Studies.FirstOrDefault(x => x.Id == data.studyID);
            var channel = data.toChannel();
            channel.Study = study;
            
            //set parent url
            channel.Parent.url = mapUrl(protocol.Id.ToString(), study.Id.ToString(), channel.Parent.Id.ToString());

            //add channel and ff to database
            context.Channels.Add(channel);
            context.FunctionalFiles.Add(channel.Parent);
            await context.SaveChangesAsync();

            //create the actual file
            Directory.CreateDirectory(mapFile(protocol.Id.ToString(), study.Id.ToString(), ""));
            FileStream fs = new FileStream(mapFile(protocol.Id.ToString(), study.Id.ToString(), channel.Parent.Id.ToString()), FileMode.CreateNew);

            
            history.HistoryEvent += $"create Channel {study.Id} {channel.Id}";

            //create upload task
            Guid uploadid = Guid.NewGuid();
            FileController.q.Add(new FileController.QueueItem
            {
                uploadid = uploadid,
                fs = fs,
                sessionid = session.Sessionid,
                md5 = System.Text.Encoding.ASCII.GetBytes(data.md5)
            });

            
            return uploadid;
        }
    }
}
