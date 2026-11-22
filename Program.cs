using EFCore03.Models;
using Microsoft.EntityFrameworkCore;

namespace EFCore03
{
    internal class Program
    {
        static void Main()
        {
            using AppDbContext context = new AppDbContext();

            DbInitializer.DataSeed(context);

            Console.WriteLine("\n===== Create =====");
            AddInstructor();
            AddCourse(1, 4);
            AddCourse(99, 4);
            AddCourse(1, 99);
            AddStudents();
            EnrollStudent(5, 1);
            EnrollStudent(1, 1);
            EnrollStudent(1, 6);
            EnrollStudent(99, 1);

            Console.WriteLine("\n===== Read =====");
            FindCourse(3, "Advanced EF Core");
            FindCourse(99, "Unknown Course");
            FilterCourses(40, 100);
            PrintSubCategories(1);
            CourseRoster(1);

            Console.WriteLine("\n===== Update =====");
            ChangePrice(1, 54.99m);
            UpdateProgress(1, 1, 100);
            UpdateProgress(1, 1, 150);
            UpdateProgress(99, 99, 50);
            PublishCourse(5);

            Console.WriteLine("\n===== Delete =====");
            Unenroll(3, 4);
            DeleteCategoryUnsafe(3);
            DeleteCategorySafe(1);
            DeleteCategorySafe(7);
            DeleteCourse(3);

            Console.WriteLine("\n===== Bulk Operations =====");
            BulkPriceRaise(CourseLevel.Beginner);
            BulkDeleteDropped();
        }

        static void AddInstructor()
        {
            using AppDbContext context = new AppDbContext();

            Instructor instructor = new Instructor
            {
                FullName = "Mariam Nabil",
                Email = "mariam.nabil@example.com",
                Bio = "Data engineer and instructor.",
                DateJoined = DateTime.Now
            };

            context.Instructors.Add(instructor);
            context.SaveChanges();

            Console.WriteLine($"Instructor added with Id = {instructor.Id}");
        }

        static void AddCourse(int instructorId, int categoryId)
        {
            using AppDbContext context = new AppDbContext();

            if (!context.Instructors.Any(i => i.Id == instructorId))
            {
                Console.WriteLine($"Cannot add course: instructor {instructorId} does not exist.");
                return;
            }

            if (!context.Categories.Any(c => c.Id == categoryId))
            {
                Console.WriteLine($"Cannot add course: category {categoryId} does not exist.");
                return;
            }

            Course course = new Course
            {
                Title = "Design Patterns in C#",
                Description = "Common design patterns with practical examples.",
                Price = 89.99m,
                Level = CourseLevel.Intermediate,
                IsPublished = false,
                InstructorId = instructorId,
                CategoryId = categoryId
            };

            context.Courses.Add(course);
            context.SaveChanges();

            Console.WriteLine($"Course added with Id = {course.Id}");
        }

        static void AddStudents()
        {
            using AppDbContext context = new AppDbContext();

            List<Student> students = new List<Student>
            {
                new Student { FullName = "Hana Mostafa", Email = "hana@example.com", DateOfBirth = new DateTime(2001, 1, 12) },
                new Student { FullName = "Tamer Fathy", Email = "tamer@example.com", DateOfBirth = new DateTime(1997, 9, 25) },
                new Student { FullName = "Salma Ibrahim", Email = "salma@example.com", DateOfBirth = new DateTime(2000, 12, 5) }
            };

            context.Students.AddRange(students);
            context.SaveChanges();

            foreach (Student student in students)
            {
                Console.WriteLine($"Student added: {student.FullName} | Id = {student.Id}");
            }
        }

        static void EnrollStudent(int studentId, int courseId)
        {
            using AppDbContext context = new AppDbContext();

            Student? student = context.Students.Find(studentId);
            if (student == null)
            {
                Console.WriteLine($"Enrollment refused: student {studentId} does not exist.");
                return;
            }

            Course? course = context.Courses.Find(courseId);
            if (course == null)
            {
                Console.WriteLine($"Enrollment refused: course {courseId} does not exist.");
                return;
            }

            if (!course.IsPublished)
            {
                Console.WriteLine($"Enrollment refused: course '{course.Title}' is not published.");
                return;
            }

            if (context.Enrollments.Any(e => e.StudentId == studentId && e.CourseId == courseId))
            {
                Console.WriteLine($"Enrollment refused: {student.FullName} is already enrolled in '{course.Title}'.");
                return;
            }

            Enrollment enrollment = new Enrollment
            {
                StudentId = studentId,
                CourseId = courseId,
                EnrollmentDate = DateTime.Now,
                Progress = 0,
                Status = EnrollmentStatus.Active
            };

            context.Enrollments.Add(enrollment);
            context.SaveChanges();

            Console.WriteLine($"{student.FullName} enrolled in '{course.Title}'.");
        }

        static void FindCourse(int id, string title)
        {
            using AppDbContext context = new AppDbContext();

            Course? byId = context.Courses.Find(id);
            if (byId == null)
            {
                Console.WriteLine($"Course with Id {id}: not found");
            }
            else
            {
                Console.WriteLine($"Course with Id {id}: {byId.Title}");
            }

            Course? byTitle = context.Courses.FirstOrDefault(c => c.Title == title);
            if (byTitle == null)
            {
                Console.WriteLine($"Course with title '{title}': not found");
            }
            else
            {
                Console.WriteLine($"Course with title '{title}': Id = {byTitle.Id}");
            }
        }

        static void FilterCourses(decimal minPrice, decimal maxPrice)
        {
            using AppDbContext context = new AppDbContext();

            var courses = context.Courses
                .Where(c => c.Price >= minPrice && c.Price <= maxPrice)
                .OrderByDescending(c => c.Price)
                .Select(c => new { c.Title, c.Level, c.Price })
                .ToList();

            Console.WriteLine($"Courses priced between {minPrice} and {maxPrice}:");

            if (courses.Count == 0)
            {
                Console.WriteLine("No courses found.");
                return;
            }

            foreach (var c in courses)
            {
                Console.WriteLine($"{c.Title} | {c.Level} | {c.Price}");
            }
        }

        static void PrintSubCategories(int categoryId)
        {
            using AppDbContext context = new AppDbContext();

            Category? category = context.Categories.Find(categoryId);
            if (category == null)
            {
                Console.WriteLine($"Category {categoryId} not found.");
            }
            else
            {
                List<Category> subCategories = context.Categories
                    .Where(c => c.ParentCategoryId == categoryId)
                    .ToList();

                Console.WriteLine($"Sub-categories of '{category.Name}':");

                if (subCategories.Count == 0)
                {
                    Console.WriteLine("None.");
                }

                foreach (Category sub in subCategories)
                {
                    Console.WriteLine($"- {sub.Name}");
                }
            }

            List<Category> topLevel = context.Categories
                .Where(c => c.ParentCategoryId == null)
                .ToList();

            Console.WriteLine("Top-level categories:");
            foreach (Category top in topLevel)
            {
                Console.WriteLine($"- {top.Name}");
            }
        }

        static void CourseRoster(int courseId)
        {
            using AppDbContext context = new AppDbContext();

            Course? course = context.Courses.Find(courseId);
            if (course == null)
            {
                Console.WriteLine($"Course {courseId} not found.");
                return;
            }

            List<int> studentIds = context.Enrollments
                .Where(e => e.CourseId == courseId)
                .Select(e => e.StudentId)
                .ToList();

            Console.WriteLine($"Course '{course.Title}' has {studentIds.Count} enrolled student(s):");

            List<Student> students = context.Students
                .Where(s => studentIds.Contains(s.Id))
                .ToList();

            foreach (Student student in students)
            {
                Console.WriteLine($"- {student.FullName}");
            }
        }

        static void ChangePrice(int courseId, decimal newPrice)
        {
            using AppDbContext context = new AppDbContext();

            Course? course = context.Courses.Find(courseId);
            if (course == null)
            {
                Console.WriteLine($"Course {courseId} not found.");
                return;
            }

            if (newPrice <= 0)
            {
                Console.WriteLine("Price must be greater than zero.");
                return;
            }

            decimal oldPrice = course.Price;
            course.Price = newPrice;
            context.SaveChanges();

            Console.WriteLine($"Price of '{course.Title}' changed from {oldPrice} to {newPrice}.");
        }

        static void UpdateProgress(int studentId, int courseId, int progress)
        {
            using AppDbContext context = new AppDbContext();

            if (progress < 0 || progress > 100)
            {
                Console.WriteLine($"Invalid progress {progress}: it must be between 0 and 100.");
                return;
            }

            Enrollment? enrollment = context.Enrollments.Find(studentId, courseId);
            if (enrollment == null)
            {
                Console.WriteLine($"Enrollment (student {studentId}, course {courseId}) not found.");
                return;
            }

            enrollment.Progress = progress;

            if (progress == 100)
            {
                enrollment.Status = EnrollmentStatus.Completed;
            }

            context.SaveChanges();

            Console.WriteLine($"Progress updated to {progress}. Status = {enrollment.Status}");
        }

        static void PublishCourse(int courseId)
        {
            using AppDbContext context = new AppDbContext();

            Course? course = context.Courses.Find(courseId);
            if (course == null)
            {
                Console.WriteLine($"Course {courseId} not found.");
                return;
            }

            if (course.IsPublished)
            {
                Console.WriteLine($"Course '{course.Title}' is already published.");
                return;
            }

            course.IsPublished = true;
            course.PublishedDate = DateTime.Now;
            context.SaveChanges();

            Console.WriteLine($"Course '{course.Title}' published on {course.PublishedDate}.");
        }

        static void Unenroll(int studentId, int courseId)
        {
            using AppDbContext context = new AppDbContext();

            Enrollment? enrollment = context.Enrollments.Find(studentId, courseId);
            if (enrollment == null)
            {
                Console.WriteLine($"Enrollment (student {studentId}, course {courseId}) not found.");
                return;
            }

            context.Enrollments.Remove(enrollment);
            context.SaveChanges();

            Console.WriteLine($"Student {studentId} unenrolled from course {courseId}.");
        }

        static void DeleteCategoryUnsafe(int categoryId)
        {
            using AppDbContext context = new AppDbContext();

            Category? category = context.Categories.Find(categoryId);
            if (category == null)
            {
                Console.WriteLine($"Category {categoryId} not found.");
                return;
            }

            try
            {
                context.Categories.Remove(category);
                context.SaveChanges();
                Console.WriteLine($"Category '{category.Name}' deleted.");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}");
                Console.WriteLine($"Inner: {ex.InnerException?.Message}");
            }
        }

        static void DeleteCategorySafe(int categoryId)
        {
            using AppDbContext context = new AppDbContext();

            Category? category = context.Categories.Find(categoryId);
            if (category == null)
            {
                Console.WriteLine($"Category {categoryId} not found.");
                return;
            }

            if (context.Categories.Any(c => c.ParentCategoryId == categoryId))
            {
                Console.WriteLine($"Cannot delete '{category.Name}': it still has sub-categories.");
                return;
            }

            if (context.Courses.Any(c => c.CategoryId == categoryId))
            {
                Console.WriteLine($"Cannot delete '{category.Name}': it still has courses.");
                return;
            }

            context.Categories.Remove(category);
            context.SaveChanges();

            Console.WriteLine($"Category '{category.Name}' deleted safely.");
        }

        static void DeleteCourse(int courseId)
        {
            using AppDbContext context = new AppDbContext();

            Course? course = context.Courses.Find(courseId);
            if (course == null)
            {
                Console.WriteLine($"Course {courseId} not found.");
                return;
            }

            int before = context.Enrollments.Count(e => e.CourseId == courseId);
            Console.WriteLine($"Enrollments of '{course.Title}' before delete: {before}");

            context.Courses.Remove(course);
            context.SaveChanges();

            // The Enrollment -> Course relationship is configured with DeleteBehavior.Cascade,
            // so SQL Server deletes the related enrollments automatically when the course is deleted.
            int after = context.Enrollments.Count(e => e.CourseId == courseId);
            Console.WriteLine($"Enrollments of '{course.Title}' after delete: {after}");
        }

        static void BulkPriceRaise(CourseLevel level)
        {
            using AppDbContext context = new AppDbContext();

            int rows = context.Courses
                .Where(c => c.IsPublished && c.Level == level)
                .ExecuteUpdate(s => s.SetProperty(c => c.Price, c => c.Price * 1.10m));

            Console.WriteLine($"Price raised by 10% for {rows} published {level} course(s).");
        }

        static void BulkDeleteDropped()
        {
            using AppDbContext context = new AppDbContext();

            int rows = context.Enrollments
                .Where(e => e.Status == EnrollmentStatus.Dropped)
                .ExecuteDelete();

            Console.WriteLine($"{rows} dropped enrollment(s) deleted.");
        }
    }
}