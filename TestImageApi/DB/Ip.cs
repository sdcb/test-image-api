using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TestImageApi.DB;

[Table("IP")]
public partial class Ip
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Address { get; set; } = null!;

    public DateTime CreateTime { get; set; }

    [InverseProperty("Ip")]
    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    [InverseProperty("Ip")]
    public virtual ICollection<ZipFile> ZipFiles { get; set; } = new List<ZipFile>();
}
