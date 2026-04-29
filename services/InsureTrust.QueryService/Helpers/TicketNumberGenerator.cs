namespace InsureTrust.QueryService.Helpers
{
    public static class TicketNumberGenerator
    {
        public static string Generate(string? lastTicketNumber)
        {
            if (string.IsNullOrWhiteSpace(lastTicketNumber))
                return "SUP3001";

            var numericPart = lastTicketNumber.Replace("SUP", "");

            if (!int.TryParse(numericPart, out int number))
                return "SUP3001";

            return $"SUP{number + 1}";
        }
    }
}