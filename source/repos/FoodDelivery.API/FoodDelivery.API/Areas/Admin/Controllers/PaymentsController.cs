using FoodDelivery.Application.DTOs.Payments;
using FoodDelivery.Domain.Entities;
using FoodDelivery.Domain.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodDelivery.API.Areas.Admin.Controllers;

[Area("Admin")]
[Route("api/[area]/[controller]")]
[ApiController]
    [Authorize(Roles = "Admin")]
public class PaymentsController(IRepository<Payment> paymentRepo) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PaymentDto>>> GetAll()
    {
        var payments = await paymentRepo.GetAsync();
        var response = payments.Select(p => new PaymentDto(p.Id, p.OrderId, p.Method, p.Status, p.TransactionRef, p.PaidAt));
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PaymentDto>> GetById(int id)
    {
        var payment = await paymentRepo.GetOneAsync(p => p.Id == id);
        if (payment is null) return NotFound();
        return Ok(new PaymentDto(payment.Id, payment.OrderId, payment.Method, payment.Status, payment.TransactionRef, payment.PaidAt));
    }

    [HttpPost]
    public async Task<ActionResult<PaymentDto>> Create([FromBody] CreatePaymentDto request)
    {
        var payment = new Payment
        {
            OrderId = request.OrderId,
            Method = request.Method,
            Status = request.Status,
            TransactionRef = request.TransactionRef,
            PaidAt = request.PaidAt
        };

        await paymentRepo.AddAsync(payment);
        await paymentRepo.CommitAsync();

        return CreatedAtAction(nameof(GetById), new { id = payment.Id }, new PaymentDto(payment.Id, payment.OrderId, payment.Method, payment.Status, payment.TransactionRef, payment.PaidAt));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePaymentDto request)
    {
        if (id != request.Id) return BadRequest("ID mismatch");

        var payment = await paymentRepo.GetOneAsync(p => p.Id == id);
        if (payment is null) return NotFound();

        payment.OrderId = request.OrderId;
        payment.Method = request.Method;
        payment.Status = request.Status;
        payment.TransactionRef = request.TransactionRef;
        payment.PaidAt = request.PaidAt;

        paymentRepo.Update(payment);
        await paymentRepo.CommitAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var payment = await paymentRepo.GetOneAsync(p => p.Id == id);
        if (payment is null) return NotFound();

        paymentRepo.Delete(payment);
        await paymentRepo.CommitAsync();
        return NoContent();
    }
}
