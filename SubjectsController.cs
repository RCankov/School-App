using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp6_24.DTO;
using SchoolWebApp6_24.Services;

namespace SchoolWebApp6_24.Controllers {
    [Authorize(Roles = "Admin")]
    public class SubjectsController : Controller {
        private SubjectService subjectService;

        public SubjectsController(SubjectService subjectService) {
            this.subjectService = subjectService;
        }

        public IActionResult Index() {
            IEnumerable<SubjectDTO> allSubjects = subjectService.GetSubjects();
            return View(allSubjects);
        }
        public IActionResult Create() {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateAsync(SubjectDTO subjectDTO) {
                await subjectService.AddSubjectAsync(subjectDTO);
                return RedirectToAction("Index");
            
   
        }
        [HttpGet]
        public async Task<IActionResult> UpdateAsync(int id) {
            var subjectToEdit = await subjectService.GetByIdAsync(id);
            if (subjectToEdit == null) {
                return View("NotFound");
            }
            return View(subjectToEdit);
        }

        [HttpPost]
        public async Task<IActionResult> Update(SubjectDTO subjectDTO, int id) {
            await subjectService.UpdateAsync(id, subjectDTO);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id) {
            var subjectToDelete = await subjectService.GetByIdAsync(id);
            if (subjectToDelete == null) {
                return View("NotFound");
            }
            await subjectService.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}
