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
    [ApiController,AuthFilter]
    public class FileController : BaseController
    {
        UserDB context = new UserDB();
        const int expireTime = 30;
        public class UploadInfo
        {
            public Guid uploadID;
            public Guid protocolID;
            public Guid functionalFileID;
            public string filename;
            public DateTime createtime;
            public string comment;
        }

        public static List<UploadInfo> queue = new List<UploadInfo>();

        [Route("test/{uploadid}")]
        public async Task<object> test(Guid uploadid)
        {
            if (HttpContext.Request.Form.Files.Count != 1) return BadRequest("number of file wrong");
            await HttpContext.Request.Form.Files.First().CopyToAsync(new FileStream($"files/{uploadid}", FileMode.CreateNew));
            
            return Ok();
        }

        [Route("ffile/{uploadid}")]
        public async Task<object> uploadFFile(Guid uploadid)
        {
            if (HttpContext.Request.Form.Files.Count != 1) return BadRequest("number of file wrong");
            var uploadinfo = queue.Where(x => x.uploadID == uploadid).FirstOrDefault();
            if (uploadinfo == null) return NotFound("invalid uploadID");
            if ((System.DateTime.Now- uploadinfo.createtime).Minutes > expireTime)
            {
                queue.Remove(uploadinfo);
                return BadRequest("expired");
            }
            var file = await context.FunctionalFiles.FindAsync(uploadinfo.filename);
            Directory.CreateDirectory($"files/{uploadinfo.protocolID}/FunctionalFiles/");
            await HttpContext.Request.Form.Files.First().CopyToAsync(new FileStream($"files/{uploadinfo.protocolID}/FunctionalFiles/{file.FileName}",FileMode.CreateNew));
            context.Histories.Add(new History
            {
                Id = Guid.NewGuid(),
                TimeStamp = System.DateTime.Now,
                HistoryEvent = uploadinfo.comment,
                FunctionalFile = file,
                protocol = file.Study.Protocol
            });
            await context.SaveChangesAsync();
            return Ok();
        }

        [Route("afile/{uploadid}")]
        public async Task<object> uploadAFile(Guid uploadid)
        {
            if (HttpContext.Request.Form.Files.Count != 1) return BadRequest("number of file wrong");
            var uploadinfo = queue.Where(x => x.uploadID == uploadid).FirstOrDefault();
            if (uploadinfo == null) return NotFound("invalid uploadID");
            if ((System.DateTime.Now - uploadinfo.createtime).Minutes > expireTime)
            {
                queue.Remove(uploadinfo);
                return BadRequest("expired");
            }
            var file = await context.AnatomicalFiles.FindAsync(uploadinfo.filename);
            Directory.CreateDirectory($"files/{uploadinfo.protocolID}/AnatomicalFiles/");
            await HttpContext.Request.Form.Files.First().CopyToAsync(new FileStream($"files/{uploadinfo.protocolID}/AnatomicalFiles/{file.FileName}", FileMode.CreateNew));
            context.Histories.Add(new History
            {
                Id = Guid.NewGuid(),
                TimeStamp = System.DateTime.Now,
                HistoryEvent = uploadinfo.comment,
                AnatomicalFile = file,
                protocol = file.Subject.Protocol
            });
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
