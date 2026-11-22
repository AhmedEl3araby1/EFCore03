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
    public class InstructorConfigurations : IEntityTypeConfiguration<Instructor>
    {
        public void Configure(EntityTypeBuilder<Instructor> builder)
        {
            builder.Property(i => i.FullName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(i => i.Email)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(i => i.Bio)
                .HasMaxLength(1000);

            builder.Property(i => i.DateJoined)
                .IsRequired();

            builder.HasData(GetInstructors());
        }

        private static List<Instructor> GetInstructors()
        {
            return new List<Instructor>()
            {
                new Instructor { Id = 1, FullName = "Ahmed Hassan", Email = "ahmed.hassan@example.com", Bio = "Senior .NET developer and trainer.", DateJoined = new DateTime(2025, 1, 15) },
                new Instructor { Id = 2, FullName = "Sara Mahmoud", Email = "sara.mahmoud@example.com", Bio = "Full-stack engineer with 8 years of experience.", DateJoined = new DateTime(2025, 3, 10) },
                new Instructor { Id = 3, FullName = "Omar Khaled", Email = "omar.khaled@example.com", Bio = null, DateJoined = new DateTime(2025, 6, 1) }
            };
        }
    }
}