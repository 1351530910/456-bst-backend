﻿using System;
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
    public class Uploadinfo
    {
        public string Uploadid { get; set; }
        public string Fid { get; set; }
    }
    [Route("file")]
    [ApiController]
    public class FileController : FunctionalFileController
    {
        public class QueueItem
        {
            public Guid Uploadid { get; set; }
            public FileStream Fs { get; set; }
            public Guid Sessionid { get; set; }
            public byte[] Md5 { get; set; }
        }

        UserDB context = new UserDB();

        public static List<QueueItem> q = new List<QueueItem>();
        private static Timer timer;

        static FileController()
        {
            timer = new Timer
            {
                AutoReset = true,
                Interval = 1800000
            };
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
            if ((item = q.FirstOrDefault(x => x.Sessionid == session.Sessionid && x.Uploadid == uploadid)) == null)
            {
                return NotFound("Upload ID not valid.");
            }

            //if not last then copy to file and done
            if (!last)
            {
                item.Fs.Write(buffer);
                return Ok("success");
            }

            //if last compute md5 of the file
            item.Fs.Flush();
            item.Fs.Position = 0;
            var md5 = MD5.Create().ComputeHash(item.Fs);

            //if equal then done
            if (md5.SequenceEqual(item.Md5)||Program.DEBUG)
            {
                item.Fs.Close();
                q.Remove(item);
                return Ok("success");
            }
            else    //else reset the fs pointer 
            {
                item.Fs.Position = 0;
                return Ok("checksum failed");
            }
        }

        [HttpPost,AuthFilter, Route("download/ffile/{studyID}/{fileID}/"),ReadLock]
        public async Task<object> DownloadFunctionalFile(string studyID,string fileID)
        {
            var path = mapFile(protocol.Id.ToString(), studyID, fileID,"data");
            var fs = new FileStream(path, FileMode.Open);
            return new FileStreamResult(fs, "application/octet-stream");
        }

        [HttpPost, AuthFilter, Route("download/afile/{subjectID}/{fileID}/"), ReadLock]
        public async Task<object> downloadAnatomicalFile(string subjectID, string fileID)
        {
            var path = mapFile(protocol.Id.ToString(), subjectID, fileID, "anat");
            var fs = new FileStream(path, FileMode.Open);
            return new FileStreamResult(fs, "application/octet-stream");
        }


        public static Guid createFunctionalFileQueueItem(object file, Session session, string md5, string filename)
        {
            var ff = (FunctionalFile)(file.GetType().GetProperty("Parent").GetValue(file));
            var study = (Study)(file.GetType().GetProperty("Study").GetValue(file));
            return createQueueItem(
                study.Protocol.Id.ToString(),
                study.Id.ToString(),
                ff.Id.ToString(),
                session.Sessionid,
                md5,
                "data"
                );
        }

        public static Guid createStudyQueueItem(Study study, Session session, string md5)
        {
            return createQueueItem(
                study.Protocol.Id.ToString(),
                study.Id.ToString(),
                study.Id.ToString(),
                session.Sessionid,
                md5,
                "data"
                );
        }

        public static Guid createSubjectQueueItem(Subject subject, Session session, string md5)
        {
            return createQueueItem(
                subject.Protocol.Id.ToString(),
                subject.Id.ToString(),
                subject.Id.ToString(),
                session.Sessionid,
                md5,
                "anat"
                );
        }



        public static Guid createQueueItem(string protocolid,string subjectOrStudyId, string fileid, Guid sessionid, string md5, string type)
        {
            Directory.CreateDirectory(mapFile(protocolid, subjectOrStudyId, "", type));
            FileStream fs = new FileStream(mapFile(protocolid, subjectOrStudyId, fileid, type), FileMode.CreateNew);

            Guid uploadid = Guid.NewGuid();
            q.Add(new QueueItem
            {
                Uploadid = uploadid,
                Fs = fs,
                Sessionid = sessionid,
                Md5 = System.Text.Encoding.ASCII.GetBytes(md5)
            });
            return uploadid;
        }
    }
}
