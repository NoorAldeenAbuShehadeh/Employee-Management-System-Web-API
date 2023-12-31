using Employee_Management_System.Model;

namespace Employee_Managment_System_web_API.Middleware
{
    public class UserMiddleware
    {
        private readonly RequestDelegate _next;

        public UserMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var user = context.Items["CurrentUser"] as User;

            if (user != null)
            {
                context.Items["User"] = user;
            }

            await _next(context);
        }
    }
}
