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
        public IActionResult Index()
        {
            return View();
        }
    }
}