#pragma warning disable CS1701 // Assuming assembly reference matches identity

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using bst.Model;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace bst.Controllers
{
    public class UserController : Controller
    {
        private UserDB context = new UserDB();


        [HttpGet,Route("")]
        public object Index()
        {
            return "success";
        }


        [HttpGet,Route("listuser")]
        public async Task<List<User>> List()
        {
            return await context.users.ToListAsync();
        }

        [HttpPost,Route("login")]
        [ProducesResponseType(typeof(LoginOut),200)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<object> login([FromBody]LoginIn data)
        {
            var user = await context.users.Where(x => x.email == data.email && x.password == data.password).FirstOrDefaultAsync();
            if (user==null)
            {
                HttpContext.Response.StatusCode = 401;
                return "login failed";
            }
            user.sessionid = Guid.NewGuid();
            user.deviceid = data.deviceid;
            context.Entry(user).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return new LoginOut
            {
                sessionid = user.sessionid
            };
            
        }


        [HttpPost,Route("createuser")]
        [ProducesResponseType(typeof(CreateUserOut),200)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<object> Create([FromBody]CreateUserIn user)
        {
            if (!ModelState.IsValid||user==null)
            {
                if (user==null)
                {
                    return BadRequest("received no package,, recheck frontend");
                }
                else
                {
                    return BadRequest(ModelState);
                }
                
            }

            var u = new User
            {
                id = Guid.NewGuid(),
                FirstName = user.FirstName,
                LastName = user.LastName,
                email = user.email,
                password = user.password,
                sessionid = Guid.NewGuid(),
                deviceid = user.deviceid
            };
            
            context.users.Add(u);
            await context.SaveChangesAsync();
            return new CreateUserOut
            {
                sessionid = u.sessionid,
                firstname = u.FirstName,
                lastname = u.LastName,
                email = u.email
            };
        }



        [HttpGet, Route("getone")]
        public object GetSingleRow([FromQuery] string type, [FromQuery] Guid id)
        {
            switch (type)
            {
                case "protocol":
                    return context.Protocols.SingleOrDefault(x => x.id.Equals(id));
                case "subject":
                    return context.Subjects.SingleOrDefault(x => x.id.Equals(id));
                case "study":
                    return context.Studies.SingleOrDefault(x => x.id.Equals(id));
                case "history":
                    return context.Histories.SingleOrDefault(x => x.id.Equals(id));
                case "functionalfile":
                    return context.FunctionalFiles.SingleOrDefault(x => x.id.Equals(id));
                case "channel":
                    return context.Channels.SingleOrDefault(x => x.id.Equals(id));
                case "timefreq":
                    return context.TimeFreqs.SingleOrDefault(x => x.id.Equals(id));
                case "stat":
                    return context.Stats.SingleOrDefault(x => x.id.Equals(id));
                case "headmodel":
                    return context.HeadModels.SingleOrDefault(x => x.id.Equals(id));
                case "result":
                    return context.Results.SingleOrDefault(x => x.id.Equals(id));
                case "recording":
                    return context.Recordings.SingleOrDefault(x => x.id.Equals(id));
                case "matrix":
                    return context.Matrices.SingleOrDefault(x => x.id.Equals(id));
                default:
                    throw new Exception("Type not valid");
            }
        }

        [HttpGet, Route("protocol")]
        public async Task<List<Protocol>> ListProtocol()
        {
            return await context.Protocols.ToListAsync();
        }
        [HttpPost, Route("protocol")]
        public async Task<object> CreateProtocol([FromBody]ProtocolData protocol)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            //save 
            var result = ConvertProtocol(protocol);
            context.Protocols.Add(result);
            await context.SaveChangesAsync();
            //return
            return result;
        }
        [HttpPut, Route("protocol")]
        public async Task<object> UpdateProtocol([FromQuery] Guid id, [FromBody] ProtocolData item)
        {
            var protocol = ConvertProtocol(item);
            var entity = context.Protocols.Find(id);
            if (entity == null) throw new Exception("Update nothing.");

            context.Entry(entity).CurrentValues.SetValues(item);
            await context.SaveChangesAsync();
            return entity;
        }
        public Protocol ConvertProtocol(ProtocolData protocol)
        {
            return new Protocol
            {
                Comment = protocol.Comment,
                IStudy = protocol.IStudy,
                UseDefaultAnat = protocol.UseDefaultAnat,
                UseDefaultChannel = protocol.UseDefaultChannel,
                IsLocked = protocol.IsLocked,
                LockedUser = context.users.Where(u => u.id.Equals(protocol.LockedUserId)).FirstOrDefault()
            };
        }



        [HttpGet, Route("subject")]
        public async Task<List<Subject>> ListSubject()
        {
            return await context.Subjects.ToListAsync();
        }
        [HttpPost, Route("subject")]
        public async Task<object> CreateSubject([FromBody]SubjectData subject)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var protocol = context.Protocols.Where(p => p.id.Equals(subject.ProtocolId)).FirstOrDefault();
            if (protocol == null) return new Exception("Subject must belong to a valid protocol.");
            //save 
            var result = new Subject
            {
                Comment = subject.Comment,
                Filename = subject.Filename,
                Name = subject.Name,
                UseDefaultAnat = subject.UseDefaultAnat,
                UseDefaultChannel = subject.UseDefaultChannel,
                IAnatomy = subject.IAnatomy,
                IScalp = subject.IScalp,
                ICortex = subject.ICortex,
                IInnerSkull = subject.IInnerSkull,
                IOuterSkull = subject.IOuterSkull,
                IOther = subject.IOther,
                Protocol = protocol
            };
            context.Subjects.Add(result);
            await context.SaveChangesAsync();
            //return
            return result;
        }



        [HttpGet, Route("study")]
        public async Task<List<Study>> ListStudy()
        {
            return await context.Studies.ToListAsync();
        }
        [HttpPost, Route("study")]
        public async Task<object> CreateSubject([FromBody]StudyData study)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var protocol = context.Protocols.Where(p => p.id.Equals(study.ProtocolId)).FirstOrDefault();
            if (protocol == null) return new Exception("Study must belong to a valid protocol.");
            var subject = context.Subjects.Where(s => s.id.Equals(study.SubjectId)).FirstOrDefault();
            if (subject == null) return new Exception("Study must belong to a valid subject.");
            if (!protocol.id.Equals(subject.Protocol?.id)) return new Exception("The subject must belong to the protocal you input.");
            //save 
            var result = new Study
            {
                Filename = study.Filename,
                Name = study.Name,
                Condition = study.Condition,
                DateOfStudy = study.DateOfStudy,
                IChannel = study.IChannel,
                IHeadModel = study.IHeadModel,        
                Protocol = protocol,
                Subject = subject
            };
            context.Studies.Add(result);
            await context.SaveChangesAsync();
            //return
            return result;
        }


        [HttpGet, Route("history")]
        public async Task<List<History>> ListHistory()
        {
            return await context.Histories.ToListAsync();
        }
        [HttpPost, Route("history")]
        public async Task<object> CreateHistory([FromBody]HistoryData history)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var functionalfile = context.FunctionalFiles.Where(f => f.id.Equals(history.FunctionalFileId)).FirstOrDefault();
            var anatomicalfile = context.anatomicalFiles.Where(a => a.id.Equals(history.AnatomicalFileId)).FirstOrDefault();
            if (functionalfile == null && anatomicalfile == null)
                return new Exception("The history must belong to either a valid functional file or a valid anatomical file");            
            //save 
            var result = new History
            {
               TimeStamp = history.TimeStamp,
               Type = history.Type,
               HistoryEvent = history.HistoryEvent,
               FunctionalFile = functionalfile,
               AnatomicalFile = anatomicalfile
            };
            context.Histories.Add(result);
            await context.SaveChangesAsync();
            //return
            return result;
        }


        [HttpGet, Route("functionalfile")]
        public async Task<List<FunctionalFile>> ListFunctionalfile()
        {
            return await context.FunctionalFiles.ToListAsync();
        }
        [HttpPost, Route("functionalfile")]
        public async Task<object> CreateFunctionalfile([FromBody]FunctionalFileData functionalfile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var study = context.Studies.Where(s => s.id.Equals(functionalfile.StudyId)).FirstOrDefault();
            if (study == null) return new Exception("Funtional file must belong to a study.");
            //save 
            var result = new FunctionalFile
            {
                Comment = functionalfile.Comment,
                FileName = functionalfile.FileName,
                FileType = functionalfile.FileType,
                Study = study
            };
            context.FunctionalFiles.Add(result);
            await context.SaveChangesAsync();
            //return
            return result;
        }

        

        [HttpGet, Route("channel")]
        public async Task<List<Channel>> ListChannel()
        {
            return await context.Channels.ToListAsync();
        }
        [HttpPost, Route("channel")]
        public async Task<object> CreateChannel([FromBody]ChannelData channel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var functionalFile = CheckFunctionalFile(channel.ParentId);
            //save 
            var result = new Channel
            {
                NbChannels = channel.NbChannels,
                TransfMegLabels = channel.TransfMegLabels,
                TransfEegLabels = channel.TransfEegLabels,
                parent = functionalFile
            };
            context.Channels.Add(result);
            await context.SaveChangesAsync();
            //return
            return result;
        }



        [HttpGet, Route("timefreq")]
        public async Task<List<TimeFreq>> ListTimefreq()
        {
            return await context.TimeFreqs.ToListAsync();
        }
        [HttpPost, Route("timefreq")]
        public async Task<object> CreateTimefreq([FromBody]TimeFreqData timefreq)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var functionalFile = CheckFunctionalFile(timefreq.ParentId);
            //save 
            var result = new TimeFreq
            {
                Measure = timefreq.Measure,
                Method = timefreq.Method,
                NAvg = timefreq.NAvg,
                ColormapType = timefreq.ColormapType,
                DisplayUnits = timefreq.DisplayUnits,
                parent = functionalFile
            };
            context.TimeFreqs.Add(result);
            await context.SaveChangesAsync();
            //return
            return result;
        }


        [HttpGet, Route("stat")]
        public async Task<List<Stat>> ListStat()
        {
            return await context.Stats.ToListAsync();
        }
        [HttpPost, Route("stat")]
        public async Task<object> CreateStat([FromBody]StatData stat)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var functionalFile = CheckFunctionalFile(stat.ParentId);
            //save 
            var result = new Stat
            {
                Df = stat.Df,
                Correction = stat.Correction,
                Type = stat.Type,
                parent = functionalFile
            };
            context.Stats.Add(result);
            await context.SaveChangesAsync();
            //return
            return result;
        }


        [HttpGet, Route("headmodel")]
        public async Task<List<HeadModel>> ListHeadmodel()
        {
            return await context.HeadModels.ToListAsync();
        }
        [HttpPost, Route("headmodel")]
        public async Task<object> CreateHeadmodel([FromBody]HeadModelData headmodel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var functionalFile = CheckFunctionalFile(headmodel.ParentId);
            //save 
            var result = new HeadModel
            {
                Type = headmodel.Type,
                MEGMethod = headmodel.MEGMethod,
                EEGMethod = headmodel.EEGMethod,
                ECOGMethod = headmodel.ECOGMethod,
                SEEGMethod = headmodel.SEEGMethod,
                parent = functionalFile
            };
            context.HeadModels.Add(result);
            await context.SaveChangesAsync();
            //return
            return result;
        }


        [HttpGet, Route("result")]
        public async Task<List<Result>> ListResult()
        {
            return await context.Results.ToListAsync();
        }
        [HttpPost, Route("result")]
        public async Task<object> CreateResult([FromBody]ResultData result)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var functionalFile = CheckFunctionalFile(result.ParentId);
            //save 
            var o = new Result
            {
                IsLink = result.IsLink,
                NComponents = result.NComponents,
                Function = result.Function,
                NAvg = result.NAvg,
                ColormapType = result.ColormapType,
                DisplayUnits = result.DisplayUnits,
                parent = functionalFile
            };
            context.Results.Add(o);
            await context.SaveChangesAsync();
            //return
            return o;
        }



        [HttpGet, Route("recording")]
        public async Task<List<Recording>> ListRecording()
        {
            return await context.Recordings.ToListAsync();
        }
        [HttpPost, Route("recording")]
        public async Task<object> CreateRecording([FromBody]RecordingData recording)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var functionalFile = CheckFunctionalFile(recording.ParentId);
            //save 
            var result = new Recording
            {
                Format = recording.Format,
                Device = recording.Device,
                Byteorder = recording.Byteorder,
                DataType = recording.DataType,
                NAvg = recording.NAvg,
                SFreq = recording.SFreq,
                TimeStart = recording.TimeStart,
                TimeEnd = recording.TimeEnd,
                SamplesStart = recording.SamplesStart,
                SamplesEnd = recording.SamplesEnd,
                CurrCrfComp = recording.CurrCrfComp,
                DestCtfComp = recording.DestCtfComp,
                Acq_Date = recording.Acq_Date,
                ColormapType = recording.ColormapType,
                DisplayUnits = recording.DisplayUnits,
                IsBids = recording.IsBids,
                parent = functionalFile
            };
            context.Recordings.Add(result);
            await context.SaveChangesAsync();
            //return
            return result;
        }



        [HttpGet, Route("matrix")]
        public async Task<List<Matrix>> ListMatrix()
        {
            return await context.Matrices.ToListAsync();
        }
        [HttpPost, Route("matrix")]
        public async Task<object> CreateMatrix([FromBody]MatrixData matrix)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var functionalFile = CheckFunctionalFile(matrix.ParentId);
            //save 
            var result = new Matrix
            {
                NAvg = matrix.NAvg,
                DisplayUnits = matrix.DisplayUnits,
                parent = functionalFile
            };
            context.Matrices.Add(result);
            await context.SaveChangesAsync();
            //return
            return result;
        }



        public FunctionalFile CheckFunctionalFile(Guid fileid)
        {
            var functionalFile = context.FunctionalFiles.Where(f => f.id.Equals(fileid)).FirstOrDefault();
            if (functionalFile == null) throw new Exception("The file must be a functional file.");
            return functionalFile;
        }




    }
}
