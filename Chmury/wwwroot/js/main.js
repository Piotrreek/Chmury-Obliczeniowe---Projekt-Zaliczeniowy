const RelationType = {
    Student: "student",
    Professor: "professor",
    Course: "course"
}

const createUniversity = async () => {
    const name = document.querySelector(".university-name")?.value;
    const error = document.querySelector(".error");

    if (!name) {
        error.innerText = "Nazwa nie może być pusta";
        return;
    }

    const body = {
        universityName: name
    };

    await httpPost("/university", body);
}

const createCourse = async () => {
    const name = document.querySelector(".course-name").value;
    const universityName = document.querySelector(".university").value;
    const error = document.querySelector(".error");
    
    if (!name) {
        error.innerText = "Nazwa nie może być pusta";
        return;
    }
    
    const body = {
        courseName: name
    };
    
    await httpPost(`/university/${universityName}/course`, body);
}

const createProfessor = async () => {
    const name = document.querySelector(".professor-name").value;
    const universityName = document.querySelector(".university").value;
    const error = document.querySelector(".error");

    if (!name) {
        error.innerText = "Nazwa nie może być pusta";
        return;
    }

    const body = {
        professorName: name
    };

    await httpPost(`/university/${universityName}/professor`, body);
}

const createStudent = async () => {
    const studentName = document.querySelector(".student-name").value;
    const universityName = document.querySelector(".university").value;
    const error = document.querySelector(".error");
    
    if (!studentName) {
        error.innerText = "Imię i nazwisko nie może być puste";
        return;
    }
    
    const body = {
        studentName: studentName
    };
    
    await httpPost(`/university/${universityName}/student`, body);
}

const assignStudentToCourse = async () => {
    const studentName = document.querySelector(".student").value.split("-")[0];
    const courseName = document.querySelector(".course").value.split("-")[0];
    const universityName  = document.querySelector(".course").value.split("-")[1];
    
    await httpPatch(`/university/${universityName}/student/${studentName}/course/${courseName}`);
}

const assignProfessorToCourse = async () => {
    const professorName = document.querySelector(".professor").value.split("-")[0];
    const courseName = document.querySelector(".course").value.split("-")[0];
    const universityName  = document.querySelector(".course").value.split("-")[1];

    await httpPatch(`/university/${universityName}/professor/${professorName}/course/${courseName}`);
}

const getProfessorCourses = async (universityName, professorName) => {
    const seeResultsHeading = document.querySelector(".see-results__heading");
    seeResultsHeading.innerText = `Kursy, które prowadzi ${professorName}`;

    const results = await httpGet(`/university/${universityName}/professor/${professorName}/course`);
    if (!results) {
        return;
    }

    const resultsList = document.querySelector(".see-results");
    resultsList.innerHTML = '';
    results.map(course => {
        const li = document.createElement("li");
        li.innerText = course.name;
        resultsList.appendChild(li);
    })
}

const getStudentCourses = async (universityName, studentName) => {
    const seeResultsHeading = document.querySelector(".see-results__heading");
    seeResultsHeading.innerText = `Kursy, w których uczestniczy ${studentName}`;
    
    const results = await httpGet(`/university/${universityName}/student/${studentName}/course`);
    if (!results) {
        return;
    }

    const resultsList = document.querySelector(".see-results");
    resultsList.innerHTML = '';
    results.map(course => {
        const li = document.createElement("li");
        li.innerText = course.name;
        resultsList.appendChild(li);
    })
}

const getUniversityProfessors = async (university) => {
    await getUniversityRelationResults(university, RelationType.Professor);
}

const getUniversityCourses = async (university) => {
    await getUniversityRelationResults(university, RelationType.Course);
}

const getUniversityStudents = async (university) => {
    await getUniversityRelationResults(university, RelationType.Student);
}

const getUniversityRelationResults = async (university, relationType) => {
    const seeResultsHeading = document.querySelector(".see-results__heading");
    switch (relationType) {
        case RelationType.Student:
            seeResultsHeading.innerText = `Studenci uczelni ${university}`;
            break;
        case RelationType.Professor:
            seeResultsHeading.innerText = `Profesorowie uczelni ${university}`;
            break;
        case RelationType.Course:
            seeResultsHeading.innerText = `Kursy prowadzone na uczelni ${university}`;
            break;
    }

    const results = await httpGet(`/university/${university}/${relationType}`)
    if (!results) {
        return;
    }

    const resultsList = document.querySelector(".see-results");
    resultsList.innerHTML = '';
    results.map(student => {
        const li = document.createElement("li");
        li.innerText = student.name;
        resultsList.appendChild(li);
    })
}

const httpGet = async (url) => {
    const response = await (fetch(url, {
        method: 'GET'
    }));

    if (response.ok) {
        return await response.json();
    }

    const error = document.querySelector(".see-results__error");
    error.innerText = "Wystąpił błąd po stronie serwera.";
    return null;
}

const httpPost = async (url, body = null) => {
    const response = await fetch(url, {
        method: 'POST',
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(body)
    });

    const responseInfo = document.querySelector('.response-info');

    if (!response.ok) {
        responseInfo.innerText = "Wystąpił błąd po stronie serwera.";
        return;
    }

    responseInfo.innerText = "Operacja przebiegła pomyślnie";
}

const httpPatch = async (url, body = null) => {
    const response = await fetch(url, {
        method: 'PATCH',
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(body)
    });

    const responseInfo = document.querySelector('.response-info');

    if (!response.ok) {
        responseInfo.innerText = "Wystąpił błąd po stronie serwera."
    }

    responseInfo.innerText = "Operacja przebiegła pomyślnie";
}


document.querySelectorAll("form").forEach((form) =>
    form.addEventListener("click", (e) => e.preventDefault()));