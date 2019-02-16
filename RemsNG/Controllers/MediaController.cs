using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RemsNG.Common.Exceptions;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using RemsNG.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RemsNG.Controllers
{
    [Route("api/v1/media")]
    public class MediaController : Controller
    {
        private IHostingEnvironment hostingEnvironment;
        private IImageManagers imageService;
        private readonly ILogger logger;
        public MediaController(IHostingEnvironment _hostingEnvironment,
            IImageManagers _imageService, ILoggerFactory loggerFactory)
        {
            hostingEnvironment = _hostingEnvironment;
            imageService = _imageService;
        }

        // GET api/values/5
        [HttpGet("ownerid/{id}")]
        public async Task<List<ImagesModel>> ByOwnerId(Guid id)
        {
            return await imageService.ByOwnerId(id);
        }

        [RemsRequirementAttribute("UPLOAD_MEDIA")]
        [HttpPost]
        public async Task<object> Post([FromBody]ImageExtensionModel img)
        {
            if (string.IsNullOrEmpty(img.imgBase64))
            {
                throw new InvalidCredentialsException("Image is required!!!");
            }
            int dotIndex = img.ImgFilename.IndexOf('.');
            img.ImgFilename = $"{img.ImgType}{DateTime.Now.Ticks}.{img.ImgFilename.Substring(dotIndex + 1)}".ToLower();// img.imgFilename.Substring(dotIndex+1) img.imgFilename.Insert(dotIndex - 1, DateTime.Now.ToString("ddmmyyyyhhmmss"));
            string filePath = Path.Combine(hostingEnvironment.WebRootPath, "images", img.ImgFilename);
            bool result = false;
            await Task.Run(() =>
            {
                ImgUtils imgUtils = new ImgUtils();
                result = imgUtils.WriteFile(filePath, img.imgBase64);
            });

            if (result)
            {
                img.Id = Guid.NewGuid();
                img.CreatedBy = User.Identity.Name;
                Response response = await imageService.Add(img);
                if (response.code == MsgCode_Enum.SUCCESS)
                {
                    return Ok(response);
                }
                else
                {
                    return BadRequest(response);
                }
            }
            else
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Request failed to save file. Please try again.."
                });
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
