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

namespace bst.Controllers
{
    [Route("file")]
    [ApiController]
    public class FileController : BaseController
    {
        public class QueueItem
        {
            public Guid uploadid { get; set; }
            public FileStream fs { get; set; }
            public Guid sessionid { get; set; }
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
            AuthFilter.sessions.Add(new Session
            {
                Sessionid = Guid.Empty,
                Userid = Guid.Empty,
                LastActive = System.DateTime.Now
            });
            q.Add(new QueueItem
            {
                uploadid = Guid.Empty,
                sessionid = Guid.Empty,
                fs = new FileStream("test.bin", FileMode.CreateNew)
            });
        }

        private static void clearQueue(object sender, ElapsedEventArgs e)
        {
            var t = System.DateTime.Now.AddMinutes(-AuthFilter.EXPIRETIME);
            var expired = AuthFilter.sessions.Where(x => x.LastActive < t);
            
        }

        [HttpPost, AuthFilter,Route("upload/{uploadid}/{last}")]
        public async Task<object> testupload(Guid uploadid, bool last)
        {
            if (HttpContext.Request.ContentLength > 0)
            {
                byte[] buffer = new byte[(int)HttpContext.Request.ContentLength];
                await HttpContext.Request.Body.ReadAsync(buffer, 0, buffer.Length);
                QueueItem item;
                if ((item = q.FirstOrDefault(x=>x.sessionid==session.Sessionid&&x.uploadid == uploadid))!=null)
                {
                    item.fs.Write(buffer);
                    if (last)
                    {
                        item.fs.Flush();
                        item.fs.Close();
                        q.Remove(item);
                    }
                    return Ok();
                }
                return NotFound("Upload ID not valid.");
            }
            return NoContent();
           
        }

    }
}
