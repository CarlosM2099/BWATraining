using BlazingPizza.Shared;
using Microsoft.JSInterop;
using System.Collections.Generic;

namespace BlazingPizza.Client.Services
{
    public class OrderState //AppState
    {
        public Pizza ConfiguringPizza { get; private set; }
        public bool ShowingConfigureDialog { get; private set; }
        public Order Order { get; private set; } = new Order();
        
        public void ShowConfigurePizzaDialog(PizzaSpecial special)
        {
            ConfiguringPizza = new Pizza()
            {
                Special = special,
                SpecialId = special.Id,
                Size = Pizza.DefaultSize,
                Toppings = new List<PizzaTopping>()
            };

            ShowingConfigureDialog = true;
        }

        public void CancelConfigurePizzaDialog()
        {
            ConfiguringPizza = null;
            ShowingConfigureDialog = false;
        }

        public void ConfirmConfigurePizzaDialog()
        {
            Order.Pizzas.Add(ConfiguringPizza);
            ConfiguringPizza = null;
            ShowingConfigureDialog = false;
        }

        public  void RemoveConfiguredPizza(Pizza pizza)
        {
           
            Order.Pizzas.Remove(pizza);
        }

        public void ResetOrder()
        {
            Order = new Order();
        }

        public void ReplaceOrder(Order order)
        {
            Order = order;
        }
    }
}
