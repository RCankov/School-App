using Magistri.DTO;
using Magistri.Services;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Xml;

namespace Magistri.Controllers {
    public class FileUploadController : Controller {
        private StudentService studentService;

        public FileUploadController(StudentService studentService) {
            this.studentService = studentService;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file) {
            string filePath = Path.GetFullPath(file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create)) {
                await file.CopyToAsync(stream);
                stream.Close();

                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(filePath); 
                XmlElement root = xmlDocument.DocumentElement;

                foreach (XmlNode node in root.SelectNodes("/Students/Student")) {
                    StudentDto studentDto = new StudentDto() {
                        DateOfBirth = DateOnly.Parse(node.ChildNodes[2].InnerText, CultureInfo.CreateSpecificCulture("cs-CZ")),
                        FirstName = node.ChildNodes[0].InnerText,
                        LastName = node.ChildNodes[1].InnerText
                    };
                    await studentService.CreateStudentAsync(studentDto);
                }
            }
            return RedirectToAction("Index", "Students");
        }
    }
}
