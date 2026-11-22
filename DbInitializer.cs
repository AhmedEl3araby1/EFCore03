using EFCore03.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCore03
{
    public class DbInitializer
    {
        public static void DataSeed(AppDbContext context)
        {
            context.Database.Migrate();

            if (!context.Students.Any())
            {
                context.Students.AddRange(
                    new Student { FullName = "Mona Hassan", Email = "mona@example.com", DateOfBirth = new DateTime(2000, 5, 14) },
                    new Student { FullName = "Youssef Ali", Email = "youssef@example.com", DateOfBirth = new DateTime(1999, 8, 3) },
                    new Student { FullName = "Nada Samir", Email = "nada@example.com", DateOfBirth = new DateTime(2001, 11, 22) },
                    new Student { FullName = "Karim Adel", Email = "karim@example.com", DateOfBirth = new DateTime(1998, 2, 9) },
                    new Student { FullName = "Laila Tarek", Email = "laila@example.com", DateOfBirth = new DateTime(2002, 7, 30) }
                );
                context.SaveChanges();
            }

            if (!context.Enrollments.Any())
            {
                List<Student> students = context.Students.OrderBy(s => s.Id).Take(5).ToList();

                if (students.Count < 5)
                {
                    return;
                }

                context.Enrollments.AddRange(
                    new Enrollment { StudentId = students[0].Id, CourseId = 1, EnrollmentDate = new DateTime(2026, 4, 1), Progress = 40, Status = EnrollmentStatus.Active, FinalGrade = null },
                    new Enrollment { StudentId = students[0].Id, CourseId = 2, EnrollmentDate = new DateTime(2026, 4, 2), Progress = 100, Status = EnrollmentStatus.Completed, FinalGrade = 92.5m },
                    new Enrollment { StudentId = students[1].Id, CourseId = 1, EnrollmentDate = new DateTime(2026, 4, 3), Progress = 100, Status = EnrollmentStatus.Completed, FinalGrade = 85m },
                    new Enrollment { StudentId = students[1].Id, CourseId = 3, EnrollmentDate = new DateTime(2026, 4, 5), Progress = 20, Status = EnrollmentStatus.Active, FinalGrade = null },
                    new Enrollment { StudentId = students[2].Id, CourseId = 2, EnrollmentDate = new DateTime(2026, 4, 6), Progress = 10, Status = EnrollmentStatus.Dropped, FinalGrade = null },
                    new Enrollment { StudentId = students[2].Id, CourseId = 4, EnrollmentDate = new DateTime(2026, 4, 7), Progress = 65, Status = EnrollmentStatus.Active, FinalGrade = null },
                    new Enrollment { StudentId = students[3].Id, CourseId = 1, EnrollmentDate = new DateTime(2026, 4, 8), Progress = 5, Status = EnrollmentStatus.Dropped, FinalGrade = null },
                    new Enrollment { StudentId = students[4].Id, CourseId = 3, EnrollmentDate = new DateTime(2026, 4, 9), Progress = 100, Status = EnrollmentStatus.Completed, FinalGrade = 78m }
                );
                context.SaveChanges();
            }
        }
    }
}