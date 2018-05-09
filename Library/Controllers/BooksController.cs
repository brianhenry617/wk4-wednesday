using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Library.Models;
using System;

namespace Library.Controllers
{
    public class BooksController : Controller
    {
      [HttpGet("/books")]
      public ActionResult Index()
      {
          List<Book> allBooks = Book.GetAllBooks();
          return View(allBooks);
      }
      [HttpGet("/books/new")]
      public ActionResult CreateForm()
      {
        return View("BookForm");
      }
      [HttpPost("/bookForm")]
      public ActionResult Create()
      {
        Book newBook = new Book(Request.Form["bookName"], Request.Form["bookGenre"]);
        newBook.Save();
        return RedirectToAction("Success", "Home");
      }

    }
}
