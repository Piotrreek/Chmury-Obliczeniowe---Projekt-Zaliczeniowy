namespace Chmury.Models;

public class ManageStudentsDto(IEnumerable<StudentDto> students, IEnumerable<UniversityDto> universities,
    IEnumerable<CourseDto> courses)
{
    public IEnumerable<StudentDto> Students { get; set; } = students;
    public IEnumerable<UniversityDto> Universities { get; set; } = universities;
    public IEnumerable<CourseDto> Courses { get; set; } = courses;
}