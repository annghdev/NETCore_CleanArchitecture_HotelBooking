using FluentValidation;
using HotelBooking.Application.Common.Exceptions;
using HotelBooking.Domain.Exceptions;

namespace HotelBooking.API.Middlewares;

public class GlobalExceptionHandlingMiddleware(ILogger<GlobalExceptionHandlingMiddleware> logger) : IMiddleware
{
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
		try
		{
			await next(context);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, ex.Message);
			await HandleExceptionAsync(context, ex);
		}
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
		int statusCode = GetStatusCode(ex);
		var errors = GetErrors(ex);

		context.Response.StatusCode = statusCode;
		context.Response.ContentType = "application/json";
		await context.Response.WriteAsJsonAsync(errors);

    }
	private int GetStatusCode(Exception ex)
	{
		return ex switch
		{
			NotFoundException => StatusCodes.Status404NotFound,
			ValidationException => StatusCodes.Status400BadRequest,
			ArgumentException => StatusCodes.Status400BadRequest,
			InvalidOperationException => StatusCodes.Status400BadRequest,
			AccessDeniedException => StatusCodes.Status403Forbidden,
			_ => StatusCodes.Status500InternalServerError,
		};
	}
	private object GetErrors(Exception ex)
	{
		if(ex is ValidationException validationException)
		{
			return validationException.Errors.Select(e=>e.ErrorMessage);
		}
		return ex.Message;
	}
}
