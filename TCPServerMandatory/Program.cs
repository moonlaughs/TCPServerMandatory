using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using ClassLibraryMandatory;

namespace TCPServerMandatory
{
    class Program
    {
        private static readonly int PORT = 777;

        static void Main(string[] args)
        {
            IPAddress localAddress = IPAddress.Loopback;
            TcpListener serverSocket = new TcpListener(localAddress, PORT);

            serverSocket.Start();

            Console.WriteLine("TCP Server runnin on port number: " + PORT);

            while (true)
            {
                try
                {
                    TcpClient client = serverSocket.AcceptTcpClient();
                    Console.WriteLine("Incoming client");
                    Task.Run((() => DoComunicate(client)));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        private static void DoComunicate(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            StreamReader reader = new StreamReader(stream);
            StreamWriter writer = new StreamWriter(stream);

            while (true)
            {
                ClassLibraryMandatory.WeightConverter obj = new WeightConverter();

                string request = reader.ReadLine();

                if (request != null)
                {
                    Console.WriteLine("Request: " + request);

                    string response = null;
                    string[] myArray = request.Split(' ');

                    if (request.Split(' ').Length == 2)
                    {
                        if ((myArray[0] == "TOGRAM" || myArray[0] == "TOOUNCE") && myArray[1] != null)
                        {
                            try
                            {
                                double value = double.Parse(myArray[1]);

                                if (myArray[1] == "TOGRAM")
                                {
                                    response = obj.OuncesToGram(value).ToString();
                                }
                                else
                                {
                                    response = obj.GramToOunces(value).ToString();
                                }

                                Console.WriteLine("Responce: " + response);
                                writer.WriteLine(response + "\n ");
                                Console.WriteLine();
                                writer.WriteLine();
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("WRONG INPUT");
                                writer.WriteLine("Please inseart valid number.");
                                Console.WriteLine();
                                writer.WriteLine();
                                writer.Flush();
                                continue;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Available actions: \nTOGRAM [number]\nTOOUNCE [number]");
                            writer.WriteLine("No such action available");
                            Console.WriteLine();
                            writer.WriteLine();
                        }
                    }
                    if (request == "STOP")
                    {
                        break;
                    }
                    else
                    {
                        if (request.Split(' ').Length != 2)
                        {
                            Console.WriteLine("No such action available");
                            Console.WriteLine();
                            writer.WriteLine("No such action available");
                            writer.WriteLine();
                        }
                    }
                    writer.Flush();
                }
            }
            client.Close();
            Console.WriteLine("Client disconnected.\nWaiting...");
        }
    }
}
