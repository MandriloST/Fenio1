# Fenio1 API

Formula 1 prognoze i rezultati utrka – .NET 8 Web API s SQLite i EF Core.

---

## Arhitektura

```
Fenio1/
├── src/
│   ├── Fenio1.Core/               # Domenska logika (entiteti, servisi, interfejsi)
│   │   ├── Entities/              # Svi EF Core entiteti
│   │   ├── Interfaces/            # IServices (IScoringService, IExternalApiService)
│   │   └── Services/              # ScoringService, ExternalApiService (stub)
│   │
│   ├── Fenio1.Infrastructure/     # Pristup bazi i infrastruktura
│   │   ├── Data/                  # AppDbContext (EF Core + SQLite)
│   │   ├── Migrations/            # EF Core migracije
│   │   └── Services/              # JwtService
│   │
│   └── Fenio1.API/                # ASP.NET Core Web API
│       ├── Controllers/           # Svi kontroleri
│       ├── DTOs/                  # Request/Response modeli
│       └── Middleware/            # ErrorHandlingMiddleware
└── Fenio1.sln
```

---

## Pokretanje

### Preduvjeti
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### 1. Kloniraj / raspakiraj projekt

### 2. Pokreni API

```bash
cd src/Fenio1.API
dotnet run
```

API će automatski:
- Kreirati `fenio1.db` SQLite bazu
- Pokrenuti migracije
- Ubaciti seed podatke (vozači, timovi, staze, sezone 2025)

### 3. Swagger UI

Otvori browser: `https://localhost:5001` (ili `http://localhost:5000`)

---

## Autentifikacija

JWT Bearer token. Korisnici se **ne mogu registrirati** kroz API – samo admin može kreirati korisnike.

### Zadani korisnici (seed data)

| Username | Lozinka  | Uloga |
|----------|----------|-------|
| `admin`  | `Admin@123` | Admin |
| `user1`  | `User@123`  | User  |

### Login

```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "Admin@123"
}
```

**Response:**
```json
{
  "token": "eyJ...",
  "username": "admin",
  "email": "admin@fenio1.com",
  "role": "Admin",
  "expiresAt": "2025-01-02T00:00:00Z"
}
```

U Swagger UI – klikni **Authorize** → unesi: `Bearer eyJ...`

---

## Uloge i dozvole

| Endpoint kategorija           | User | Admin |
|-------------------------------|------|-------|
| Login                         | ✅   | ✅   |
| Čitanje vozača, timova, staza | ✅   | ✅   |
| Unos/ažuriranje prognoza      | ✅   | ✅   |
| Čitanje leaderboarda          | ✅   | ✅   |
| Upravljanje korisnicima       | ❌   | ✅   |
| Kreiranje vozača/timova/staza | ❌   | ✅   |
| Unos rezultata utrka          | ❌   | ✅   |
| Import iz vanjskog API-a      | ❌   | ✅   |

---

## API Endpointovi

### Autentifikacija
| Metoda | URL | Opis |
|--------|-----|------|
| POST | `/api/auth/login` | Login, vraća JWT token |

### Korisnici (Admin)
| Metoda | URL | Opis |
|--------|-----|------|
| GET | `/api/users` | Svi korisnici |
| GET | `/api/users/{id}` | Jedan korisnik |
| POST | `/api/users` | Novi korisnik |
| PUT | `/api/users/{id}` | Ažuriraj korisnika |
| DELETE | `/api/users/{id}` | Obriši korisnika |

### Sezone
| Metoda | URL | Opis |
|--------|-----|------|
| GET | `/api/seasons` | Sve sezone |
| GET | `/api/seasons/active` | Aktivna sezona |
| POST | `/api/seasons` | Nova sezona (Admin) |
| PUT | `/api/seasons/{id}` | Ažuriraj sezonu (Admin) |

### Timovi
| Metoda | URL | Opis |
|--------|-----|------|
| GET | `/api/teams` | Svi timovi |
| POST | `/api/teams` | Novi tim (Admin) |
| PUT | `/api/teams/{id}` | Ažuriraj tim (Admin) |
| DELETE | `/api/teams/{id}` | Deaktiviraj tim (Admin) |

### Vozači
| Metoda | URL | Opis |
|--------|-----|------|
| GET | `/api/drivers` | Svi vozači |
| GET | `/api/drivers/summary` | Sažetak za dropdown liste |
| POST | `/api/drivers` | Novi vozač (Admin) |
| PUT | `/api/drivers/{id}` | Ažuriraj vozača (Admin) |
| DELETE | `/api/drivers/{id}` | Deaktiviraj vozača (Admin) |

### Staze
| Metoda | URL | Opis |
|--------|-----|------|
| GET | `/api/circuits` | Sve staze |
| POST | `/api/circuits` | Nova staza (Admin) |
| PUT | `/api/circuits/{id}` | Ažuriraj stazu (Admin) |
| DELETE | `/api/circuits/{id}` | Obriši stazu (Admin) |

### Utrke
| Metoda | URL | Opis |
|--------|-----|------|
| GET | `/api/races?seasonId=1` | Utrke (filtrirano po sezoni) |
| GET | `/api/races/{id}` | Detalji utrke |
| GET | `/api/races/{id}/results` | Rezultati utrke |
| POST | `/api/races` | Nova utrka (Admin) |
| PUT | `/api/races/{id}` | Ažuriraj utrku (Admin) |
| DELETE | `/api/races/{id}` | Obriši utrku (Admin) |
| **POST** | **`/api/races/{id}/results`** | **Unos rezultata (Admin)** |
| POST | `/api/races/{id}/results/import-external` | Import iz vanjskog API-a (Admin, stub) |

### Prognoze
| Metoda | URL | Opis |
|--------|-----|------|
| GET | `/api/predictions/my` | Moje prognoze |
| GET | `/api/predictions/my/race/{raceId}` | Moja prognoza za utrku |
| **POST** | **`/api/predictions/race/{raceId}`** | **Unos/ažuriranje prognoze** |
| GET | `/api/predictions/race/{raceId}` | Sve prognoze za utrku (Admin) |
| PATCH | `/api/predictions/{id}/lock?locked=true` | Zaključaj prognozu (Admin) |
| GET | `/api/predictions/my/score/race/{raceId}` | Moji bodovi za utrku |

### Leaderboard
| Metoda | URL | Opis |
|--------|-----|------|
| GET | `/api/leaderboard/active` | Leaderboard aktivne sezone |
| GET | `/api/leaderboard/season/{id}` | Leaderboard sezone |
| GET | `/api/leaderboard/race/{raceId}` | Bodovi po utrci |
| POST | `/api/leaderboard/race/{raceId}/recalculate` | Ponovi izračun (Admin) |

---

## Unos prognoze – primjer

```http
POST /api/predictions/race/1
Authorization: Bearer {token}
Content-Type: application/json

{
  "racePositions": [
    { "position": 1, "driverId": 7 },
    { "position": 2, "driverId": 1 },
    { "position": 3, "driverId": 3 },
    { "position": 4, "driverId": 4 },
    { "position": 5, "driverId": 8 },
    { "position": 6, "driverId": 5 },
    { "position": 7, "driverId": 9 },
    { "position": 8, "driverId": 14 },
    { "position": 9, "driverId": 13 },
    { "position": 10, "driverId": 15 }
  ],
  "qualifyingPositions": [
    { "position": 1, "driverId": 7 },
    { "position": 2, "driverId": 1 },
    { "position": 3, "driverId": 8 }
  ],
  "sprintPositions": null,
  "predictedDnfDriverId": 2,
  "predictedFastestLapDriverId": 7,
  "predictedScoredPointsDriverId": 13,
  "predictedFastestPitStopDriverId": 4,
  "predictedDriverOfTheDayId": 7,
  "predictedSafetyCarCount": 1
}
```

## Unos rezultata utrke (Admin) – primjer

```http
POST /api/races/1/results
Authorization: Bearer {admin_token}
Content-Type: application/json

{
  "racePositions": [
    { "position": 1, "driverId": 7 },
    { "position": 2, "driverId": 1 },
    ...
  ],
  "qualifyingPositions": [
    { "position": 1, "driverId": 7 },
    { "position": 2, "driverId": 1 },
    { "position": 3, "driverId": 3 }
  ],
  "sprintPositions": null,
  "extras": {
    "dnfDriverId": 2,
    "fastestLapDriverId": 7,
    "scoredPointsDriverId": 13,
    "fastestPitStopDriverId": 4,
    "driverOfTheDayId": 7,
    "safetyCarCount": 1
  }
}
```

---

## Sustav bodovanja

Svaki **pogođeni podatak** = **2 boda**.

Kategorije koje donose bodove:
- Pozicija u utrci (1-10) → po 2 boda za svaku točnu poziciju = max **20 bodova**
- Pozicija u kvalifikacijama (1-3) → po 2 boda = max **6 bodova**
- Pozicija u sprint utrci (1-3, ako postoji) → po 2 boda = max **6 bodova**
- Did Not Finish vozač → **2 boda**
- Fastest Lap vozač → **2 boda**
- Scored Points vozač → **2 boda**
- Fastest Pit Stop vozač → **2 boda**
- Driver of the Day → **2 boda**
- Broj Safety Car izlazaka → **2 boda**

**Maksimum po utrci bez sprinta: 36 bodova**
**Maksimum po utrci sa sprintom: 42 boda**

Bodovi se automatski izračunavaju čim admin unese rezultate utrke.

---

## Vanjski API – implementacija

Stub za vanjski API servis je u:
`src/Fenio1.Core/Services/ExternalApiService.cs`

Implementirajte `FetchRaceResultsAsync` prema odabranom F1 API-u:

- **[OpenF1 API](https://openf1.org/)** – besplatan, real-time (preporučeno)
- **[Ergast API](http://ergast.com/mrd/)** – besplatan, ali deprecated
- **[SportMonks F1](https://docs.sportmonks.com/)** – plaćen

Nakon implementacije, `ExternalRaceData` model mapira podatke u Fenio1 format.  
Utrka mora imati postavljen `externalApiId` koji se koristi za API poziv.

---

## Baza podataka

SQLite baza `fenio1.db` se automatski kreira pri prvom pokretanju.

### Ručni unos korisnika u bazu

```sql
INSERT INTO Users (Username, Email, PasswordHash, Role, IsActive, CreatedAt)
VALUES ('novi_user', 'novi@email.com', '$2a$11$...bcrypt_hash...', 0, 1, datetime('now'));
```

Za generiranje BCrypt hasha koristite npr. `BCrypt.Net.BCrypt.HashPassword("lozinka")`.

---

## Konfiguracija

`appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=fenio1.db"
  },
  "Jwt": {
    "Key": "PROMIJENITE_U_PRODUKCIJI_minimum_32_znaka!",
    "Issuer": "Fenio1API",
    "Audience": "Fenio1Clients",
    "ExpiryHours": "24"
  }
}
```

> ⚠️ **Važno:** Promijenite `Jwt:Key` u produkciji!

---

## Tehnologije

| Tehnologija | Verzija | Svrha |
|-------------|---------|-------|
| .NET | 8.0 | Runtime |
| ASP.NET Core | 8.0 | Web API framework |
| Entity Framework Core | 8.0 | ORM |
| SQLite | - | Baza podataka |
| JWT Bearer | 8.0 | Autentifikacija |
| BCrypt.Net-Next | 4.0.3 | Hash lozinki |
| Swashbuckle (Swagger) | 6.5 | API dokumentacija |
