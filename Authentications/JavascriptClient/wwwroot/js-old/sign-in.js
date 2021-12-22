var createState = function () {
    return "SessionValueMakeItABitLongerasdfhjsadoighasdifjdsalkhrfakwelyrosdpiufghasidkgewr";
};

var createNonce = function () {
    return "NonceValuedsafliudsayatroiewewryie123";
};

var signIn = function () {
    var redirectUri = "https://localhost:44310/Home/SignIn";
    var responseType = "id_token token";
    var scope = "openid ApiOneScopes";
    var authUrl =
        "/connect/authorize/callback" +
"?client_id=client_id_js" +
"&redirect_uri=" + encodeURIComponent(redirectUri) +
"&response_type=" + encodeURIComponent(responseType) +
"&scope=" + encodeURIComponent(scope) +
"&nonce="+ createNonce() +
"&state="+ createState();
    
    var returnUrl = encodeURIComponent(authUrl);

    window.location.href = "https://localhost:44386/Auth/Login?ReturnUrl=" + returnUrl;
}


