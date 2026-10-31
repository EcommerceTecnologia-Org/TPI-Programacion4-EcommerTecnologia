using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore; // para usar .Include()

namespace Infrastructure.Repositories;

// Hereda la funcionalidad generica de Repository<Order> e implementa IOrderRepository
public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(ApplicationContext context) : base(context)
    {
    }

    // Método para traer todas las órdenes incluyendo sus detalles
    public List<Order> GetAllWithDetails()
    {
        return _context.Orders
            .Include(o => o.OrderDetails)
            .ToList();
    }

    // Método para traer una orden por ID incluyendo sus detalles
    public Order? GetByIdWithDetails(int id)
    {
        return _context.Orders
            .Include(o => o.OrderDetails)
            .FirstOrDefault(o => o.Id == id);
    }
}
