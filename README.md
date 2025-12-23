# Notatnik

.NET 8.0 SDK
SQL Server LocalDB
Uruchomienie
Przygotuj bazę danych:

dotnet ef database update
Uruchom aplikację:

dotnet run
Aplikacja będzie dostępna na:

Strona główna: http://localhost:5000 (lub 5001)
Testy: http://localhost:5000/test lub http://localhost:5000/index.html
Swagger: http://localhost:5000/swagger
Endpointy
POST /register - Rejestracja użytkownika
POST /login - Logowanie (zwraca JWT token)
GET /notes - Lista notatek (wymaga autoryzacji)
POST /notes - Utwórz notatkę (wymaga autoryzacji)
PUT /notes/{id} - Zaktualizuj notatkę (wymaga autoryzacji)
DELETE /notes/{id} - Usuń notatkę (wymaga autoryzacji)
Testy
Uruchom aplikację i otwórz w przeglądarce: http://localhost:5000/test
