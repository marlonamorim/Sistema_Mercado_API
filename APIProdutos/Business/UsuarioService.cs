using System.Net.Http;
using APIProdutos.Helpers;
using APIProdutos.Models;

namespace APIProdutos.Business
{
    public class UsuarioService
    {
        private readonly IntegrationAPIRest _integrationAPIRest;

        public UsuarioService(IntegrationAPIRest integrationAPIRest) =>
            _integrationAPIRest = integrationAPIRest;

        public HttpResponseMessage Obter(Usuario usuario) =>
            _integrationAPIRest.ExecutePostRestUrl("login", null, usuario.Nome, usuario.ChaveAcesso);
    }
}