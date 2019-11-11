using System;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using bst.Model;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.Timers;
using System.Security.Cryptography;

namespace bst.Controllers
{
    [Route("file")]
    [ApiController]
    public class FileController : FunctionalFileController
    {
        public class QueueItem
        {
            public Guid uploadid { get; set; }
            public FileStream fs { get; set; }
            public Guid sessionid { get; set; }
            public byte[] md5 { get; set; }
        }

        UserDB context = new UserDB();

        public static List<QueueItem> q = new List<QueueItem>();
        private static Timer timer;

        static FileController()
        {
            timer = new Timer();
            timer.AutoReset = true;
            timer.Interval = 1800000;
            timer.Elapsed += clearQueue;
        }

        private static void clearQueue(object sender, ElapsedEventArgs e)
        {
            var t = System.DateTime.Now.AddMinutes(-AuthFilter.EXPIRETIME);
            var expired = AuthFilter.sessions.Where(x => x.LastActive < t);
        }

        [HttpPost, AuthFilter,Route("upload/{uploadid}/{last}")]
        public async Task<object> upload(Guid uploadid, bool last)
        {
            if (HttpContext.Request.ContentLength <= 0)
            {
                return NoContent();
            }
            
            //read buffer
            byte[] buffer = new byte[(int)HttpContext.Request.ContentLength];
            await HttpContext.Request.Body.ReadAsync(buffer, 0, buffer.Length);

            //check if upload item still available
            QueueItem item;
            if ((item = q.FirstOrDefault(x => x.sessionid == session.Sessionid && x.uploadid == uploadid)) == null)
            {
                return NotFound("Upload ID not valid.");
            }

            //copy to file
            item.fs.Write(buffer); 

            //if not last then done
            if (!last)
            {
                return Ok("success");
            }

            //if last compute md5 of the file
            item.fs.Flush();
            item.fs.Position = 0;
            var md5 = MD5.Create().ComputeHash(item.fs);

            //if equal then done
            if (md5.SequenceEqual(item.md5))
            {
                item.fs.Close();
                q.Remove(item);
                return Ok("success");
            }
            else    //else reset the fs pointer 
            {
                item.fs.Position = 0;
                return Ok("checksum failed");
            }
        }
        [HttpGet,AuthFilter,ReadLock,Route("download/{studyID}/{fileID}/{start}/{count}")]
        public async Task<object> download(string studyID,string fileID,long start,int count)
        {
            var path = mapFile(protocol.Id.ToString(), studyID, fileID);
            var fs = new FileStream(path, FileMode.Open);
            var bytes = new byte[count];
            fs.Position = start;
            await fs.ReadAsync(bytes, 0, count);
            return new FileContentResult(bytes,"application/octet-stream");
        }


    }
}
