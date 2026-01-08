using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Ws_GIntegracionBus.Middlewares
{
    
    public class AuthMiddleware : DelegatingHandler
    {
        /*
            private const string TOKEN_VALIDO = "123456";

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (request.Headers.Contains("Authorization"))
                {
                    var token = request.Headers.GetValues("Authorization").FirstOrDefault();
                    if (token == TOKEN_VALIDO)
                        return await base.SendAsync(request, cancellationToken);
                }

                return request.CreateResponse(HttpStatusCode.Unauthorized, new
                {
                    mensaje = "Acceso no autorizado. Debe incluir un token válido."
                });
            }*/
    }
}
