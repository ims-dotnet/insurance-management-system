using InsureTrust.ClaimService.Repositories;

namespace InsureTrust.ClaimService.Helpers
{
    public static class ClaimNumberGenerator
    {
        public static async Task<string> GenerateNextClaimNumberAsync(IClaimRepository repo)
        {
            var lastClaimNumber = await repo.GetLastClaimNumberAsync();
            if (string.IsNullOrEmpty(lastClaimNumber) || !lastClaimNumber.StartsWith("CLM"))
            {
                return "CLM4001";
            }

            string numberPart = lastClaimNumber.Substring(3);
            if (int.TryParse(numberPart, out int currentNumber))
            {
                return $"CLM{currentNumber + 1}";
            }

            return "CLM4001";
        }
    }
}
