using Magistri.DTO;
using Magistri.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Protocol;

namespace Magistri.Controllers {
    [Authorize]
    public class GradesController : Controller {
        GradeService gradeService;

        public GradesController(GradeService gradeService) {
            this.gradeService = gradeService;
        }
        [HttpGet]
        public async Task<IActionResult> IndexAsync() {
            var gradeVMs = await gradeService.GetAllGradesAsync();
            return View(gradeVMs);
        }
        [Authorize(Roles = "Teacher, Admin")]
        [HttpGet]
        public async Task<IActionResult> CreateAsync() {
            await FillSelects();
            return View();
        }

        private async Task FillSelects() {
            var dropdownsData = await gradeService.GetDropdownsData();
            ViewBag.Students = new SelectList(dropdownsData.Students, "Id", "FullName");
            ViewBag.Subjects = new SelectList(dropdownsData.Subjects, "Id", "Name");
        }
        [Authorize(Roles = "Teacher, Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateAsync(GradeDto newGrade) {
            await gradeService.CreateAsync(newGrade);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "Teacher, Admin")]
        public async Task<IActionResult> EditAsync(int id) {
            var gradeToEdit = await gradeService.GetByIdAsync(id);
            if (gradeToEdit==null) {
                return View("NotFound");
            }
            await FillSelects();
            return View(gradeToEdit);
        }

        [HttpPost]
        [Authorize(Roles = "Teacher, Admin")]
        public async Task<IActionResult> EditAsync(GradeDto editedGrade) {
            await gradeService.UpdateAsync(editedGrade);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteAsync(int id) {
            await gradeService.DeleteAsync(id);
            return RedirectToAction("Index");
        }

    }
}
