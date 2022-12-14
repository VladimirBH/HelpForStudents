window.globalUrl = "https://localhost:5001/api/";
let subjects = [];

async function onLoadBodyIndex() {
    //let refreshToken = getCookie("refresh_token");
    let accessToken = await refreshTokenPair();
    if(accessToken == null)
    {
        return;
    }
    localStorage.AccessToken = JSON.stringify({
        token: accessToken.AccessToken,
        expiredIn: accessToken.ExpiredInAccessToken,
        creationDateTime: accessToken.CreationDateTime
    });
    
    let btnUserInfo = document.getElementById("ButtonUserMenu");
    let divSignIn = document.getElementById("SignInSignUp");

    divSignIn.style.visibility = "hidden";

    btnUserInfo.style.visibility = "visible";
    btnUserInfo.textContent = localStorage.UserInfo.name + " " + localStorage.UserInfo.surname;
    
    var user = JSON.parse(localStorage.UserInfo);
    btnUserInfo.firstChild.data = user.name + ' ' + user.surname;
    
    let themes = JSON.parse(getThemes());

    let selectDiploma = document.getElementById("diplomas");
    let selectCourse = document.getElementById("course_works");

    for(let i = 0; i < length(themes); i++)
    {
        let theme = document.createElement("options");
        if(themes[i].TypeTheme == "diploma")
        {
            theme.value = themes[i].Name;
            theme.innerHTML = themes[i].Name;
            selectDiploma.appendChild(theme)
        }
        else if(themes[i].TypeTheme == "course")
        {
            theme.value = themes[i].Name;
            theme.innerHTML = themes[i].Name;
            selectCourse.appendChild(theme)
        }
    } 
    subjects = Json.parse(getSubjects());
}

async function getSubjects() {
    let response = await fetch(globalUrl + "subjects/");
    if(response.ok) {
        let json = await response.json();
    } else {
        alert("Ошибка HTTP: " + response.status);
    }
}

async function getThemes() {
    let response = await fetch(globalUrl + "themes/");
    if(response.ok) {
        let json = await response.json();
    } else {
        alert("Ошибка HTTP: " + response.status);
    }
}

async function refreshTokenPair() {
    let response = await fetch(globalUrl + "token/refreshaccess", {
        method: 'GET',
        credentials: "include",
        headers: 
        {
            'Origin': 'https://localhost:5001/api/token/refreshaccess'
        }  
    });
    if(response.ok) {
        let json = await response.json();
        return json;
    } else if(response.status == 403) {
        return null;
    }
}



