using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BlazingPizza.Server.Models;
using BlazingPizza.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazingPizza.Server.Controllers
{
    [Route("orders")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly PizzaStoreContext storeContext;
        public OrdersController(PizzaStoreContext storeContext)
        {
            this.storeContext = storeContext;
        }

        [HttpPost]
        public async Task<ActionResult<int>> PlaceOrder(Order order)
        {            
            order.CreatedTime = DateTime.Now;
            order.DeliveryLocation = new LatLong(47.613092, -122.205702);

            foreach (var pizza in order.Pizzas)
            {
                pizza.SpecialId = pizza.Special.Id;
                pizza.Special = null;

                foreach (var topping in pizza.Toppings)
                {
                    topping.ToppingId = topping.Topping.Id;
                    topping.Topping = null;
                }
            }

            storeContext.Orders.Attach(order);
            await storeContext.SaveChangesAsync();
            return order.OrderId;
        }

        [HttpGet]
        public async Task<ActionResult<List<OrderWithStatus>>> GetOrders()
        {
            var orders = await storeContext.Orders
                .Include(o => o.DeliveryLocation)
                .Include(o => o.Pizzas).ThenInclude(p => p.Special)
                .Include(o => o.Pizzas).ThenInclude(p => p.Toppings)
                .ThenInclude(t => t.Topping)
                .OrderByDescending(o => o.CreatedTime)
                .ToListAsync();

            return orders
                .Select(o => OrderWithStatus.FromOrder(o))
                .ToList();
        }

        [HttpGet("{orderId}")]
        public async Task<ActionResult<OrderWithStatus>> GetOrderWithStatus(int orderId)
        {
            var order = await storeContext.Orders
              .Where(o => o.OrderId == orderId)
              .Include(o => o.DeliveryLocation)
              .Include(o => o.Pizzas).ThenInclude(p => p.Special)
              .Include(o => o.Pizzas).ThenInclude(p => p.Toppings)
              .ThenInclude(t => t.Topping)
              .OrderByDescending(o => o.CreatedTime)
              .Select(o => OrderWithStatus.FromOrder(o))
              .SingleOrDefaultAsync();

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

    }
}
