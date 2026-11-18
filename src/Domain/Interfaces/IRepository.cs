namespace Domain.Interfaces;
// Repositorio genérico que define las operaciones CRUD básicas para cualquier entidad.

public interface IRepository<T> where T : class
{   
    // Consultas (Get By Id / Get All)
    // Busca una entidad por su ID. Devuelve null si no existe

    T? GetById(int id);
    // Devuelve la lista completa de entidades registradas

    List<T> List();
    // Alta
    // Agrega una nueva entidad al contexto (requiere SaveChanges para persistir)

    T Add(T entity);
    // Modificacion
    // Marca una entidad como modificada en el Change Tracker (requiere SaveChanges)

    void Update(T entity);
    // Baja
    // Marca una entidad para ser eliminada de la base de datos

    void Delete(T entity);
    // Persiste en la base de datos todos los cambios pendientes. Devuelve la cantidad de filas afectadas

    int SaveChanges();
}