using System.ComponentModel.DataAnnotations.Schema;

namespace Magistri.Models {
    public class Student {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        [NotMapped]
        public string FullName { get=>$"{LastName}, {FirstName}"; }
    }
}
