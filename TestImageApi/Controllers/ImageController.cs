using Microsoft.AspNetCore.Mvc;
using System.Web;
using TestImageApi.DB;
using TestImageApi.Services;

namespace TestImageApi.Controllers;

[Route("/api/image"), ApiController]
public class ImageController(ImageDB _db, IPSimpleService _ip, WebsiteUrlService _my, S3Service _s3) : ControllerBase
{
    [HttpGet]
    public ActionResult<string[]> List()
    {
        var datas = _db.Images
            .Where(x => x.Ipid == _ip.GetIPId())
            .Select(x => new { x.Id, x.FileName });
        return Ok(datas.Select(x => _my.GetImageUrl(x.Id, x.FileName)));
    }

    [HttpGet]
    [Route("{id:int}/{fileName}")]
    public IActionResult Get(int id)
    {
        var image = _db.Images.Find(id);
        if (image == null)
        {
            return NotFound();
        }
        if (image.Ipid != _ip.GetIPId())
        {
            return Unauthorized();
        }
        return Redirect(_s3.GetDownloadUrl(image.S3key));
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult> Post([FromForm] IFormFileCollection files)
    {
        int ipId = _ip.GetIPId();

        if (files.Count == 0)
        {
            return BadRequest("No files uploaded");
        }

        List<Image> effectedImages = [];
        foreach (IFormFile file in files)
        {
            if (!file.ContentType.StartsWith("image/"))
            {
                return BadRequest("Only image files are allowed");
            }

            var image = new Image
            {
                FileName = file.FileName,
                CreateTime = DateTime.Now,
                Ipid = ipId,
                S3key = await _s3.Upload(file.FileName, file.OpenReadStream())
            };
            effectedImages.Add(image);
            _db.Images.Add(image);
        }
        _db.SaveChanges();

        return Ok(effectedImages.Select(x => _my.GetImageUrl(x.Id, x.FileName)));
    }

    [HttpPut]
    [Consumes("multipart/form-data")]
    [Route("{id:int}")]
    public async Task<ActionResult> Put(int id)
    {
        var image = _db.Images.Find(id);
        if (image == null)
        {
            return NotFound();
        }

        if (image.Ipid != _ip.GetIPId())
        {
            return Unauthorized();
        }

        List<Image> effectedImages = [];
        foreach (IFormFile file in Request.Form.Files)
        {
            if (!file.ContentType.StartsWith("image/"))
            {
                return BadRequest("Only image files are allowed");
            }

            if (image.FileName != file.FileName)
            {
                await _s3.Delete(image.S3key);
                await _s3.Upload(file.FileName, file.OpenReadStream());
                image.FileName = file.FileName;
            }
            else
            {
                await _s3.Patch(image.S3key, file.OpenReadStream());
            }
            image.UpdateTime = DateTime.Now;
            effectedImages.Add(image);

        }
        _db.SaveChanges();

        return Ok(effectedImages.Select(x => _my.GetImageUrl(x.Id, x.FileName)));
    }

    [HttpDelete]
    [Route("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var image = _db.Images.Find(id);
        if (image == null)
        {
            return NotFound();
        }

        if (image.Ipid != _ip.GetIPId())
        {
            return Unauthorized();
        }

        _db.Images.Remove(image);
        await _s3.Delete(image.S3key);
        _db.SaveChanges();

        return Ok();
    }
}
