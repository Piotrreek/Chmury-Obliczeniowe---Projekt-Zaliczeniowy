namespace Chmury.Models;

public class MarkOneCourseAsPrerequisiteToAnotherRequest
{
    public string PrerequisiteCourseName { get; set; } = default!;
    public string AdvancedCourseName { get; set; } = default!;
}