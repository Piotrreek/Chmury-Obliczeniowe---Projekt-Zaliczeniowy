using Chmury.Models;
using OneOf;
using OneOf.Types;

namespace Chmury.Services;

using Students = List<StudentDto>;
using Professors = List<ProfessorDto>;
using Courses = List<CourseDto>;

public interface IUniversityService
{
    Task<OneOf<Success, string>> CreateUniversity(string universityName);
    Task<OneOf<Success, string>> CreateProfessor(string universityName, string professorName);
    Task<OneOf<Success, string>> CreateCourse(string courseName, string universityName);
    Task<OneOf<Success<Professors>, string>> GetUniversityProfessors(string universityName);
    Task<OneOf<Success<Courses>, string>> GetUniversityCourses(string universityName);

    Task<OneOf<Success, string>> AssignProfessorToCourse(string professorName,
        string courseName, string universityName);

    Task<OneOf<Success<Students>, string>> GetUniversityStudents(string universityName);
    Task<OneOf<Success, string>> CreateStudent(string studentName, string universityName);

    Task<OneOf<Success, string>> EnrollStudentToCourse(string studentName,
        string courseName, string universityName);

    Task<OneOf<Success, string>> MarkOneCourseAsPrerequisiteToAnother(string prerequisiteCourseName,
        string advancedCourseName, string universityName);

    Task<OneOf<Success<List<UniversityDto>>, string>> GetUniversities(string searchPhrase = "");
    Task<OneOf<Success<Students>, string>> GetStudents();
    Task<OneOf<Success<Courses>, string>> GetCourses();
    Task<OneOf<Success<Professors>, string>> GetProfessors();
    Task<OneOf<Success<Courses>, string>> GetStudentCourses(string studentName, string universityName);
    Task<OneOf<Success<Courses>, string>> GetProfessorCourses(string professorName, string universityName);
}

public class UniversityService(INeo4jService neo4JService) : IUniversityService
{
    private readonly INeo4jService _neo4JService = neo4JService;
    private readonly Dictionary<string, object> _parameters = new();
    private string _query = string.Empty;

    public async Task<OneOf<Success, string>> CreateUniversity(string universityName)
    {
        _query = "MERGE (:University {name: $universityName})";
        _parameters.Add("universityName", universityName);

        return await Create();
    }

    public async Task<OneOf<Success, string>> CreateProfessor(string universityName, string professorName)
    {
        _query = """
                 MATCH (university:University {name: $universityName})
                 MERGE (:Professor {name: $professorName})-[:WORKS_AT]->(university)
                 """;
        _parameters.Add("universityName", universityName);
        _parameters.Add("professorName", professorName);

        return await Create();
    }

    public async Task<OneOf<Success, string>> CreateCourse(string courseName, string universityName)
    {
        _query = """
                 MATCH (university:University {name: $universityName})
                 MERGE (:Course {name: $courseName})-[:LED_ON]->(university)
                 """;
        _parameters.Add("courseName", courseName);
        _parameters.Add("universityName", universityName);

        return await Create();
    }

    public async Task<OneOf<Success<Professors>, string>> GetUniversityProfessors(string universityName)
    {
        _query = """
                 MATCH (professor:Professor)-[:WORKS_AT]->(university:University {name: $universityName})
                 RETURN professor
                 """;
        _parameters.Add("universityName", universityName);

        return await _neo4JService.ReadListAsync<ProfessorDto>(_query, _parameters);
    }

    public async Task<OneOf<Success<Courses>, string>> GetUniversityCourses(string universityName)
    {
        _query = """
                 MATCH (course:Course)-[:LED_ON]->(university:University {name: $universityName})
                 RETURN course
                 """;
        _parameters.Add("universityName", universityName);

        return await _neo4JService.ReadListAsync<CourseDto>(_query, _parameters);
    }

    public async Task<OneOf<Success, string>> AssignProfessorToCourse(string professorName, string courseName,
        string universityName)
    {
        _query = """
                 MATCH (university:University {name: $universityName})
                 MATCH (professor:Professor {name: $professorName})-[:WORKS_AT]->(university)
                 MATCH (course:Course {name: $courseName})-[:LED_ON]->(university)
                 MERGE (professor)-[:TEACHES]->(course);
                 """;
        _parameters.Add("professorName", professorName);
        _parameters.Add("courseName", courseName);
        _parameters.Add("universityName", universityName);

        return await Create();
    }

    public async Task<OneOf<Success<Students>, string>> GetUniversityStudents(string universityName)
    {
        _query = """
                 MATCH (student:Student)-[:LEARNS_AT]->(university:University {name: $universityName})
                 RETURN student
                 """;
        _parameters.Add("universityName", universityName);

        return await _neo4JService.ReadListAsync<StudentDto>(_query, _parameters);
    }

    public async Task<OneOf<Success, string>> CreateStudent(string studentName, string universityName)
    {
        _query = """
                 MATCH (university:University {name: $universityName})
                 MERGE (:Student {name: $studentName})-[:LEARNS_AT]->(university)
                 """;
        _parameters.Add("studentName", studentName);
        _parameters.Add("universityName", universityName);

        return await Create();
    }

    public async Task<OneOf<Success, string>> EnrollStudentToCourse(string studentName, string courseName,
        string universityName)
    {
        _query = """
                 MATCH (university:University {name: $universityName})
                 MATCH (student:Student {name: $studentName})-[:LEARNS_AT]->(university)
                 MATCH (course:Course {name: $courseName})-[:LED_ON]->(university)
                 MERGE (student)-[:ENROLLED_INTO]->(course)
                 """;
        _parameters.Add("studentName", studentName);
        _parameters.Add("courseName", courseName);
        _parameters.Add("universityName", universityName);

        return await Create();
    }

    public async Task<OneOf<Success, string>> MarkOneCourseAsPrerequisiteToAnother(string prerequisiteCourseName,
        string advancedCourseName, string universityName)
    {
        _query = """
                 MATCH (university:University {name: $universityName})
                 MATCH (advancedCourse:Course {name: $advancedCourseName})-[:LED_ON]->(university)
                 MATCH (prerequisiteCourse:Course {name: $prerequisiteCourseName})-[:LED_ON]->(university)
                 MERGE (prerequisiteCourse)-[:PREREQUISITE]->(advancedCourse)
                 """;
        _parameters.Add("advancedCourseName", advancedCourseName);
        _parameters.Add("prerequisiteCourseName", prerequisiteCourseName);
        _parameters.Add("universityName", universityName);

        return await Create();
    }

    public async Task<OneOf<Success<List<UniversityDto>>, string>> GetUniversities(string searchPhrase = "")
    {
        _query = """
                 MATCH (u:University)
                 WHERE toUpper(u.name) CONTAINS toUpper($searchPhrase)
                 RETURN u
                 """;
        _parameters.Add("searchPhrase", searchPhrase);

        return await _neo4JService.ReadListAsync<UniversityDto>(_query, _parameters);
    }

    public async Task<OneOf<Success<Students>, string>> GetStudents()
    {
        _query = """
                 MATCH (s:Student)-[:LEARNS_AT]->(u:University)
                 RETURN s.name AS name, u.name AS universityName
                 """;

        return await _neo4JService.ReadListAsync<StudentDto>(_query, new List<string> { "name", "universityName" });
    }

    public async Task<OneOf<Success<Courses>, string>> GetCourses()
    {
        _query = """
                 MATCH (c:Course)-[:LED_ON]->(u:University)
                 RETURN c.name AS name, u.name AS universityName
                 """;

        return await _neo4JService.ReadListAsync<CourseDto>(_query, new List<string> { "name", "universityName" });
    }

    public async Task<OneOf<Success<Professors>, string>> GetProfessors()
    {
        _query = """
                 MATCH (p:Professor)-[:WORKS_AT]->(u:University)
                 RETURN p.name AS name, u.name AS universityName
                 """;

        return await _neo4JService.ReadListAsync<ProfessorDto>(_query, new List<string> { "name", "universityName" });
    }

    public async Task<OneOf<Success<Courses>, string>> GetStudentCourses(string studentName, string universityName)
    {
        _query = """
                 MATCH (u:University {name: $universityName})
                 MATCH (u)<-[:LEARNS_AT]-(student:Student {name: $studentName})-[:ENROLLED_INTO]->(course:Course)-[:LED_ON]->(u)
                 RETURN course
                 """;
        _parameters.Add("studentName", studentName);
        _parameters.Add("universityName", universityName);

        return await _neo4JService.ReadListAsync<CourseDto>(_query, _parameters);
    }

    public async Task<OneOf<Success<Courses>, string>> GetProfessorCourses(string professorName, string universityName)
    {
        _query = """
                 MATCH (u:University {name: $universityName})
                 MATCH (u)<-[:WORKS_AT]-(professor:Professor {name: $professorName})-[:TEACHES]->(course:Course)-[:LED_ON]->(u)
                 RETURN course
                 """;
        _parameters.Add("professorName", professorName);
        _parameters.Add("universityName", universityName);

        return await _neo4JService.ReadListAsync<CourseDto>(_query, _parameters);
    }

    private async Task<OneOf<Success, string>> Create()
    {
        return await _neo4JService.WriteAsync<int>(_query, _parameters);
    }
}