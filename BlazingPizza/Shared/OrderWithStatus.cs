using BlazingPizza.ComponentsLibrary.Map;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace BlazingPizza.Shared
{
    public class OrderWithStatus
    {
        public readonly static TimeSpan PreparationDuration = TimeSpan.FromSeconds(10);

        public readonly static TimeSpan DeliveryDuration = TimeSpan.FromMinutes(1);

        public Order Order { get; set; }

        public string StatusText { get; set; }

        public List<Marker> MapMarkers { get; set; }
        public static OrderWithStatus FromOrder(Order order)
        {
            string message;
            List<Marker> markers;
            var dispatchTime = order.CreatedTime.Add(PreparationDuration);

            if (DateTime.Now < dispatchTime)
            {
                message = "Preparando";
                markers = new List<Marker> { ToMapMarker("Usted", order.DeliveryLocation, showPopup: true) };
            }
            else if (DateTime.Now < dispatchTime + DeliveryDuration)
            {
                message = "En Camino";
                var startPosition = ComputeStartPosition(order);
                var proportionOfDeliveryCompleted = Math.Min(1, (DateTime.Now - dispatchTime).TotalMilliseconds / DeliveryDuration.TotalMilliseconds);
                var driverPosition = LatLong.Interpolate(startPosition, order.DeliveryLocation, proportionOfDeliveryCompleted);
                markers = new List<Marker>
                {
                    ToMapMarker("Usted",order.DeliveryLocation),
                    ToMapMarker("Repartidor", driverPosition,  true)
                };
            }
            else
            {
                message = "Entregado";
                markers = new List<Marker> 
                {
                    ToMapMarker("Ubicacion de entrega",order.DeliveryLocation, true)
                };
            }

            return new OrderWithStatus { Order = order, StatusText = message, MapMarkers = markers };
        }

        static Marker ToMapMarker(string description, LatLong deliveryLocation,
            bool showPopup = false) => new Marker { Description = description, X = deliveryLocation.Longitude, Y = deliveryLocation.Latitude, ShowPopup = showPopup };

        static LatLong ComputeStartPosition(Order order)
        {
            var random = new Random(order.OrderId);
            var distance = 0.01 + random.NextDouble() * 0.02;
            var angle = random.NextDouble() * Math.PI * 2;
            var offset = (distance * Math.Cos(angle), distance * Math.Sin(angle));

            return new LatLong(order.DeliveryLocation.Latitude + offset.Item1, order.DeliveryLocation.Longitude + offset.Item2);

        }
    }
}
