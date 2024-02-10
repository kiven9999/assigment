using Cricut.Orders.Api.Mappings;
using Cricut.Orders.Api.ViewModels;
using Cricut.Orders.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Cricut.Orders.Api.Controllers
{
    [Route("v1/orders")]
    [ApiController]
    public class OrdersController : Controller
    {
        private readonly IOrderDomain _orderDomain;

        public OrdersController(IOrderDomain orderDomain)
        {
            _orderDomain = orderDomain;
        }

        [HttpPost]
        public async Task<ActionResult<OrderViewModel>> CreateNewOrderAsync([FromBody] NewOrderViewModel newOrderVM)
        {
            var newOrder = newOrderVM.ToDomainModel();
            var savedOrder = await _orderDomain.CreateNewOrderAsync(newOrder);

            double orderTotal = savedOrder.OrderItems.Sum(item => item.Product.Price * item.Quantity);

            if (orderTotal >= 25)
            {
                orderTotal *= 0.9; 
            }

            savedOrder.Total = orderTotal;

            return Ok(savedOrder.ToViewModel());
        }

        [HttpGet("{customerId}")]
        public async Task<ActionResult<OrderViewModel[]>> GetOrdersForCustomerAsync(int customerId)
        {
            int[] staticCustomerIds = { 12345, 54321 };

            if (!staticCustomerIds.Contains(customerId))
            {
                return NotFound();
            }

            var staticOrders = GetStaticOrdersForCustomer(customerId);

            if (staticOrders == null || staticOrders.Length == 0)
            {
                return NotFound();
            }

            var orderViewModels = staticOrders.Select(order => order.ToViewModel()).ToArray();

            return Ok(orderViewModels);
        }

        private Order[] GetStaticOrdersForCustomer(int customerId)
        {
            
            if (customerId == 12345)
            {
                return new Order[] { new Order(), new Order() }; 
            }
            else if (customerId == 54321)
            {
                return new Order[] { new Order(), new Order(), new Order() };
            }
            else
            {
                return null; 
            }
        }
    }
}
