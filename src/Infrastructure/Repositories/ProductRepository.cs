using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

// ProductRepository hereda todos los metodos CRUD basicos de Repository<Product>
public class ProductRepository : Repository<Product>, IProductRepository // implementa IProductRepository para admitir consultas especificas de productos si fuera necesario
{
    // El constructor recibe el ApplicationContext y se lo pasa a la clase base (Repository<Product>)
    public ProductRepository(ApplicationContext context) : base(context)
    {
    }
    
    // Implementacion de metodos especificos
    public List<Product> GetByCategoryId(int categoryId)
    {
        // Filtramos usando el _dbSet heredado de la clase base
        return _dbSet.Where(p => p.CategoryId == categoryId).ToList();
    }
}