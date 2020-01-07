using Pole.ReliableMessage.Abstraction;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace Pole.ReliableMessage.Utils
{
    class DefaultServiceIPv4AddressProvider : IServiceIPv4AddressProvider
    {
        private readonly ReliableMessageOption _options;
        private string _ipAddress;
        public DefaultServiceIPv4AddressProvider(IOptions<ReliableMessageOption> options)
        {
            _options = options.Value;
            Init();
        }

        private void Init()
        {
            var gatewayAddress = _options.NetworkInterfaceGatewayAddress;
            NetworkInterface networkInterface = null;
            if (string.IsNullOrEmpty(_options.NetworkInterfaceGatewayAddress))
            {
                networkInterface = NetworkInterface.GetAllNetworkInterfaces()
                                     .OrderByDescending(c => c.Speed)
                                     .Where(m => m.NetworkInterfaceType != NetworkInterfaceType.Loopback && m.OperationalStatus == OperationalStatus.Up)
                                     .FirstOrDefault();
            }
            else
            {
                networkInterface = NetworkInterface.GetAllNetworkInterfaces()
.OrderByDescending(c => c.Speed).Where(m => m.NetworkInterfaceType != NetworkInterfaceType.Loopback && m.OperationalStatus == OperationalStatus.Up).Where(m => m.GetIPProperties().GatewayAddresses.FirstOrDefault(c => c.Address.AddressFamily == AddressFamily.InterNetwork)?.Address.ToString() == gatewayAddress)
.FirstOrDefault();
            }
            if (networkInterface == null)
            {
                throw new Exception($"Not found correct NetworkInterface, option.NetworkInterfaceGatewayAddress:{gatewayAddress}");
            }
            var props = networkInterface.GetIPProperties();
            // get first IPV4 address assigned to this interface
            var firstIpV4Address = props.UnicastAddresses
                .Where(c => c.Address.AddressFamily == AddressFamily.InterNetwork)
                .Select(c => c.Address)
                .FirstOrDefault();
            _ipAddress = firstIpV4Address.ToString();
        }

        public string Get()
        {
            return _ipAddress;
        }
    }
}
