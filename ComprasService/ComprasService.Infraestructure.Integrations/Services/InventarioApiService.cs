using ComprasService.Application.Dto.Integrations;
using ComprasService.Application.Interfaces.Integrations;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ComprasService.Infraestructure.Integrations.Services
{
    public class InventarioApiService : IInventarioApiService
    {
        private readonly HttpClient _httpClient;
        public InventarioApiService(HttpClient httpClient) 
        { 
            _httpClient = httpClient;
        }

        public async Task<InventarioCompraActualizadoDto> ActualizarInventario(ProductosCompraDto compra, string jwt)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "actualizar-inventario-compras");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

            // Serializar el body
            var body = new
            {
                tipoMovimiento = compra.TipoMovimiento,
                listaProductos = compra.ListaProductos.Select(p => new
                {
                    idProducto = p.IdProducto,
                    cantidad = p.Cantidad
                }),
                observaciones = compra.Observaciones
            };

            request.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException($"Error al actualizar el inventario: {response.StatusCode}");
            }

            var json = await response.Content.ReadAsStringAsync();

            using var document = JsonDocument.Parse(json);

            var root = document.RootElement;

            if (!root.TryGetProperty("data", out var dataElement) ||
                !dataElement.TryGetProperty("attributes", out var attributes))
            {
                throw new ApplicationException("El JSON no contiene la estructura esperada: 'data.attributes'.");
            }

            return new InventarioCompraActualizadoDto
            {
                IdMovimiento = attributes.GetProperty("idMovimiento").GetInt32(),
                TipoMovimiento = (byte)attributes.GetProperty("tipoMovimiento").GetInt32()
            };
        }

        public async Task<InventarioProductoDto> ObtenerInventarioProducto(int idProducto, string jwt)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"obtener-existencias/{idProducto}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException($"Error al consultar el inventario  del producto {idProducto}: {response.StatusCode}");
            }

            var json = await response.Content.ReadAsStringAsync();

            using var document = JsonDocument.Parse(json);

            var root = document.RootElement;

            if (!root.TryGetProperty("data", out var dataElement) ||
                !dataElement.TryGetProperty("attributes", out var attributes))
            {
                throw new ApplicationException("El JSON no contiene la estructura esperada: 'data.attributes'.");
            }

            return new InventarioProductoDto
            {
                IdProducto = attributes.GetProperty("idProducto").GetInt32(),
                Cantidad = attributes.GetProperty("existenciasActuales").GetInt32(),
            };
        }
    }
}
