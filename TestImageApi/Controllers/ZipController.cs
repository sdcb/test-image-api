using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;
using TestImageApi.DB;
using TestImageApi.Services;
using ZipFile = TestImageApi.DB.ZipFile;

namespace TestImageApi.Controllers;

[Route("api/zip")]
public class ZipController(ImageDB _db, IPSimpleService _ip, WebsiteUrlService _my, S3Service _s3) : ControllerBase
{
    [HttpGet]
    public IActionResult List()
    {
        var datas = _db.ZipFiles
            .Where(x => x.Ipid == _ip.GetIPId())
            .Select(x => new { x.Id, x.FileName });
        return Ok(datas.Select(x => _my.GetZipUrl(x.Id, x.FileName)));
    }

    [HttpGet]
    [Route("{id:int}/{fileName}")]
    public IActionResult Get(int id)
    {
        ZipFile? zip = _db.ZipFiles
            .Include(x => x.Images)
            .SingleOrDefault(x => x.Id == id);
        if (zip == null)
        {
            return NotFound();
        }
        if (zip.Ipid != _ip.GetIPId())
        {
            return Unauthorized();
        }
        return Ok(zip.Images.Select(x => _my.GetImageUrl(x.Id, x.FileName)));
    }

    [HttpGet]
    [Route("{id:int}/{fileName}/download")]
    public IActionResult Download(int id)
    {
        ZipFile? zip = _db.ZipFiles.Find(id);
        if (zip == null)
        {
            return NotFound();
        }
        if (zip.Ipid != _ip.GetIPId())
        {
            return Unauthorized();
        }
        return Redirect(_s3.GetDownloadUrl(zip.S3key));
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

        if (files.Count > 1)
        {
            return BadRequest("Only one file is allowed");
        }

        ZipFile zipFileInDB = new()
        {
            FileName = files[0].FileName,
            CreateTime = DateTime.Now,
            Ipid = ipId,
            S3key = await _s3.Upload(files[0].FileName, files[0].OpenReadStream(), leaveOpen: true),
        };
        _db.ZipFiles.Add(zipFileInDB);
        _db.SaveChanges();

        List<Image> images = [];
        if (files[0].ContentType != "application/zip")
        {
            return BadRequest("Only zip files are allowed");
        }

        ZipArchive archive = new(files[0].OpenReadStream(), ZipArchiveMode.Read, leaveOpen: false);
        List<Image> effectedImages = [];
        HashSet<string> allowedExtensions = [".jpg", ".jpeg", ".png", ".bmp", ".avif", ".webp"];
        foreach (ZipArchiveEntry entry in archive.Entries.Where(x => allowedExtensions.Any(ext => x.FullName.EndsWith(ext, StringComparison.OrdinalIgnoreCase))))
        {
            using MemoryStream ms = new();
            entry.Open().CopyTo(ms);
            ms.Position = 0;

            Image image = new()
            {
                FileName = entry.Name,
                CreateTime = DateTime.Now,
                Ipid = ipId,
                S3key = await _s3.Upload(entry.Name, ms),
                ZipId = zipFileInDB.Id,
            };
            effectedImages.Add(image);
            _db.Images.Add(image);
        }
        _db.SaveChanges();

        return Ok(_my.GetZipUrl(zipFileInDB.Id, zipFileInDB.FileName));
    }

    [HttpDelete]
    [Route("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        ZipFile? zip = _db.ZipFiles
            .Include(x => x.Images)
            .SingleOrDefault(x => x.Id == id);
        if (zip == null)
        {
            return NotFound();
        }
        if (zip.Ipid != _ip.GetIPId())
        {
            return Unauthorized();
        }
        foreach (Image image in zip.Images)
        {
            await _s3.Delete(image.S3key);
            _db.Images.Remove(image);
        }
        _db.ZipFiles.Remove(zip);
        await _s3.Delete(zip.S3key);
        int effectedRows = _db.SaveChanges();
        return Ok(effectedRows);
    }
}
