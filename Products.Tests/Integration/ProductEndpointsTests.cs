using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Products.Tests.Helpers;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace Products.Tests.Integration
{
    public class ProductEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public ProductEndpointsTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task PostProduct_WithValidToken_ReturnsCreated()
        {
            var token = await AuthorisationHelper.GetJwtTokenAsync(_client);

            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var product = new
            {
                name = "Shirt",
                colour = "Blue",
                price = 29.99m
            };

            var response = await _client.PostAsJsonAsync("/products", product);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
            Assert.Equal("Shirt", result!["name"]?.ToString());
        }

        [Fact]
        public async Task PostProduct_WithoutToken_Returns401()
        {
            var client = _factory.CreateClient();

            var product = new
            {
                name = "Ferrari 250 GTO",
                colour = "Red",
                price = 1000000.00m
            };

            var response = await client.PostAsJsonAsync("/products", product);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetProducts_WithColourFilter_ReturnsFilteredList()
        {
            var token = await AuthorisationHelper.GetJwtTokenAsync(_client);

            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Add two products
            await _client.PostAsJsonAsync("/products", new { name = "Ferrari 250 GTO", colour = "Red", price = 1000000.00m });
            await _client.PostAsJsonAsync("/products", new { name = "Aston Martin DB 5", colour = "Silver", price = 500000.00m });
            await _client.PostAsJsonAsync("/products", new { name = "Jaguar E-Type", colour = "Green", price = 300000.00m });

            // Filter by Red
            var response = await _client.GetAsync("/products?colour=Red");
            response.EnsureSuccessStatusCode();

            var products = await response.Content.ReadFromJsonAsync<List<Dictionary<string, object>>>();

            Assert.Single(products!);
            Assert.NotNull(products);
            Assert.NotEmpty(products);

            var name = products![0]["name"]?.ToString();
            Assert.False(string.IsNullOrEmpty(name));
            Assert.Equal("Ferrari 250 GTO", name.ToString());
        }

        [Fact]
        public async Task PostProduct_WithInvalidData_ReturnsValidationProblem()
        {
            var token = await AuthorisationHelper.GetJwtTokenAsync(_client);

            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var invalidProduct = new
            {
                name = "",            // Empty name
                colour = "",          // Empty colour
                price = -50.0m         // Invalid price
            };

            var response = await _client.PostAsJsonAsync("/products", invalidProduct);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var errorResult = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

            Assert.True(errorResult!.Errors.ContainsKey("Name"));
            Assert.True(errorResult.Errors.ContainsKey("Colour"));
            Assert.True(errorResult.Errors.ContainsKey("Price"));
        }


    }
}
