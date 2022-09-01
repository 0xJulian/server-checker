using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows.Input;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace server_controller
{
        public class data
        {
                public string Name { get; set; }
                public string IpAddr { get; set; }
        }
        class Program
        {
                static void JsonWriter(string name, string ip)
                {
                        List<data> _data = LoadJson();
                        if (_data == null)
                        {
                                _data = new List<data>();
                        }
                        _data.Add(new data()
                        {
                               Name = name,
                               IpAddr = ip
                        });

                        string json = JsonSerializer.Serialize(_data);
                        File.WriteAllText(@"config.json", json);
                }

                public static List<data> LoadJson()
                {
                        using (StreamReader r = new StreamReader("config.json"))
                        {
                                string json = r.ReadToEnd();
                                List<data> items = JsonConvert.DeserializeObject<List<data>>(json);
                                return items;
                        }
                }


                static void AddServer(Hashtable serversTable)
                {
                        LoadJson();
                        string name;
                        string ipAddr;
                        IPAddress finalIp = IPAddress.Parse("0.0.0.0");
                        Console.Clear();
                        Console.WriteLine("--- Add server ---");
                        Console.Write("Name >>");
                        name = Console.ReadLine();
                        Console.Write("IP >>");
                        ipAddr = Console.ReadLine();
                        Console.WriteLine("-------------------");
                        try
                        {
                                if (ipAddr != null) finalIp = IPAddress.Parse(ipAddr);
                                serversTable.Add(name, finalIp);
                        }
                        catch (Exception e)
                        {
                                Console.WriteLine("ERROR: " + e.Message);
                                return;
                        }
                        
                        JsonWriter(name, ipAddr);
                        Console.WriteLine("Server has been added!");
                }

                public static void Log(Hashtable hashtable)
                {
                        while (true)
                        {
                                Ping ping = new Ping();
                                foreach (DictionaryEntry o in hashtable)
                                {
                                        PingReply pingReply = ping.Send((IPAddress)o.Value ??
                                                                        throw new InvalidOperationException());
                                        if (pingReply.Status == IPStatus.Success)
                                        {
                                                Console.ForegroundColor
                                                        = ConsoleColor.Green;
                                                Console.WriteLine(o.Key + " is alive!");
                                        }
                                        else
                                        {
                                                Console.ForegroundColor
                                                        = ConsoleColor.Red;
                                                Console.WriteLine(o.Key + " is dead!");
                                                Console.ForegroundColor
                                                        = ConsoleColor.Green;
                                        }
                                }
                                Thread.Sleep(100);
                        }
                }
                static void Main(string[] args)
                {
                        int inpt;
                        var allServers = LoadJson();
                        Hashtable hashtable = new Hashtable();
                        if (allServers != null)
                        {
                                foreach (data o in allServers)
                                {
                                        hashtable.Add(o.Name, IPAddress.Parse(o.IpAddr));
                                }
                        }
                        Console.WriteLine("--- Menu ---");
                        Console.WriteLine("<0> Add server");
                        Console.WriteLine("<1> Log");
                        Console.WriteLine("------------");
                        inpt = Convert.ToInt32(Console.ReadLine());
                        switch (inpt)
                        {
                                case 0:
                                        AddServer(hashtable);
                                        break;
                                case 1:
                                        Log(hashtable);
                                        break;
                                default:
                                        return;
                        }
                        
                }
        }
}
