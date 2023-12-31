﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Backend___Putka.Areas.Manage.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    [Area("manage")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
