using Domain.Entities;

namespace Domain.Interfaces
{
    // Hereda las operaciones CRUD basicas (GetById, GetAll, Add, Update, Delete) de IRepository<OrderDetail>
    public interface IOrderDetailRepository : IRepository<OrderDetail>
    {
        // Métodos específicos de OrderDetail acá
    }
}