using Microsoft.EntityFrameworkCore;
using SchoolWebApp6_24.DTO;
using SchoolWebApp6_24.Models;
using System.Linq;

namespace SchoolWebApp6_24.Services {
    public class SubjectService {
        private ApplicationDbContext _dbContext;

        public SubjectService(ApplicationDbContext dbContext) {
            _dbContext = dbContext;
        }

        public IEnumerable<SubjectDTO> GetSubjects() {
            var allSubjects = _dbContext.Subjects;
            var subjectsDtos = new List<SubjectDTO>();
            foreach (var subject in allSubjects) {
                subjectsDtos.Add(ModelToDto(subject));
            }
            return subjectsDtos;
        }

        public async Task AddSubjectAsync(SubjectDTO subjectDto) {
            await _dbContext.Subjects.AddAsync(
                DtoToModel(subjectDto)
            );
            await _dbContext.SaveChangesAsync();
        }

        private static Subject DtoToModel(SubjectDTO subjectDto) {
            return new Subject {
                Id = subjectDto.Id,
                Name = subjectDto.Name
            };
        }

        private async Task<Subject> VerifyExistence(int id) {
            var subject =  await _dbContext.Subjects.FirstOrDefaultAsync(subject => subject.Id == id);
            if (subject == null) {
                return null;
            }
            return subject;
        }

        internal async Task<SubjectDTO> GetByIdAsync(int id) {
            var subject = await VerifyExistence(id);
            return ModelToDto(subject);
        }

        private static SubjectDTO ModelToDto(Subject subject) {
            return new SubjectDTO {
               Name = subject.Name,
                Id = subject.Id
            };
        }

        internal async Task UpdateAsync(int id, SubjectDTO subjectDTO) {
            _dbContext.Subjects.Update(DtoToModel(subjectDTO));
            await _dbContext.SaveChangesAsync();
        }

        internal async Task DeleteAsync(int id) {
            var subjectToDelete = await _dbContext.Subjects.FirstOrDefaultAsync(subject => subject.Id == id);
            //if (subjectToDelete == null) {
            //    return null;
            //}
            _dbContext.Subjects.Remove(subjectToDelete);
            await _dbContext.SaveChangesAsync();
        }
    }
}
