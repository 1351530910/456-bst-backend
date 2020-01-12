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

        [HttpPost, Route("createChannel"), AuthFilter, WriteLock, ProducesResponseType(typeof(uploadinfo), 200)]
        public async Task<object> createChannel([FromBody]ChannelData data)
        {
            //create channel data
            var study = protocol.Studies.FirstOrDefault(x => x.Id == data.StudyID);
            var channel = new Channel(data)
            {
                Study = study
            };

            //set parent url
            channel.Parent.Url = mapUrl(protocol.Id.ToString(), study.Id.ToString(), channel.Parent.Id.ToString());

            //add channel and ff to database
            context.Channels.Add(channel);
            context.FunctionalFiles.Add(channel.Parent);
            history.HistoryEvent += $"create Channel {study.Id} {channel.Id}";
            await context.SaveChangesAsync();

            return new uploadinfo
            {
                uploadid = FileController.createFunctionalFileQueueItem(channel, session, data.Md5).ToString(),
                ffid = channel.Parent.Id.ToString()
            };
        }
        [HttpPost, Route("createTimeFreq"), AuthFilter, WriteLock, ProducesResponseType(typeof(uploadinfo), 200)]
        public async Task<object> createTimeFreq([FromBody]TimeFreqData data)
        {
            //create channel data
            var study = protocol.Studies.FirstOrDefault(x => x.Id == data.StudyID);
            var obj = new TimeFreq(data)
            {
                Study = study
            };

            //set parent url
            obj.Parent.Url = mapUrl(protocol.Id.ToString(), study.Id.ToString(), obj.Parent.Id.ToString());

            //add channel and ff to database
            context.TimeFreqs.Add(obj);
            context.FunctionalFiles.Add(obj.Parent);
            history.HistoryEvent += $"create {obj.GetType().Name} {study.Id} {obj.Id}";
            await context.SaveChangesAsync();

            return new uploadinfo
            {
                uploadid = FileController.createFunctionalFileQueueItem(obj, session, data.Md5).ToString(),
                ffid = obj.Parent.Id.ToString()
            };
        }
        [HttpPost, Route("createStat"), AuthFilter, WriteLock, ProducesResponseType(typeof(uploadinfo), 200)]
        public async Task<object> createStat([FromBody]StatData data)
        {
            //create channel data
            var study = protocol.Studies.FirstOrDefault(x => x.Id == data.StudyID);
            var obj = new Stat(data)
            {
                Study = study
            };

            //set parent url
            obj.Parent.Url = mapUrl(protocol.Id.ToString(), study.Id.ToString(), obj.Parent.Id.ToString());

            //add channel and ff to database
            context.Stats.Add(obj);
            context.FunctionalFiles.Add(obj.Parent);
            history.HistoryEvent += $"create {obj.GetType().Name} {study.Id} {obj.Id}";
            await context.SaveChangesAsync();

            return new uploadinfo
            {
                uploadid = FileController.createFunctionalFileQueueItem(obj, session, data.Md5).ToString(),
                ffid = obj.Parent.Id.ToString()
            };
        }
        [HttpPost, Route("createHeadModel"), AuthFilter, WriteLock, ProducesResponseType(typeof(uploadinfo), 200)]
        public async Task<object> createHeadModel([FromBody]HeadModelData data)
        {
            //create channel data
            var study = protocol.Studies.FirstOrDefault(x => x.Id == data.StudyID);
            var obj = new HeadModel(data)
            {
                Study = study
            };

            //set parent url
            obj.Parent.Url = mapUrl(protocol.Id.ToString(), study.Id.ToString(), obj.Parent.Id.ToString());

            //add channel and ff to database
            context.HeadModels.Add(obj);
            context.FunctionalFiles.Add(obj.Parent);
            history.HistoryEvent += $"create {obj.GetType().Name} {study.Id} {obj.Id}";
            await context.SaveChangesAsync();

            return new uploadinfo
            {
                uploadid = FileController.createFunctionalFileQueueItem(obj, session, data.Md5).ToString(),
                ffid = obj.Parent.Id.ToString()
            };
        }
        [HttpPost, Route("createResult"), AuthFilter, WriteLock, ProducesResponseType(typeof(uploadinfo), 200)]
        public async Task<object> createResult([FromBody]ResultData data)
        {
            //create channel data
            var study = protocol.Studies.FirstOrDefault(x => x.Id == data.StudyID);
            var obj = new Result(data)
            {
                Study = study
            };

            //set parent url
            obj.Parent.Url = mapUrl(protocol.Id.ToString(), study.Id.ToString(), obj.Parent.Id.ToString());

            //add channel and ff to database
            context.Results.Add(obj);
            context.FunctionalFiles.Add(obj.Parent);
            history.HistoryEvent += $"create {obj.GetType().Name} {study.Id} {obj.Id}";
            await context.SaveChangesAsync();

            return new uploadinfo
            {
                uploadid = FileController.createFunctionalFileQueueItem(obj, session, data.Md5).ToString(),
                ffid = obj.Parent.Id.ToString()
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
        [HttpPost, Route("createMatrix"), AuthFilter, WriteLock, ProducesResponseType(typeof(uploadinfo), 200)]
        public async Task<object> createMatrix([FromBody]MatrixData data)
        {
            //create channel data
            var study = protocol.Studies.FirstOrDefault(x => x.Id == data.StudyID);
            var obj = new Matrix(data)
            {
                Study = study
            };

            //set parent url
            obj.Parent.Url = mapUrl(protocol.Id.ToString(), study.Id.ToString(), obj.Parent.Id.ToString());

            //add channel and ff to database
            context.Matrices.Add(obj);
            context.FunctionalFiles.Add(obj.Parent);
            history.HistoryEvent += $"create {obj.GetType().Name} {study.Id} {obj.Id}";
            await context.SaveChangesAsync();

            return new uploadinfo
            {
                uploadid = FileController.createFunctionalFileQueueItem(obj, session, data.Md5).ToString(),
                ffid = obj.Parent.Id.ToString()
            };
        }
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="firstLayer"> protocolID</param>
        /// <param name="SecondLayer"> studyID</param>
        /// <param name="filename"> fileID</param>
        /// <returns></returns>
        public static string mapFile(string firstLayer,string SecondLayer,string filename)
        {
            if (string.IsNullOrEmpty(SecondLayer))
            {
                return $"./wwwroot/files/{firstLayer}/ffiles/{filename}.dat";
            }
            else
            {
                return $"./wwwroot/files/{firstLayer}/ffiles/{SecondLayer}/{filename}.dat";
            }
        }
        /// <summary>
        ///  "./files/{firstLayer}/ffiles/{SecondLayer}/{filename}";
        /// </summary>
        /// <param name="firstLayer"> protocolID</param>
        /// <param name="SecondLayer"> studyID</param>
        /// <param name="filename"> fileID</param>
        /// <returns></returns>
        public static string mapUrl(string firstLayer, string SecondLayer, string filename)
        {
            if (string.IsNullOrEmpty(SecondLayer))
            {
                return $"/files/{firstLayer}/ffiles/{filename}";
            }
            else
            {
                return $"/files/{firstLayer}/ffiles/{SecondLayer}/{filename}";
            }
        }

    }

}