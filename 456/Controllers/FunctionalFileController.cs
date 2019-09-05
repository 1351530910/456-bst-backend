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
        public static Dictionary<Guid, string> UploadQueue = new Dictionary<Guid, string>();

        [HttpPost,Route("upload/{itemid}"),AuthFilter]
        public async Task<object> upload(Guid itemid)
        {
            var user = (User)HttpContext.Items["user"];
            var session = (Session)HttpContext.Items["session"];

            if (HttpContext.Request.Form.Files.Count!=1)
            {
                return BadRequest("unsupported number of files");
            } 

            if (!UploadQueue.ContainsKey(itemid))
            {
                return BadRequest("upload request not found");
            }

            var file = HttpContext.Request.Form.Files[0];

            await file.CopyToAsync(new FileStream(UploadQueue[itemid], FileMode.CreateNew));

            UploadQueue.Remove(itemid);

            return Ok();
        }
    }
}