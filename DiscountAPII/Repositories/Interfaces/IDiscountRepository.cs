using Discount.APII.Entities;

namespace Discount.APII.Repositories.Interfaces
{
    public interface IDiscountRepository
    {
        Task<List<Coupon>> GetDiscounts(string bookName);
        Task<bool> CreateDiscount(Coupon coupon);
        Task<bool> UpdateDiscount(Coupon coupon);
        Task<bool> DeleteDiscount(string bookName);
    }
}