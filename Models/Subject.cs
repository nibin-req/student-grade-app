namespace Crud_App.Models
{
    public class Subject
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }

        public List<Student> Students { get; set; }
    }
}
