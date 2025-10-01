using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace LumenApi.Web.Middlewares;

public class JwtMiddleware(RequestDelegate next)
{
  private readonly RequestDelegate _next = next;

  public async Task Invoke(HttpContext context)
  {
    var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split().Last();
    
    if (token != null) {
      AttachUserToContext(context, token);
    }
    await _next(context);
  } 
  
  private void AttachUserToContext(HttpContext context,string? token) {
    try
    {
      var tokenHandler = new JwtSecurityTokenHandler();
      var key = Encoding.ASCII.GetBytes("This_is_Lumen_Seceret_Key_For_Jwt");
      tokenHandler.ValidateToken(token,new TokenValidationParameters
      {
        ValidateIssuerSigningKey=true,
        IssuerSigningKey=new SymmetricSecurityKey(key),
        ValidateIssuer=false,
        ValidateAudience=false,
        ClockSkew=TimeSpan.Zero

      },out SecurityToken validatedToken);
      JwtSecurityToken jwtToken = (JwtSecurityToken)validatedToken;
      var user = jwtToken.Claims;//.First(x => x.Type == "UserId");
      var userIdClaim = user.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
      context.Items["UserId"] = userIdClaim?.Value; // Store UserId in context
      context.Items["User"] = user;
    }
    catch (Exception)
    {

    }
  }
}
public static  class JwtTokenExtensions
{
  public static IApplicationBuilder UseJwtTokenMiddleware(this IApplicationBuilder builder)
  {
   return builder.UseMiddleware<JwtMiddleware>();
  }
}
