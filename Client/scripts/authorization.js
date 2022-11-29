const globalUrl = "https://localhost:5001/api/";

async function signIn()
{
    let email = document.getElementById("email")
    let password = document.getElementById("password")
    let user = {
        email: email.value,
        password: password.value
    };

    let tokenPair = JSON.parse(await sendRequest(globalUrl + "user/signin", user));
    //document.cookie = "refresh_token=" + tokenPair.RefreshToken;
    alert("refresh_token=" + tokenPair.RefreshToken);
}

async function sendRequest(url, data)
{
    let response = await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json;charset=utf-8',
            'Origin': 'https://localhost:5001'
        },
        body: JSON.stringify(data)
    });
    alert(response.status);
    if(response.ok) {
        let json = await response.json;
        alert("Все окей");
        return json;
    } else if(response.status == 403){
        alert("<h3>Неверный логин/пароль</h3>");
    } else {
        alert("Ошибка HTTP: " + response.status);
    }
}