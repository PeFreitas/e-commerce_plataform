using Catalog.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Adicionar serviços ao contêiner.

// 1. Configurar o DbContext para PostgreSQL
//    Ele vai automaticamente buscar a string de conexão do Secret Manager em desenvolvimento.
builder.Services.AddDbContext<CatalogContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configurar o pipeline de requisições HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Endpoints para CRUD de Produtos (ainda como placeholders)
app.MapGet("/api/products", () => "Todos os produtos").WithName("GetProducts");
app.MapGet("/api/products/{id}", (int id) => $"Produto com ID: {id}").WithName("GetProductById");
app.MapPost("/api/products", () => "Produto criado").WithName("CreateProduct");
app.MapPut("/api/products/{id}", (int id) => $"Produto com ID: {id} atualizado").WithName("UpdateProduct");
app.MapDelete("/api/products/{id}", (int id) => $"Produto com ID: {id} deletado").WithName("DeleteProduct");


app.Run();