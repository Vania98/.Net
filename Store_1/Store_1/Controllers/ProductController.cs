using Store_1.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using MassTransit;

namespace Store_1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        private readonly IPublishEndpoint _publishEndpoint;
        public ProductController (IPublishEndpoint publish ,IConfiguration config)
        {
            _configuration = config;
            _publishEndpoint = publish;

        }
        //Обробляє запити HTTP GET на маршруті
        // Отримує всі продукти з бази даних і повертає список Product об’єктів.
        [HttpGet]
        [Route("GetAll")]
        public List<Product> GetAllArticle()
        {
            List<Product> articles = new List<Product>();
            SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            SqlCommand cmd = new SqlCommand("Select * from Product", conn);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            foreach(DataRow row in dt.Rows)
            {
                Product obj = new Product();
                obj.Id = int.Parse(row["id"].ToString());
                obj.ProductName = row["ProductName"].ToString();
                obj.Price = decimal.Parse(row["Price"].ToString());
                obj.Description = row["Description"].ToString();
                obj.SellerId = int.Parse(row["SellerID"].ToString());
                
                articles.Add(obj);
            }

            return articles;
        }
        //Обробляє запити HTTP POST на маршруті 
        // Вставляє новий продукт у базу даних, використовуючи параметри з наданого Productоб’єкта.
        // Крім того, він публікує щойно створений продукт за допомогою MassTransit.
        [HttpPost]
        [Route("CreateProduct")]
        public IActionResult CreateArticle(Product product)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    conn.Open();

                    string insertQuery = "INSERT INTO Product (ProductName, Price, Description, SellerID) VALUES (@ProductName, @Price, @Description, @SellerID);";

                    using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@ProductName", product.ProductName);
                        cmd.Parameters.AddWithValue("@Price", product.Price);
                        cmd.Parameters.AddWithValue("@Description", product.Description);
                        cmd.Parameters.AddWithValue("@SellerID", product.SellerId);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            _publishEndpoint.Publish<Product>(product);
                            return Ok("Продукт створено");
                        }
                        else
                        {
                            return BadRequest("Продукт не був створений");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Помилка під час створення продукту: " + ex.Message);
            }
        }

        //Обробляє запити HTTP PUT на маршруті 
        // Оновлює наявний продукт у базі даних на основі наданого Productоб’єкта.
        [HttpPut]
        [Route("UpdateProduct")]
        public IActionResult UpdateArticle(Product product)
        {
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                conn.Open();
                string query = "update Product set ProductName = @ProductName, Price = @Price, Description = @Description, SellerID = @SellerID where id = @id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", product.Id);
                    cmd.Parameters.AddWithValue("@ProductName", product.ProductName);
                    cmd.Parameters.AddWithValue("@Price", product.Price);
                    cmd.Parameters.AddWithValue("@Description", product.Description);
                    cmd.Parameters.AddWithValue("@SellerID", product.SellerId);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        return Ok(GetAllArticle());
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }
        }
        //Обробляє запити HTTP DELETE на маршруті 
        //Видаляє товар із бази даних на основі наданого ProductID.
        [HttpDelete]
        [Route("DeleteProduct")]
        public IActionResult DeleteArticle(int ProductID)
        {
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("delete from Product where id=@id ", conn))
                {
                    cmd.Parameters.AddWithValue("@id", ProductID);

                    int rowAffected  = cmd.ExecuteNonQuery();

                    if(rowAffected > 0)
                    {
                        return Ok("Продукт Видалено");
                    }
                    else { return NotFound(); }
                }
            }

        }

    }
}
