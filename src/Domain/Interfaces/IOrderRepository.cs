using Domain.Entities;

namespace Domain.Interfaces;

// Hereda las operaciones CRUD basicas (GetById, GetAll, Add, Update, Delete) de IRepository<Order>
public interface IOrderRepository : IRepository<Order>
    {
        // Métodos específicos para obtener órdenes incluyendo los detalles
        List<Order> GetAllWithDetails();
        Order? GetByIdWithDetails(int id);
    }