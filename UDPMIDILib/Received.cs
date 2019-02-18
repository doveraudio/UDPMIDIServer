using System.Net;

namespace UDPMIDILib
{
    public struct Received
    {
        public IPEndPoint Sender;
        public string Message;
    }
}
