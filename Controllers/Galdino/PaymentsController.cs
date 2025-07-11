using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartSell.Api.Data;
using SmartSell.Api.Models.Galdino;
using System.Text.Json;

namespace SmartSell.Api.Controllers.Galdino
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly GaldinoDbContext _context;

        public PaymentsController(GaldinoDbContext context)
        {
            _context = context;
        }

        // GET: api/payments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetPayments(
            [FromQuery] int? studentId = null,
            [FromQuery] string? status = null,
            [FromQuery] string? month = null)
        {
            var query = _context.Pagamentos.AsQueryable();

            if (studentId.HasValue)
                query = query.Where(p => p._studentId == studentId.Value);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(p => p._status == status);

            if (!string.IsNullOrEmpty(month))
                query = query.Where(p => p._month == month);

            var pagamentos = await query.ToListAsync();

            var result = pagamentos.Select(p => new
            {
                id = p._id,
                studentId = p._studentId,
                amount = p._amount,
                month = p._month,
                year = p._year,
                status = p._status,
                paymentMethod = p._paymentMethod,
                paymentDate = p._paymentDate?.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                dueDate = p._dueDate.ToString("yyyy-MM-ddTHH:mm:ssZ")
            });

            return Ok(result);
        }

        // GET: api/payments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetPayment(int id)
        {
            var pagamento = await _context.Pagamentos.FindAsync(id);

            if (pagamento == null)
            {
                return NotFound(new { error = new { message = "Pagamento não encontrado", code = "PAYMENT_NOT_FOUND" } });
            }

            var result = new
            {
                id = pagamento._id,
                studentId = pagamento._studentId,
                amount = pagamento._amount,
                month = pagamento._month,
                year = pagamento._year,
                status = pagamento._status,
                paymentMethod = pagamento._paymentMethod,
                paymentDate = pagamento._paymentDate?.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                dueDate = pagamento._dueDate.ToString("yyyy-MM-ddTHH:mm:ssZ")
            };

            return Ok(result);
        }

        // POST: api/payments
        [HttpPost]
        public async Task<ActionResult<object>> CreatePayment([FromBody] JsonElement body)
        {
            try
            {
                var pagamento = new Pagamento
                {
                    _studentId = body.GetProperty("studentId").GetInt32(),
                    _amount = body.GetProperty("amount").GetDecimal(),
                    _month = body.GetProperty("month").GetString() ?? "",
                    _year = body.GetProperty("year").GetInt32(),
                    _status = body.GetProperty("status").GetString() ?? "pending",
                    _dueDate = DateTime.Parse(body.GetProperty("dueDate").GetString() ?? DateTime.Now.AddDays(30).ToString()),
                    _createdAt = DateTime.Now
                };

                if (body.TryGetProperty("paymentMethod", out var paymentMethodElement))
                    pagamento._paymentMethod = paymentMethodElement.GetString();

                if (body.TryGetProperty("paymentDate", out var paymentDateElement))
                    pagamento._paymentDate = DateTime.Parse(paymentDateElement.GetString() ?? "");

                _context.Pagamentos.Add(pagamento);
                await _context.SaveChangesAsync();

                var result = new
                {
                    id = pagamento._id,
                    studentId = pagamento._studentId,
                    amount = pagamento._amount,
                    month = pagamento._month,
                    year = pagamento._year,
                    status = pagamento._status,
                    paymentMethod = pagamento._paymentMethod,
                    paymentDate = pagamento._paymentDate?.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    dueDate = pagamento._dueDate.ToString("yyyy-MM-ddTHH:mm:ssZ")
                };

                return CreatedAtAction(nameof(GetPayment), new { id = pagamento._id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = new { message = "Dados inválidos", code = "INVALID_DATA", details = ex.Message } });
            }
        }

        // PUT: api/payments/5
        [HttpPut("{id}")]
        public async Task<ActionResult<object>> UpdatePayment(int id, [FromBody] JsonElement body)
        {
            var pagamento = await _context.Pagamentos.FindAsync(id);
            if (pagamento == null)
            {
                return NotFound(new { error = new { message = "Pagamento não encontrado", code = "PAYMENT_NOT_FOUND" } });
            }

            try
            {
                if (body.TryGetProperty("amount", out var amountElement))
                    pagamento._amount = amountElement.GetDecimal();

                if (body.TryGetProperty("status", out var statusElement))
                    pagamento._status = statusElement.GetString() ?? pagamento._status;

                if (body.TryGetProperty("paymentMethod", out var paymentMethodElement))
                    pagamento._paymentMethod = paymentMethodElement.GetString();

                if (body.TryGetProperty("paymentDate", out var paymentDateElement))
                    pagamento._paymentDate = DateTime.Parse(paymentDateElement.GetString() ?? "");

                if (body.TryGetProperty("dueDate", out var dueDateElement))
                    pagamento._dueDate = DateTime.Parse(dueDateElement.GetString() ?? pagamento._dueDate.ToString());

                await _context.SaveChangesAsync();

                var result = new
                {
                    id = pagamento._id,
                    studentId = pagamento._studentId,
                    amount = pagamento._amount,
                    month = pagamento._month,
                    year = pagamento._year,
                    status = pagamento._status,
                    paymentMethod = pagamento._paymentMethod,
                    paymentDate = pagamento._paymentDate?.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    dueDate = pagamento._dueDate.ToString("yyyy-MM-ddTHH:mm:ssZ")
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = new { message = "Dados inválidos", code = "INVALID_DATA", details = ex.Message } });
            }
        }

        // PATCH: api/payments/5/mark-paid
        [HttpPatch("{id}/mark-paid")]
        public async Task<ActionResult<object>> MarkPaymentAsPaid(int id)
        {
            var pagamento = await _context.Pagamentos.FindAsync(id);
            if (pagamento == null)
            {
                return NotFound(new { error = new { message = "Pagamento não encontrado", code = "PAYMENT_NOT_FOUND" } });
            }

            pagamento._status = "paid";
            pagamento._paymentDate = DateTime.Now;
            pagamento._paymentMethod = pagamento._paymentMethod ?? "PIX";

            await _context.SaveChangesAsync();

            var result = new
            {
                id = pagamento._id,
                studentId = pagamento._studentId,
                amount = pagamento._amount,
                month = pagamento._month,
                year = pagamento._year,
                status = pagamento._status,
                paymentMethod = pagamento._paymentMethod,
                paymentDate = pagamento._paymentDate?.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                dueDate = pagamento._dueDate.ToString("yyyy-MM-ddTHH:mm:ssZ")
            };

            return Ok(result);
        }

        // DELETE: api/payments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var pagamento = await _context.Pagamentos.FindAsync(id);
            if (pagamento == null)
            {
                return NotFound(new { error = new { message = "Pagamento não encontrado", code = "PAYMENT_NOT_FOUND" } });
            }

            _context.Pagamentos.Remove(pagamento);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
