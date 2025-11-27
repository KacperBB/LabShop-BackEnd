using Microsoft.EntityFrameworkCore;
using Shop.Api.Models;

namespace Shop.Api.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(ShopDbContext context)
        {
            // Tworzy bazę, jeśli nie istnieje
            await context.Database.EnsureCreatedAsync();

            // jeśli są już produkty, nie seedujemy drugi raz
            if (await context.Products.AnyAsync())
                return;

            var categories = new List<Category>
            {
                new Category { Name = "Elektronika", Slug = "elektronika" },
                new Category { Name = "Książki", Slug = "ksiazki" },
                new Category { Name = "Dom i ogród", Slug = "dom-ogrod" }
            };

            context.Categories.AddRange(categories);
            await context.SaveChangesAsync();

            // pomocniczo bierzemy Id kategorii
            var catEle = categories[0].Id;
            var catBooks = categories[1].Id;
            var catHome = categories[2].Id;

            var products = new List<Product>
            {
                new Product { Name = "Smartfon X1", Description = "Nowoczesny smartfon z ekranem OLED.", Price = 1999m, StockQuantity = 10, CategoryId = catEle, ImageUrl = "/images/smartfon-x1.jpg" },
                new Product { Name = "Laptop Pro 15", Description = "Laptop do pracy i gier.", Price = 4999m, StockQuantity = 5, CategoryId = catEle, ImageUrl = "/images/laptop-pro-15.jpg" },
                new Product { Name = "Słuchawki bezprzewodowe", Description = "Dobre brzmienie, ANC.", Price = 399m, StockQuantity = 20, CategoryId = catEle, ImageUrl = "/images/sluchawki.jpg" },
                new Product { Name = "Klawiatura mechaniczna", Description = "Klawiatura dla graczy.", Price = 299m, StockQuantity = 15, CategoryId = catEle, ImageUrl = "/images/klawiatura.jpg" },
                new Product { Name = "Mysz gamingowa", Description = "Precyzyjna i wygodna.", Price = 199m, StockQuantity = 25, CategoryId = catEle, ImageUrl = "/images/mysz.jpg" },

                new Product { Name = "Książka C# od podstaw", Description = "Podręcznik programowania w C#.", Price = 89m, StockQuantity = 30, CategoryId = catBooks, ImageUrl = "/images/csharp-book.jpg" },
                new Product { Name = "ASP.NET Core w praktyce", Description = "Budowa aplikacji webowych.", Price = 99m, StockQuantity = 20, CategoryId = catBooks, ImageUrl = "/images/aspnet-book.jpg" },
                new Product { Name = "Wzorce projektowe", Description = "Książka o wzorcach projektowych.", Price = 129m, StockQuantity = 12, CategoryId = catBooks, ImageUrl = "/images/patterns-book.jpg" },
                new Product { Name = "Algorytmy i struktury danych", Description = "Podstawy CS.", Price = 119m, StockQuantity = 8, CategoryId = catBooks, ImageUrl = "/images/algos-book.jpg" },
                new Product { Name = "Książka o architekturze REST", Description = "Projektowanie API.", Price = 79m, StockQuantity = 18, CategoryId = catBooks, ImageUrl = "/images/rest-book.jpg" },

                new Product { Name = "Zestaw noży kuchennych", Description = "Solidne noże ze stali nierdzewnej.", Price = 249m, StockQuantity = 10, CategoryId = catHome, ImageUrl = "/images/noze.jpg" },
                new Product { Name = "Patelnia ceramiczna", Description = "Do zdrowego gotowania.", Price = 139m, StockQuantity = 15, CategoryId = catHome, ImageUrl = "/images/patelnia.jpg" },
                new Product { Name = "Zestaw garnków", Description = "Komplet garnków do kuchni.", Price = 399m, StockQuantity = 7, CategoryId = catHome, ImageUrl = "/images/garnki.jpg" },
                new Product { Name = "Koc polarowy", Description = "Miękki i ciepły koc.", Price = 69m, StockQuantity = 25, CategoryId = catHome, ImageUrl = "/images/koc.jpg" },
                new Product { Name = "Poduszka ergonomiczna", Description = "Wygodna poduszka do spania.", Price = 89m, StockQuantity = 20, CategoryId = catHome, ImageUrl = "/images/poduszka.jpg" },

                // jeszcze kilka, żeby dobić do ~25
                new Product { Name = "Monitor 24\"", Description = "Monitor Full HD do pracy.", Price = 799m, StockQuantity = 9, CategoryId = catEle, ImageUrl = "/images/monitor-24.jpg" },
                new Product { Name = "Monitor 27\"", Description = "Monitor QHD do gier.", Price = 1299m, StockQuantity = 6, CategoryId = catEle, ImageUrl = "/images/monitor-27.jpg" },
                new Product { Name = "Powerbank 10 000 mAh", Description = "Ładowanie w podróży.", Price = 99m, StockQuantity = 40, CategoryId = catEle, ImageUrl = "/images/powerbank.jpg" },
                new Product { Name = "Router Wi-Fi 6", Description = "Szybka sieć w domu.", Price = 349m, StockQuantity = 11, CategoryId = catEle, ImageUrl = "/images/router.jpg" },

                new Product { Name = "Lampka biurkowa LED", Description = "Do pracy przy biurku.", Price = 79m, StockQuantity = 16, CategoryId = catHome, ImageUrl = "/images/lampka.jpg" },
                new Product { Name = "Organizer na biurko", Description = "Porządek w akcesoriach.", Price = 49m, StockQuantity = 30, CategoryId = catHome, ImageUrl = "/images/organizer.jpg" },
                new Product { Name = "Półka na książki", Description = "Drewniana półka.", Price = 159m, StockQuantity = 5, CategoryId = catHome, ImageUrl = "/images/polka.jpg" },
                new Product { Name = "Kubek termiczny", Description = "Do kawy i herbaty.", Price = 69m, StockQuantity = 22, CategoryId = catHome, ImageUrl = "/images/kubek.jpg" },
                new Product { Name = "Notes w kratkę", Description = "Do notatek i planów.", Price = 19m, StockQuantity = 50, CategoryId = catBooks, ImageUrl = "/images/notes.jpg" }
            };

            context.Products.AddRange(products);
            await context.SaveChangesAsync();
        }
    }
}
