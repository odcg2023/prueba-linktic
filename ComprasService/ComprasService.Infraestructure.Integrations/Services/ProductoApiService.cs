using ComprasService.Application.Dto.Integrations;
using ComprasService.Application.Interfaces.Integrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ComprasService.Infraestructure.Integrations.Services
{
    public class ProductoApiService : IProductoApiService
    {
        private readonly HttpClient _httpClient;

        public ProductoApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<ProductoDto> ObtenerProductoPorIdAsync(int idProducto, string jwtToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"obtener-producto-por-id/{idProducto}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException($"Error al consultar el producto {idProducto}: {response.StatusCode}");
            }

            var json = await response.Content.ReadAsStringAsync();

            using var document = JsonDocument.Parse(json);

            var root = document.RootElement;

            if (!root.TryGetProperty("data", out var dataElement) ||
                !dataElement.TryGetProperty("attributes", out var attributes))
            {
                throw new ApplicationException("El JSON no contiene la estructura esperada: 'data.attributes'.");
            }

            return new ProductoDto
            {
                IdProducto = attributes.GetProperty("idProducto").GetInt32(),
                NombreProducto = attributes.GetProperty("nombreProducto").GetString() ?? string.Empty,
                Descripcion = attributes.GetProperty("descripcion").GetString() ?? string.Empty,
                Precio = attributes.GetProperty("precio").GetDecimal(),
                Activo = attributes.GetProperty("activo").GetBoolean()
            };
        }

    }
}
