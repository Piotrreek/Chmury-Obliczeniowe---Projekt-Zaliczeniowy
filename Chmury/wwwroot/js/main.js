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
    }

    const body = {
        universityName: name
    };

    await httpPost("/university", body);
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
        responseInfo.innerText = "Wystąpił błąd po stronie serwera."
    }

    responseInfo.innerText = "Pomyślnie dodano uczelnię";
}

document.querySelectorAll("form").forEach((form) =>
    form.addEventListener("click", (e) => e.preventDefault()));