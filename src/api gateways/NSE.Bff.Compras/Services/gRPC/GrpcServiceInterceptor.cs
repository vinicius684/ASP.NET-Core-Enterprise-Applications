using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace NSE.Bff.Compras.Services.gRPC
{
    public class GrpcServiceInterceptor : Interceptor
    {
        private readonly ILogger<GrpcServiceInterceptor> _logger;
        private readonly IHttpContextAccessor _httpContextAccesor;//não posso injetar meu IAspNetUser aqui pq essa classe vai trabalhr singleton, ent vou ter que trabalhar com o "pai dele", httpContext

        public GrpcServiceInterceptor(
            ILogger<GrpcServiceInterceptor> logger,
            IHttpContextAccessor httpContextAccesor)
        {
            _logger = logger;
            _httpContextAccesor = httpContextAccesor;
        }

        //UnaryCall é a forma como vou chamar como vou chamar meu serviço lá do meu server. Funciona +- como se fosse um middleware
        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
            TRequest request, 
            ClientInterceptorContext<TRequest, TResponse> context,
            AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            var token = _httpContextAccesor.HttpContext.Request.Headers["Authorization"];//obtendo o Header Authorization

            var headers = new Metadata
            {
                {"Authorization", token}
            };

            var options = context.Options.WithHeaders(headers);//pegando esse contexto de request que interceptei e adicionando headers nele, dai me retorna uma option que vou usar na intercepção
            context = new ClientInterceptorContext<TRequest, TResponse>(context.Method, context.Host, options);//argumentos: request, server, meus headers novos

            return base.AsyncUnaryCall(request, context, continuation);

            /*
                No geral, interceptei o request, coloquei umas options com os headers já passando a autorização com o token e depois criei um contexto novo com os mesmo dados do antigo
                (só que passando essas opções pra ele colocar o meu header com a autenticação).
                -Interceptei -> coloquei um metadado de Header dentro da requisição -> agora pode continuar
             */
        }
    }
}