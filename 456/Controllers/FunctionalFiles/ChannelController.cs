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
            var channel = new Channel(data);
            channel.Study = study;
            
            //set parent url
            channel.Parent.url = mapUrl(protocol.Id.ToString(), study.Id.ToString(), channel.Parent.Id.ToString());

            //add channel and ff to database
            context.Channels.Add(channel);
            context.FunctionalFiles.Add(channel.Parent);
            history.HistoryEvent += $"create Channel {study.Id} {channel.Id}";
            await context.SaveChangesAsync();

            return FileController.createFunctionalFileQueueItem(channel,session,data.md5);
        }
    }
}
