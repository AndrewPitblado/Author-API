
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text.Json;

using System.Diagnostics.Contracts;
using System.Collections;
using System.Text.Json.Nodes;
using System.Data.SqlTypes;
using System;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

internal static class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers()
            .AddNewtonsoftJson();
        

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();


        var myorigins = "_myAllowSpecificOrigins";
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: myorigins, policy =>
            {
                policy.WithOrigins("https://localhost:7209", "http://localhost:4200").AllowAnyHeader().AllowAnyMethod();
            });
        });

        //string keyVaultName = Environment.GetEnvironmentVariable("KEY_VAULT_NAME");

        var kvUri = "https://auth-app-vault.vault.azure.net/";
        var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());
        




        //builder.Services.AddDbContext<DataContext>(options =>
        //{
        //    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
        //});


        /*

        builder.Services.AddCors(options => options.AddPolicy(name: "AuthorOrigins",
            policy =>
            {
               policy.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader();

            }));
        */

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        //app.UseCors("AuthorOrigins");
        app.UseHttpsRedirection();
        

        string connectionString = app.Configuration.GetConnectionString("DefaultConnection")!;
        //var sqlConnString = client.GetSecret("mySecret1");
        try
        {
            using var conn = new SqlConnection(connectionString);
            conn.Open();


        }

        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }



        app.MapGet("/author/{id}", (string id) =>

        {
            JsonArray json = new JsonArray();
            JsonObject Author = new JsonObject();

               
            //var rows = new List<string>();
            using var conn = new SqlConnection(connectionString);
            conn.Open();

            var command = new SqlCommand("SELECT * FROM authors WHERE au_id = @au_id", conn);
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@au_id", id);
            using SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                Author.Add("id",reader.GetString(0));
                Author.Add("FirstName", reader.GetString(1));
                Author.Add("LastName", reader.GetString(2));
                Author.Add("Phone", reader.GetString(3));
                Author.Add("Contract", reader.GetBoolean(8));

                //rows.Add($"{reader.GetString(0)}, {reader.GetString(1)}," +
                       //$"{reader.GetString(2)}, {reader.GetString(3)}, {reader.GetBoolean(8)}"
                        //     );

                if (!reader.IsDBNull(reader.GetOrdinal("address")))
                    Author.Add("Address",reader.GetString(reader.GetOrdinal("address")));
                if (!reader.IsDBNull(reader.GetOrdinal("city")))
                    Author.Add("City", reader.GetString(reader.GetOrdinal("city")));


                if (!reader.IsDBNull(reader.GetOrdinal("state")))
                    Author.Add("State", reader.GetString(reader.GetOrdinal("state")));
                if (!reader.IsDBNull(reader.GetOrdinal("zip")))
                    Author.Add("Zip", reader.GetString(reader.GetOrdinal("zip")));
                
                
            }

            return Author;
        });

        app.MapGet("/author", () =>
        {
            
            JsonArray data = new JsonArray();
            
            //var rows = new List<KeyValuePair<string,string>>(10);

            using var conn = new SqlConnection(connectionString);
            conn.Open();

            var command = new SqlCommand("SELECT * FROM authors", conn);
            using SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {

                while (reader.Read())

                {
                    /*
                    Author Auth = new Author();
                    Auth.au_id = $"{reader["au_id"]}";
                    Auth.au_fname = (string)reader["au_fname"];
                    Auth.au_lname = (string)reader["au_lname"];
                    Auth.phone = (string)reader["phone"];
                    Auth.address = (string)reader["address"];
                    Auth.city = (string)reader["city"];
                    Auth.state = (string)reader["state"];
                    Auth.zip = (string)reader["zip"];
                    Auth.contract = (string)reader["contract"];

                   rows.Add(Auth);
                    */
                    JsonObject Author = new JsonObject();
                    Author.Add("id", reader.GetString(0));
                    Author.Add("FirstName", reader.GetString(1));
                    Author.Add("LastName", reader.GetString(2));
                    Author.Add("Phone", reader.GetString(3));
                    Author.Add("Contract", reader.GetBoolean(8));

                    if (!reader.IsDBNull(reader.GetOrdinal("address")))
                        Author.Add("Address", reader.GetString(4));
                    if (!reader.IsDBNull(reader.GetOrdinal("city")))
                        Author.Add("City", reader.GetString(5));
                    if (!reader.IsDBNull(reader.GetOrdinal("state")))
                        Author.Add("State", reader.GetString(6));
                    if (!reader.IsDBNull(reader.GetOrdinal("zip")))
                        Author.Add("Zip", reader.GetString(7));
                    data.Add(Author);

                    /*
                    rows.Insert(0, new KeyValuePair<string, string>("Id", reader.GetString(0)));
                    rows.Insert(1, new KeyValuePair<string, string>("FirstName", reader.GetString(1)));
                    rows.Insert(2, new KeyValuePair<string, string>("LastName", reader.GetString(2)));
                    rows.Insert(3, new KeyValuePair<string, string>("Phone", reader.GetString(3)));
                    rows.Insert(4, new KeyValuePair<string, string>("Contract", reader.GetBoolean(8).ToString()));


                    if (!reader.IsDBNull(reader.GetOrdinal("address")))
                        rows.Insert(5,new KeyValuePair<string, string>("Address",reader.GetString(reader.GetOrdinal("address"))));
                    if (!reader.IsDBNull(reader.GetOrdinal("city")))
                        rows.Insert(6, new KeyValuePair<string, string>("City", reader.GetString(reader.GetOrdinal("city"))));


                    if (!reader.IsDBNull(reader.GetOrdinal("state")))
                        rows.Insert(7, new KeyValuePair<string, string>("State", reader.GetString(reader.GetOrdinal("state"))));
                    if (!reader.IsDBNull(reader.GetOrdinal("zip")))
                        rows.Insert(8, new KeyValuePair<string, string>("Zip", reader.GetString(reader.GetOrdinal("zip"))));

                    */





                    /*
                    rows.Add($"{reader.GetString(0)}, {reader.GetString(1)}," +
                        $"{reader.GetString(2)}, {reader.GetString(3)}, {reader.GetBoolean(8)}"
                              );
                    */
                }

            }
            
            //var json = JsonSerializer.SerializeToElement(data);
            return data;
        })
        .WithName("GetAuthors");

        app.MapPost("/author", (Author author) =>
        {
            using var conn = new SqlConnection(connectionString);
            conn.Open();

            var command = new SqlCommand(
                "INSERT INTO authors (au_id, au_fname, au_lname, phone, address, city, state, zip, contract) VALUES (@au_id, @au_lname, @au_fname, @phone, @add, @city, @state, @zip, @contract)",
                conn
                );
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@au_id", author.id);
            command.Parameters.AddWithValue("@au_fname", author.firstName);
            command.Parameters.AddWithValue("@au_lname", author.lastName);

            

            command.Parameters.AddWithValue("@phone", author.phone);

            command.Parameters.AddWithValue("@add", author.address);
            command.Parameters.AddWithValue("@city", author.city);
            command.Parameters.AddWithValue("@state", author.state);
            command.Parameters.AddWithValue("@zip", author.zip);
            command.Parameters.AddWithValue("@contract", author.contract);
            using SqlDataReader reader = command.ExecuteReader();

        })
            .WithName("CreateAuthor")
            .WithTags("Setters");



        app.MapPut("/author/{id}", (string id, Author author) =>
        {
            using var conn = new SqlConnection(connectionString);
            conn.Open();

            var command = new SqlCommand(
                "UPDATE authors SET au_fname = @firstName, au_lname = @lastName, phone = @phone, address = @add, city = @city, state = @state, zip = @zip, contract = @contract  WHERE au_id = @au_id",
                conn);


            command.Parameters.Clear();
            command.Parameters.AddWithValue("@au_id", id);
            command.Parameters.AddWithValue("@firstName", author.firstName);
            command.Parameters.AddWithValue("@lastName", author.lastName);
            command.Parameters.AddWithValue("@phone", author.phone);
            command.Parameters.AddWithValue("@add", author.address);
            command.Parameters.AddWithValue("@city", author.city);
            
            command.Parameters.AddWithValue("@state", author.state);
            command.Parameters.AddWithValue("@zip", author.zip);
            command.Parameters.AddWithValue("@contract", author.contract);



            using SqlDataReader reader = command.ExecuteReader();


             

        })
            .WithName("UpdateAuthor")
            .WithTags("Put");

        app.MapGet("/books", () =>
        {
        JsonArray data = new JsonArray();

        //var rows = new List<KeyValuePair<string,string>>(10);

        using var conn = new SqlConnection(connectionString);
        conn.Open();

        var command = new SqlCommand("SELECT * FROM titles", conn);
        using SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {

                while (reader.Read())

                {

                    JsonObject Book = new JsonObject();
                    Book.Add("id", reader.GetString(0));
                    Book.Add("Title", reader.GetString(1));
                    Book.Add("Type", reader.GetString(2));
                    Book.Add("PubDate", reader.GetDateTime(9));


                    if (!reader.IsDBNull(reader.GetOrdinal("pub_id")))
                        Book.Add("pub_id", reader.GetString(3));
                    if (!reader.IsDBNull(reader.GetOrdinal("price")))
                        Book.Add("Price", reader.GetDecimal(4));
                    if (!reader.IsDBNull(reader.GetOrdinal("advance")))
                        Book.Add("Advance", reader.GetDecimal(5));
                    if (!reader.IsDBNull(reader.GetOrdinal("royalty")))
                        Book.Add("Royalty", reader.GetInt32(6));
                    if (!reader.IsDBNull(reader.GetOrdinal("ytd_sales")))
                        Book.Add("YTD_Sales", reader.GetInt32(7));
                    if (!reader.IsDBNull(reader.GetOrdinal("notes")))
                        Book.Add("Notes", reader.GetString(8));

                    data.Add(Book);


                } 

            }

            //var json = JsonSerializer.SerializeToElement(data);
            return data;








        });



       

        app.MapGet("/Books/{id}", (string id) =>

        {
            JsonArray json = new JsonArray();
            


            //var rows = new List<string>();
            using var conn = new SqlConnection(connectionString);
            conn.Open();

            var command = new SqlCommand(
                "SELECT title, type, price,notes, pubdate\r\nFROM authors\r\nINNER JOIN titleauthor\r\nON authors.au_id = titleauthor.au_id\r\n  INNER JOIN titles\r\n  ON titles.title_id = titleauthor.title_id\r\n\tWHERE authors.au_id = @au_id",
                
                
                conn);
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@au_id", id);
            using SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())

                {
                    JsonObject Books = new JsonObject();
                    //Books.Add("id", reader.GetString(0));
                    // Books.Add("FirstName", reader.GetString(1));
                    // Books.Add("LastName", reader.GetString(2));
                    // Books.Add("Phone", reader.GetString(3));
                    // Books.Add("Contract", reader.GetBoolean(8));

                    //rows.Add($"{reader.GetString(0)}, {reader.GetString(1)}," +
                    //$"{reader.GetString(2)}, {reader.GetString(3)}, {reader.GetBoolean(8)}"
                    //     );

                    //if (!reader.IsDBNull(reader.GetOrdinal("title_id")))
                    //   Books.Add("title_id", reader.GetString(reader.GetOrdinal("title_id")));
                    if (!reader.IsDBNull(reader.GetOrdinal("title")))
                        Books.Add("title", reader.GetString(reader.GetOrdinal("title")));


                    if (!reader.IsDBNull(reader.GetOrdinal("type")))
                        Books.Add("type", reader.GetString(reader.GetOrdinal("type")));
                    if (!reader.IsDBNull(reader.GetOrdinal("pubdate")))
                        Books.Add("pubdate", reader.GetDateTime(reader.GetOrdinal("pubdate")));
                    if (!reader.IsDBNull(reader.GetOrdinal("price")))
                        Books.Add("price", reader.GetDecimal(reader.GetOrdinal("price")));
                    if (!reader.IsDBNull(reader.GetOrdinal("notes")))
                        Books.Add("notes", reader.GetString(reader.GetOrdinal("notes")));

                    json.Add(Books);
                }
            }

            return json;
        });










        /*
        app.MapDelete("/author", () =>
        {
            using var conn = new SqlConnection(connectionString);
            conn.Open();

            var Id = "";

            var command = new SqlCommand(
                "DELETE FROM authors WHERE au_id = @au_id",
                conn
                );
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@au_id", Id);

            using SqlDataReader reader = command.ExecuteReader();

        })
            .WithName("DeleteAuthor")
            .WithTags("Delete");
        */

        app.MapDelete("/author/{id}", (string id) =>
        {
            using var conn = new SqlConnection(connectionString);
            conn.Open();

           

            var command = new SqlCommand(
                "DELETE FROM authors WHERE au_id = @au_id",
                conn
                );

            command.Parameters.Clear();
            command.Parameters.AddWithValue("@au_id", id);

            using SqlDataReader reader = command.ExecuteReader();



        })
            .WithName("DeleteAuthor")
            .WithTags("Delete");




        DataSet ds = new DataSet();
        SelectRows(ds, connectionString, "SELECT * FROM authors");
        Console.WriteLine(ds.ToString());
        



        
        app.UseCors(myorigins);
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }

    public static string SafeGetString(this SqlDataReader reader, int colIndex)
    {
        
        if (!reader.IsDBNull(colIndex))
        {
            return reader[colIndex].ToString();
        }
        return string.Empty;
    }



    public static DataSet SelectRows(DataSet dataSet, 
        string connectionString,string queryString)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = new SqlCommand(
                queryString, connection);
            adapter.Fill(dataSet);
            
            return dataSet;
             
        }

       
    }

   
    public static SqlDataAdapter CreateSqlDataAdapter(SqlConnection connection)
    {
        SqlDataAdapter adapter = new SqlDataAdapter();

        adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;


        adapter.SelectCommand = new SqlCommand(
            "SELECT * FROM authors", connection
            );

        adapter.DeleteCommand = new SqlCommand(
            "DELETE FROM authors WHERE au_id = @au_id", connection
            );

        adapter.DeleteCommand.Parameters.Add("@au_id", SqlDbType.UniqueIdentifier);

        return adapter;

    }

   



}









public class Author

{
  


    [Required]
    public string id { get; set; }
    [Required]
    public string firstName { get; set; }
    [Required]

    public string lastName { get; set; }
    [Required]
    public string phone { get; set; }
    [Required]
    public bool contract { get; set; }

    public string? city { get; set; }
    public string? state { get; set; }
    public string? zip { get; set; }
    public string? address { get; set; }
}

public class Book
{
    [Required]
    public string id { get; set; }
    [Required]
    public string title { get; set; }
    [Required]
    public string type { get; set; }
    [Required]
    public DateTime pubdate { get; set; }
    public string? pub_id { get; set; }
    public SqlMoney? price { get; set; }
    public SqlMoney? advance { get;set; }
    public int ? royalty { get; set; } 
    public int? ytd_sales { get; set; }
    public string? notes { get; set; }  










}
