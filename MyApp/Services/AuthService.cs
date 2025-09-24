using Microsoft.IdentityModel.Tokens;
using MyApp.DTOs.Auth;
using MyApp.Entities;
using MyApp.Repositories.Interfaces;
using MyApp.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MyApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IConfiguration _config;

        public AuthService(
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IConfiguration config)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _config = config;
        }

        public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
        {
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null) return null;

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = passwordHash,
                Role = "User",
                IsActive = true
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return await GenerateAuthResponse(user);
        }

        public async Task<AuthResponse?> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null) return null;

            if (!user.IsActive)
                return new AuthResponse
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role,
                    AccessToken = string.Empty,
                    RefreshToken = string.Empty,
                    ExpiresAt = DateTime.UtcNow
                };

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return null;

            return await GenerateAuthResponse(user);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email);
        }

        // Refresh token flow
        public async Task<RefreshResponse?> RefreshTokenAsync(RefreshRequest request)
        {
            var existing = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);
            if (existing == null || !existing.IsActive) return null;

            var user = existing.User;

            var newJwt = GenerateJwtToken(user);
            var newRefresh = GenerateRefreshToken(user.Id);

            // Revoke old refresh token
            existing.RevokedAt = DateTime.UtcNow;
            await _refreshTokenRepository.UpdateAsync(existing);

            // Save new refresh token
            await _refreshTokenRepository.AddAsync(newRefresh);
            await _refreshTokenRepository.SaveChangesAsync();

            return new RefreshResponse
            {
                AccessToken = newJwt,
                RefreshToken = newRefresh.Token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:ExpireMinutes"]!))
            };
        }

        public async Task RevokeRefreshTokenAsync(string refreshToken)
        {
            var existing = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
            if (existing != null && existing.IsActive)
            {
                existing.RevokedAt = DateTime.UtcNow;
                await _refreshTokenRepository.UpdateAsync(existing);
                await _refreshTokenRepository.SaveChangesAsync();
            }
        }

        // Generate AuthResponse (tokens only, controller sets cookies)
        private async Task<AuthResponse> GenerateAuthResponse(User user)
        {
            var jwt = GenerateJwtToken(user);
            var refresh = GenerateRefreshToken(user.Id);

            await _refreshTokenRepository.AddAsync(refresh);
            await _refreshTokenRepository.SaveChangesAsync();

            return new AuthResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                AccessToken = jwt,
                RefreshToken = refresh.Token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:ExpireMinutes"]!))
            };
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:ExpireMinutes"]!)),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Generate strong refresh token
        private RefreshToken GenerateRefreshToken(int userId)
        {
            int expireDays = int.TryParse(_config["Jwt:RefreshTokenExpireDays"], out var days)
                ? days
                : 7; // default 7 days

            var randomBytes = new byte[256]; // 256 bytes = 2048 bits
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            return new RefreshToken
            {
                UserId = userId,
                Token = Convert.ToBase64String(randomBytes),
                ExpiresAt = DateTime.UtcNow.AddDays(expireDays)
            };
        }
    }
}
