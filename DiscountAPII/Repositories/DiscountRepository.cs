using Dapper;
using Discount.APII.Entities;
using Discount.APII.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Threading.Tasks;

namespace Discount.APII.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly IConfiguration _configuration;

        public DiscountRepository(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<List<Coupon>> GetDiscounts(string bookName)
        {
            using var connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));

            var coupons = await connection.QueryAsync<Coupon>
                ("SELECT * FROM Coupon WHERE BookName = @BookName", new { BookName = bookName });

            if (coupons == null || !coupons.Any())
            {
                // Повертаємо пустий список або можна обробити інакше, відповідно до вашого вимог.
                return new List<Coupon> { new Coupon { BookName = "No Discount", Amount = 0, Description = "No Discount Desc" } };
            }

            return coupons.ToList();
        }


        public async Task<bool> CreateDiscount(Coupon coupon)
        {
            using var connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));

            var affected =
                await connection.ExecuteAsync
                    ("INSERT INTO Coupon (BookName, Description, Amount) VALUES (@BookName, @Description, @Amount)",
                            new { BookName = coupon.BookName, Description = coupon.Description, Amount = coupon.Amount });

            if (affected == 0)
                return false;

            return true;
        }

        public async Task<bool> UpdateDiscount(Coupon coupon)
        {
            using var connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));

            var affected = await connection.ExecuteAsync
                    ("UPDATE Coupon SET BookName=@BookName, Description = @Description, Amount = @Amount WHERE Id = @Id",
                            new { BookName = coupon.BookName, Description = coupon.Description, Amount = coupon.Amount, Id = coupon.Id });

            if (affected == 0)
                return false;

            return true;
        }

        public async Task<bool> DeleteDiscount(string bookName)
        {
            using var connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));

            var affected = await connection.ExecuteAsync("DELETE FROM Coupon WHERE BookName = @BookName",
                new { BookName = bookName });

            if (affected == 0)
                return false;

            return true;
        }
    }
}
