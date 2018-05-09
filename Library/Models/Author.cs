using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System;

namespace Library
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
        bool idEquality = this.GetId() == newBook.GetId();
        bool nameEquality = this.GetName() == newBook.GetName();
        return (idEquality && nameEquality);
      }
    }
    public override int GetHashCode()
    {
      return this.GetName().GetHashCode();
    }

    public static List<Author> GetAllAuthor()
    {
      List<Author> allAuthors = new List<Author> {};
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM author;";
      var rdr = cmd.ExecuteReader() as MySqlCommand;
      while(rdr.Read())
      {
        int authorId = rdr.GetInt32(0);
        string bookName = rdr.GetString(1);
        Author newAuthor = new Author(bookName, bookId);
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
      cmd.CommandText = @"INSERT INTO Authors (name) VALUES (@thisAuthorName);";

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
      int bookId = 0;
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
  }
}
