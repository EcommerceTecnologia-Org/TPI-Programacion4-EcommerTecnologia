using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

// base generica que implementa el contrato IRepository<T>
public class Repository<T> : IRepository<T> where T : class // 'where T : class' restringe el tipo T para que siempre sea una clase
{
    // Inyeccion del contexto de base de datos de Entity Framework
    protected readonly ApplicationContext _context; // Usamos 'protected' para que las clases hijas puedan acceder a ellos
    
    // DbSet representa la tabla especifica correspondiente a la entidad T
    protected readonly DbSet<T> _dbSet;

    public Repository(ApplicationContext context)
    {
        _context = context;
        // Obtenemos el conjunto de datos correspondiente a la entidad T desde el DbContext
        _dbSet = _context.Set<T>();
    }

    // Obtiene una entidad por su clave primaria (Id)
    public T? GetById(int id)
    {
        return _dbSet.Find(id);
    }

    // Obtiene el listado completo de registros de la tabla
    public List<T> List()
    {
        return _dbSet.ToList();
    }

    // Agrega una nueva entidad y la guarda automaticamente en la base de datos
    public T Add(T entity)
    {
        _dbSet.Add(entity);
        SaveChanges();
        return entity;
    }

    // Actualiza una entidad existente y persiste el cambio de inmediato
    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    // Elimina un registro de la tabla y guarda los cambios
    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
        SaveChanges();
    }

    // Metodo auxiliar para guardar cambios en la base de datos
    public int SaveChanges()
    {
        return _context.SaveChanges();
    }
}