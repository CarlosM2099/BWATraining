using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using BlazingPizza.Server.Models;
using BlazingPizza.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebPush;
using System.Text.Json;

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
            order.UserId = User.GetUserId();

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

            var subscription = await storeContext.NotificationSubscriptions.
                FirstOrDefaultAsync(u => u.UserId == User.GetUserId());

            if (subscription is { })
            {
                _ = TrackAndSendNotificationAsync(order, subscription);
            }

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
                .Where(o => o.UserId == User.GetUserId())
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
              .Where(o => o.UserId == User.GetUserId())
              .OrderByDescending(o => o.CreatedTime)
              .Select(o => OrderWithStatus.FromOrder(o))
              .SingleOrDefaultAsync();

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        private static async Task SendNotificationAsync(Order order, NotificationSubscription subscription, string message)
        {
            var publicKey = "BLC8GOevpcpjQiLkO7JmVClQjycvTCYWm6Cq_a7wJZlstGTVZvwGFFHMYfXt6Njyvgx_GlXJeo5cSiZ1y4JOx1o";
            var privateKey = "OrubzSz3yWACscZXjFQrrtDwCKg-TGFuWhluQ2wLXDo";
            var pushSubscription = new PushSubscription()
            {
                Endpoint = subscription.Url,
                Auth = subscription.Auth,
                P256DH = subscription.P256dh
            };

            var vapiDetails = new VapidDetails("mailto:someone@example.com", publicKey, privateKey);
            var webPushClient = new WebPushClient();

            try
            {
                var payload = JsonSerializer.Serialize(new
                {
                    message,
                    url = $"myorders/{order.OrderId}"
                });

                await webPushClient.SendNotificationAsync(pushSubscription, payload, vapiDetails);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Error al enviar la notificacion push. {e.Message}");
            };
        }

        private static async Task TrackAndSendNotificationAsync(Order order, NotificationSubscription subscription)
        {
            await Task.Delay(OrderWithStatus.PreparationDuration);
            await SendNotificationAsync(order, subscription, "Tu orden ya esta en camino!");
            await Task.Delay(OrderWithStatus.DeliveryDuration);
            await SendNotificationAsync(order, subscription, "Tu orden ha sido entregada! Buen provecho!");
        }
    }
}
