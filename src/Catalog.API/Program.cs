using Catalog.API.Data;
using Catalog.API.Models; // Adicione este using para o Product
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Adicionar serviços ao contêiner.
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

// --- ENDPOINTS COM LÓGICA DE BANCO DE DADOS ---

// GET: /api/products - Obter todos os produtos
app.MapGet("/api/products", async (CatalogContext context) =>
{
    var products = await context.Products.ToListAsync();
    return Results.Ok(products);
})
.WithName("GetProducts")
.Produces<List<Product>>(StatusCodes.Status200OK); // Informa ao Swagger o que esperar

// GET: /api/products/{id} - Obter produto por ID
app.MapGet("/api/products/{id}", async (int id, CatalogContext context) =>
{
    var product = await context.Products.FindAsync(id);
    return product is not null ? Results.Ok(product) : Results.NotFound();
})
.WithName("GetProductById")
.Produces<Product>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);

// POST: /api/products - Criar um novo produto
app.MapPost("/api/products", async (Product product, CatalogContext context) =>
{
    context.Products.Add(product);
    await context.SaveChangesAsync();
    return Results.CreatedAtRoute("GetProductById", new { id = product.Id }, product);
})
.WithName("CreateProduct")
.Produces<Product>(StatusCodes.Status201Created);

// PUT: /api/products/{id} - Atualizar um produto
app.MapPut("/api/products/{id}", async (int id, Product inputProduct, CatalogContext context) =>
{
    var product = await context.Products.FindAsync(id);
    if (product is null)
    {
        return Results.NotFound();
    }

    product.Name = inputProduct.Name;
    product.Description = inputProduct.Description;
    product.Price = inputProduct.Price;
    product.Stock = inputProduct.Stock;

    await context.SaveChangesAsync();
    return Results.NoContent();
})
.WithName("UpdateProduct")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound);

// DELETE: /api/products/{id} - Deletar um produto
app.MapDelete("/api/products/{id}", async (int id, CatalogContext context) =>
{
    var product = await context.Products.FindAsync(id);
    if (product is null)
    {
        return Results.NotFound();
    }

    context.Products.Remove(product);
    await context.SaveChangesAsync();
    return Results.NoContent();
})
.WithName("DeleteProduct")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound);

app.Run();