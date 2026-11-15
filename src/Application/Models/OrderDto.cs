using Domain.Entities;


namespace Application.Models
{
    public class OrderDto
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public double Total { get; set; }

        // Agregamos la lista de detalles de la orden (permitiendo nulos si la orden viene sin detalles)
        public List<OrderDetailDto>? OrderDetails { get; set; }

        // Mapea una entidad Order a OrderDto
        public static OrderDto Create(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                ClientId = order.ClientId,
                Total = order.Total,

                // Valida si la lista de detalles no es nula; si tiene datos los mapea, y si es nula devuelve una lista vacía para evitar errores
                OrderDetails = order.OrderDetails != null ? OrderDetailDto.Create(order.OrderDetails) : new List<OrderDetailDto>()
            };
        }

        // Mapea una lista de órdenes
        public static List<OrderDto> Create(IEnumerable<Order> orders)
        {
            return orders.Select(Create).ToList();
        }
    }
}