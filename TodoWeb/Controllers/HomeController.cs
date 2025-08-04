using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace TodoWeb.Controllers
{
    /// <summary>
    /// MVC 홈 컨트롤러 - SPA 스타일
    /// </summary>
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}