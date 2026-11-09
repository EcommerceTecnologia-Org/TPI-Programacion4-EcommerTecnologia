using System;
using System.Collections.Generic;
using System.Linq;
using Application.Models;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class OrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
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

        public OrderDto GetById(int id)
        {
            var order = _orderRepository.GetByIdWithDetails(id);

            if (order == null)
            {
                throw new KeyNotFoundException("La orden no fue encontrada.");
            }

            return OrderDto.Create(order);
        }

        public OrderDto Create(int clientId, PostOrderRequest request)
        {
            // Instanciamos la orden usando el constructor de dominio (las validaciones de total corren ahí dentro)
            var order = new Order(clientId, request.Total);

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