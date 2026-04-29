using AutoMapper;
using InsureTrust.PaymentService.DTOs;
using InsureTrust.PaymentService.Exceptions;
using InsureTrust.PaymentService.Models;
using InsureTrust.PaymentService.Repositories;
using InsureTrust.PaymentService.Helpers;

namespace InsureTrust.PaymentService.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _repository;
        private readonly IPaymentGateway _gateway;
        private readonly IProductServiceClient _productClient;
        private readonly IMapper _mapper;

        public PaymentService(
            IPaymentRepository repository,
            IPaymentGateway gateway,
            IProductServiceClient productClient,
            IMapper mapper)
        {
            _repository = repository;
            _gateway = gateway;
            _productClient = productClient;
            _mapper = mapper;
        }

        public async Task<InitiatePaymentResponseDto> InitiateFirstPaymentAsync(
            InitiateFirstPaymentDto dto,
            int userId)
        {
            ArgumentNullException.ThrowIfNull(dto);

            // Fetch real amount from Product Service
            var initialPolicy = await _productClient.GetPolicyByPolicyIdAsync(dto.PolicyId);
            if (initialPolicy == null)
                throw new NotFoundException("Policy product not found.");

            decimal amount = initialPolicy.PackageAmount;

            var gatewayResult = await _gateway.ProcessAsync(amount, dto.PaymentMethod);

            int? generatedUserPolicyId = null;
            if (gatewayResult.Success)
            {
                // Register the policy in ProductService and get real ID/Number
                var productResponse = await _productClient.RegisterNewPolicyAsync(userId, dto.PolicyId, amount);
                generatedUserPolicyId = productResponse?.UserPolicyId;
            }

            var payment = new Payment
            {
                UserId = userId,
                PolicyId = dto.PolicyId,
                UserPolicyId = generatedUserPolicyId,
                Amount = amount,
                PaymentMethod = dto.PaymentMethod,
                PaymentNumber = PaymentHelper.GeneratePaymentNumber(),
                TransactionId = gatewayResult.TransactionId ?? string.Empty,
                Status = gatewayResult.Success ? "Success" : "Failed",
                PaymentDate = DateTime.UtcNow,
                Remarks = gatewayResult.Success
                    ? "First Payment Successful - Pending Admin Approval"
                    : "Payment Failed"
            };

            await _repository.AddAsync(payment);
            await _repository.SaveChangesAsync();

            var response = _mapper.Map<InitiatePaymentResponseDto>(payment);
            response.GeneratedUserPolicyId = payment.UserPolicyId;

            return response;
        }

        public async Task<InitiatePaymentResponseDto> InitiateRenewalPaymentAsync(
            InitiateRenewalPaymentDto dto,
            int userId)
        {
            var policy = await _productClient.GetPolicyByNumberAsync(dto.PolicyNumber);

            if (policy == null)
                throw new NotFoundException("Policy not found for renewal.");

            // Use the real amount from Product Service instead of hardcoded value
            decimal baseAmount = policy.PackageAmount;
            decimal finalAmount = CalculateRenewalAmount(policy, baseAmount);

            var gatewayResult = await _gateway.ProcessAsync(finalAmount, dto.PaymentMethod);

            var payment = new Payment
            {
                UserId = userId,
                PolicyId = policy.PolicyId,
                UserPolicyId = policy.UserPolicyId, // Saving the real ID from lookup
                Amount = finalAmount,
                PaymentMethod = dto.PaymentMethod,
                PaymentNumber = PaymentHelper.GeneratePaymentNumber(),
                TransactionId = gatewayResult.TransactionId ?? string.Empty,
                Status = gatewayResult.Success ? "Success" : "Failed",
                PaymentDate = DateTime.UtcNow,
                Remarks = gatewayResult.Success
                    ? "Renewal Payment Successful"
                    : "Payment Failed"
            };

            if (payment.Status == "Success")
            {
                try
                {
                    var isRenewed = await _productClient.RenewPolicyByNumberAsync(dto.PolicyNumber);

                    payment.Remarks = isRenewed
                        ? "Policy Renewed Successfully"
                        : "Payment Success but Renewal Failed";
                }
                catch
                {
                    payment.Remarks = "Payment Success but Renewal Service Error";
                }
            }

            await _repository.AddAsync(payment);
            await _repository.SaveChangesAsync();

            return _mapper.Map<InitiatePaymentResponseDto>(payment);
        }

        //public async Task<IEnumerable<PaymentDto>> GetHistoryAsync(int userId)
        //{
        //    var payments = await _repository.GetByUserIdAsync(userId);
        //    return _mapper.Map<List<PaymentDto>>(payments);
        //}
        public async Task<IEnumerable<PaymentDto>> GetHistoryAsync(int userId)
        {
            var payments = await _repository.GetByUserIdAsync(userId);
            var result = new List<PaymentDto>();

            foreach (var x in payments)
            {
                var dto = _mapper.Map<PaymentDto>(x);

                // 🔹 Mock for now (Swagger testing)
                dto.PolicyNumber = $"POL-{x.UserPolicyId ?? x.PolicyId}";

                // 🔹 Real integration later
                /*
                if (x.UserPolicyId.HasValue && x.UserPolicyId.Value > 0)
                {
                    var policy = await _productClient.GetPolicyAsync(x.UserPolicyId.Value);
                    dto.PolicyNumber = policy?.PolicyNumber ?? string.Empty;
                }
                else if (x.PolicyId.HasValue && x.PolicyId.Value > 0)
                {
                    var policy = await _productClient.GetPolicyByPolicyIdAsync(x.PolicyId.Value);
                    dto.PolicyNumber = policy?.PolicyNumber ?? string.Empty;
                }
                */

                result.Add(dto);
            }

            return result;
        }

        public async Task<IEnumerable<PaymentDto>> GetAllPaymentsAsync()
        {
            var payments = await _repository.GetAllAsync();
            var result = new List<PaymentDto>();

            foreach (var x in payments)
            {
                var dto = _mapper.Map<PaymentDto>(x);
                dto.PolicyNumber = $"POL-{x.UserPolicyId ?? x.PolicyId}";
                result.Add(dto);
            }

            return result;
        }

        public async Task<bool> ApprovePaymentAsync(int paymentId)
        {
            var payment = await _repository.GetByIdAsync(paymentId);
            if (payment == null) 
                throw new NotFoundException("Payment record not found.");

            // Replace "Pending Admin Approval" tag with "Admin Approved" stamp
            if (!string.IsNullOrWhiteSpace(payment.Remarks))
            {
                payment.Remarks = payment.Remarks
                    .Replace("Pending Admin Approval", "Admin Approved",
                             StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                payment.Remarks = "Admin Approved";
            }

            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectPaymentAsync(int paymentId, string reason)
        {
            var payment = await _repository.GetByIdAsync(paymentId);
            if (payment == null) 
                throw new NotFoundException("Payment record not found.");

            // Mark as rejected and include reason
            string rejectionTag = $"Admin Rejected: {reason}";
            
            if (!string.IsNullOrWhiteSpace(payment.Remarks))
            {
                payment.Remarks = payment.Remarks
                    .Replace("Pending Admin Approval", rejectionTag,
                             StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                payment.Remarks = rejectionTag;
            }

            await _repository.SaveChangesAsync();
            return true;
        }

        private static decimal CalculateRenewalAmount(ProductPolicyDto policy, decimal baseAmount)
        {
            if (DateTime.UtcNow <= policy.ExpiryDate)
                return baseAmount;

            decimal penaltyPercent = policy.PolicyType switch
            {
                "Health" => 0.05m,
                "TermLife" => 0.08m,
                "Vehicle" => 0.08m,
                "Property" => 0.12m,
                "Home" => 0.12m,
                _ => 0m
            };

            return baseAmount + (baseAmount * penaltyPercent);
        }

        public async Task<ProductPolicyDto?> GetPolicyDetailsByNumberAsync(string policyNumber)
        {
            return await _productClient.GetPolicyByNumberAsync(policyNumber);
        }

        public async Task<ProductPolicyDto?> GetPolicyDetailsByIdAsync(int policyId)
        {
            return await _productClient.GetPolicyByPolicyIdAsync(policyId);
        }
    }
}