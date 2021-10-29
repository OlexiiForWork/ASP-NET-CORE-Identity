﻿using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Server.Controllers
{
    public class OAuthController : Controller
    {
        [HttpGet]
        public IActionResult Authorize(
            string response_type,// autorization flow type
            string client_id,    //client_id
            string redirect_uri,
            string scope,        //what info i want = email, grandma, tel
            string state)        //random string generated to confirm that we are goin to back to the same client 
        {
            // ?a=foo&b=bar
            var query = new QueryBuilder();
            query.Add("redirectUri",redirect_uri);
            query.Add("state", state);
            return View(model: query.ToString());
        }
        [HttpPost]
        public IActionResult Authorize(string username,
                                        string redirectUri, 
                                        string state)
        {
            const string code = "BABABABABABA";
            var query = new QueryBuilder();
            query.Add("code", code);
            query.Add("state", state);
            var yy = query.ToString();
            //{ query.ToString()}
            return Redirect($"{redirectUri}{query.ToString()}");
        }

        public async Task<IActionResult> Token(string grant_type,  //flow of access_token request
                                   string code,        // confirmayion of the authentication 
                                   string redirect_uri,
                                   string client_id)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,"sume_id"),//Ид юзера
                new Claim("granny","cookie"),
            };

            var secretBytes = Encoding.UTF8.GetBytes(Constants.Secret);
            var key = new SymmetricSecurityKey(secretBytes);
            //var key = new AsymmetricSecurityKey();


            var algoritm = SecurityAlgorithms.HmacSha256;

            var signingCredentials = new SigningCredentials(key, algoritm);

            var token = new JwtSecurityToken(
                Constants.Issuer,
                Constants.Audiance,
                claims,
                notBefore: System.DateTime.Now,
                expires: System.DateTime.Now.AddHours(1),
                signingCredentials
                );

            var access_token = new JwtSecurityTokenHandler().WriteToken(token);

            var responseObject = new
            {
                access_token,
                token_type = "Bearer",
                raw_сlaim = "oauthTutorial"
            };

            //var responseJson = JsonConvert.SerializeObject(responseObject);
            //var responseByte = Encoding.UTF8.GetBytes(responseJson);

            //await Response.Body.WriteAsync(responseByte,0, responseByte.Length);  
            //return Redirect(redirect_uri);
            return Ok(responseObject);
        }

    }
}