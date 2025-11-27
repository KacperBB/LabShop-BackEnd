# Shop.Api

Krótkie REST API sklepu zbudowane w ASP.NET Core (.NET 10) z użyciem Entity Framework Core i Identity. Zawiera: zarządzanie produktami, kategoriami, tagami, recenzjami, zamówieniami, kuponami, promocjami oraz prostą autoryzację JWT.
Projekt jest częścią projektu LabShop-FrontEnd

## Wymagania
- .NET 10 SDK
- SQL Server (lub inny provider skonfigurowany w `Program.cs`)
- Visual Studio 2026 (opcjonalnie) lub inny edytor

## Konfiguracja
W `appsettings.json` / `appsettings.Development.json` ustaw:

Jeśli brak `Jwt:Key`, aplikacja użyje klucza developerskiego — nie używać na produkcji.

## Uruchomienie lokalne
1. Przywróć pakiety: `dotnet restore` lub w Visual Studio użyj __Restore__.
2. Wykonaj migracje:
   - `dotnet ef database update`
   - lub uruchom __Package Manager Console__ i wykonaj migracje tam.
3. Uruchom aplikację: `dotnet run` lub naciśnij __F5__ w Visual Studio.

Seed danych: `SeedData.InitializeAsync` tworzy przykładowe kategorie i produkty jeśli tabela `Products` jest pusta.

## Autoryzacja
Autentykacja oparta na ASP.NET Identity + JWT.
- `POST /api/auth/register` — rejestracja użytkownika.
- `POST /api/auth/login` — logowanie; zwraca token JWT.
- Token umieść w nagłówku `Authorization: Bearer <token>` dla endpointów chronionych.

Token zawiera claim `ClaimTypes.NameIdentifier` (user.Id) — użyj `User.FindFirstValue(ClaimTypes.NameIdentifier)`.

## Najważniejsze endpointy (skrót)

Auth
- `POST /api/auth/register`
  - Body: `{ "email","password","firstName?","lastName?" }`
- `POST /api/auth/login`
  - Body: `{ "email","password" }`
  - Response: `{ "token","email","roles" }`

Produkty (`/api/products`)
- `GET /api/products` — lista
- `GET /api/products/{id}` — szczegóły
- `POST /api/products` — utworzenie
- `PUT /api/products/{id}` — aktualizacja
- `DELETE /api/products/{id}` — usunięcie
- `GET /api/products/search?...` — filtrowanie (categoryId, tagId, minPrice, maxPrice, query)

Recenzje (`/api/products/{productId}/reviews`)
- `GET` — lista recenzji
- `POST` (Authorize) — upsert recenzji `{ "rating", "comment" }`
- `DELETE {id}` (Authorize) — autor lub Admin/Moderator może usuwać

Zamówienia (`/api/orders`)
- `GET /api/orders`, `GET /api/orders/{id}`, `GET /api/orders/by-email/{email}`
- `POST /api/orders` — tworzenie zamówienia (obsługa kuponów)
- `POST /api/orders/{id}/pay` — oznacz jako zapłacone
- `GET /api/orders/stats` — statystyki

Panel admina (`/api/admin/*`)
- Produkty: `GET`, `PUT {id}/category`, `PUT {id}/tags` (role Admin, Moderator)
- Zamówienia: `GET`, `PUT {id}/status` (Admin, Moderator)
- Kupony: `POST /api/admin/coupons/generate` (Admin/Moderator)
- Użytkownicy: `POST /api/admin/users/{userId}/roles` (wymaga roli Admin)

