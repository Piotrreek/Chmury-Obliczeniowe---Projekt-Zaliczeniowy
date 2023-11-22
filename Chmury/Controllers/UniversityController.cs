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
    public IActionResult ManageStudents()
    {
        return View();
    }

    [Route("/manage-professors")]
    public IActionResult ManageProfessors()
    {
        return View();
    }

    [Route("/manage-courses")]
    public IActionResult ManageCourses()
    {
        return View();
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