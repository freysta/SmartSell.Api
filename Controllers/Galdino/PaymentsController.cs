using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartSell.Api.DAO;
using SmartSell.Api.Data;
using SmartSell.Api.Models.Galdino;
using System.Text.Json;

namespace SmartSell.Api.Controllers.Galdino
{
    [ApiController]
    [Route("api/payments")]
    public class PaymentsController : ControllerBase
    {
        private readonly PagamentoDAO _pagamentoDAO;
        private readonly AlunoDAO _alunoDAO;

        public PaymentsController(GaldinoDbContext context)
        {
            _pagamentoDAO = new PagamentoDAO(context);
            _alunoDAO = new AlunoDAO(context);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetPayments(
            [FromQuery] int? studentId = null,
            [FromQuery] string? status = null,
            [FromQuery] string? month = null)
        {
            var pagamentos = _pagamentoDAO.GetAll();

            if (studentId.HasValue)
                pagamentos = pagamentos.Where(p => p._alunoId == studentId.Value).ToList();

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<StatusPagamentoEnum>(status, out var statusEnum))
                pagamentos = pagamentos.Where(p => p._status == statusEnum).ToList();

            if (!string.IsNullOrEmpty(month))
                pagamentos = pagamentos.Where(p => p._referenciaMes == month).ToList();

            var result = pagamentos.Select(p => new
            {
                id = p._id,
                studentId = p._alunoId,
                studentName = "N/A",
                amount = p._valor,
                month = p._referenciaMes,
                year = DateTime.Now.Year,
                status = p._status,
                paymentMethod = p._formaPagamento,
                paymentDate = p._dataPagamento.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                dueDate = p._dataPagamento.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                createdAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetPayment(int id)
        {
            var pagamento = _pagamentoDAO.GetById(id);

            if (pagamento == null)
            {
                return NotFound(new { error = new { message = "Pagamento não encontrado", code = "PAYMENT_NOT_FOUND" } });
            }

            var result = new
            {
                id = pagamento._id,
                studentId = pagamento._alunoId,
                studentName = "N/A",
                amount = pagamento._valor,
                month = pagamento._referenciaMes,
                year = DateTime.Now.Year,
                status = pagamento._status,
                paymentMethod = pagamento._formaPagamento,
                paymentDate = pagamento._dataPagamento.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                dueDate = pagamento._dataPagamento.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                createdAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
            };

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<object>> CreatePayment([FromBody] JsonElement body)
        {
            try
            {
                var studentId = body.GetProperty("studentId").GetInt32();
                var amount = body.GetProperty("amount").GetDecimal();
                var month = body.GetProperty("month").GetString() ?? "";
                var status = body.GetProperty("status").GetString() ?? "Pendente";

                var aluno = _alunoDAO.GetById(studentId);
                if (aluno == null)
                {
                    return BadRequest(new { error = new { message = "Aluno não encontrado", code = "STUDENT_NOT_FOUND" } });
                }

                var pagamento = new Pagamento
                {
                    _alunoId = studentId,
                    _valor = amount,
                    _referenciaMes = month,
                    _status = Enum.TryParse<StatusPagamentoEnum>(status, out var statusEnum) ? statusEnum : StatusPagamentoEnum.Pendente,
                    _dataPagamento = DateTime.Now
                };

                if (body.TryGetProperty("paymentMethod", out var paymentMethodElement))
                {
                    var paymentMethodStr = paymentMethodElement.GetString();
                    if (Enum.TryParse<FormaPagamentoEnum>(paymentMethodStr, out var paymentMethodEnum))
                        pagamento._formaPagamento = paymentMethodEnum;
                }

                if (body.TryGetProperty("paymentDate", out var paymentDateElement))
                    pagamento._dataPagamento = paymentDateElement.GetDateTime();

                _pagamentoDAO.Create(pagamento);

                var result = new
                {
                    id = pagamento._id,
                    studentId = pagamento._alunoId,
                    studentName = "N/A",
                    amount = pagamento._valor,
                    month = pagamento._referenciaMes,
                    year = DateTime.Now.Year,
                    status = pagamento._status,
                    paymentMethod = pagamento._formaPagamento,
                    paymentDate = pagamento._dataPagamento.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    dueDate = pagamento._dataPagamento.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    createdAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
                };

                return CreatedAtAction(nameof(GetPayment), new { id = pagamento._id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = new { message = "Dados inválidos", code = "INVALID_DATA", details = ex.Message } });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<object>> UpdatePayment(int id, [FromBody] JsonElement body)
        {
            var pagamento = _pagamentoDAO.GetById(id);
            if (pagamento == null)
            {
                return NotFound(new { error = new { message = "Pagamento não encontrado", code = "PAYMENT_NOT_FOUND" } });
            }

            try
            {
                if (body.TryGetProperty("amount", out var amountElement))
                    pagamento._valor = amountElement.GetDecimal();

                if (body.TryGetProperty("status", out var statusElement))
                {
                    var statusStr = statusElement.GetString();
                    if (Enum.TryParse<StatusPagamentoEnum>(statusStr, out var statusEnum))
                        pagamento._status = statusEnum;
                }

                if (body.TryGetProperty("paymentMethod", out var paymentMethodElement))
                {
                    var paymentMethodStr = paymentMethodElement.GetString();
                    if (Enum.TryParse<FormaPagamentoEnum>(paymentMethodStr, out var paymentMethodEnum))
                        pagamento._formaPagamento = paymentMethodEnum;
                }

                if (body.TryGetProperty("paymentDate", out var paymentDateElement))
                    pagamento._dataPagamento = paymentDateElement.GetDateTime();

                _pagamentoDAO.Update(pagamento);

                var result = new
                {
                    id = pagamento._id,
                    studentId = pagamento._alunoId,
                    studentName = "N/A",
                    amount = pagamento._valor,
                    month = pagamento._referenciaMes,
                    year = DateTime.Now.Year,
                    status = pagamento._status,
                    paymentMethod = pagamento._formaPagamento,
                    paymentDate = pagamento._dataPagamento.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    dueDate = pagamento._dataPagamento.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    createdAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = new { message = "Dados inválidos", code = "INVALID_DATA", details = ex.Message } });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var pagamento = _pagamentoDAO.GetById(id);
            if (pagamento == null)
            {
                return NotFound(new { error = new { message = "Pagamento não encontrado", code = "PAYMENT_NOT_FOUND" } });
            }

            _pagamentoDAO.Delete(id);

            return NoContent();
        }

        [HttpPost("{id}/confirm")]
        public async Task<ActionResult<object>> ConfirmPayment(int id, [FromBody] JsonElement body)
        {
            var pagamento = _pagamentoDAO.GetById(id);
            if (pagamento == null)
            {
                return NotFound(new { error = new { message = "Pagamento não encontrado", code = "PAYMENT_NOT_FOUND" } });
            }

            try
            {
                pagamento._dataPagamento = DateTime.Now;
                if (body.TryGetProperty("paymentMethod", out var paymentMethodElement))
                {
                    var paymentMethodStr = paymentMethodElement.GetString();
                    if (Enum.TryParse<FormaPagamentoEnum>(paymentMethodStr, out var paymentMethodEnum))
                        pagamento._formaPagamento = paymentMethodEnum;
                }

                _pagamentoDAO.Update(pagamento);

                var result = new
                {
                    id = pagamento._id,
                    studentId = pagamento._alunoId,
                    studentName = "N/A",
                    amount = pagamento._valor,
                    month = pagamento._referenciaMes,
                    year = DateTime.Now.Year,
                    status = pagamento._status,
                    paymentMethod = pagamento._formaPagamento,
                    paymentDate = pagamento._dataPagamento.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    dueDate = pagamento._dataPagamento.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    createdAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = new { message = "Erro ao confirmar pagamento", code = "CONFIRMATION_ERROR", details = ex.Message } });
            }
        }
    }
}
