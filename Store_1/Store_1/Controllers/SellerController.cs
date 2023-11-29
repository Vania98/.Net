using Store_1.Model;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Store_1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SellerController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public SellerController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        //обробляє запит HTTP GET на маршруті 
        //та отримує всіх продавців із таблиці «Продавець» за допомогою Dapper.
        [HttpGet]
        [Route("GetAll")]
        public async Task<ActionResult<List<Seller>>> GetAllSeller()
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            IEnumerable<Seller> seller = await SelelAllSeller(connection);
            return Ok(seller);

        }
        //Dapper - це легка та швидка бібліотека для взаємодії з базами даних у .NET. Вона є однією з найпопулярніших бібліотек ORM
        //(Object-Relational Mapping) та дозволяє розробникам виконувати запити до бази даних, використовуючи чистий SQL.
        //обробляє запит HTTP GET на маршруті 
        //та отримує певного продавця за ідентифікатором із таблиці «Продавець» за допомогою Dapper.
        [HttpGet]
        [Route("GetByID")]
        public async Task<ActionResult<List<Seller>>> GetAuthor(int sellerId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            var seller = await connection.QueryFirstAsync<Seller>("select * from Seller where id = @id",
                new {id = sellerId});
            return Ok(seller);

        }
        //Обробляє запит HTTP POST і вставляє нового продавця в таблицю «Продавець» за допомогою Dapper.
        [HttpPost]
        public async Task<ActionResult<List<Seller>>> CreatedAuthor(Seller seller)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("insert into Seller (SellerName, Email) values (@SellerName, @Email)",seller);
            return Ok(await SelelAllSeller(connection));

        }

        //обробляє запит HTTP PUT і оновлює існуючого продавця в таблиці «Продавець» на основі наданого Seller об’єкта за допомогою Dapper.
        [HttpPut]
        public async Task<ActionResult<List<Seller>>> UpdateAuthor(Seller seller)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync(
                "update Seller set SellerName = @SellerName, Email = @Email where id = @id",
                 new{id = seller.Id , SellerName = seller.SellerName, Email = seller.Email});
            return Ok(await SelelAllSeller(connection));

        }
        //Обробляє запит HTTP DELETE на маршруті 
        //та видаляє продавця за ідентифікатором із таблиці «Продавець» за допомогою Dapper.
        [HttpDelete]
        [Route("DeleteSellerByID")]
        public async Task<ActionResult<List<Seller>>> DeleteAuthor(int sellerId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("delete from Seller where id = @id", new {id = sellerId});
            return Ok(await SelelAllSeller(connection));

        }
        //Dapper використовується для виконання операцій з базою даних (вставлення, оновлення, видалення) за допомогою асинхронних методів
        //( ExecuteAsync, QueryFirstAsync, QueryAsync) на SqlConnection.
        //Асинхронні методи використовуються для виконання запитів і повернення результатів у вигляді ActionResult<List<Seller>>.
        //ActionResult використовується для повернення HTTP-відповідей із певними даними.
        //Task використовується для виконання асинхронних операцій.
        //
        private static async Task<IEnumerable<Seller>> SelelAllSeller(SqlConnection connection)
        {
            return await connection.QueryAsync<Seller>("select * from Seller");
        }
    }
}
