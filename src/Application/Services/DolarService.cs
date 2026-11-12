using Application.Models;
using System.Net.Http.Json;


namespace Application.Services;

// Servicio que consulta la API de terceros dolarapi.com para obtener la cotizacion oficial del dolar
public class DolarService
{
    private readonly HttpClient _httpClient;

    public DolarService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Devuelve el valor de venta del dolar oficial, o null si la API externa falla
    public async Task<double?> GetVentaOficial()
    {
        try
        {
            var respuesta = await _httpClient.GetFromJsonAsync<DolarResponseDto>("https://dolarapi.com/v1/dolares/oficial");

            if (respuesta == null)
            {
                return null;
            }

            return (double)respuesta.Venta;
        }
        catch (Exception)
        {
            // Si la API externa no responde, los productos se devuelven sin PriceUsd y la API no se cae
            return null;
        }
    }
}