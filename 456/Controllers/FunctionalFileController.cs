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