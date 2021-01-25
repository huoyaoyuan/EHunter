using System;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace EHunter.Pixiv.Services
{
    internal class DirectConnectHandler : HttpMessageHandler
    {
        private readonly HttpMessageInvoker _handler = new(new SocketsHttpHandler
        {
            ConnectCallback = async (context, ct) =>
            {
                string ip = context.DnsEndPoint.Host switch
                {
                    "oauth.secure.pixiv.net" => "210.140.131.199",
                    "app-api.pixiv.net" => "210.140.131.199",
                    "i.pximg.net" => "210.140.92.143",
                    _ => throw new InvalidOperationException("This handler only accepts Pixiv api hosts.")
                };

                var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                await socket.ConnectAsync(IPAddress.Parse(ip), 443, cancellationToken: ct)
                    .ConfigureAwait(false);
                var networkStream = new NetworkStream(socket, true);
                var sslStream = new SslStream(networkStream, false,
#pragma warning disable CA5359
                    // TODO: handle hard coded server fingerprint
                    (sender, cert, chain, error) => true);
#pragma warning restore CA5359

                await sslStream.AuthenticateAsClientAsync("").ConfigureAwait(false);
                return sslStream;
            }
        });

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _handler.Dispose();
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string? newUri = request.RequestUri!.ToString().Replace("https", "http", StringComparison.Ordinal);
            request.RequestUri = new(newUri);
            return _handler.SendAsync(request, cancellationToken);
        }
    }
}
