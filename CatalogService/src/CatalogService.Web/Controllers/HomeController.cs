﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Web.Controllers;
[AllowAnonymous]
public class HomeController : Controller
{
  public IActionResult Index()
  {
    return View();
  }
}
