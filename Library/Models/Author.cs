using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System;

namespace Library.Models
{
  public class Author
  {
    private string _name;
    private int _id;

    public Author(string authorName, int id = 0)
    {
      _name = authorName;
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
    public int GetId()
    {
      return _id;
    }
    public override bool Equals(System.Object otherAuthor)
    {
      if (!(otherAuthor is Author))
      {
        return false;
      }
      else
      {
        Author newAuthor = (Author) otherAuthor;
        bool idEquality = this.GetId() == newAuthor.GetId();
        bool nameEquality = this.GetName() == newAuthor.GetName();
        return (idEquality && nameEquality);
      }
    }
    public override int GetHashCode()
    {
      return this.GetName().GetHashCode();
    }

    public static List<Author> GetAllAuthors()
    {
      List<Author> allAuthors = new List<Author> {};
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM authors;";
      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      while(rdr.Read())
      {
        int authorId = rdr.GetInt32(0);
        string authorName = rdr.GetString(1);
        Author newAuthor = new Author(authorName, authorId);
        allAuthors.Add(newAuthor);
      }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return allAuthors;
    }
    public void Save()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO authors (name) VALUES (@thisAuthorName);";

      cmd.Parameters.Add(new MySqlParameter("@thisAuthorName", _name));

      cmd.ExecuteNonQuery();
      _id = (int) cmd.LastInsertedId;
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }

    }
    public static Author Find(int id)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM authors WHERE id = (@searchId);";

      MySqlParameter searchId = new MySqlParameter();
      searchId.ParameterName = "@searchId";
      searchId.Value = id;
      cmd.Parameters.Add(searchId);

      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      int authorId = 0;
      string authorName = "";

      while(rdr.Read())
      {
        authorId = rdr.GetInt32(0);
        authorName = rdr.GetString(1);
      }
      Author newAuthor = new Author(authorName, authorId);
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return newAuthor;
    }
    public List<Book> GetBooks()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT books.* FROM authors
      JOIN books_authors ON (author_id=books_authors.author_id)
      JOIN books ON (books_authors.book_id=books.id)
      WHERE author_id=@AuthorId;";

      cmd.Parameters.Add(new MySqlParameter("@AuthorId", _id));

      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      List<Book> books = new List<Book> {};
      while(rdr.Read())
      {
        int bookId = rdr.GetInt32(0);
        string bookName = rdr.GetString(1);
        string bookGenre = rdr.GetString(2);
        Book newBook = new Book(bookName, bookGenre, bookId);
        books.Add(newBook);
      }
      rdr.Dispose();

      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return books;
    }
    public void AddBook(Book newBook)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO books_authors (book_id, author_id) VALUES (@bookId, @authorId);";

      cmd.Parameters.Add(new MySqlParameter("@authorId", _id));
      cmd.Parameters.Add(new MySqlParameter("@bookId", newBook.GetId()));

      cmd.ExecuteNonQuery();
      _id = (int) cmd.LastInsertedId;
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
      cmd.CommandText = @"DELETE FROM authors;";
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
      cmd.CommandText = @"DELETE FROM authors WHERE id = @authorId; DELETE FROM books_authors WHERE author_id = @authorId;";

      MySqlParameter authorIdParameter = new MySqlParameter();
      authorIdParameter.ParameterName = "@authorId";
      authorIdParameter.Value = this.GetId();
      cmd.Parameters.Add(authorIdParameter);

      cmd.ExecuteNonQuery();
      if (conn != null)
      {
        conn.Close();
      }
    }
  }
}
