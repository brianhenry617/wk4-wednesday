using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using Library.Models;
using Library;
using MySql.Data.MySqlClient;

namespace Library.Tests
{

  [TestClass]
  public class BookTests : IDisposable
  {
    public BookTests()
    {
      DBConfiguration.ConnectionString = "server=localhost;user id=root;password=root;port=8889;database=library;";
    }
    public void Dispose()
    {
      Book.DeleteAll();
      Author.DeleteAll();
    }

    [TestMethod]
    public void Equals_OverrideTrueForSameDescription_Book()
    {
      //Arrange, Act
      Book firstBook = new Book("Crime and Punishment", "Novel");
      Book secondBook = new Book("Crime and Punishment", "Novel");

      //Assert
      Assert.AreEqual(firstBook, secondBook);
    }
    [TestMethod]
    public void Save_SavesItemToDatabase_Book()
    {
      Book testBook = new Book("Crime and Punishment", "Novel");
      testBook.Save();

      List<Book> result = Book.GetAllBooks();
      List<Book> testList = new List<Book>{testBook};

      CollectionAssert.AreEqual(result, testList);

    }
    [TestMethod]
    public void Save_DatabaseAssignsIdToObject_Id()
    {
      //Arrange
      Book testBook = new Book("Crime and Punishment", "Novel");
      testBook.Save();

      //Act
      Book savedBook = Book.GetAllBooks()[0];

      int result = savedBook.GetId();
      int testId = testBook.GetId();

      //Assert
      Assert.AreEqual(result, testId);
    }
    [TestMethod]
    public void Find_FindsBookInDatabase_Book()
    {
      //Arrange
      Book testBook = new Book("Crime and Punishment", "Novel");
      testBook.Save();

      //Act
      Book foundBook = Book.Find(testBook.GetId());

      //Assert
      Assert.AreEqual(testBook, foundBook);
    }
    [TestMethod]
    public void GetAuthors_ReturnsAllBookAuthors_AuthorsList()
    {
      //Arrange
      Book testBook = new Book("Crime and Punishment", "Novel");
      testBook.Save();

      Author testAuthor1 = new Author("Fyodor Dostoyevsky");
      testAuthor1.Save();

      Author testAuthor2 = new Author("Steven Gerrard");
      testAuthor2.Save();

      //Act
      testBook.AddAuthor(testAuthor1);
      testBook.AddAuthor(testAuthor2);
      List<Author> result = testBook.GetAuthors();
      List<Author> testList = new List<Author> {testAuthor1, testAuthor2};

      //Assert
      CollectionAssert.AreEqual(testList, result);
    }
    [TestMethod]
    public void Delete_DeletesBookAssociationsFromDatabase_BookList()
    {
      //Arrange
      Author testAuthor = new Author("Fyodor Dostoyevsky");
      testAuthor.Save();

      string testBookName = "Mow the lawn";
      string testBookGenre = "Tale";
      Book testBook = new Book(testBookName, testBookGenre);
      testBook.Save();

      //Act
      testBook.AddAuthor(testAuthor);
      testBook.Delete();

      List<Book> resultAuthorBooks = testAuthor.GetBooks();
      List<Book> testAuthorBooks = new List<Book> {};

      //Assert
      CollectionAssert.AreEqual(testAuthorBooks, resultAuthorBooks);
    }

  }
}
