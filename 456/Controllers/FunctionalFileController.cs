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
    public class FunctionalFileController : BaseController
    {
        #region ffiles

        [HttpPost, Route("createChannel"), AuthFilter, WriteLock, ProducesResponseType(typeof(Uploadinfo), 200)]
        public async Task<object> createChannel([FromBody]ChannelData data)
        {
            //create channel data
            var study = protocol.Studies.FirstOrDefault(x => x.Id == data.StudyID);
            var channel = new Channel(data)
            {
                Study = study
            };

            //set parent url
            channel.Parent.Url = mapUrl(protocol.Id.ToString(), study.Id.ToString(), channel.Parent.Id.ToString(), "data");

            //add channel and ff to database
            context.Channels.Add(channel);
            context.FunctionalFiles.Add(channel.Parent);
            history.HistoryEvent += $"create Channel {study.Id} {channel.Id}";
            await context.SaveChangesAsync();

            return new Uploadinfo
            {
                Uploadid = FileController.createFunctionalFileQueueItem(channel, session, data.Md5, data.FileName).ToString(),
                Fid = channel.Parent.Id.ToString()
            };
        }
        [HttpPost, Route("createTimeFreq"), AuthFilter, WriteLock, ProducesResponseType(typeof(Uploadinfo), 200)]
        public async Task<object> createTimeFreq([FromBody]TimeFreqData data)
        {
            var study = protocol.Studies.FirstOrDefault(x => x.Id == data.StudyID);
            var obj = new TimeFreq(data)
            {
                Study = study
            };

            //set parent url
            obj.Parent.Url = mapUrl(protocol.Id.ToString(), study.Id.ToString(), obj.Parent.Id.ToString(), "data");

            context.TimeFreqs.Add(obj);
            context.FunctionalFiles.Add(obj.Parent);
            history.HistoryEvent += $"create {obj.GetType().Name} {study.Id} {obj.Id}";
            await context.SaveChangesAsync();

            return new Uploadinfo
            {
                Uploadid = FileController.createFunctionalFileQueueItem(obj, session, data.Md5, data.FileName).ToString(),
                Fid = obj.Parent.Id.ToString()
            };
        }
        [HttpPost, Route("createStat"), AuthFilter, WriteLock, ProducesResponseType(typeof(Uploadinfo), 200)]
        public async Task<object> createStat([FromBody]StatData data)
        {
            var study = protocol.Studies.FirstOrDefault(x => x.Id == data.StudyID);
            var obj = new Stat(data)
            {
                Study = study
            };

            //set parent url
            obj.Parent.Url = mapUrl(protocol.Id.ToString(), study.Id.ToString(), obj.Parent.Id.ToString(), "data");

            context.Stats.Add(obj);
            context.FunctionalFiles.Add(obj.Parent);
            history.HistoryEvent += $"create {obj.GetType().Name} {study.Id} {obj.Id}";
            await context.SaveChangesAsync();

            return new Uploadinfo
            {
                Uploadid = FileController.createFunctionalFileQueueItem(obj, session, data.Md5, data.FileName).ToString(),
                Fid = obj.Parent.Id.ToString()
            };
        }
        [HttpPost, Route("createHeadModel"), AuthFilter, WriteLock, ProducesResponseType(typeof(Uploadinfo), 200)]
        public async Task<object> createHeadModel([FromBody]HeadModelData data)
        {
            var study = protocol.Studies.FirstOrDefault(x => x.Id == data.StudyID);
            var obj = new HeadModel(data)
            {
                Study = study
            };

            //set parent url
            obj.Parent.Url = mapUrl(protocol.Id.ToString(), study.Id.ToString(), obj.Parent.Id.ToString(), "data");

            context.HeadModels.Add(obj);
            context.FunctionalFiles.Add(obj.Parent);
            history.HistoryEvent += $"create {obj.GetType().Name} {study.Id} {obj.Id}";
            await context.SaveChangesAsync();

            return new Uploadinfo
            {
                Uploadid = FileController.createFunctionalFileQueueItem(obj, session, data.Md5, data.FileName).ToString(),
                Fid = obj.Parent.Id.ToString()
            };
        }
        [HttpPost, Route("createResult"), AuthFilter, WriteLock, ProducesResponseType(typeof(Uploadinfo), 200)]
        public async Task<object> createResult([FromBody]ResultData data)
        {
            var study = protocol.Studies.FirstOrDefault(x => x.Id == data.StudyID);
            var obj = new Result(data)
            {
                Study = study
            };

            //set parent url
            obj.Parent.Url = mapUrl(protocol.Id.ToString(), study.Id.ToString(), obj.Parent.Id.ToString(), "data");

            context.Results.Add(obj);
            context.FunctionalFiles.Add(obj.Parent);
            history.HistoryEvent += $"create {obj.GetType().Name} {study.Id} {obj.Id}";
            await context.SaveChangesAsync();

            return new Uploadinfo
            {
                Uploadid = FileController.createFunctionalFileQueueItem(obj, session, data.Md5, data.FileName).ToString(),
                Fid = obj.Parent.Id.ToString()
            };
        }
        /*
        [HttpPost, Route("createRecording"), AuthFilter, WriteLock, ProducesResponseType(typeof(uploadinfo), 200)]
        public async Task<object> createRecording([FromBody]RecordingData data)
        {
            //create channel data
            var study = protocol.Studies.FirstOrDefault(x => x.Id == data.studyID);
            var obj = new Recording(data);
            obj.Study = study;

            //set parent url
            obj.Parent.Url = mapUrl(protocol.Id.ToString(), study.Id.ToString(), obj.Parent.Id.ToString());

            //add channel and ff to database
            context.Recordings.Add(obj);
            context.FunctionalFiles.Add(obj.Parent);
            history.HistoryEvent += $"create {obj.GetType().Name} {study.Id} {obj.Id}";
            await context.SaveChangesAsync();

            return new uploadinfo
            {
                uploadid = FileController.createFunctionalFileQueueItem(obj, session, data.md5).ToString(),
                ffid = obj.Parent.Id.ToString()
            };
        }
        */
        [HttpPost, Route("createMatrix"), AuthFilter, WriteLock, ProducesResponseType(typeof(Uploadinfo), 200)]
        public async Task<object> createMatrix([FromBody]MatrixData data)
        {
            var study = protocol.Studies.FirstOrDefault(x => x.Id == data.StudyID);
            var obj = new Matrix(data)
            {
                Study = study
            };

            //set parent url
            obj.Parent.Url = mapUrl(protocol.Id.ToString(), study.Id.ToString(), obj.Parent.Id.ToString(),"data");

            //add channel and ff to database
            context.Matrices.Add(obj);
            context.FunctionalFiles.Add(obj.Parent);
            history.HistoryEvent += $"create {obj.GetType().Name} {study.Id} {obj.Id}";
            await context.SaveChangesAsync();

            return new Uploadinfo
            {
                Uploadid = FileController.createFunctionalFileQueueItem(obj, session, data.Md5, data.FileName).ToString(),
                Fid = obj.Parent.Id.ToString()
            };
        }

        [HttpPost, Route("createOther"), AuthFilter, WriteLock, ProducesResponseType(typeof(Uploadinfo), 200)]
        public async Task<object> createOtherFunctionalFile([FromBody]OtherData data)
        {
            //create channel data
            var study = protocol.Studies.FirstOrDefault(x => x.Id == data.StudyID);
            var obj = new Other(data)
            {
                Study = study
            };

            //set parent url
            obj.Parent.Url = mapUrl(protocol.Id.ToString(), study.Id.ToString(), obj.Parent.Id.ToString(), "data");

            //add channel and ff to database
            context.Others.Add(obj);
            context.FunctionalFiles.Add(obj.Parent);
            history.HistoryEvent += $"create {obj.GetType().Name} {study.Id} {obj.Id}";
            await context.SaveChangesAsync();

            return new Uploadinfo
            {
                Uploadid = FileController.createFunctionalFileQueueItem(obj, session, data.Md5,data.FileName).ToString(),
                Fid = obj.Parent.Id.ToString()
            };
        }

        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="protocolid"> protocolID</param>
        /// <param name="subjectOrStudyId"> studyID</param>
        /// <param name="fileid"> fileID</param>
        /// <returns></returns>
        public static string mapFile(string protocolid,string subjectOrStudyId,string fileid,string type)
        {
            if (string.IsNullOrEmpty(subjectOrStudyId))
            {
                return $"./wwwroot/protocols/{protocolid}/{type}/{fileid}.bin";
            }
            else
            {
                return $"./wwwroot/protocols/{protocolid}/{type}/{subjectOrStudyId}/{fileid}.bin";
            }
        }
        /// <summary>
        ///  "./files/{firstLayer}/ffiles/{SecondLayer}/{filename}";
        /// </summary>
        /// <param name="protocolid"> protocolID</param>
        /// <param name="subjectOrStudyId"> studyID</param>
        /// <param name="fileid"> fileID</param>
        /// <returns></returns>
        public static string mapUrl(string protocolid, string subjectOrStudyId, string fileid, string type)
        {
            if (string.IsNullOrEmpty(subjectOrStudyId))
            {
                return $"/protocols/{protocolid}/{type}/{fileid}";
            }
            else if (subjectOrStudyId.Equals(fileid))
            {
                return $"/protocols/{protocolid}/{type}/{subjectOrStudyId}/{subjectOrStudyId}";
            }
            else
            {
                return $"/protocols/{protocolid}/{type}/{subjectOrStudyId}/{fileid}";
            }
        }




    }

}