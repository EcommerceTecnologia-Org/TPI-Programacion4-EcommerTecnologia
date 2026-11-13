using Application.Models;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class OrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;

        public OrderService(IOrderRepository orderRepository, IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        public List<OrderDto> GetOrdersForUser(int userId, string role)
        {
            List<Order> orders;

            // Si es Admin o Superadmin, trae todas con detalles; si es cliente, filtra las suyas
            if (role == "Admin" || role == "Superadmin")
            {
                orders = _orderRepository.GetAllWithDetails();
            }
            else
            {
                // filtra únicamente por su ClientId
                orders = _orderRepository.GetAllWithDetails().Where(o => o.ClientId == userId).ToList();
            }

            if (orders == null || !orders.Any())
            {
                throw new KeyNotFoundException("No se encontraron órdenes registradas.");
            }

            return OrderDto.Create(orders);
        }

        public OrderDto GetById(int id, int currentUserId, string role)
        {
            var order = _orderRepository.GetByIdWithDetails(id);

            if (order == null)
            {
                throw new KeyNotFoundException("La orden no fue encontrada.");
            }

            // Si es un cliente común, validamos que la orden le pertenezca
            if (role != "Admin" && role != "Superadmin" && order.ClientId != currentUserId)
            {
                throw new UnauthorizedAccessException("No tenés permisos para ver esta orden.");
            }

            return OrderDto.Create(order);
        }

        public OrderDto Create(int clientId, PostOrderRequest request)
        {
            double calculatedTotal = 0;
            var orderDetails = new List<OrderDetail>();

            foreach (var itemRequest in request.Items)
            {
                var product = _productRepository.GetById(itemRequest.ProductId);
                if (product == null)
                {
                    throw new KeyNotFoundException($"El producto con ID {itemRequest.ProductId} no fue encontrado.");
                }

                // Si el producto está inactivo (baja lógica), no se puede comprar
                if (!product.Active)
                {
                    throw new InvalidOperationException($"El producto con ID {itemRequest.ProductId} no se encuentra activo para la venta.");
                }

                // Descontamos stock de forma segura mediante la entidad
                product.SubtractStock(itemRequest.Quantity);
                _productRepository.Update(product); // // Registramos el cambio en el Change Tracker (sin impactar todavía en la BDD)

                // Acumulamos el total usando el precio real del producto en la BD
                calculatedTotal += product.Price * itemRequest.Quantity;

                // Creamos el detalle usando el constructor de dominio de OrderDetail
                var orderDetail = new OrderDetail(product.Id, itemRequest.Quantity, product.Price);
                orderDetails.Add(orderDetail);
            }

            // Creamos la orden con el total calculado de forma segura en el servidor
            var order = new Order(clientId, calculatedTotal);

            // Asociamos los detalles a la orden
            foreach (var detail in orderDetails)
            {
                order.OrderDetails.Add(detail);
            }
            // Al agregar la orden, se ejecuta el SaveChanges() y se persiste todo de forma atómica:
            // La orden, sus detalles y los stocks descontados de los productos.
            _orderRepository.Add(order);

            return OrderDto.Create(order);
        }
        public void Delete(int id)
        {
            var order = _orderRepository.GetById(id);
            if (order == null)
            {
                throw new KeyNotFoundException("La orden no fue encontrada");
            }
            _orderRepository.Delete(order);
        }
    }
}