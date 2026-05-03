using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using InsureTrust.IdentityService.DTOs;
using InsureTrust.IdentityService.Models;
using InsureTrust.IdentityService.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace InsureTrust.IdentityService.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly PasswordHasher<User> _passwordHasher = new();

        public AuthService(IUserRepository userRepository, IConfiguration configuration, IMapper mapper)
        {
            ArgumentNullException.ThrowIfNull(userRepository);
            ArgumentNullException.ThrowIfNull(configuration);
            ArgumentNullException.ThrowIfNull(mapper);

            _userRepository = userRepository;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<UserDto> RegisterAsync(RegisterDto dto, string uploadPath)
        {
            ArgumentNullException.ThrowIfNull(dto);
            ArgumentException.ThrowIfNullOrWhiteSpace(uploadPath);

            var normalizedEmail = dto.Email.Trim().ToLowerInvariant();
            var normalizedPan = dto.PanCard.Trim().ToUpperInvariant();

            if (await _userRepository.ExistsByEmailAsync(normalizedEmail))
                throw new InvalidOperationException("Email already registered.");

            if (await _userRepository.ExistsByPanCardAsync(normalizedPan))
                throw new InvalidOperationException("PAN card already registered.");

            var user = new User
            {
                UserNumber = await GenerateUserNumberAsync(),
                Name = dto.Name.Trim(),
                Email = normalizedEmail,
                PhoneNo = dto.PhoneNo.Trim(),
                PanCard = normalizedPan,
                Role = "Customer",
                KycStatus = "Pending",
                Balance = 0m,
                CreatedAt = DateTime.UtcNow
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            if (dto.KycDocument is not null && dto.KycDocument.Length > 0)
            {
                user.KycDocumentPath = await SaveKycDocumentAsync(dto.KycDocument, uploadPath);
            }

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return _mapper.Map<UserDto>(user);
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var email = dto.Email.Trim().ToLowerInvariant();
            var user = await _userRepository.GetByEmailAsync(email);

            if (user is null)
                throw new UnauthorizedAccessException("Invalid email or password.");

            var verify = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (verify == PasswordVerificationResult.Failed)
                throw new UnauthorizedAccessException("Invalid email or password.");

            return new LoginResponseDto
            {
                Token = GenerateJwtToken(user),
                User = _mapper.Map<UserDto>(user)
            };
        }

        public async Task<LoginResponseDto> AdminLoginAsync(LoginDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var email = dto.Email.Trim().ToLowerInvariant();
            var user = await _userRepository.GetByEmailAsync(email);

            if (user is null || !string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase))
                throw new UnauthorizedAccessException("Invalid admin credentials.");

            var verify = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (verify == PasswordVerificationResult.Failed)
                throw new UnauthorizedAccessException("Invalid admin credentials.");

            return new LoginResponseDto
            {
                Token = GenerateJwtToken(user),
                User = _mapper.Map<UserDto>(user)
            };
        }

        public async Task<UserDto> GetProfileAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user is null)
                throw new KeyNotFoundException("User not found.");

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> UpdateProfileAsync(int userId, UpdateProfileDto dto, string uploadPath)
        {
            ArgumentNullException.ThrowIfNull(dto);
            ArgumentException.ThrowIfNullOrWhiteSpace(uploadPath);

            var user = await _userRepository.GetByIdAsync(userId);
            if (user is null)
                throw new KeyNotFoundException("User not found.");

            if (!string.IsNullOrWhiteSpace(dto.Name))
                user.Name = dto.Name.Trim();

            if (!string.IsNullOrWhiteSpace(dto.PhoneNo))
                user.PhoneNo = dto.PhoneNo.Trim();

            if (dto.KycDocument is not null && dto.KycDocument.Length > 0)
            {
                user.KycDocumentPath = await SaveKycDocumentAsync(dto.KycDocument, uploadPath);
                user.KycStatus = "Pending";
            }

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            return _mapper.Map<UserDto>(user);
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<bool> UpdateUserBalanceAsync(int userId, decimal amountToDeduct)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user is null)
                throw new KeyNotFoundException("User not found.");

            user.Balance += amountToDeduct; // Add balance for claim payout

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            return true;
        }

        private async Task<string> GenerateUserNumberAsync()
        {
            var users = await _userRepository.GetAllAsync();

            var maxNumber = users
                .Select(u => u.UserNumber)
                .Where(n => !string.IsNullOrWhiteSpace(n) && n.StartsWith("BKL", StringComparison.OrdinalIgnoreCase))
                .Select(n => int.TryParse(n[3..], out var value) ? value : 1000)
                .DefaultIfEmpty(1000)
                .Max();

            return $"BKL{maxNumber + 1}";
        }

        private static async Task<string> SaveKycDocumentAsync(IFormFile file, string uploadPath)
        {
            Directory.CreateDirectory(uploadPath);

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var fullPath = Path.Combine(uploadPath, fileName);

            await using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/kyc/{fileName}";
        }

        private string GenerateJwtToken(User user)
        {
            var key = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key missing.");
            var issuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer missing.");
            var audience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience missing.");
            var expiryMinutes = int.TryParse(_configuration["Jwt:ExpiryMinutes"], out var mins) ? mins : 60;

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Name),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, user.Role),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}