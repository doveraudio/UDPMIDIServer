﻿using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UDPMIDILib
{


    public class UdpUser : UdpBase
    {

        private UdpUser()
        {
        }
        private int index;
        private string hostname;
        private int port;
        private bool active;
        public string Hostname { get => hostname; set => hostname = value; }
        public string IpAddress { get => hostname; set => hostname = value; }

        public int Port { get => port; set => port = value; }
        public bool Active
        {
            get
            {
                return active;
            }

            set
            {
                if (value)
                {
                    active = true;


                }
                else {

                    active = false;
                    try
                    {
                        this.Close();
                    }
                    catch (System.Exception)
                    {

                        
                    }
                    
                }
                
            }
        }

        public int Index { get => index; set => index = value; }

        public void Connect() {

            this.Client = ConnectTo(this.hostname, this.port).Client;
        }

        public void Close() {
            this.Client.Close();

        }

        public void Dispose() {

            this.Client.Dispose();

        }

        public static UdpUser ConnectTo(string hostname, int port)
        {
            var connection = new UdpUser();
            connection.Client.Connect(hostname, port);
            return connection;
        }


        public void Send(string message)
        {
            var datagram = Encoding.ASCII.GetBytes(message);
            this.Client.Send(datagram, datagram.Length);
        }
    }
}
