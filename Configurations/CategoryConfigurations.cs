using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EFCore03.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFCore03.Configurations
{
    public class CategoryConfigurations : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Description)
                .HasMaxLength(500);

            builder.HasOne(c => c.ParentCategory)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(c => c.ParentCategoryId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(GetCategories());
        }

        private static List<Category> GetCategories()
        {
            return new List<Category>()
            {
                new Category { Id = 1, Name = "Programming", Description = "All programming courses", ParentCategoryId = null },
                new Category { Id = 2, Name = "Backend", Description = "Server-side development", ParentCategoryId = 1 },
                new Category { Id = 3, Name = "Frontend", Description = "Client-side development", ParentCategoryId = 1 },
                new Category { Id = 4, Name = "ASP.NET", Description = null, ParentCategoryId = 2 },
                new Category { Id = 5, Name = "React", Description = null, ParentCategoryId = 3 },
                new Category { Id = 6, Name = "Design", Description = "Design and creativity", ParentCategoryId = null },
                new Category { Id = 7, Name = "Data Science", Description = "Data and analytics", ParentCategoryId = null }
            };
        }
    }
}