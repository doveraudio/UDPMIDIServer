using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sanford;
using Sanford.Collections.Generic;
using Sanford.Multimedia;
using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UDPMIDILib;

namespace UDPMIDISimpleClient
{
    class Program
    {
        static List<UdpUser> BroadcastClients;
        static List<UdpUser> ListenClients;
        static int MidiDevice;
        static string BroadcastServerIp;
        static string BroadcastServerPort;
        static string ListenServerIp;
        static string ListenServerPort;
        static bool BroadcastActive;
        static bool ListenActive;
        static List<UDPMidiInputDevice> inputDevices;
        static List<UDPMidiOutputDevice> outputDevices;
        static string read;
        static string remote;
        static bool run = true;
        static void Main(string[] args)
        {
            initializeCollections();
            getListenServerAddresses(0);
            getBroadcastServerAddresses(0);

            //var device = new InputDevice(0);
            //device.MessageReceived += Indevice_MessageReceived;
            //device.ShortMessageReceived += Indevice_ShortMessageReceived;
            //device.StartRecording();
            addInputDevices();
            addOutputDevices();
            initializeConnections();


            startRecording();
            foreach (UdpUser client in ListenClients)
            {
                startListening(client);

            }


            Console.WriteLine("Type 'quit' to quit.");
            do
            {

                //read = Console.ReadLine();
                if (read == "quit") { Console.WriteLine(read + "ting!"); run = false; }

                //client.Send(read);

            } while (read != "quit");

        }

        private static void startListening(UdpUser client)
        {
            Task.Factory.StartNew(async () =>
            {
                while (run)
                {
                    try
                    {
                        Received received = await client.Receive();

                        Console.WriteLine(received.Message);
                        if (received.Message.Contains("quit") | read.Contains("quit"))
                            break;
                    }
                    catch (Exception Ex)
                    {
                        Debug.Write(Ex);
                    }
                }
            });
        }

        private static void startRecording()
        {
            foreach (UDPMidiInputDevice device in inputDevices)
            {
                if (device.Active)
                {
                    device.Device.StartRecording();
                }
            }
        }

        private static void stopRecording()
        {
            foreach (UDPMidiInputDevice device in inputDevices)
            {
                if (device.Active)
                {
                    device.Device.StopRecording();
                }
            }
        }

        private static void initializeConnections()
        {
            foreach (UdpUser client in BroadcastClients)
            {

                client.Connect();
            }

            foreach (UdpUser client in ListenClients)
            {

                client.Connect();
            }
        }
        private static void closeConnections()
        {
            foreach (UdpUser client in BroadcastClients)
            {

                client.Close();
            }
            foreach (UdpUser client in ListenClients)
            {
                client.Close();
            }
        }

        private static void initializeCollections()
        {
            inputDevices = new List<UDPMidiInputDevice>();
            outputDevices = new List<UDPMidiOutputDevice>();
            BroadcastClients = new List<UdpUser>();
            ListenClients = new List<UdpUser>();

        }

        private static void addOutputDevices()
        {
            bool complete = false;
            int index = -1;
            do
            {
                Console.WriteLine("Activate/Deactivate Midi Output Devices:");
                getMidiOutputDevices();
                Console.WriteLine("Enter Index of device to activate or deactivate");
                Console.Write("?>");
                string input = Console.ReadLine();
                int.TryParse(input, out index);
                if (index >= 0 && index < OutputDevice.DeviceCount)
                {

                    if (outputDevices[index].Active)
                    {
                        outputDevices[index].Active = false;
                        //DetachEvents(outputDevices[index].Device);

                    }
                    else
                    {
                        outputDevices[index].Active = true;
                        //AttachEvents(inputDevices[index].Device);
                    }
                }
                Console.WriteLine("Activate/Deactivate more Midi Output Devices?");
                Console.Write("?>");
                read = Console.ReadLine();
                if (read.ToLower().Contains("no"))
                {
                    complete = true;
                }

            } while (!complete);
        }
        private static void addOutputDevice(int index)
        {
            outputDevices.Add(new UDPMidiOutputDevice(index));
        }

        private static void addInputDevices()
        {
            bool complete = false;
            int index = -1;
            do
            {
                Console.WriteLine("Activate/Deactivate Midi Input Devices:");
                getMidiInputDevices();
                Console.WriteLine("Enter Index of device to activate or deactivate");
                Console.Write("?>");
                string read = Console.ReadLine();
                int.TryParse(read, out index);
                if (index >= 0 && index < InputDevice.DeviceCount)
                {

                    if (inputDevices[index].Active)
                    {
                        inputDevices[index].Active = false;
                        DetachEvents(inputDevices[index].Device);

                    }
                    else
                    {
                        inputDevices[index].Active = true;
                        AttachEvents(inputDevices[index].Device);
                    }
                }
                Console.WriteLine("Activate/Deactivate more Midi Input Devices?");
                Console.Write("?>");
                read = Console.ReadLine();
                if (read.ToLower().Contains("no"))
                {
                    complete = true;
                }

            } while (!complete);
        }
        private static void addInputDevice(int index)
        {
            inputDevices.Add(new UDPMidiInputDevice(index));

        }

        private static void AttachEvents(InputDevice device)
        {
            device.MessageReceived += Indevice_MessageReceived;
            device.ShortMessageReceived += Indevice_ShortMessageReceived;
            device.ChannelMessageReceived += Indevice_ChannelMessageReceived;
            device.SysCommonMessageReceived += Indevice_SysCommonMessageReceived;
            device.SysExMessageReceived += Indevice_SysExMessageReceived;
            device.SysRealtimeMessageReceived += Indevice_SysRealtimeMessageReceived;
            device.Error += Indevice_Error;
        }
        private static void DetachEvents(InputDevice device)
        {
            device.MessageReceived -= Indevice_MessageReceived;
            device.ShortMessageReceived -= Indevice_ShortMessageReceived;
            device.ChannelMessageReceived -= Indevice_ChannelMessageReceived;
            device.SysCommonMessageReceived -= Indevice_SysCommonMessageReceived;
            device.SysExMessageReceived -= Indevice_SysExMessageReceived;
            device.SysRealtimeMessageReceived -= Indevice_SysRealtimeMessageReceived;
            device.Error -= Indevice_Error;
        }
        private static void getServerPort()
        {
            //throw new NotImplementedException();
        }

        private static void getBroadcastServerAddresses(EntryMode mode = 0)
        {
            //throw new NotImplementedException();
            string ip = "";
            string port = "";
            bool complete = false;
            switch (mode)
            {
                case EntryMode.Add:
                    AddBroadCastServerAddresses();
                    break;
                case EntryMode.Remove:
                    RemoveBroadCastServerAddresses();
                    break;
                case EntryMode.Clear:
                    ClearBroadCastServerAddresses();
                    break;
                default:
                    break;
            }


        }
        private static void getListenServerAddresses(EntryMode mode = 0)
        {
            //throw new NotImplementedException();
            string ip = "";
            string port = "";
            bool complete = false;
            switch (mode)
            {
                case EntryMode.Add:
                    AddListenServerAddresses();
                    break;
                case EntryMode.Remove:
                    RemoveListenServerAddresses();
                    break;
                case EntryMode.Clear:
                    ClearListenServerAddresses();
                    break;
                default:
                    break;
            }


        }

        private static void ClearListenServerAddresses()
        {
            foreach (UdpUser client in ListenClients)
            {

                client.Close();
                client.Dispose();


            }
            ListenClients.Clear();
        }

        private static void ClearBroadCastServerAddresses()
        {
            foreach (UdpUser client in BroadcastClients)
            {

                client.Close();
                client.Dispose();


            }
            BroadcastClients.Clear();
        }

        private static void RemoveBroadCastServerAddresses()
        {
            string ip = "";
            string port = "";
            int index;
            bool complete = false;
            do
            {
                for (int i = 0; i < BroadcastClients.Count; i++)
                {
                    Console.WriteLine("{0}:{1}", i, BroadcastClients[i].Hostname + ":" + BroadcastClients[i].Port.ToString());

                }
                Console.WriteLine("\nEnter Broadcast Server IP Address index to remove:\n (q to quit) \n");
                Console.Write("?>");
                read = Console.ReadLine();
                if (!read.ToLower().Equals("q"))
                {
                    if (int.TryParse(read, out index))
                    {
                        if (BroadcastClients.Count >= index)
                        {
                            BroadcastClients.RemoveAt(index);
                        }
                    }
                }

                Console.WriteLine("Remove more Broadcast Addresses?");
                Console.Write("?>");
                read = Console.ReadLine();
                if (read.ToLower().Contains("no"))
                {
                    complete = true;
                }

            } while (!complete);
        }

        private static void AddBroadCastServerAddresses()
        {
            string ip = "";
            int port = 32123;
            int index;
            bool complete = false;
            do
            {
                Console.WriteLine("{0} Broadcast Servers Found:\n", BroadcastClients.Count);
                for (int i = 0; i < BroadcastClients.Count; i++)
                {
                    Console.WriteLine("{0}:{1}", i, BroadcastClients[i].Hostname + ":" + BroadcastClients[i].Port.ToString());

                }
                Console.WriteLine("\nEnter Broadcast Server IP Address:\n (defaults to [127.0.0.1], q to quit) \n");
                Console.Write("?>");
                read = Console.ReadLine();
                if (read.ToLower().Equals("q"))
                {

                    complete = true;
                    break;
                }
                if (read == "")
                {
                    ip = "127.0.0.1";
                }
                else try
                    {
                        ip = IPAddress.Parse(read).ToString();
                    }
                    catch (Exception ex)
                    {

                        ip = "127.0.0.1";
                    }

                Console.WriteLine("Enter Broadcast Server Port:\n (defaults to [32123]), q to quit) \n");
                Console.Write("?>");
                read = Console.ReadLine();
                if (read.ToLower().Equals("q"))
                {

                    complete = true;
                    break;
                }

                if (read == "")
                {
                    port = 32123;
                }
                else
                {
                    int.TryParse(read, out port);
                }
                Console.WriteLine("IP Address: {0}, Port: {1}\nOkay?\n<yes|no>", ip, port);
                Console.Write("?>");
                read = Console.ReadLine();
                if (read.ToLower().Contains("yes"))
                {


                    UdpUser broadcast = UdpUser.ConnectTo(ip, port);
                    broadcast.Hostname = ip;
                    broadcast.Port = port;
                    broadcast.Active = false;
                    broadcast.Index = BroadcastClients.Count;
                    BroadcastClients.Add(broadcast);
                    read = "";

                }
                Console.WriteLine("Add more Broadcast Addresses?");
                Console.Write("?>");
                read = Console.ReadLine();
                if (read.ToLower().Equals("q"))
                {

                    complete = true;
                    break;
                }
                if (read.ToLower().Contains("no"))
                {
                    complete = true;
                    break;
                }

            } while (!complete);
        }

        private static void AddListenServerAddresses()
        {
            //throw new NotImplementedException();
            string ip = "";
            int port = 32123;
            bool complete = false;

            do
            {

                Console.WriteLine("{0} Listen Servers Found:\n", ListenClients.Count);

                for (int i = 0; i < ListenClients.Count; i++)
                {
                    Console.WriteLine("{0}:{1}", i, ListenClients[i].Hostname + ":" + ListenClients[i].Port.ToString());

                }

                Console.WriteLine("\nEnter Listen Server IP Address:\n (defaults to [127.0.0.1], q to quit) \n");
                Console.Write("?>");
                read = Console.ReadLine();
                if (read.ToLower().Equals("q"))
                {

                    complete = true;
                    break;
                }
                if (read == "")
                {
                    ip = "127.0.0.1";
                }
                else try
                    {
                        read = IPAddress.Parse(read).ToString();
                    }
                    catch (Exception ex)
                    {

                        ip = "127.0.0.1";
                    }

                Console.WriteLine("Enter Listen Server Port:\n (defaults to [32123], q to quit) \n");
                Console.Write("?>");
                read = Console.ReadLine();
                if (read.ToLower().Equals("q"))
                {

                    complete = true;
                    break;
                }

                if (read == "")
                {
                    port = 32123;
                }
                else { int.TryParse(read, out port); }
                Console.WriteLine("IP Address: {0}, Port: {1}\nOkay?\n<yes|no|quit>", ip, port);
                Console.Write("?>");
                read = Console.ReadLine();
                if (read.ToLower().Contains("q"))
                {

                    complete = true;
                    break;
                }
                if (read == "yes")
                {
                    UdpUser listen = UdpUser.ConnectTo(ip, port);
                    listen.Hostname = ip;
                    listen.Port = port;
                    listen.Active = false;
                    listen.Index = ListenClients.Count;
                    UDPMIDIMessage m = new UDPMIDIMessage("subscribe", JsonConvert.SerializeObject(listen, typeof(UdpUser), null));
                    listen.Connect();
                    listen.Send(m.ToJson());
                    ListenClients.Add(listen);
                    read = "";

                }
                Console.WriteLine("Add more Listen Addresses?");
                Console.Write("?>");
                read = Console.ReadLine();
                if (read.ToLower().Equals("q"))
                {

                    complete = true;
                    break;
                }
                if (read.ToLower().Contains("no"))
                {
                    complete = true;
                    break;
                }
            } while (!complete);

        }

        private static void RemoveListenServerAddresses()
        {
            string ip = "";
            string port = "";
            int index;
            bool complete = false;
            do
            {
                for (int i = 0; i < ListenClients.Count; i++)
                {
                    Console.WriteLine("{0}:{1}", i, ListenClients[i].Hostname + ":" + ListenClients[i].Port.ToString());

                }
                Console.WriteLine("\nEnter Listen Server IP Address index to remove:\n (q to quit) \n");
                Console.Write("?>");
                read = Console.ReadLine();
                if (!read.ToLower().Equals("q"))
                {
                    if (int.TryParse(read, out index))
                    {
                        if (ListenClients.Count >= index)
                        {
                            ListenClients.RemoveAt(index);
                        }
                    }
                }

                Console.WriteLine("Remove more Listen Addresses?");
                Console.Write("?>");
                read = Console.ReadLine();
                if (read.ToLower().Contains("no"))
                {
                    complete = true;
                }

            } while (!complete);
        }



        private static void getMidiInputDevices()
        {
            bool exists = true;
            int index = 0;
            int count = InputDevice.DeviceCount;

            Console.WriteLine("Found {0} input devices", count);
            for (int i = 0; i < count; i++)
            {
                if (inputDevices.Count <= i)
                {
                    addInputDevice(i);
                }

                Console.WriteLine("{0}: {1}, Active: {2}", inputDevices[i].Index, inputDevices[i].Caps.name, inputDevices[i].Active.ToString());


            }





        }
        private static void getMidiOutputDevices()
        {

            bool exists = true;
            int index = 0;
            int count = OutputDevice.DeviceCount;

            Console.WriteLine("Found {0} output devices", count);
            for (int i = 0; i < count; i++)
            {
                if (outputDevices.Count <= i)
                {
                    addOutputDevice(i);
                }

                Console.WriteLine("{0}: {1}, Active: {2}", outputDevices[i].Index, outputDevices[i].Caps.name, outputDevices[i].Active.ToString());


            }





        }
        private static void LogOutMessage(object e)
        {
            Console.WriteLine(e.ToString());

        }

        private static void SendEncoded(object e, string mode)
        {


            string encoded = JsonConvert.SerializeObject(e, Formatting.Indented);
            UDPMIDIMessage m = new UDPMIDIMessage(mode, encoded);
            //LogOutMessage(encoded);
            foreach (UdpUser client in BroadcastClients)
            {
                client.Send(m.ToJson());
            }

        }

        private static void Indevice_Error(object sender, Sanford.Multimedia.ErrorEventArgs e)
        {
            SendEncoded(e.Error, "error");
            //LogOutMessage(e.Error);
            //client.Send(e.Error.Message.ToString());
        }

        private static void Indevice_ShortMessageReceived(object sender, ShortMessageEventArgs e)
        {
            SendEncoded(e.Message, "short");
            //client.Send(e.Message.MessageType.ToString());
            //LogOutMessage(e.Message);
        }

        private static void Indevice_MessageReceived(IMidiMessage message)
        {
            string encoded = JsonConvert.SerializeObject(message, Formatting.Indented);
            UDPMIDIMessage m = new UDPMIDIMessage("message", encoded);
            //Console.WriteLine(m.ToJson());
            foreach (UdpUser client in BroadcastClients)
            {
                client.Send(m.ToJson());
            }

        }

        private static void Indevice_SysRealtimeMessageReceived(object sender, SysRealtimeMessageEventArgs e)
        {
            SendEncoded(e.Message, "sysrealtime");
            // client.Send(e.Message.MessageType.ToString());
        }

        private static void Indevice_SysExMessageReceived(object sender, SysExMessageEventArgs e)
        {
            SendEncoded(e.Message, "sysex");
            //  client.Send(e.Message.MessageType.ToString());
        }

        private static void Indevice_SysCommonMessageReceived(object sender, SysCommonMessageEventArgs e)
        {
            SendEncoded(e.Message, "syscommon");
            //  client.Send(e.Message.MessageType.ToString());
        }

        private static void Indevice_ChannelMessageReceived(object sender, ChannelMessageEventArgs e)
        {
            SendEncoded(e.Message, "channel");
            //  client.Send(e.Message.MessageType.ToString());
        }
    }

    public enum ServerMode
    {

        connect, disconnect, message, channel, shortmessage, sysrealtime, sysex, syscommon

    }
}
