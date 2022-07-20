using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace CatalogService.UnitTests;
public class ExceptionMiddleware
{
  private readonly RequestDelegate _next;
  private readonly IWebHostEnvironment _webHostEnvironment;

  public ExceptionMiddleware(RequestDelegate next, IWebHostEnvironment webHostEnvironment)
  {
    _next = next;
    _webHostEnvironment = webHostEnvironment;
  }

  public async Task Invoke(HttpContext context)
  {
    try
    {
      await _next(context);
    }
    catch (Exception e)
    {
      if (context.Response.HasStarted)
      {
        throw;
      }

      int statusCode = 500;

      context.Response.Clear();
      context.Response.StatusCode = statusCode;
      context.Response.ContentType = "application/json";

      var response = new
      {
        Message = e.Message,
        Exception = SerializeException(e)
      };

      var body = JsonConvert.SerializeObject(response);
      await context.Response.WriteAsync(body);
    }
  }

  private string? SerializeException(Exception e)
  {
    if (_webHostEnvironment.IsProduction())
    {
      // we do not include the Exception in production to avoid leaking sensitive information
      return null;
    }
    return e.ToString();
  }
}
