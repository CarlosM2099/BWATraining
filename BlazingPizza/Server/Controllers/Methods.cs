using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlazingPizza.Server.Controllers
{
    public static class Methods
    {
        public static string GetUserId(this ClaimsPrincipal claims)
        {
            return claims?.FindFirst(ClaimTypes.Name).Value;
        }
    }
}
