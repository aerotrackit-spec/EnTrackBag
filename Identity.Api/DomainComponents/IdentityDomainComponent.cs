using Identity.Api.Data.Entities;
using Identity.Api.Data.Repositories;
using Identity.Api.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Identity.Api.DomainComponents;

public class IdentityDomainComponent : IIdentityDomainComponent
{
    private readonly IIdentityRepository _identityRepository;
    private readonly string _jwtKey;
    private readonly string _jwtIssuer;
    private readonly string _jwtAudience;
    private readonly string _encryptionKey;

    // MANDATORY REQUIREMENT: Standard constructor injection layout only (No primary constructors)
    public IdentityDomainComponent(IIdentityRepository identityRepository, IConfiguration configuration, IPasswordHasher<UserEntity> passwordHasher)
    {
        _identityRepository = identityRepository;

        // OPTIMIZATION: Read and cache configuration items at setup initialization phase
        _jwtKey = configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key missing");
        _jwtIssuer = configuration["Jwt:Issuer"] ?? string.Empty;
        _jwtAudience = configuration["Jwt:Audience"] ?? string.Empty;
        _encryptionKey = configuration["Security:EncryptionKey"] ?? "MySecretEncryptionKey123!";
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request, string? ip, string? userAgent, string? machineName, CancellationToken ct)
    {
        var user = await _identityRepository.GetUserForLoginAsync(request.UserName, ct);
        if (user is null || !user.IsActive)
            return null;

        // OPTIMIZATION: Pass the pre-cached key parameter directly 
        string decryptedDatabasePassword = DecryptAes256(user.PasswordHash, _encryptionKey);
        if (request.Password != decryptedDatabasePassword)
            return null;

        var roles = user.UserRoles.Select(x => x.Role.Name).ToArray();
        var permissionAccess = await _identityRepository.GetPermissionAccessAsync(user.Id, ct);
        var now = DateTime.UtcNow;
        var expires = now.AddHours(8);

        // Stage 1: Add Session and Save changes immediately to acquire the Database-generated SessionId
        var session = new UserSessionEntity
        {
            UserId = user.Id,
            LoginAt = now,
            RemoteIp = ip,
            UserAgent = userAgent,
            TokenIssuedAt = now,
            TokenExpiresAt = expires,
            IsActive = true
        };
        user.LastLoginAt = now;

        await _identityRepository.AddSessionAsync(session, ct);
        await _identityRepository.SaveChangesAsync(ct);

        // Stage 2: Append the Audit Event record now that session.SessionId is safely generated
        var auditEvent = new AuditEventEntity
        {
            UserId = user.Id,
            UserName = user.UserName,
            SessionId = session.SessionId,
            Action = "Login",
            Description = "User login",
            RemoteIp = ip,
            UserAgent = userAgent,
            Success = true
        };

        await _identityRepository.AddAuditEventAsync(auditEvent, ct);
        await _identityRepository.SaveChangesAsync(ct);

        // OPTIMIZATION: Pre-calculate allocation metrics capacity limit boundaries to speed up runtime execution loops
        int expectedClaimsCapacity = 4 + roles.Length + (permissionAccess.Count() * 2);

        var claims = new List<Claim>(expectedClaimsCapacity)
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName),
            new("display_name", user.DisplayName ?? user.UserName),
            new("session_id", session.SessionId.ToString())
        };

        foreach (var r in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, r));
        }

        foreach (var p in permissionAccess)
        {
            claims.Add(new Claim("permission", p.Code));
            // OPTIMIZATION: Avoid multi-string concatenation heap leaks by using string interpolation
            claims.Add(new Claim("permission_access", $"{p.Code}:{p.AccessType}"));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
        var token = new JwtSecurityToken(
            _jwtIssuer,
            _jwtAudience,
            claims,
            expires: expires,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        var permissionsDto = permissionAccess.Select(x => new PermissionAccessDto(x.Code, x.AccessType)).ToArray();

        return new LoginResponseDto(
            new JwtSecurityTokenHandler().WriteToken(token),
            expires,
            user.UserName,
            user.DisplayName ?? user.UserName,
            roles,
            permissionsDto,
            session.SessionId
        );
    }

    private static string DecryptAes256(string cipherText, string encryptionKey)
    {
        if (string.IsNullOrEmpty(cipherText))
            return string.Empty;

        // 1. .Trim() handles trailing spaces from fixed-length SQL columns (e.g. CHAR)
        byte[] allBytes = Convert.FromBase64String(cipherText.Trim());

        using var aes = Aes.Create();
        aes.Key = SHA256.HashData(Encoding.UTF8.GetBytes(encryptionKey));

        byte[] iv = new byte[16];
        if (allBytes.Length < iv.Length)
            throw new CryptographicException("Invalid ciphertext layout formatting length.");

        // 2. OPTIMIZATION: Uses direct system-level memory pointers (much faster than Array.Copy)
        Buffer.BlockCopy(allBytes, 0, iv, 0, iv.Length);
        aes.IV = iv;

        // 3. OPTIMIZATION: Uses modern, flat 'using' scopes to drastically cut down nested garbage collection allocations
        using var ms = new MemoryStream(allBytes, iv.Length, allBytes.Length - iv.Length);
        using var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
        using var sr = new StreamReader(cs, Encoding.UTF8);

        return sr.ReadToEnd();
    }


    //private static string DecryptAes256(string cipherText, string encryptionKey)
    //{
    //    if (string.IsNullOrEmpty(cipherText))
    //        return string.Empty;

    //    byte[] allBytes = Convert.FromBase64String(cipherText.Trim());
    //    using Aes aes = Aes.Create();
    //    aes.Key = SHA256.HashData(Encoding.UTF8.GetBytes(encryptionKey));

    //    byte[] iv = new byte[16];
    //    if (allBytes.Length < iv.Length)
    //        throw new CryptographicException("Invalid ciphertext layout formatting length.");

    //    Array.Copy(allBytes, 0, iv, 0, iv.Length);
    //    aes.IV = iv;

    //    using MemoryStream ms = new MemoryStream(allBytes, iv.Length, allBytes.Length - iv.Length);
    //    using ICryptoTransform decryptor = aes.CreateDecryptor();
    //    using CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
    //    using StreamReader sr = new StreamReader(cs, Encoding.UTF8);

    //    return sr.ReadToEnd();
    //}
}
