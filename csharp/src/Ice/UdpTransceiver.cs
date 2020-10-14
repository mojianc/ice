// Copyright (c) ZeroC, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ZeroC.Ice
{
    internal sealed class UdpTransceiver : ITransceiver
    {
        public Socket Socket { get; }
        internal IPEndPoint? MulticastAddress { get; private set; }

        // The maximum IP datagram size is 65535. Subtract 20 bytes for the IP header and 8 bytes for the UDP header
        // to get the maximum payload.
        private const int MaxPacketSize = 65535 - UdpOverhead;
        private const int UdpOverhead = 20 + 8;

        private IPEndPoint _addr;
        private readonly Communicator _communicator;
        private readonly bool _incoming;
        private readonly string? _multicastInterface;
        private EndPoint? _peerAddr;
        private readonly int _rcvSize;
        private readonly int _sndSize;
        private readonly IPEndPoint? _sourceAddr;

        public Endpoint Bind(UdpEndpoint endpoint)
        {
            Debug.Assert(_incoming);
            try
            {
                if (Network.IsMulticast(_addr))
                {
                    Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);

                    MulticastAddress = _addr;
                    if (OperatingSystem.IsWindows())
                    {
                        // Windows does not allow binding to the multicast address itself so we bind to INADDR_ANY
                        // instead. As a result, bi-directional connection won't work because the source address won't
                        // be the multicast address and the client will therefore reject the datagram.
                        if (_addr.AddressFamily == AddressFamily.InterNetwork)
                        {
                            _addr = new IPEndPoint(IPAddress.Any, _addr.Port);
                        }
                        else
                        {
                            _addr = new IPEndPoint(IPAddress.IPv6Any, _addr.Port);
                        }
                    }

                    Socket.Bind(_addr);
                    _addr = (IPEndPoint)Socket.LocalEndPoint!;

                    if (endpoint.Port == 0)
                    {
                        MulticastAddress.Port = _addr.Port;
                    }

                    Network.SetMulticastGroup(Socket, MulticastAddress.Address, _multicastInterface);
                }
                else
                {
                    Socket.Bind(_addr);
                    _addr = (IPEndPoint)Socket.LocalEndPoint!;
                }
            }
            catch (SocketException ex)
            {
                throw new TransportException(ex);
            }

            Debug.Assert(endpoint != null);
            return endpoint.Clone((ushort)_addr.Port);
        }

        public void CheckSendSize(int size)
        {
            // The maximum packetSize is either the maximum allowable UDP packet size, or the UDP send buffer size
            // (which ever is smaller).
            int packetSize = Math.Min(MaxPacketSize, _sndSize - UdpOverhead);
            if (packetSize < size)
            {
                throw new DatagramLimitException($"cannot send more than {packetSize} bytes with UDP");
            }
        }

        public ValueTask CloseAsync(Exception exception, CancellationToken cancel) => new ValueTask();

        public ValueTask DisposeAsync()
        {
            Socket.Dispose();
            return new ValueTask();
        }

        public async ValueTask InitializeAsync(CancellationToken cancel)
        {
            if (!_incoming)
            {
                try
                {
                    if (_sourceAddr != null)
                    {
                        Socket.Bind(_sourceAddr);
                    }

                    // TODO: fix to use the cancellable ConnectAsync with 5.0
                    await Socket.ConnectAsync(_addr).WaitAsync(cancel).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    throw new ConnectFailedException(ex);
                }
            }
        }

        public async ValueTask<ArraySegment<byte>> ReceiveDatagramAsync(CancellationToken cancel)
        {
            int packetSize = Math.Min(MaxPacketSize, _rcvSize - UdpOverhead);
            ArraySegment<byte> buffer = new byte[packetSize];

            int received = 0;
            try
            {
                // TODO: Workaround for https://github.com/dotnet/corefx/issues/31182
                if (!_incoming ||
                    (OperatingSystem.IsMacOS() &&
                     Socket.AddressFamily == AddressFamily.InterNetworkV6 && Socket.DualMode))
                {
                    received = await Socket.ReceiveAsync(buffer, SocketFlags.None, cancel).ConfigureAwait(false);
                }
                else
                {
                    EndPoint? peerAddr = _peerAddr;
                    if (peerAddr == null)
                    {
                        if (_addr.AddressFamily == AddressFamily.InterNetwork)
                        {
                            peerAddr = new IPEndPoint(IPAddress.Any, 0);
                        }
                        else
                        {
                            Debug.Assert(_addr.AddressFamily == AddressFamily.InterNetworkV6);
                            peerAddr = new IPEndPoint(IPAddress.IPv6Any, 0);
                        }
                    }

                    // TODO: Fix to use the cancellable API with 5.0
                    SocketReceiveFromResult result =
                        await Socket.ReceiveFromAsync(buffer,
                                                   SocketFlags.None,
                                                   peerAddr).WaitAsync(cancel).ConfigureAwait(false);
                    _peerAddr = result.RemoteEndPoint;
                    received = result.ReceivedBytes;
                }
            }
            catch (SocketException e) when (e.SocketErrorCode == SocketError.MessageSize)
            {
                // Ignore and return an empty buffer if the datagram is too large.
            }
            catch (SocketException e)
            {
                if (e.IsConnectionLost())
                {
                    throw new ConnectionLostException();
                }
                throw new TransportException(e);
            }

            return buffer.Slice(0, received);
        }

        public ValueTask<int> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancel) =>
            throw new InvalidOperationException();

        public override string ToString()
        {
            try
            {
                var sb = new StringBuilder();
                if (_incoming)
                {
                    sb.Append("local address = " + Network.LocalAddrToString(Network.GetLocalAddress(Socket)));
                    if (_peerAddr != null)
                    {
                        sb.Append($"\nremote address = {_peerAddr}");
                    }
                }
                else
                {
                    sb.Append(Network.SocketToString(Socket));
                }
                if (MulticastAddress != null)
                {
                    sb.Append($"\nmulticast address = {MulticastAddress}");
                }

                List<string> interfaces;
                if (MulticastAddress == null)
                {
                    interfaces = Network.GetHostsForEndpointExpand(_addr.ToString(), Network.EnableBoth, true);
                }
                else
                {
                    interfaces = Network.GetInterfacesForMulticast(_multicastInterface,
                                                                   Network.GetIPVersion(MulticastAddress.Address));
                }
                if (interfaces.Count != 0)
                {
                    sb.Append("\nlocal interfaces = ");
                    sb.Append(string.Join(", ", interfaces));
                }
                return sb.ToString();
            }
            catch (ObjectDisposedException)
            {
                return "<closed>";
            }
        }

        public async ValueTask<int> SendAsync(IList<ArraySegment<byte>> buffer, CancellationToken cancel)
        {
            int count = buffer.GetByteCount();

            // The caller is supposed to check the send size before by calling checkSendSize
            Debug.Assert(Math.Min(MaxPacketSize, _sndSize - UdpOverhead) >= count);

            if (_incoming && _peerAddr == null)
            {
                throw new TransportException("cannot send datagram to undefined peer");
            }

            try
            {
                if (!_incoming)
                {
                    // TODO: Use cancellable API once https://github.com/dotnet/runtime/issues/33417 is fixed.
                    return await Socket.SendAsync(buffer, SocketFlags.None).WaitAsync(cancel).ConfigureAwait(false);
                }
                else
                {
                    Debug.Assert(_peerAddr != null);
                    // TODO: Fix to use the cancellable API with 5.0
                    return await Socket.SendToAsync(buffer.GetSegment(0, count),
                                                    SocketFlags.None,
                                                    _peerAddr).WaitAsync(cancel).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                if (ex.IsConnectionLost())
                {
                    throw new ConnectionLostException(ex);
                }
                throw new TransportException(ex);
            }
        }

        // Only for use by UdpConnector.
        internal UdpTransceiver(
            Communicator communicator,
            EndPoint addr,
            IPAddress? sourceAddr,
            string? multicastInterface,
            int multicastTtl)
        {
            _communicator = communicator;
            _addr = (IPEndPoint)addr;
            _multicastInterface = multicastInterface;
            _incoming = false;
            if (sourceAddr != null)
            {
                _sourceAddr = new IPEndPoint(sourceAddr, 0);
            }

            Socket = Network.CreateSocket(true, _addr.AddressFamily);
            try
            {
                Network.SetBufSize(Socket, _communicator, Transport.UDP);
                _rcvSize = (int)Socket.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer)!;
                _sndSize = (int)Socket.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer)!;

                if (Network.IsMulticast(_addr))
                {
                    if (_multicastInterface != null)
                    {
                        Debug.Assert(_multicastInterface.Length > 0);
                        Network.SetMulticastInterface(Socket, _multicastInterface, _addr.AddressFamily);
                    }
                    if (multicastTtl != -1)
                    {
                        Network.SetMulticastTtl(Socket, multicastTtl, _addr.AddressFamily);
                    }
                }
            }
            catch (SocketException ex)
            {
                Socket.CloseNoThrow();
                throw new TransportException(ex);
            }
        }

        // Only for use by UdpEndpoint.
        internal UdpTransceiver(UdpEndpoint endpoint, Communicator communicator)
        {
            _communicator = communicator;
            _addr = Network.GetAddressForServerEndpoint(endpoint.Host, endpoint.Port, Network.EnableBoth);
            _multicastInterface = endpoint.MulticastInterface;
            _incoming = true;

            Socket = Network.CreateServerSocket(endpoint, _addr.AddressFamily);
            try
            {
                Network.SetBufSize(Socket, _communicator, Transport.UDP);
                _rcvSize = (int)Socket.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer)!;
                _sndSize = (int)Socket.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer)!;
            }
            catch (SocketException ex)
            {
                Socket.CloseNoThrow();
                throw new TransportException(ex);
            }
        }
    }
}
