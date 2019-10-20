using System;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using bst.Model;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;

namespace bst.Controllers
{
    [Route("file")]
    [ApiController]
    public class FileController : BaseController
    {
        UserDB context = new UserDB();
        const int expireTime = 30;
        public static Dictionary<Guid, FileStream> queue = new Dictionary<Guid, FileStream>();


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
