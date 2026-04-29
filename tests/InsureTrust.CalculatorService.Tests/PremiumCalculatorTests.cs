using Moq;
using InsureTrust.CalculatorService.Services;
using InsureTrust.CalculatorService.DTOs;
using InsureTrust.CalculatorService.Options;
using Microsoft.Extensions.Options;
using Xunit;

namespace InsureTrust.CalculatorService.Tests
{
    public class PremiumCalculatorTests
    {
        private readonly Mock<IOptions<CalculationOptions>> _optionsMock;
        private readonly InsureTrust.CalculatorService.Services.CalculatorService _service;

        public PremiumCalculatorTests()
        {
            _optionsMock = new Mock<IOptions<CalculationOptions>>();
            _optionsMock.Setup(o => o.Value).Returns(new CalculationOptions 
            { 
                AnnualReturnRate = 0.1m,
                AgeFactor18To30 = 1.0m,
                AgeFactor31To45 = 1.2m,
                AgeFactor46To70 = 1.5m,
                PersonalCategoryFactor = 1.0m,
                BusinessCategoryFactor = 1.3m
            });

            _service = new InsureTrust.CalculatorService.Services.CalculatorService(_optionsMock.Object);
        }

        [Fact]
        public void Estimate_ValidInput_ReturnsPositiveAmount()
        {
            // Arrange
            var dto = new CalculatorRequestDto 
            { 
                PackageAmount = 1000,
                Age = 30,
                Tenure = 12,
                PolicyCategory = "Personal"
            };

            // Act
            var result = _service.Estimate(dto);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.EstimatedPremium > 0);
        }
    }
}
