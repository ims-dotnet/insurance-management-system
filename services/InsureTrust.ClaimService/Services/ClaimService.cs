using AutoMapper;
using InsureTrust.ClaimService.DTOs;
using InsureTrust.ClaimService.Helpers;
using InsureTrust.ClaimService.Models;
using InsureTrust.ClaimService.Repositories;
using System.Text.Json;

namespace InsureTrust.ClaimService.Services
{
    public class ClaimService : IClaimService
    {
        private readonly IClaimRepository _repo;
        private readonly IMapper _mapper;
        private readonly ILogger<ClaimService> _logger;

        public ClaimService(IClaimRepository repo, IMapper mapper, ILogger<ClaimService> logger)
        {
            _repo = repo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ClaimDto>> GetMyClaimsAsync(int userId)
        {
            var claims = await _repo.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<ClaimDto>>(claims);
        }

        public async Task<IEnumerable<ClaimDto>> GetAllClaimsAsync()
        {
            var claims = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<ClaimDto>>(claims);
        }

        public async Task<ClaimDto> SubmitClaimAsync(int policyId, SubmitClaimDto dto, int userId, string uploadPath)
        {
            var docPaths = new List<string>();
            if (dto.Documents != null && dto.Documents.Any())
            {
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                foreach (var file in dto.Documents)
                {
                    var ext = Path.GetExtension(file.FileName).ToLower();
                    var fileName = Guid.NewGuid().ToString() + ext;
                    var filePath = Path.Combine(uploadPath, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    docPaths.Add($"/uploads/claims/{fileName}");
                }
            }

            var claim = new Claim
            {
                ClaimNumber = await ClaimNumberGenerator.GenerateNextClaimNumberAsync(_repo),
                UserId = userId,
                UserPolicyId = policyId,
                Description = dto.Description,
                MaturityAmount = dto.MaturityAmount,
                ClaimStatus = "Pending",
                SubmittedAt = DateTime.UtcNow,
                DocumentPathsJson = JsonSerializer.Serialize(docPaths)
            };

            await _repo.AddAsync(claim);
            await _repo.SaveChangesAsync();

            _logger.LogInformation("New claim submitted: {ClaimNumber} for Policy {PolicyId} by User {UserId}", claim.ClaimNumber, policyId, userId);

            return _mapper.Map<ClaimDto>(claim);
        }

        public async Task<ClaimDto> UpdateClaimAsync(int claimId, UpdateClaimDto dto)
        {
            var claim = await _repo.GetByIdAsync(claimId);
            if (claim == null) throw new KeyNotFoundException("Claim not found");

            claim.ClaimStatus = dto.Action; // "Approved" or "Denied"
            claim.AdminRemarks = dto.AdminRemarks;
            claim.ProcessedAt = DateTime.UtcNow;

            await _repo.SaveChangesAsync();

            _logger.LogWarning("Claim {ClaimId} status updated to {Status} by Admin. Remarks: {Remarks}", claimId, dto.Action, dto.AdminRemarks);
            
            return _mapper.Map<ClaimDto>(claim);
        }
    }
}