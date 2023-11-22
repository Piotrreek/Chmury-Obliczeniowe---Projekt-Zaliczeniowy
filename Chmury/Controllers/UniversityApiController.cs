using Chmury.Models;
using Chmury.Services;
using Microsoft.AspNetCore.Mvc;
using OneOf;
using OneOf.Types;

namespace Chmury.Controllers;

[ApiController]
[Route("university")]
public class UniversityApiController(IUniversityService universityService) : ControllerBase
{
    private readonly IUniversityService _universityService = universityService;

    [HttpPost]
    public async Task<IActionResult> CreateUniversity([FromBody] CreateUniversityRequest request)
    {
        var result = await _universityService.CreateUniversity(request.UniversityName);

        return result.Match<IActionResult>(
            _ => Ok(),
            error => StatusCode(StatusCodes.Status503ServiceUnavailable, error)
        );
    }

    [HttpPost("{universityName:required}/professor")]
    public async Task<IActionResult> CreateProfessor([FromRoute] string universityName,
        [FromBody] CreateProfessorRequest request)
    {
        var result = await _universityService.CreateProfessor(universityName, request.ProfessorName);

        return HandleResult(result);
    }

    [HttpPost("{universityName:required}/course")]
    public async Task<IActionResult> CreateCourse([FromRoute] string universityName,
        [FromBody] CreateCourseRequest request)
    {
        var result = await _universityService.CreateCourse(request.CourseName, universityName);

        return HandleResult(result);
    }

    [HttpGet("{universityName:required}/student")]
    public async Task<IActionResult> GetUniversityStudents([FromRoute] string universityName)
    {
        var result = await _universityService.GetUniversityStudents(universityName);

        return HandleResult(result);
    }
    
    [HttpGet("{universityName:required}/professor")]
    public async Task<IActionResult> GetUniversityProfessors([FromRoute] string universityName)
    {
        var result = await _universityService.GetUniversityProfessors(universityName);

        return HandleResult(result);
    }
    
    [HttpGet("{universityName:required}/course")]
    public async Task<IActionResult> GetUniversityCourses([FromRoute] string universityName)
    {
        var result = await _universityService.GetUniversityCourses(universityName);

        return HandleResult(result);
    }

    [HttpPost("{universityName:required}/student")]
    public async Task<IActionResult> CreateStudent([FromRoute] string universityName,
        [FromBody] CreateStudentRequest request)
    {
        var result = await _universityService.CreateStudent(request.StudentName, universityName);

        return HandleResult(result);
    }

    [HttpPatch("{universityName:required}/professor/{professorName:required}/course/{courseName:required}")]
    public async Task<IActionResult> AssignProfessorToCourse([FromRoute] string universityName,
        [FromRoute] string professorName,
        [FromRoute] string courseName)
    {
        var result =
            await _universityService.AssignProfessorToCourse(professorName,
                courseName, universityName);

        return HandleResult(result);
    }

    [HttpPatch("{universityName:required}/student/{studentName:required}/course/{courseName:required}")]
    public async Task<IActionResult> EnrollStudentToCourse([FromRoute] string universityName,
        [FromRoute] string studentName,
        [FromRoute] string courseName)
    {
        var result =
            await _universityService.EnrollStudentToCourse(studentName, courseName, universityName);

        return HandleResult(result);
    }

    [HttpPatch("{universityName:required}/course")]
    public async Task<IActionResult> MarkOneCourseAsPrerequisiteToAnother([FromRoute] string universityName,
        [FromBody] MarkOneCourseAsPrerequisiteToAnotherRequest request)
    {
        var result =
            await _universityService.MarkOneCourseAsPrerequisiteToAnother(request.PrerequisiteCourseName,
                request.AdvancedCourseName, universityName);

        return HandleResult(result);
    }

    private IActionResult HandleResult(OneOf<Success, string> result)
    {
        return result.Match<IActionResult>(
            _ => Ok(),
            error => StatusCode(StatusCodes.Status503ServiceUnavailable, error)
        );
    }

    private IActionResult HandleResult<T>(OneOf<Success<List<T>>, string> result)
    {
        return result.Match<IActionResult>(
            ok => Ok(ok.Value),
            _ => StatusCode(StatusCodes.Status503ServiceUnavailable)
        );
    }
}