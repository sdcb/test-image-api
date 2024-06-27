using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TestImageApi.DB;

[Table("ZipFile")]
[Index("Ipid", Name = "IX_ZipFile")]
public partial class ZipFile
{
    [Key]
    public int Id { get; set; }

    [StringLength(200)]
    public string FileName { get; set; } = null!;

    [Column("S3Key")]
    [StringLength(500)]
    [Unicode(false)]
    public string S3key { get; set; } = null!;

    public DateTime CreateTime { get; set; }

    public DateTime? UpdateTime { get; set; }

    [Column("IPId")]
    public int Ipid { get; set; }

    [InverseProperty("Zip")]
    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    [ForeignKey("Ipid")]
    [InverseProperty("ZipFiles")]
    public virtual Ip Ip { get; set; } = null!;
}
