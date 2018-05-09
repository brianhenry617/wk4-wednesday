using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System;

namespace Library.Models
{
  public class Book
  {
    private string _name;
    private string _genre;
    private int _id;

    public Book(string bookName, string Genre, int id = 0)
    {
      _name = bookName;
      _genre = Genre;
      _id = id;
    }
    public string GetName()
    {
      return _name;
    }
    public void SetName(string newName)
    {
      _name = newName;
    }
    public string GetGenre()
    {
      return _genre;
    }
    public void SetGenre(string newGenre)
    {
      _genre = newGenre;
    }
    public int GetId()
    {
      return _id;
    }
    public override bool Equals(System.Object otherBook)
    {
      if (!(otherBook is Book))
      {
        return false;
      }
      else
      {
        Book newBook = (Book) otherBook;
        bool idEquality = this.GetId() == newBook.GetId();
        bool nameEquality = this.GetName() == newBook.GetName();
        bool genreEquality = this.GetGenre() == newBook.GetGenre();
        return (idEquality && nameEquality && genreEquality);
      }
    }
    public override int GetHashCode()
    {
      return this.GetName().GetHashCode();
    }

    public static List<Book> GetAllBooks()
    {
      List<Book> allBooks = new List<Book> {};
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM books;";
      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      while(rdr.Read())
      {
        int bookId = rdr.GetInt32(0);
        string bookName = rdr.GetString(1);
        string bookGenre = rdr.GetString(2);
        Book newBook = new Book(bookName, bookGenre, bookId);
        allBooks.Add(newBook);
      }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return allBooks;
    }
    public void Save()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO books (name, genre) VALUES (@thisBookName, @thisBookGenre);";

      cmd.Parameters.Add(new MySqlParameter("@thisBookName", _name));
      cmd.Parameters.Add(new MySqlParameter("@thisBookGenre", _genre));

      cmd.ExecuteNonQuery();
      _id = (int) cmd.LastInsertedId;
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }

    }
    public static Book Find(int id)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM books WHERE id = (@searchId);";

      MySqlParameter searchId = new MySqlParameter();
      searchId.ParameterName = "@searchId";
      searchId.Value = id;
      cmd.Parameters.Add(searchId);

      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      int bookId = 0;
      string bookName = "";
      string bookGenre = "";

      while(rdr.Read())
      {
        bookId = rdr.GetInt32(0);
        bookName = rdr.GetString(1);
        bookGenre = rdr.GetString(2);
      }
      Book newBook = new Book(bookName, bookGenre, bookId);
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return newBook;
    }
    public void AddAuthor(Author newAuthor)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO books_authors (book_id, author_id) VALUES (@bookId, @authorId);";

      cmd.Parameters.Add(new MySqlParameter("@bookId", _id));
      cmd.Parameters.Add(new MySqlParameter("@authorId", newAuthor.GetId()));

      cmd.ExecuteNonQuery();
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }
    public List<Author> GetAuthors()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT authors.* FROM books
        JOIN books_authors ON (books.id=books_authors.book_id)
        JOIN authors ON (books_authors.author_id=authors.id)
      WHERE books.id=@BookId;";

      cmd.Parameters.Add(new MySqlParameter("@BookId", _id));

      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      List<Author> authors = new List<Author> {};
      while(rdr.Read())
      {
        int authorId = rdr.GetInt32(0);
        string authorName = rdr.GetString(1);
        Author newAuthor = new Author(authorName, authorId);
        authors.Add(newAuthor);
      }

      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return authors;
    }
    public void UpdateBookName(string newName)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"UPDATE books SET name = @newName WHERE id = @searchId;";

      MySqlParameter searchId = new MySqlParameter();
      searchId.ParameterName = "@searchId";
      searchId.Value = _id;
      cmd.Parameters.Add(searchId);

      MySqlParameter name = new MySqlParameter();
      name.ParameterName = "@newName";
      name.Value = newName;
      cmd.Parameters.Add(name);

      cmd.ExecuteNonQuery();
      _name = newName;
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }
    public static void DeleteAll()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"DELETE FROM books;";
      cmd.ExecuteNonQuery();
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public void Delete()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"DELETE FROM books WHERE id = @bookId; DELETE FROM books_authors WHERE book_id = @bookId;";

      MySqlParameter bookIdParameter = new MySqlParameter();
      bookIdParameter.ParameterName = "@bookId";
      bookIdParameter.Value = this.GetId();
      cmd.Parameters.Add(bookIdParameter);

      cmd.ExecuteNonQuery();
      if (conn != null)
      {
        conn.Close();
      }
    }
  }
}
