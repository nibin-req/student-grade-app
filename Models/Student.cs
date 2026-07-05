namespace Crud_App.Models
{
    public class Student
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }

        public int SubjectId { get; set; }
        public Subject Subject { get; set; }

        public int Grade { get; set; }

        // BUSINESS RULE (IMPORTANT)
        public string Remarks => Grade >= 75 ? "PASS" : "FAIL";
    }
}
