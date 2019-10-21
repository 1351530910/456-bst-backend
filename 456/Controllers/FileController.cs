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
        public struct QueueItem
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
        }

        private static void clearQueue(object sender, ElapsedEventArgs e)
        {
            var t = System.DateTime.Now.AddMinutes(-AuthFilter.EXPIRETIME);
            var expired = AuthFilter.sessions.Where(x => x.LastActive < t);
            
        }


        [HttpPost, Route("upload/{uploadid}/{last}")]
        public async Task<object> upload(Guid uploadid, bool last)
        {

            if (HttpContext.Request.Form.Files.Count != 1) return BadRequest("number of file wrong");
            if (queue.ContainsKey(uploadid))
            {
                await HttpContext.Request.Form.Files.First().CopyToAsync(queue[uploadid]);
                if (last)
                {
                    queue[uploadid].Flush();
                    queue[uploadid].Close();
                    queue.Remove(uploadid);
                }
            }
            return Ok();
        }



        [HttpPost, Route("testupload/{uploadid}/{last}")]
        public async Task<object> testupload(Guid uploadid, bool last)
        {
            if (HttpContext.Request.ContentLength > 0)
            {
                byte[] buffer = new byte[(int)HttpContext.Request.ContentLength];
                await HttpContext.Request.Body.ReadAsync(buffer, 0, buffer.Length);
                if (queue.ContainsKey(uploadid))
                {
                    queue[uploadid].Write(buffer);
                    if (last)
                    {
                        queue[uploadid].Flush();
                        queue[uploadid].Close();
                        queue.Remove(uploadid);
                    }
                    return Ok();
                }
                return NotFound("Upload ID not valid.");
            }
            return NoContent();
           
        }

    }
}
