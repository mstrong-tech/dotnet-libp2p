// SPDX-FileCopyrightText: 2023 Demerzel Solutions Limited
// SPDX-License-Identifier: MIT

using Google.Protobuf;
using Nethermind.Libp2p.Core;
using Microsoft.Extensions.Logging;

namespace Nethermind.Libp2p.Protocols;

/// <summary>
///     https://github.com/libp2p/specs/tree/master/identify
/// </summary>
public class IdentifyProtocol : IProtocol
{
    private const string SubProtocolId = "ipfs/0.1.0";

    private readonly ILogger? _logger;
    private readonly IPeerFactoryBuilder peerFactoryBuilder;

    public IdentifyProtocol(IPeerFactoryBuilder peerFactoryBuilder, ILoggerFactory? loggerFactory = null)
    {
        _logger = loggerFactory?.CreateLogger<IdentifyProtocol>();
        this.peerFactoryBuilder = peerFactoryBuilder;
    }

    public string Id => "/ipfs/id/1.0.0";

    public async Task DialAsync(IChannel channel, IChannelFactory channelFactory,
        IPeerContext context)
    {
        _logger?.LogInformation("Dial");

        try
        {
            Identify identity = await channel.ReadPrefixedProtobufAsync(Identify.Parser);

            _logger?.LogInformation("Received peer info: {0}", identity);
            context.RemotePeer.Identity = Identity.FromPublicKey(identity.PublicKey.ToByteArray());

            if (context.RemotePeer.Identity.PublicKey.ToByteString() != identity.PublicKey)
            {
                throw new PeerConnectionException();
            }
        }
        catch
        {
            throw;
        }
    }

    public async Task ListenAsync(IChannel channel, IChannelFactory channelFactory,
        IPeerContext context)
    {
        _logger?.LogInformation("Listen");

        try
        {
            Identify identify = new()
            {
                AgentVersion = "github.com/Nethermind/dotnet-libp2p/samples@1.0.0",
                ProtocolVersion = SubProtocolId,
                ListenAddrs = { ByteString.CopyFrom(context.LocalEndpoint.ToByteArray()) },
                ObservedAddr = ByteString.CopyFrom(context.RemoteEndpoint.ToByteArray()),
                PublicKey = context.LocalPeer.Identity.PublicKey.ToByteString(),
                Protocols = { peerFactoryBuilder.AppLayerProtocols.Select(p => p.Id) }
            };
            byte[] ar = new byte[identify.CalculateSize()];
            identify.WriteTo(ar);
            await channel.WriteSizeAndDataAsync(ar);
            _logger?.LogDebug("Sent peer info {0}", identify);
            _logger?.LogInformation("Sent peer id to {0}", context.RemotePeer.Address);
        }
        catch
        {

        }
    }
}