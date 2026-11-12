using System.Text.Json.Serialization;

namespace Application.Models;

// DTO que mapea la respuesta de la API externa dolarapi.com
public class DolarResponseDto
{
    [JsonPropertyName("moneda")]
    public string Moneda { get; set; } = string.Empty;

    [JsonPropertyName("casa")]
    public string Casa { get; set; } = string.Empty;

    [JsonPropertyName("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [JsonPropertyName("compra")]
    public decimal Compra { get; set; }

    [JsonPropertyName("venta")]
    public decimal Venta { get; set; }

    [JsonPropertyName("fechaActualizacion")]
    public DateTime FechaActualizacion { get; set; }
}