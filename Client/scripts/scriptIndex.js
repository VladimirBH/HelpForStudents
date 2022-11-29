const url = "https://localhost:5001/api/"

async function getSubjects()
{
    let response = await fetch(url + "subjects/");
    if(response.ok) {
        let json = await response.json;
    } else {
        alert("Ошибка HTTP: " + response.status);
    }
}