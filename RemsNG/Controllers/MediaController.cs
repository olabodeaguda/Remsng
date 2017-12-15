using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RemsNG.ORM;
using RemsNG.Models;
using RemsNG.Exceptions;
using RemsNG.Utilities;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using RemsNG.Services.Interfaces;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RemsNG.Controllers
{
    [Route("api/v1/media")]
    public class MediaController : Controller
    {
        private IHostingEnvironment hostingEnvironment;
        private IImageService imageService;
        private readonly ILogger logger;
        public MediaController(IHostingEnvironment _hostingEnvironment, 
            IImageService _imageService, ILoggerFactory loggerFactory)
        {
            this.hostingEnvironment = _hostingEnvironment;
            this.imageService = _imageService;
        }

        // GET api/values/5
        [HttpGet("ownerid/{id}")]
        public async Task<List<Images>> ByOwnerId(Guid id)
        {
            return await imageService.ByOwnerId(id);
        }

        [HttpPost]
        public async Task<object> Post([FromBody]ImageExtensionModel img)
        {
            if (string.IsNullOrEmpty(img.imgBase64))
            {
                throw new InvalidCredentialsException("Image is required!!!");
            }
            int dotIndex = img.imgFilename.IndexOf('.');
            img.imgFilename = $"{img.imgType}{DateTime.Now.Ticks}.{img.imgFilename.Substring(dotIndex + 1)}".ToLower();// img.imgFilename.Substring(dotIndex+1) img.imgFilename.Insert(dotIndex - 1, DateTime.Now.ToString("ddmmyyyyhhmmss"));
            string filePath = Path.Combine(this.hostingEnvironment.WebRootPath, "images", img.imgFilename);
            bool result = false;
            await Task.Run(() =>
            {
                ImgUtils imgUtils = new ImgUtils();
                result = imgUtils.WriteFile(filePath, img.imgBase64);
            });

            if (result)
            {
                img.id = Guid.NewGuid();
                img.createdBy = User.Identity.Name;
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
