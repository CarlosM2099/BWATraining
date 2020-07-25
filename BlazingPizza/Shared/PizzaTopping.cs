using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BlazingPizza.Shared
{
    public class PizzaTopping
    {        
        public int ToppingId { get; set; }
        public int PizzaId { get; set; }
        public Topping Topping { get; set; }

    }
}
