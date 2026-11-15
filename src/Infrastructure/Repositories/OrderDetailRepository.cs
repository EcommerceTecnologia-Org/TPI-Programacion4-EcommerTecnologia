using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repositories;
// Hereda la funcionalidad generica de Repository<OrderDetail> e implementa IOrderDetailRepository
public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository // implementa IOrderDetailRepository para admitir consultas especificas de orderDetail si fuera necesario
{
    // El constructor recibe el ApplicationContext y se lo pasa a la clase base (Repository<OrderDetail>)
    public OrderDetailRepository(ApplicationContext context) : base(context)
    {
    }
    // Implementacion de metodos especificos
}
