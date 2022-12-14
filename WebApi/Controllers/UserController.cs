using System;
using System.Net;
using System.Text.Json;
using System.Security.Authentication;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Features;
using WebApi.DataAccess.Contracts;
using WebApi.DataAccess.Database;
using WebApi.Classes;

namespace WebApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _iUserRepository;

        public UserController(IUserRepository iUserRepository) 
        {
            _iUserRepository = iUserRepository;
        }
        
        // GET: api/<UserController>
        [HttpGet]
        public ActionResult<JsonDocument> Get()
        {
            try
            {
                var jsonString = JsonSerializer.Serialize(_iUserRepository.GetAll());
                var json = JsonDocument.Parse(jsonString);
                return json;
            }
            catch (AuthenticationException)
            {
                return StatusCode(401);
            }
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public ActionResult<JsonDocument> Get(int id)
        {
            try
            {
                var jsonString = JsonSerializer.Serialize(_iUserRepository.GetById(id));
                var json = JsonDocument.Parse(jsonString);
                return json;
            }
            catch (AuthenticationException)
            {
                return StatusCode(401);
            }
        }

        // POST api/<UserController>/SignUp
        [HttpPost]
        public ActionResult SignUp(User user)
        {
            if(_iUserRepository.CheckEmailForDuplication(user.Email))
            {
                return StatusCode(401);
            }
            user.EmailConfirmed = false;
            _iUserRepository.Add(user);
            _iUserRepository.SaveChanges();
            _iUserRepository.SubmitEmailAsync(
                email: user.Email, 
                message: "<h3> Благодарим за регистрацию на нашем сайте." + 
                        "Для окончания регистрации необходимо подтвердить почту.<br> " +
                        "Введите код подтверждения, указанный ниже.</h3>");
            return StatusCode(200);
        }

        // PUT api/<UserController>/5
        /*[HttpPut]
        public void UpdateUser(User user)
        {
            _iUserRepository.Update(user);
            _iUserRepository.SaveChanges();
        }*/

        [HttpPut]
        public void UpdateProfile(User user)
        {
            _iUserRepository.Update(user);
            _iUserRepository.SaveChanges();
        }
        // GET api/<UserController>/GetCurrentUserInfo
        [HttpGet]
        public ActionResult<JsonDocument> GetCurrentUserInfo()
        {
            try
            {
                string accessToken = HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
                if(accessToken == null)
                {
                    return StatusCode(401);
                }
                var jsonString = JsonSerializer.Serialize(_iUserRepository.GetCurrentUserInfo(accessToken));
                var json = JsonDocument.Parse(jsonString);
                return json;
            }
            catch (AuthenticationException)
            {
                return StatusCode(401);
            }
        }
        
        // POST api/<UserController>/SignIn
        [AllowAnonymous]
        [HttpPost]
        public ActionResult<JsonDocument> SignIn(AuthorizationData dataAuth)
        {
            try
            {
                UserToken userTokenPair = _iUserRepository.Authorization(dataAuth);

                CookieOptions cookieOptions = new CookieOptions();
                cookieOptions.Expires = DateTimeOffset.Now.AddMilliseconds(userTokenPair.ExpiredInRefreshToken);
                cookieOptions.Path = "/";
                cookieOptions.HttpOnly = true;
                cookieOptions.SameSite = SameSiteMode.None;
                cookieOptions.Secure = true;

                var userToken = new 
                {
                    Name = userTokenPair.Name,
                    Surname = userTokenPair.Surname,
                    Email = userTokenPair.Email,
                    EmailConfirmed = userTokenPair.EmailConfirmed,
                    AccessToken = userTokenPair.AccessToken,
                    ExpiredIn = userTokenPair.ExpiredInAccessToken
                };
                var jsonString = JsonSerializer.Serialize(userToken);
                var json = JsonDocument.Parse(jsonString);
                Response.Cookies.Append("refresh_token", userTokenPair.RefreshToken, cookieOptions);
                return json; 
            }
            catch (AuthenticationException)
            {
                return StatusCode(401);
            }
        }
        
        [HttpPost]
        public ActionResult<bool> ConfirmEmail(string email, string code)
        {    
            if(_iUserRepository.CheckCodeFromEmail(email, code))
            {
                User user = _iUserRepository.GetByEmail(email);
                user.EmailConfirmed = true;
                _iUserRepository.Update(user);
                _iUserRepository.SaveChanges();
                return Ok();
            }
            return StatusCode(401);
        }

        [HttpPost]
        public ActionResult ResetPassword(string email, string code)
        {    
            if(_iUserRepository.CheckCodeFromEmail(email, code))
            {
                return StatusCode(200);
            }
            return StatusCode(400);
        }

        [HttpPost]
        public ActionResult<string> SendMessageToRestoreAccount([FromBody]string email)
        {
            if(!_iUserRepository.CheckEmailForDuplication(email))
            {
                return StatusCode(401);
            }
            _iUserRepository.SubmitEmailAsync(
                email: email, 
                message: "<h3> Восстановление пароля. " + 
                        "Для восстановления пароля необходимо ввести код подтверждения, указанный ниже.<br></h3>");
            return StatusCode(200);
        }
        
        /*
        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            var user = _iUserRepository.GetById(id);
            _iUserRepository.Remove(user);
            _iUserRepository.SaveChanges();
        }*/
    }
}