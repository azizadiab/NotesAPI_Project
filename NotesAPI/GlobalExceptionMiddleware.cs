using Microsoft.AspNetCore.Mvc.ApiExplorer;
using NotesAPI.Response;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NotesAPI
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

    public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task  InvokeAsync(HttpContext context)
        {

            try
            {
                await _next(context);

            }catch(Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var response = new ApiResponse<string>
                {
                    Success = false,
                    Message = "An unexpected error occurred",
                    Data = null

                };

                await context.Response.WriteAsJsonAsync(response);
            }
                                 
        }
      
    }
}
