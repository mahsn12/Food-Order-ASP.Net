using FoodDelivery.Domain.Enums;

namespace FoodDelivery.Application.DTOs.Payments;

public record PaymentDto(int Id, int OrderId, PaymentMethod Method, PaymentStatus Status, string TransactionRef, DateTime? PaidAt);
public record CreatePaymentDto(int OrderId, PaymentMethod Method, PaymentStatus Status, string TransactionRef, DateTime? PaidAt);
public record UpdatePaymentDto(int Id, int OrderId, PaymentMethod Method, PaymentStatus Status, string TransactionRef, DateTime? PaidAt);
