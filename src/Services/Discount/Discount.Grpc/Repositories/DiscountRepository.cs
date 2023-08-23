using Dapper;
using Discount.Grpc.Entities;
using Npgsql;

namespace Discount.Grpc.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly IConfiguration _configuration;
        private NpgsqlConnection connection;

        public DiscountRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> CreateDiscount(Coupon coupon)
        {
            var connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var affected = await connection.ExecuteAsync("Insert into Coupon(ProductName, Amount, Description) values(@ProductName, @Amount, @Description)",
                new
                {
                    ProductName = coupon.ProductName,
                    Amount = coupon.Amount,
                    Description = coupon.Description,
                });
            if (affected == 0)
                return false;

            return true;
        }

        public async Task<bool> DeleteDiscount(string productName)
        {
            //var connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            //var affected = await connection.ExecuteAsync("Delete from Coupon where Id = @Id",
            //    new
            //    {
            //        Id = coupon.Id
            //    });

            var connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var affected = await connection.ExecuteAsync("Delete from Coupon where ProductName = @Id",
                new
                {
                    ProductName= productName,
                });
            if (affected == 0)
                return false;

            return true;
        }

        public async Task<Coupon> GetDiscount(string productName)
        {
            var connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            //throw new NotImplementedException();
            var coupon = await connection.QueryFirstOrDefaultAsync<Coupon>
                ("Select * from Coupon where ProductName =@ProductName", new { ProductName = productName });
            if (coupon is null)
            {
                return new Coupon
                {
                    Amount = 0,
                    ProductName = "No Discount",
                    Description = "No Discount Description"
                };
            }
            return coupon;
        }

        public async Task<bool> UpdateDiscount(Coupon coupon)
        {
            var connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var affected = await connection.ExecuteAsync("Update Coupon set ProductName = @ProductName, Amount = @Amount, Description = @Description where Id = @Id",
                new
                {
                    ProductName = coupon.ProductName,
                    Amount = coupon.Amount,
                    Description = coupon.Description,
                    Id = coupon.Id
                });
            if (affected == 0)
                return false;

            return true;
        }
    }
}
