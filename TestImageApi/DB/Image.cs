using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TestImageApi.DB;

[Table("Image")]
[Index("Ipid", Name = "IX_Image_IP")]
[Index("ZipId", Name = "IX_Image_Zip")]
public partial class Image
{
    [Key]
    public int Id { get; set; }

    [Column("S3Key")]
    [StringLength(500)]
    [Unicode(false)]
    public string S3key { get; set; } = null!;

    [StringLength(200)]
    public string FileName { get; set; } = null!;

    public DateTime CreateTime { get; set; }

    public DateTime? UpdateTime { get; set; }

    [Column("IPId")]
    public int Ipid { get; set; }

    public int? ZipId { get; set; }

    [ForeignKey("Ipid")]
    [InverseProperty("Images")]
    public virtual Ip Ip { get; set; } = null!;

    [ForeignKey("ZipId")]
    [InverseProperty("Images")]
    public virtual ZipFile? Zip { get; set; }
}
