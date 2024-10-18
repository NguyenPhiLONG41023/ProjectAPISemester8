using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Configuration
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Product");
            builder.HasKey(x => x.ProductId);
            builder.Property(x => x.ProductName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Description).HasColumnType("Text"); //moi
            builder.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            builder.HasOne(x => x.Brand).WithMany(x => x.Products).HasForeignKey(x => x.BrandId);
        }
    }
}
