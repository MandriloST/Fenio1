namespace Fenio1.API.DTOs;

using Fenio1.Core.Entities;

// ========================
//  USER
// ========================

public record UserDto(int Id, string Username, string Email, string Role, bool IsActive, DateTime CreatedAt);

public record CreateUserRequest(
    string Username,
    string Email,
    string Password,
    UserRole Role = UserRole.User
);

public record UpdateUserRequest(
    string? Email,
    string? Password,
    UserRole? Role,
    bool? IsActive
);

// ========================
//  TEAM
// ========================

public record TeamDto(int Id, string Name, string ShortName, string Nationality, string? PrimaryColor, bool IsActive);

public record CreateTeamRequest(
    string Name,
    string ShortName,
    string Nationality,
    string? PrimaryColor
);

public record UpdateTeamRequest(
    string? Name,
    string? ShortName,
    string? Nationality,
    string? PrimaryColor,
    bool? IsActive
);

// ========================
//  DRIVER
// ========================

public record DriverDto(
    int Id,
    string FirstName,
    string LastName,
    string FullName,
    string Code,
    int DriverNumber,
    string Nationality,
    bool IsActive,
    int? TeamId,
    string? TeamName
);

public record DriverSummaryDto(int Id, string FullName, string Code, int DriverNumber, string? TeamName);

public record CreateDriverRequest(
    string FirstName,
    string LastName,
    string Code,
    int DriverNumber,
    string Nationality,
    DateTime DateOfBirth,
    int? TeamId
);

public record UpdateDriverRequest(
    string? FirstName,
    string? LastName,
    string? Code,
    int? DriverNumber,
    string? Nationality,
    int? TeamId,
    bool? IsActive
);

// ========================
//  CIRCUIT
// ========================

public record CircuitDto(
    int Id,
    string Name,
    string Location,
    string Country,
    string CountryCode,
    int LapCount,
    decimal LengthKm
);

public record CreateCircuitRequest(
    string Name,
    string Location,
    string Country,
    string CountryCode,
    int LapCount,
    decimal LengthKm
);

public record UpdateCircuitRequest(
    string? Name,
    string? Location,
    string? Country,
    string? CountryCode,
    int? LapCount,
    decimal? LengthKm
);
