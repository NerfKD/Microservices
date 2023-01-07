using Dapper;
using FreeCourse.Shared.Dtos;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Services.Discount.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly IConfiguration _configuration;
        private readonly IDbConnection _dbConnection;
        public DiscountService(IConfiguration configuration)
        {
            _configuration = configuration;
            _dbConnection = new NpgsqlConnection(_configuration.GetConnectionString("PostgreSql"));
        }
        public async Task<Response<NoContent>> Delete(int id)
        {
            var deleteStatus = await _dbConnection.ExecuteAsync("delete from discount where id=@id", new { Id = id });
            return deleteStatus > 0 ? Response<NoContent>.Success(204) : Response<NoContent>.Fail("discount not found", 404);
        }

        public async Task<Response<List<Models.Discount>>> GetAll()
        {
            var discounts = await _dbConnection.QueryAsync<Models.Discount>("select * from discount");
            return Response<List<Models.Discount>>.Success(discounts.ToList(), 200);
        }

        public  async Task<Response<Models.Discount>> GetByCodeAndUserId(string code, string userId)
        {
            var discount = await _dbConnection.QueryFirstOrDefaultAsync<Models.Discount>("select * from discount where code=@Code and userid = @UserId", new { Code =code, UserId = userId });
            return discount == null ? Response<Models.Discount>.Fail("discount not found", 404) : Response<Models.Discount>.Success(discount, 200);
        }

        public async Task<Response<Models.Discount>> GetById(int id)
        {
            var discount = await _dbConnection.QueryFirstOrDefaultAsync<Models.Discount>("select * from discount where id=@Id", new { Id = id });

            if (discount == null)
            {
                return Response<Models.Discount>.Fail("discount not found", 404);
            }
            return Response<Models.Discount>.Success(discount, 200);
        }

        public async Task<Response<NoContent>> Save(Models.Discount discount)
        {
            var saveStatus = await _dbConnection.ExecuteAsync("insert into discount (userId,rate,code) values (@UserId,@Rate,@Code)", discount);
            if (saveStatus > 0)
            {
                return Response<NoContent>.Success(204);
            }
            return Response<NoContent>.Fail("an error occurred while adding", 500);
        }

        public async Task<Response<NoContent>> Update(Models.Discount discount)
        {
            var updateStatus = await _dbConnection.ExecuteAsync("update discount set userid= @UserId, code=@Code, rate=@Rate where id =@Id", discount);
            if (updateStatus > 0)
            {
                return Response<NoContent>.Success(204);
            }
            return Response<NoContent>.Fail("discount not found", 404);

        }
    }
}
