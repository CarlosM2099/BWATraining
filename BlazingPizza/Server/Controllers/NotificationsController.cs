using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazingPizza.Server.Models;
using BlazingPizza.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlazingPizza.Server.Controllers
{
    [Route("notifications")]
    [ApiController]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly PizzaStoreContext context;

        public NotificationsController(PizzaStoreContext context)
        {
            this.context = context;
        }

        [HttpPut("subscribe")]
        public async Task<NotificationSubscription> Subscribe(NotificationSubscription subscription)
        {
            var user = User.GetUserId();
            var oldSubscriptions = context.NotificationSubscriptions.Where(e => e.UserId == user);

            context.NotificationSubscriptions.RemoveRange(oldSubscriptions);

            // Store new subscription
            subscription.UserId = user;
            context.NotificationSubscriptions.Attach(subscription);

            await context.SaveChangesAsync();
            return subscription;
        }
    }
}
