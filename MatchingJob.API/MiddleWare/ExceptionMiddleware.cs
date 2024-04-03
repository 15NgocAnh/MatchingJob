﻿using MatchingJob.API.Models;
using System.Net;

namespace MatchingJob.API.MiddleWare
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ExceptionMiddleware(RequestDelegate next, ILoggerFactory logger)
        {
            _logger = logger.CreateLogger<ExceptionMiddleware>();
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message}");

                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                await httpContext.Response.WriteAsync(new ErrorDetailsModel
                {
                    StatusCode = httpContext.Response.StatusCode,
                    Message = "Something went wrong. Please try again later."
                }.ToString());
            }
        }
    }
}