using System.Collections.Generic;
using System.Linq;
using Application.Models;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class OrderDetailService
    {
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IProductRepository _productRepository;

        public OrderDetailService(IOrderDetailRepository orderDetailRepository, IProductRepository productRepository)
        {
            _orderDetailRepository = orderDetailRepository;
            _productRepository = productRepository;
        }

        public List<OrderDetailDto> GetAll()
        {
            var details = _orderDetailRepository.List();
            
            if (details == null || !details.Any())
            {
                throw new KeyNotFoundException("No se encontraron detalles de ordenes registrados");
            }

            return OrderDetailDto.Create(details);
        }

        public OrderDetailDto GetById(int id)
        {
            var detail = _orderDetailRepository.GetById(id);
            if (detail == null)
            {
                throw new KeyNotFoundException("El detalle de la orden no fue encontrado");
            }
            return OrderDetailDto.Create(detail);
        }

        public OrderDetailDto Create(PostOrderDetailRequest request)
        {
            // Validamos que el producto exista antes de asociarlo al detalle
            var product = _productRepository.GetById(request.ProductId);
            if (product == null)
            {
                throw new KeyNotFoundException("El producto indicado en el detalle no existe");
            }

            // Instanciamos el detalle usando el constructor de dominio (las validaciones de quantity y unitPrice corren ahí)
            var orderDetail = new OrderDetail(
                request.ProductId,
                request.Quantity,
                request.UnitPrice
            );

            _orderDetailRepository.Add(orderDetail);

            return OrderDetailDto.Create(orderDetail);
        }

        public void Delete(int id)
        {
            var detail = _orderDetailRepository.GetById(id);
            if (detail == null)
            {
                throw new KeyNotFoundException("El detalle de la orden no fue encontrado");
            }
            _orderDetailRepository.Delete(detail);
        }
    }
}