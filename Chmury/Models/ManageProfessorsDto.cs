namespace Chmury.Models;

public class ManageProfessorsDto(IEnumerable<ProfessorDto> professors, IEnumerable<UniversityDto> universities,
    List<CourseDto> courses)
{
    public IEnumerable<ProfessorDto> Professors { get; set; } = professors;
    public IEnumerable<UniversityDto> Universities { get; set; } = universities;
    public List<CourseDto> Courses { get; set; } = courses;
}