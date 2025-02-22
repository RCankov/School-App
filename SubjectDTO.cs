using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp6_24.DTO {
    public class SubjectDTO {
        public int Id { get; set; }
        //[MaxLength(10)]
        [StringLength(10, ErrorMessage ="Název musí být delší než 4 a kratší než 10.", MinimumLength =4)]
        [DisplayName("Subject name")]
        public string Name { get; set; }
    }
}
