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
            if (md5.SequenceEqual(item.md5)||Program.DEBUG)
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

        [HttpPost,AuthFilter, Route("download/{studyID}/{fileID}"),ReadLock]
        public async Task<object> download(string studyID,string fileID,long start,int count)
        {
            var path = mapFile(protocol.Id.ToString(), studyID, fileID);
            var fs = new FileStream(path, FileMode.Open);
            return new FileStreamResult(fs, "application/octet-stream");
        }
        public static Guid createFunctionalFileQueueItem(object file, Session session, string md5)
        {
            var ff = (FunctionalFile)(file.GetType().GetProperty("Parent").GetValue(file));
            var study = (Study)(file.GetType().GetProperty("Study").GetValue(file));
            return createQueueItem(
                study.Protocol.Id.ToString(),
                study.Id.ToString(),
                ff.Id.ToString(),
                session.Sessionid,
                md5
                );
        }
        public static Guid createQueueItem(string firstLayer,string secondLayer,string filename,Guid sessionid,string md5)
        {
            Directory.CreateDirectory(mapFile(firstLayer, secondLayer, ""));
            FileStream fs = new FileStream(mapFile(firstLayer, secondLayer, filename), FileMode.CreateNew);

            Guid uploadid = Guid.NewGuid();
            var md = System.Text.Encoding.ASCII.GetBytes(md5);
            q.Add(new QueueItem
            {
                uploadid = uploadid,
                fs = fs,
                sessionid = sessionid,
                md5 = System.Text.Encoding.ASCII.GetBytes(md5)
            });
            return uploadid;
        }
    }
}
