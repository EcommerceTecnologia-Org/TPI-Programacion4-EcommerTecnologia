namespace Domain.Interfaces;

public interface IRepository<T> where T : class
{   
    // Consultas (Get By Id / Get All)
    T? GetById(int id);
    List<T> List();
    // Alta
    T Add(T entity);
    // Modificacion
    void Update(T entity);
    // Baja
    void Delete(T entity);
    int SaveChanges();
}