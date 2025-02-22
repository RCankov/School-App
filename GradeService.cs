
using Magistri.DTO;
using Magistri.Models;
using Magistri.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Magistri.Services {
    public class GradeService {
        ApplicationDbContext context;
        public GradeService(ApplicationDbContext dbContext) {
            context = dbContext;
        }

        internal async Task<GradesDropdownsVM> GetDropdownsData() {
            var gradesDropdownsData = new GradesDropdownsVM() {
                Students = await context.Students.OrderBy(student => student.LastName).ToListAsync(),
                Subjects = await context.Subjects.OrderBy(subject => subject.Name).ToListAsync(),
            };
            return gradesDropdownsData;
        }

        public async Task CreateAsync(GradeDto newGrade) {
            Grade gradeToInsert = await DtoToModelAsync(newGrade);
            await context.Grades.AddAsync(gradeToInsert);
            await context.SaveChangesAsync();
        }

        private async Task<Grade> DtoToModelAsync(GradeDto newGrade) {
            return new Grade {
                Id = newGrade.Id,
                Date = DateTime.Now,
                Mark = newGrade.Mark,
                Student = await context.Students.FirstOrDefaultAsync(st => st.Id == newGrade.StudentId),
                Subject = await context.Subjects.FirstOrDefaultAsync(sub => sub.Id == newGrade.SubjectId),
                Topic = newGrade.Topic,
            };
        }

        public async Task<IEnumerable<GradesVM>> GetAllGradesAsync() {
            var grades = await context.Grades.Include(gr=>gr.Student).Include(gr=>gr.Subject).ToListAsync();
            List<GradesVM> gradesVMs = new List<GradesVM>();
            foreach (var grade in grades) {
                gradesVMs.Add(new GradesVM {
                    Id = grade.Id,
                    Date = grade.Date,
                    Mark = grade.Mark,
                    StudentName = grade.Student.FullName,
                    SubjectName = grade.Subject.Name,
                    Topic = grade.Topic,
                });
            }
            return gradesVMs;
        }

        internal async Task<GradeDto> GetByIdAsync(int id) {
            var gradeToEdit = await context.Grades.Include(gr=>gr.Student).Include(gr=>gr.Subject).FirstOrDefaultAsync(gr => gr.Id == id);
            if (gradeToEdit == null) {
                return null;
            }
            return ModelToDto(gradeToEdit);
        }

        private GradeDto ModelToDto(Grade gradeToEdit) {
            return new GradeDto {
                Id = gradeToEdit.Id,
                Date = gradeToEdit.Date,
                Mark = gradeToEdit.Mark,
                StudentId = gradeToEdit.Student.Id,
                SubjectId = gradeToEdit.Subject.Id,
                Topic = gradeToEdit.Topic,
            };
        }

        internal async Task UpdateAsync(GradeDto editedGrade) {
            context.Grades.Update(await DtoToModelAsync(editedGrade));
            await context.SaveChangesAsync();
        }

        internal async Task DeleteAsync(int id) {
            var gradeToDelete = await context.Grades.FirstOrDefaultAsync(gr => gr.Id == id);
            context.Grades.Remove(gradeToDelete);
            await context.SaveChangesAsync();
        }
    }
}
