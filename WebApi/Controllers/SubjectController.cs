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

namespace WebApi
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SubjectController : Controller
    {
        private readonly ISubjectRepository _iSubjectRepository;
        public SubjectController(ISubjectRepository iSubjectRepository) 
        {
            _iSubjectRepository = iSubjectRepository;
        }
        
        // GET: api/<UserController>
        [HttpGet]
        public ActionResult<JsonDocument> Get()
        {
            try
            {
                var jsonString = JsonSerializer.Serialize(_iSubjectRepository.GetAll());
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
                var jsonString = JsonSerializer.Serialize(_iSubjectRepository.GetById(id));
                var json = JsonDocument.Parse(jsonString);
                return json;
            }
            catch (AuthenticationException)
            {
                return StatusCode(401);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<JsonDocument> BuySubject(int id)
        {
            try
            {
                var subject = _iSubjectRepository.GetById(id);
                return null;
            }
            catch (AuthenticationException)
            {
                return StatusCode(401);
            }
        }
        /*
        // POST api/<UserController>/CreateUser
        [HttpPost]
        public void CreateUser(User user)
        {
            _iUserRepository.Add(user);
            _iUserRepository.SaveChanges();
        }

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