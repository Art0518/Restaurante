using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Ws_GIntegracionBus.Middlewares
{
    public class ErrorMiddleware : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                return await base.SendAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                var response = request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    mensaje = "Error interno del servidor.",
                    detalle = ex.Message
                });

                return response;
            }
        }
    }
}
