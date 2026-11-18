using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

// Hereda la funcionalidad generica de Repository<Category> e implementa ICategoryRepository
public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationContext context) : base(context)
    {
    }
    
    // Busca una categoría por su nombre ignorando mayúsculas/minúsculas

    public Category? GetByName(string name)
    {
        return _dbSet
            .FirstOrDefault(c => c.Name.ToLower() == name.ToLower());
    }
}