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
    public class CourseConfigurations : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.Property(c => c.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Description)
                .HasMaxLength(2000);

            builder.Property(c => c.Price)
                .IsRequired()
                .HasPrecision(10, 2);

            builder.Property(c => c.Level)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(c => c.IsPublished)
                .IsRequired();

            builder.Property(c => c.PublishedDate)
                .IsRequired(false);

            builder.HasOne(c => c.Instructor)
                .WithMany(i => i.Courses)
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Category)
                .WithMany(cat => cat.Courses)
                .HasForeignKey(c => c.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(GetCourses());
        }

        private static List<Course> GetCourses()
        {
            return new List<Course>()
            {
                new Course { Id = 1, Title = "C# Fundamentals", Description = "Learn C# from scratch.", Price = 49.99m, Level = CourseLevel.Beginner, IsPublished = true, PublishedDate = new DateTime(2026, 1, 10), InstructorId = 1, CategoryId = 1 },
                new Course { Id = 2, Title = "ASP.NET Core Web API", Description = "Build REST APIs with ASP.NET Core.", Price = 79.99m, Level = CourseLevel.Intermediate, IsPublished = true, PublishedDate = new DateTime(2026, 2, 5), InstructorId = 1, CategoryId = 4 },
                new Course { Id = 3, Title = "Advanced EF Core", Description = "Deep dive into EF Core.", Price = 99.99m, Level = CourseLevel.Advanced, IsPublished = true, PublishedDate = new DateTime(2026, 3, 1), InstructorId = 2, CategoryId = 2 },
                new Course { Id = 4, Title = "React for Beginners", Description = null, Price = 59.99m, Level = CourseLevel.Beginner, IsPublished = true, PublishedDate = new DateTime(2026, 3, 20), InstructorId = 2, CategoryId = 5 },
                new Course { Id = 5, Title = "Microservices Architecture", Description = "Design distributed systems.", Price = 129.99m, Level = CourseLevel.Advanced, IsPublished = false, PublishedDate = null, InstructorId = 3, CategoryId = 2 },
                new Course { Id = 6, Title = "UI Design Basics", Description = null, Price = 39.99m, Level = CourseLevel.Beginner, IsPublished = false, PublishedDate = null, InstructorId = 3, CategoryId = 6 }
            };
        }
    }
}