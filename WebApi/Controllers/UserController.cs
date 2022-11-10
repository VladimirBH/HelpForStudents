using System;
using System.Text.Json;
using System.Security.Authentication;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.DataAccess.Contracts;
using WebApi.DataAccess.Database;
using WebApi.Classes;

namespace WebApi.Controllers
{
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

        // GET api/<UserController>/GetCurrentUserInfo
        [HttpGet]
        public ActionResult<JsonDocument> GetCurrentUserInfo()
        {
            try
            {
                var httpContext = new HttpContextAccessor();
                if(httpContext.HttpContext == null)
                {
                    return StatusCode(401);
                }
                var accessToken =  httpContext.HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
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
        [HttpPost]
        public ActionResult<JsonDocument> SignIn(AuthorizationData dataAuth)
        {
            try
            {
                var jsonString = JsonSerializer.Serialize(_iUserRepository.Authorization(dataAuth));
                var json = JsonDocument.Parse(jsonString);
                return json;
            }
            catch (AuthenticationException)
            {
                return StatusCode(403);
            }
        }
        
        // POST api/<UserController>/CreateUser
        [HttpPost]
        public void CreateUser(User user)
        {
            _iUserRepository.Add(user);
            _iUserRepository.SaveChanges();
        }

        [HttpPost]
        public void SubmitEmail(string email)
        {
            _iUserRepository.SubmitEmail(email);
        }
        /*
        // PUT api/<UserController>/5
        [HttpPut]
        public void Put(User user)
        {
            _iUserRepository.Update(user);
            _iUserRepository.SaveChanges();
        }

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