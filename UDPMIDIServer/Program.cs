﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sanford.Multimedia.Midi;
using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using UDPMIDILib;
namespace UDPMIDIServer
{
    class Program
    {

        private static List<UdpUser> subscribers;
      
        static void Main(string[] args)
        {
            var server = new UdpListener();
            subscribers = new List<UdpUser>();
           IPEndPoint sender;
            string read = "";

            Task.Factory.StartNew(async () => {
                while (true) {
                    var received = await server.Receive();
                    server.Reply("copy" + received.Message, received.Sender);
                    Console.WriteLine("Received: " + received.Message + " from " + received.Sender);
                    read = received.Message;
                    sender = received.Sender;
                    UDPMIDIMessage m = (UDPMIDIMessage)JsonConvert.DeserializeObject(read, typeof(UDPMIDIMessage));
                    switch (m.Mode)
                    {
                        case "connect":
                            AddSubscriber(sender.Address.ToString(), sender.Port);
                        break;
                        case "disconnect":
                            break;
                        case "message":
                            break;
                        case "channel":
                            break;
                        case "short":
                            break;
                        case "systrealtime":
                            break;
                        case "sysex":
                            break;
                        case "syscommon":
                            break;
                        default:
                            break;
                    }
                    if (read == "quit")
                    { break; }
                    else {
                        try
                        {
                            //Console.WriteLine(read);

                            Dictionary<string, string> message = JsonConvert.DeserializeObject<Dictionary<string, string>>(read);
                            foreach (var val in message)
                            {
                                Console.WriteLine(val.Key +": "+val.Value);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(JsonConvert.SerializeObject(ex));
                            //throw;
                        }


                    }
                }
            });

           
            do
            {
                

            } while (read != "quit");


        }
        private static void AddSubscriber(string host, int port) {
            UdpUser subscriber = UdpUser.ConnectTo(host, port);
            subscriber.Active = true;
            subscriber.Connect();
            subscriber.Send("connected to server");
            subscribers.Add(subscriber);

        }
        
    }

    public class UDPMIDIMessage {
        public UDPMIDIMessage(string mode, string value) {
            this.Value = value;
            this.Mode = mode;
        }
        private string mode;
        private string value;

        public string Mode { get => mode; set => mode = value; }
        public string Value { get => value; set => this.value = value; }
    }
    public enum ServerMode
    {

        connect, disconnect, message, channel, shortmessage, sysrealtime, sysex, syscommon

    }
}
