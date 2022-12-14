using System;
using System.Text.Json;
using System.Security.Authentication;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.DataAccess.Contracts;
using WebApi.DataAccess.Database;
using WebApi.Classes;

namespace WebApi.Controllers
{
    //[Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IUserRepository _iUserRepository;
        private readonly IRefreshSessionRepository _iRefreshSessionRepository;
        public TokenController(IUserRepository iUserRepository, IRefreshSessionRepository iRefreshSessionRepository) 
        {
            _iUserRepository = iUserRepository;
            _iRefreshSessionRepository = iRefreshSessionRepository;
        }
        
        // GET: api/<TokenController>
        [HttpGet]
        public ActionResult<JsonDocument> RefreshAccess()
        {
            //var refreshToken =  HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
            try
            {
                var refreshToken = HttpContext.Request.Cookies["refresh_token"];
                if(refreshToken == null)
                {
                    throw new AuthenticationException();
                }
                RefreshSession refreshAccess = _iRefreshSessionRepository.GetByRefreshToken(refreshToken);
                if(refreshAccess == null)
                {
                    throw new AuthenticationException();
                }
                if(DateTimeOffset.UtcNow.Subtract(refreshAccess.CreationDate).Milliseconds > refreshAccess.ExpiresIn)
                {
                    throw new AuthenticationException();
                }
                
                User user = _iUserRepository.GetById(refreshAccess.UserId);

                TokenPair tokenPair = _iUserRepository.RefreshPairTokens(refreshToken);

                CookieOptions cookieOptions = new CookieOptions();
                cookieOptions.Expires = DateTimeOffset.Now.AddMilliseconds(tokenPair.ExpiredInRefreshToken);
                cookieOptions.Path = "/";
                cookieOptions.HttpOnly = true;
                cookieOptions.SameSite = SameSiteMode.None;
                cookieOptions.Secure = true;
                Response.Cookies.Append("refresh_token", tokenPair.RefreshToken, cookieOptions);

                var userToken = new 
                {
                    Name = user.Name,
                    Surname = user.Surname,
                    Email = user.Email,
                    EmailConfirmed = user.EmailConfirmed,
                    AccessToken = tokenPair.AccessToken,
                    ExpiredIn = tokenPair.ExpiredInAccessToken
                };

                var jsonString = JsonSerializer.Serialize(userToken);
                var json = JsonDocument.Parse(jsonString);
                return json;
            }
            catch (AuthenticationException ex)
            {
                return StatusCode(403);
            }

        }
    }
}