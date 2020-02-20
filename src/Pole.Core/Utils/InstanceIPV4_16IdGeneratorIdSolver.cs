using Pole.Core.Utils.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace Pole.Core.Utils
{
    public class InstanceIPV4_16IdGeneratorIdSolver : IGeneratorIdSolver
    {
        private int generatorId;
        public InstanceIPV4_16IdGeneratorIdSolver()
        {
            NetworkInterface networkInterface = NetworkInterface.GetAllNetworkInterfaces()
                                     .OrderByDescending(c => c.Speed)
                                     .Where(m => m.NetworkInterfaceType != NetworkInterfaceType.Loopback && m.OperationalStatus == OperationalStatus.Up)
                                     .FirstOrDefault();
            var props = networkInterface.GetIPProperties();
            // get first IPV4 address assigned to this interface
            var firstIpV4Address = props.UnicastAddresses
                .Where(c => c.Address.AddressFamily == AddressFamily.InterNetwork)
                .Select(c => c.Address)
                .FirstOrDefault();

            var bytes = firstIpV4Address.GetAddressBytes().TakeLast(2).Reverse().ToList();
            bytes.Add(0);
            bytes.Add(0);
            generatorId = BitConverter.ToInt32(bytes.ToArray());
        }
        public int GetGeneratorId()
        {
            return generatorId;
        }
    }
}
