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
        public TokenController(IUserRepository iUserRepository) 
        {
            _iUserRepository = iUserRepository;
        }
        
        // GET: api/<TokenController>
        [HttpGet]
        public ActionResult<JsonDocument> RefreshAccess()
        {
            var httpContext = new HttpContextAccessor();
            var refreshToken =  httpContext.HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
            try
            {
                var jsonString = JsonSerializer.Serialize(_iUserRepository.RefreshPairTokens(refreshToken));
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