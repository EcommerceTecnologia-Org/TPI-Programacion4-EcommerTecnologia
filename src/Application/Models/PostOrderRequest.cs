namespace Application.Models;

public class PostOrderRequest
{
    // Solo recibimos la lista de ítems a comprar
    public List<PostOrderDetailRequest> Items { get; set; } = new List<PostOrderDetailRequest>();
}