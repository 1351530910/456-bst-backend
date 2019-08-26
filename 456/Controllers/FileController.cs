using System;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Cors;
using bst.Model;
using System.Linq;

namespace bst.Controllers
{
    [Route("file")]
    [ApiController,AuthFilter]
    public class FileController : Controller
    {
        public UserDB context = new UserDB();

        // Get home directory based on if os is Windows or Unix - add guac directory Ex: "/home/user/guac"
        readonly string bstHomePath = (Environment.OSVersion.Platform == PlatformID.Unix ||
                   Environment.OSVersion.Platform == PlatformID.MacOSX)
    ? Path.Combine(Environment.GetEnvironmentVariable("HOME"), "bst")
    : Path.Combine(Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%"), "bst");

        // GET file/upload
        /// <summary>
        /// Receive form data containing a file, save file locally with a unique id as the name, and return the unique id
        /// </summary>
        /// <param name="file">Received IFormFile file</param>
        /// <param name="protocolid"></param>
        /// <param name="protocollocation"></param>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost("upload")]
        [EnableCors("MyPolicy")]
        public async Task<IActionResult> Upload(IFormFile file, [FromBody] Guid protocolid, [FromBody] string protocollocation)
        {
            var user = await context.Users.FindAsync(HttpContext.Items["user"]);
            var protocol = await context.Protocols.FindAsync(protocolid);
            if (protocol == null) return new NotFoundResult();
            var role = user.Roles.FirstOrDefault(r => r.Group.Id.Equals(protocol.Group.Id));
            if (role == null) return new NotFoundResult();
            if (role.Privilege > 2) throw new UnauthorizedAccessException("User doesn't have write access to this protocol.");

            // Verify the home-bst directory exists, and combine the home-bst directory with the new file name
            Directory.CreateDirectory(bstHomePath);
            var filePath = Path.Combine(bstHomePath, protocol.Group.Name, protocol.Name, protocollocation);

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

        // GET file/downlaod
        /// <summary>
        /// Return a locally stored image based on id to the requesting client
        /// </summary>
        /// <param name="protocolid"></param>
        /// <param name="protocollocation"></param>
        /// <param name="id">unique identifier for the requested file</param>
        /// <returns></returns>
        [HttpGet("download/{id}")]
        public async Task<IActionResult> Download([FromBody] Guid protocolid, [FromBody] string protocollocation)
        {
            var user = await context.Users.FindAsync(HttpContext.Items["user"]);
            var protocol = await context.Protocols.FindAsync(protocolid);
            if (protocol == null) return new NotFoundResult();
            var role = user.Roles.FirstOrDefault(r => r.Group.Id.Equals(protocol.Group.Id));
            if (role == null) return new NotFoundResult();
            if (role.Privilege > 3) throw new UnauthorizedAccessException("User doesn't have access to this protocol.");
            var path = Path.Combine(bstHomePath, protocol.Group.Name, protocol.Name, protocollocation);

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

        }
    }
}
