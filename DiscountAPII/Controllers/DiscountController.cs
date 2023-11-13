using Discount.APII.Entities;
using Discount.APII.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Discount.APII.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DiscountController : ControllerBase
    {
        private readonly IDiscountRepository _repository;
        private readonly IMemoryCache memoryCache;

        public DiscountController(IDiscountRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.memoryCache = memoryCache;
        }

        [HttpGet("{bookName}", Name = "GetDiscounts")]
        [ProducesResponseType(typeof(List<Coupon>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<List<Coupon>>> GetDiscounts(string bookName)
        {
            var cacheKey = "coupon";
            if (!memoryCache.TryGetValue(cacheKey, out List<Coupon> coupons))
            {
                coupons = await _repository.GetDiscounts(bookName);
                var cacheExpiryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(5),
                    Priority = CacheItemPriority.High,
                    SlidingExpiration = TimeSpan.FromMinutes(2)
                };
                memoryCache.Set(cacheKey, coupons, cacheExpiryOptions);
            }
            return Ok(coupons);
        }


        [HttpPost]
        [ProducesResponseType(typeof(Coupon), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Coupon>> CreateDiscount([FromBody] Coupon coupon)
        {
            await _repository.CreateDiscount(coupon);
            return CreatedAtRoute("GetDiscount", new { bookName = coupon.BookName }, coupon);
        }

        [HttpPut]
        [ProducesResponseType(typeof(Coupon), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Coupon>> UpdateBasket([FromBody] Coupon coupon)
        {
            return Ok(await _repository.UpdateDiscount(coupon));
        }

        [HttpDelete("{bookName}", Name = "DeleteDiscount")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<bool>> DeleteDiscount(string bookName)
        {
            return Ok(await _repository.DeleteDiscount(bookName));
        }
    }
}
