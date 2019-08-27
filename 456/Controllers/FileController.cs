using System;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using bst.Model;
using System.Linq;
using Microsoft.AspNetCore.Hosting;

namespace bst.Controllers
{
    [Route("file")]
    [ApiController]
    public class FileController : Controller
    {
        public UserDB context = new UserDB();

        private IHostingEnvironment env;

        public FileController(IHostingEnvironment env)
        {
            this.env = env;
        }
        /*
        // Get home directory based on if os is Windows or Unix - add bst directory Ex: "/home/user/bst"
        readonly string bstHomePath = (Environment.OSVersion.Platform == PlatformID.Unix ||
                   Environment.OSVersion.Platform == PlatformID.MacOSX)
    ? Path.Combine(Environment.GetEnvironmentVariable("HOME"), "bst")
    : Path.Combine(Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%"), "bst");
    */
        // GET file/upload
        /// <summary>
        /// Receive form data containing a file, save file on server, and return the file path
        /// </summary>
        /// <param name="file">Received IFormFile file</param>
        /// <param name="fileTransferIn"></param>
        /// <returns></returns>
        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload()
        {
            if (HttpContext.Request.Form.Files.Count < 1)
            {
                return BadRequest();
            }

            var file = HttpContext.Request.Form.Files[0];
            
            var user = await context.Users.FindAsync(HttpContext.Items["user"]);
            var protocol = await context.Protocols.FindAsync(fileTransferIn.Protocolid);
            if (protocol == null) return new NotFoundResult();
            var role = user.Roles.FirstOrDefault(r => r.Group.Id.Equals(protocol.Group.Id));
            if (role == null) return new NotFoundResult();
            if (role.Privilege > 2) throw new UnauthorizedAccessException("User doesn't have write access to this protocol.");

            // Verify the home-bst directory exists, and combine the home-bst directory with the new file name
            Directory.CreateDirectory(bstHomePath);
            
            var filePath = Path.Combine(bstHomePath, protocol.Group.Name, protocol.Name, fileTransferIn.Filelocation);
            


            // If exists old version, delete
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            // Create a new file in the home-bst directory 
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                //copy the contents of the received file to the newly created local file 
                await file.CopyToAsync(stream);
            }
            // return the file name for the locally stored file
            return Ok(filePath);
        }


        /*
        // GET file/downlaod
        /// <summary>
        /// Return a file stored on server based on file info provided
        /// </summary>
        /// <param name="fileTransferIn"></param>
        /// <returns></returns>
        [HttpGet("download")]
        public async Task<IActionResult> Download([FromBody] FileTransferIn fileTransferIn)
        {
            var user = await context.Users.FindAsync(HttpContext.Items["user"]);
            var protocol = await context.Protocols.FindAsync(fileTransferIn.Protocolid);
            if (protocol == null) return new NotFoundResult();
            var role = user.Roles.FirstOrDefault(r => r.Group.Id.Equals(protocol.Group.Id));
            if (role == null) return new NotFoundResult();
            if (role.Privilege > 3) throw new UnauthorizedAccessException("User doesn't have access to this protocol.");
            var path = Path.Combine(bstHomePath, protocol.Group.Name, protocol.Name, fileTransferIn.Filelocation);

            if (System.IO.File.Exists(path))
            {

                // Get all bytes of the file and return the file with the specified file contents 
                byte[] b = await System.IO.File.ReadAllBytesAsync(path);
                return File(b, "application/octet-stream");
            }

            else
            {
                // return error if file not found
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }*/
    }
}
