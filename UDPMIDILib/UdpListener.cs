using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UDPMIDILib
{
    public class UdpListener : UdpBase
    {

        private IPEndPoint _listenOn;
        private string hostname;
        private int port;

        public string Hostname { get => hostname; set => hostname = value; }
        public string IpAddress { get => hostname; set => hostname = value; }

        public int Port { get => port; set => port = value; }
        public UdpListener() : this(new IPEndPoint(IPAddress.Any, 32123)) { }
        public UdpListener(IPEndPoint endpoint)
        {

            _listenOn = endpoint;
            Client = new UdpClient(_listenOn);
        }
        public void Reply(string message, IPEndPoint endpoint)
        {

            var datagram = Encoding.ASCII.GetBytes(message);
            Client.Send(datagram, datagram.Length, endpoint);

        }
    }
}
