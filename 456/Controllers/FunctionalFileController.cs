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
        
        [HttpPost,Route("createChannel"),AuthFilter,WriteLock]
        public async Task<object> createChannel([FromBody]ChannelData data)
        {
            var study = protocol.Studies.FirstOrDefault(x => x.Id == data.studyID);
            var channel = data.toChannel();
            channel.Parent.Study = study;
            channel.Parent.url = mapUrl(protocol.Id.ToString(), study.Id.ToString(), channel.Parent.Id.ToString());
            context.Channels.Add(channel);
            context.FunctionalFiles.Add(channel.Parent);
            await context.SaveChangesAsync();
            Directory.CreateDirectory(mapFile(protocol.Id.ToString(),study.Id.ToString(),""));
            FileStream fs = new FileStream(mapFile(protocol.Id.ToString(), study.Id.ToString(), channel.Parent.Id.ToString()), FileMode.CreateNew);
            Guid uploadid = Guid.NewGuid();
            FileController.queue[uploadid] = fs;
            return uploadid;
        }


        [HttpPost, Route("test/{filename}")]
        public async Task<object> test(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.CreateNew);
            Guid uploadid = Guid.NewGuid();
            FileController.queue[uploadid] = fs;
            return new { result = uploadid };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="firstLayer"> protocolID</param>
        /// <param name="SecondLayer"> studyID</param>
        /// <param name="filename"> fileID</param>
        /// <returns></returns>
        private static string mapFile(string firstLayer,string SecondLayer,string filename)
        {
            if (string.IsNullOrEmpty(SecondLayer))
            {
                return $"./wwwroot/files/{firstLayer}/ffiles/{filename}.dat";
            }
            else
            {
                return $"./wwwroot/files/{firstLayer}/ffiles/{SecondLayer}/{filename}.dat";
            }
        }
        private static string mapUrl(string firstLayer, string SecondLayer, string filename)
        {
            if (string.IsNullOrEmpty(SecondLayer))
            {
                return $"/files/{firstLayer}/ffiles/{filename}";
            }
            else
            {
                return $"/files/{firstLayer}/ffiles/{SecondLayer}/{filename}";
            }
        }

    }

}