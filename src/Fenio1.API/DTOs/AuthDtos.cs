namespace Fenio1.API.DTOs;

// ========================
//  AUTH
// ========================

public record LoginRequest(string Username, string Password);

public record LoginResponse(
    string Token,
    string Username,
    string Email,
    string Role,
    DateTime ExpiresAt
);

// ========================
//  COMMON
// ========================

public record PagedResult<T>(
    IEnumerable<T> Items,
    int TotalCount,
    int Page,
    int PageSize
);
