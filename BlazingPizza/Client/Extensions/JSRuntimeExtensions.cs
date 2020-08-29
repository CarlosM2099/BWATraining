using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace BlazingPizza.Client.Extensions
{
    public static class JSRuntimeExtensions
    {
        public static async ValueTask<bool> Confirm(this IJSRuntime jsRuntime, string message)
        {
            return await jsRuntime.InvokeAsync<bool>($"confirm", message);
        }
    }
}
