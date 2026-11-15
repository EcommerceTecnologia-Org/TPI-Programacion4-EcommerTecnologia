using Domain.Entities;

namespace Domain.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    // Metodos especificos de Product
    List<Product> GetByCategoryId(int categoryId);
}