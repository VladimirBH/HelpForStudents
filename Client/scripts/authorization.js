window.globalUrl = "https://localhost:5001/api/";

async function signIn() {
    let email = document.getElementById("email");
    let password = document.getElementById("password");
    let user = {
        email: email.value,
        password: password.value
    };

    let userTokenPair = await sendPostRequestAsync(globalUrl + "user/signin", user);
    //document.cookie = "refresh_token=" + tokenPair.RefreshToken + ";expires=" + new Date() + ";SameSite=None; secure";
    //createCookie("refresh_token", userTokenPair.RefreshToken, userTokenPair.ExpiredInRefreshToken);
    let userInfo = 
    {
        name: userTokenPair.Name,
        surname: userTokenPair.Surname,
        email: userTokenPair.Email,
        emailConfirmed: userTokenPair.EmailConfirmed
    }
    //localStorage.AccessToken = userTokenPair.accessToken;
    localStorage.AccessToken = JSON.stringify({
        token: userTokenPair.AccessToken,
        expiredIn: userTokenPair.ExpiredInAccessToken,
        creationDateTime: userTokenPair.CreationDateTime
    });
    localStorage.UserInfo = JSON.stringify(userInfo);
    //alert(JSON.parse(localStorage.UserInfo).name);
    //alert(getCookie("refresh_token"));
    window.location.href = "index.html";
}

async function sendPostRequestAsync(url, data) {
    let response = await fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: {
            'Content-Type': 'application/json;charset=utf-8',
            'Origin': 'https://localhost:5001'
        },
        body: JSON.stringify(data)
    });
    if(response.ok) {
        return await response.json();
    } else if(response.status == 401){
        alert("Неверный логин/пароль");
    } else {
        alert("Что-то пошло не так");
    }
}