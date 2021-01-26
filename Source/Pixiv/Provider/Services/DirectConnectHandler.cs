﻿using System;
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

                var socket = new Socket(SocketType.Stream, ProtocolType.Tcp)
                {
                    NoDelay = true
                };

                try
                {
                    await socket.ConnectAsync(IPAddress.Parse(ip), 443, cancellationToken: ct)
                        .ConfigureAwait(false);
                    var networkStream = new NetworkStream(socket, true);
                    var sslStream = new SslStream(networkStream, false,
                        (sender, cert, chain, error) =>
                        {
                            if (error == SslPolicyErrors.None)
                                return true;

                            if (error == SslPolicyErrors.RemoteCertificateNameMismatch
                                && cert != null)
                            {
                                // TODO: Parse the X509Cert correctly

                                if (cert.Subject.Contains("pixiv", StringComparison.Ordinal)
                                    || cert.Subject.Contains("pximg", StringComparison.Ordinal))
                                    return true;
                            }

                            return false;
                        });

                    await sslStream.AuthenticateAsClientAsync("").ConfigureAwait(false);
                    return sslStream;
                }
                catch
                {
                    socket.Dispose();
                    throw;
                }
            }
        });

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _handler.Dispose();
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var ub = new UriBuilder(request.RequestUri!);
            if (ub.Scheme == "https")
                ub.Scheme = "http";
            request.RequestUri = ub.Uri;
            return _handler.SendAsync(request, cancellationToken);
        }
    }
}
