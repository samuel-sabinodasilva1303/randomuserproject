using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace RandomUserProject.Middleware
{
     /// <summary>
    /// Autor: Samuel Sabino - 30/09/2025
    /// Descrição: responsavel por tratar os responses de erro do HTTP.
    /// </summary>
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Ocorreu uma exceção: {Message}", exception.Message);

            var problemDetails = new ProblemDetails
            {
                Status = (int)HttpStatusCode.InternalServerError,
                Title = "Erro Interno do Servidor",
                Detail = exception.Message
            };

            switch (exception)
            {
                case ArgumentNullException:
                    problemDetails.Status = (int)HttpStatusCode.BadRequest;
                    problemDetails.Title = "Requisição Inválida";
                    break;

                case UnauthorizedAccessException:
                    problemDetails.Status = (int)HttpStatusCode.Unauthorized;
                    problemDetails.Title = "Não Autorizado";
                    break;

                case KeyNotFoundException:
                    problemDetails.Status = (int)HttpStatusCode.NotFound;
                    problemDetails.Title = "Recurso Não Encontrado";
                    break;

                case InvalidOperationException:
                    problemDetails.Status = (int)HttpStatusCode.BadRequest;
                    problemDetails.Title = "Operação Inválida";
                    break;
            }

            httpContext.Response.StatusCode = problemDetails.Status.Value;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}
