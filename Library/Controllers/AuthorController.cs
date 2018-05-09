using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Library.Models;
using System;

namespace Library.Controllers
{
    public class authorsController : Controller
    {
      [HttpGet("/authors")]
      public ActionResult Index()
      {
          List<Author> allauthors = Author.GetAllAuthors();
          return View(allauthors);
      }
      [HttpGet("/authors/new")]
      public ActionResult CreateForm()
      {
        return View("authorForm");
      }
      [HttpPost("/authorForm")]
      public ActionResult Create()
      {
        Author newAuthor = new Author(Request.Form["authorName"]);
        newAuthor.Save();
        return RedirectToAction("Success", "Home");
      }

    }
}
