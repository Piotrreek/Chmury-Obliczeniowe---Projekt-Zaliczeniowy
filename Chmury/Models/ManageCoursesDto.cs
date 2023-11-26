namespace Chmury.Models;

public class ManageCoursesDto(IEnumerable<CourseDto> courses, IEnumerable<UniversityDto> universities)
{
    public IEnumerable<CourseDto> Courses { get; set; } = courses;
    public IEnumerable<UniversityDto> Universities { get; set; } = universities;
}