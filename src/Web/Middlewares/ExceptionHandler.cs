using ApplicationCore.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Web.Middlewares
{
	public class ErrorDetails
	{
		public int StatusCode { get; set; }
		public string Message { get; set; }


		public override string ToString()
		{
			return JsonConvert.SerializeObject(this);
		}
	}

	public class ExceptionMiddleware
	{
		private readonly RequestDelegate next;
		private readonly ILogger logger;

		public ExceptionMiddleware(RequestDelegate next, ILogger logger)
		{
			this.next = next;
			this.logger = logger;
		}

		public async Task InvokeAsync(HttpContext httpContext)
		{
			try
			{
				await next(httpContext);
			}
			catch (Exception ex)
			{
				logger.LogError($"錯誤: {ex}");
				await HandleExceptionAsync(httpContext, ex);
			}
		}

		private static Task HandleExceptionAsync(HttpContext context, Exception exception)
		{
			context.Response.ContentType = "application/json";
			context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

			return context.Response.WriteAsync(new ErrorDetails()
			{
				StatusCode = context.Response.StatusCode,
				Message = "伺服器暫時無回應. 請稍候再試"
			}.ToString());
		}

	}
}
