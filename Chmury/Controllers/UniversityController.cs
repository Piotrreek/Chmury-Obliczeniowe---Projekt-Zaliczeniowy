using Chmury.Models;
using Chmury.Services;
using Microsoft.AspNetCore.Mvc;

namespace Chmury.Controllers;

public class UniversityController(IUniversityService universityService) : Controller
{
    private readonly IUniversityService _universityService = universityService;

    [Route("/")]
    public IActionResult Index()
    {
        return View();
    }

    [Route("/manage-students")]
    public async Task<IActionResult> ManageStudents()
    {
        var studentsResult = await _universityService.GetStudents();
        var universitiesResult = await _universityService.GetUniversities();
        var coursesResult = await _universityService.GetCourses();

        var studentsSuccessful = studentsResult.TryPickT0(out var students, out var error1);
        var universitiesSuccessful = universitiesResult.TryPickT0(out var universities, out var error2);
        var coursesSuccessful = coursesResult.TryPickT0(out var courses, out var error3);

        if (studentsSuccessful && universitiesSuccessful && coursesSuccessful)
        {
            return View(new ManageStudentsDto(students.Value, universities.Value, courses.Value));
        }

        var error = string.Join("\n", new List<string?> { error1, error2, error3 });

        return RedirectToAction("Error", new { message = error });
    }

    [Route("/manage-professors")]
    public async Task<IActionResult> ManageProfessors()
    {
        var universitiesResult = await _universityService.GetUniversities();
        var professorsResult = await _universityService.GetProfessors();
        var coursesResult = await _universityService.GetCourses();

        var coursesSuccessful = coursesResult.TryPickT0(out var courses, out var error3);
        var universitiesSuccessful = universitiesResult.TryPickT0(out var universities, out var error1);
        var professorsSuccessful = professorsResult.TryPickT0(out var professors, out var error2);

        if (universitiesSuccessful && coursesSuccessful && professorsSuccessful)
        {
            return View(new ManageProfessorsDto(professors.Value, universities.Value, courses.Value));
        }

        var error = string.Join("\n", new List<string?> { error1, error2, error3 });

        return RedirectToAction("Error", new { message = error });
    }

    [Route("/manage-courses")]
    public async Task<IActionResult> ManageCourses()
    {
        var universitiesResult = await _universityService.GetUniversities();
        var coursesResult = await _universityService.GetCourses();

        var universitiesSuccessful = universitiesResult.TryPickT0(out var universities, out var error1);
        var coursesSuccessful = coursesResult.TryPickT0(out var courses, out var error2);

        if (universitiesSuccessful && coursesSuccessful)
        {
            return View(new ManageCoursesDto(courses.Value, universities.Value));
        }

        var error = string.Join("\n", new List<string?> { error1, error2 });

        return RedirectToAction("Error", new { message = error });
    }

    [Route("/manage-universities")]
    public async Task<IActionResult> ManageUniversities()
    {
        var model = await _universityService.GetUniversities();

        return model.Match<IActionResult>(
            ok => View(ok.Value),
            error => RedirectToAction("Error", new { message = error })
        );
    }

    [Route("/error")]
    public IActionResult Error([FromQuery] string message)
    {
        return View("Error", message);
    }
}