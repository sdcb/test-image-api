using Amazon.S3;
using Amazon.S3.Model;
using System.Net;

namespace TestImageApi.Services;

public class S3Service(IConfiguration config)
{
    private readonly string _bucketName = config["S3:BucketName"] ?? throw new Exception("S3:BucketName is required");
    private readonly AmazonS3Client _s3 = new(config["S3:AccessKey"] ?? throw new Exception("S3:AccessKey is required"), config["S3:SecretKey"] ?? throw new Exception("S3:SecretKey is required"), new AmazonS3Config
    {
        ForcePathStyle = true,
        ServiceURL = config["S3:ServiceUrl"] ?? throw new Exception("S3:ServiceUrl is required"),
    });

    public async Task<string> Upload(string fileName, Stream content, bool leaveOpen = false)
    {
        string ext = Path.GetExtension(fileName);
        string s3Key = $"{DateTime.Now:yyyy/MM/dd}/{fileName}-{DateTime.Now:HHmmss}.{ext}";

        try
        {
            PutObjectResponse resp = await _s3.PutObjectAsync(new()
            {
                BucketName = _bucketName,
                Key = s3Key,
                InputStream = content,
            });
            if (resp.HttpStatusCode != HttpStatusCode.OK) throw new Exception($"上传失败 {resp.HttpStatusCode}");

            return s3Key;
        }
        finally
        {
            if (!leaveOpen) content.Close();
        }
    }

    public async Task Patch(string s3Key, Stream content)
    {
        await _s3.PutObjectAsync(new()
        {
            BucketName = _bucketName,
            Key = s3Key,
            InputStream = content,
        });
    }

    public async Task Delete(string s3Key)
    {
        await _s3.DeleteObjectAsync(new()
        {
            BucketName = _bucketName,
            Key = s3Key,
        });
    }

    public string GetDownloadUrl(string s3Key)
    {
        string downloadUrl = _s3.GetPreSignedURL(new GetPreSignedUrlRequest()
        {
            BucketName = _bucketName,
            Key = s3Key,
            Expires = DateTime.UtcNow.AddHours(1),
        });

        return downloadUrl;
    }
}
