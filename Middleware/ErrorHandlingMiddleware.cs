using System.Net;
using System.Text.Json;

namespace SmartSell.Api.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro não tratado: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new ApiErrorResponse();

            switch (exception)
            {
                case ValidationException validationEx:
                    response.Message = "Erro de validação";
                    response.Code = "VALIDATION_ERROR";
                    response.Details = validationEx.Errors;
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;

                case UnauthorizedAccessException:
                    response.Message = "Acesso não autorizado";
                    response.Code = "UNAUTHORIZED";
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    break;

                case NotFoundException notFoundEx:
                    response.Message = notFoundEx.Message;
                    response.Code = "NOT_FOUND";
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;

                case BusinessException businessEx:
                    response.Message = businessEx.Message;
                    response.Code = businessEx.Code;
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;

                case ArgumentException argEx:
                    response.Message = argEx.Message;
                    response.Code = "INVALID_ARGUMENT";
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;

                default:
                    response.Message = "Erro interno do servidor";
                    response.Code = "INTERNAL_ERROR";
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            var jsonResponse = JsonSerializer.Serialize(new { error = response }, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(jsonResponse);
        }
    }

    public class ApiErrorResponse
    {
        public string Message { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public object? Details { get; set; }
    }

    // Exceções customizadas
    public class ValidationException : Exception
    {
        public object Errors { get; }

        public ValidationException(object errors) : base("Erro de validação")
        {
            Errors = errors;
        }
    }

    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }

    public class BusinessException : Exception
    {
        public string Code { get; }

        public BusinessException(string message, string code = "BUSINESS_ERROR") : base(message)
        {
            Code = code;
        }
    }

    // Classe de resposta padronizada para sucesso
    public class ApiResponse<T>
    {
        public bool Success { get; set; } = true;
        public T? Data { get; set; }
        public string? Message { get; set; }
        public object? Meta { get; set; }

        public static ApiResponse<T> SuccessResult(T data, string? message = null)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Message = message
            };
        }

        public static ApiResponse<T> SuccessResult(T data, object meta, string? message = null)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Meta = meta,
                Message = message
            };
        }
    }

    // Classe para paginação
    public class PagedResult<T>
    {
        public IEnumerable<T> Data { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasNextPage => PageNumber < TotalPages;
        public bool HasPreviousPage => PageNumber > 1;
    }

    // Classe para filtros de paginação
    public class PaginationFilter
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public PaginationFilter()
        {
            PageNumber = PageNumber < 1 ? 1 : PageNumber;
            PageSize = PageSize > 100 ? 100 : PageSize < 1 ? 10 : PageSize;
        }

        public PaginationFilter(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber < 1 ? 1 : pageNumber;
            PageSize = pageSize > 100 ? 100 : pageSize < 1 ? 10 : pageSize;
        }
    }
}
