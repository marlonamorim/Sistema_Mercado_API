using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.IdentityModel.Tokens;
using APIProdutos.Business;
using APIProdutos.Models;
using Newtonsoft.Json.Linq;
using System.Linq;
using Microsoft.AspNetCore.Cors;

namespace APIProdutos.Controllers
{
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        [AllowAnonymous]
        [HttpPost]
        [EnableCors("POLICECORSORIGIN")]
        public object Post(
            [FromBody] Usuario usuario,
            [FromServices]UsuarioService usrService,
            [FromServices]SigningConfigurations signingConfigurations,
            [FromServices]TokenConfigurations tokenConfigurations)
        {
            bool credenciaisValidas = false;
            if (usuario != null && !String.IsNullOrWhiteSpace(usuario.Nome))
            {
                //var usuarioBase = usrService.Obter(usuario.Nome);
                //credenciaisValidas = (usuarioBase != null &&
                //    usuario.Nome == usuarioBase.Nome &&
                //    usuario.ChaveAcesso == usuarioBase.ChaveAcesso);

                var result = usrService.Obter(usuario);

                var payload = JObject.Parse(result.Content.ReadAsStringAsync().Result);
                credenciaisValidas = payload.SelectTokens("success").FirstOrDefault() != null &&
                    Convert.ToBoolean(payload.SelectTokens("success").FirstOrDefault().ToString());

            }

            if (credenciaisValidas) {
                ClaimsIdentity identity = new ClaimsIdentity(
                    new GenericIdentity(usuario.Nome, "Login"),
                    new[] {
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                        new Claim(JwtRegisteredClaimNames.UniqueName, usuario.Nome)
                    }
                );

                DateTime dataCriacao = DateTime.Now;
                DateTime dataExpiracao = dataCriacao +
                    TimeSpan.FromSeconds(tokenConfigurations.Seconds);

                var handler = new JwtSecurityTokenHandler();
                var securityToken = handler.CreateToken(new SecurityTokenDescriptor {
                    Issuer = tokenConfigurations.Issuer,
                    Audience = tokenConfigurations.Audience,
                    SigningCredentials = signingConfigurations.SigningCredentials,
                    Subject = identity,
                    NotBefore = dataCriacao,
                    Expires = dataExpiracao
                });
                var token = handler.WriteToken(securityToken);

                return new {
                    created = dataCriacao.ToString("yyyy-MM-dd HH:mm:ss"),
                    expiration = dataExpiracao.ToString("yyyy-MM-dd HH:mm:ss"),
                    accessToken = token,
                    success = true
                };
            }
            else {
                return new {
                    success = false,
                    error = "username or password incorrect"
                };
            }
        }
    }
}