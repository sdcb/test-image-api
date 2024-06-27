using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TestImageApi.DB;

public partial class ImageDB : DbContext
{
    public ImageDB()
    {
    }

    public ImageDB(DbContextOptions<ImageDB> options)
        : base(options)
    {
    }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Ip> Ips { get; set; }

    public virtual DbSet<ZipFile> ZipFiles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=DBConnectionString");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasOne(d => d.Ip).WithMany(p => p.Images)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Image_IP");

            entity.HasOne(d => d.Zip).WithMany(p => p.Images).HasConstraintName("FK_Image_ZipFile");
        });

        modelBuilder.Entity<ZipFile>(entity =>
        {
            entity.HasOne(d => d.Ip).WithMany(p => p.ZipFiles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ZipFile_IP");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
