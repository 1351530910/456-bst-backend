
using bst.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace bst.Controllers
{
    [Route("subject")]
    [ApiController]
    [AuthFilter]
    public class SubjectController : BaseController
    {
        /// <summary>
        /// get a subject
        /// </summary>
        /// <param name="subjectid"></param>
        /// <returns></returns>
        [HttpGet, Route("get/{subjectid}"), ProducesResponseType(typeof(SubjectData), 200)]
        public async Task<object> GetSubject(Guid subjectid)
        {
            
            

            var subject = await context.Subjects.FindAsync(subjectid);
            if (subject == null) return NotFound("Subject doesn't exist.");

            var participation = user.ProtocolUsers.FirstOrDefault(x => x.Protocol.Id.Equals(subject.Protocol.Id));
            if (participation == null) return NotFound("You don't have access to this subject.");


            return new SubjectData(subject);
        }

        /// <summary>
        /// create a subject
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost, Route("create"), ProducesResponseType(typeof(Guid), 200)]
        public async Task<object> CreateSubject([FromBody]SubjectData data)
        {
            
            

            var participation = user.ProtocolUsers.FirstOrDefault(x => x.Protocol.Id.Equals(data.ProtocolId));
            if (participation == null) return NotFound("You don't have access to this protocol.");
            Subject subject = new Subject
            {
                Id = Guid.NewGuid(),
                Comment = data.Comment,
                Filename = data.Filename,
                Name = data.Filename,
                UseDefaultAnat = data.UseDefaultAnat,
                UseDefaultChannel = data.UseDefaultChannel,
                IAnatomy = data.IAnatomy,
                IScalp = data.IScalp,
                ICortex = data.ICortex,
                IInnerSkull = data.IInnerSkull,
                IOuterSkull = data.IOuterSkull,
                IOther = data.IOther,
                Protocol = participation.Protocol,
                LastUpdate = System.DateTime.Now
            };
            context.Subjects.Add(subject);
            await context.SaveChangesAsync();
            return subject.Id;
        }


        
    }
}


