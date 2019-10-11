using bst.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace bst.Controllers
{
    [Route("study")]
    [AuthFilter]
    public class StudyController : BaseController
    {
        /// <summary>
        /// get a study
        /// </summary>
        /// <param name="studyid"></param>
        /// <returns></returns>
        [HttpGet, Route("get/{studyid}"), ProducesResponseType(typeof(StudyData), 200)]
        public async Task<object> GetSubject(Guid studyid)
        {
            

            var study = await context.Studies.FindAsync(studyid);
            if (study == null) return NotFound("Study doesn't exist.");

            var participation = user.ProtocolUsers.FirstOrDefault(x => x.Protocol.Id.Equals(study.Subject.Protocol.Id));
            if (participation == null) return NotFound("You don't have access to this study.");

            return new StudyData(study);
        }

        /// <summary>
        /// create a study
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost, Route("create"), ProducesResponseType(typeof(Guid), 200)]
        public async Task<object> CreateStudy([FromBody]StudyData data)
        {
            
            
            var participation = user.ProtocolUsers.FirstOrDefault(x => x.Protocol.Id.Equals(data.ProtocolId));

            if (participation == null) return Unauthorized("You don't have access to this protocol.");

            Study study = new Study
            {
                Id = Guid.NewGuid(),
                Filename = data.Filename,
                Name = data.Name,
                Condition = data.Condition,
                DateOfStudy = data.DateOfStudy,
                IChannel = data.IChannel,
                IHeadModel = data.IHeadModel,
                Subject = context.Subjects.Find(data.SubjectId),
                LastUpdate = System.DateTime.Now
            };
            context.Studies.Add(study);
            await context.SaveChangesAsync();
            return study.Id;
        }
    }
}
