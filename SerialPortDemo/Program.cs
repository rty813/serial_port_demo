using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace SerialPortDemo
{
    class Program
    {
        private static SerialPort comm;
        private static bool new_line_flag = true;
        private static Dictionary<string, string> dict;

        static void Main(string[] args)
        {
            string[] serial_port = SerialPort.GetPortNames();
            while (serial_port.Length == 0)
            {
                Console.WriteLine("没有找到串口");
                Console.ReadKey();
                serial_port = SerialPort.GetPortNames();
            }

            int port_num = 0;
            foreach (string s in serial_port)
            {
                Console.WriteLine("[" + port_num + "]: " +  s);
                port_num ++;
            }
            Console.Write("请选择一个串口：");
            int select_port_num = Console.Read()-48;
            string select_port = serial_port[select_port_num];
            Console.WriteLine("您选择的是：" + select_port);

            if (!serial_port_init(select_port))
            {
                Console.ReadKey();
                return;
            }

            try
            {
                comm.Open();
                Console.WriteLine(select_port + "已打开");
                while (true)
                {
                    ConsoleKey key;
                    key = Console.ReadKey(true).Key;
                    if (dict.ContainsKey(key.ToString()))
                    {
                        if (new_line_flag)
                        {
                            comm.WriteLine(dict[key.ToString()]);
                            Console.WriteLine(dict[key.ToString()]);    
                        }
                        else
                        {
                            comm.Write(dict[key.ToString()]);
                            Console.Write(dict[key.ToString()]);
                        }
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadKey();
                return;
            }
        }

        static bool serial_port_init(string select_port)
        {
            try
            {
                comm = new SerialPort();
                comm.NewLine = "\r\n";
                comm.PortName = select_port;

                dict = new Dictionary<string, string>();
                StreamReader reader = new StreamReader(Environment.CurrentDirectory + "\\option.txt");
                String line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] lines = line.Split('|');
                    switch (lines[0])
                    {
                        case "baudrate":
                            comm.BaudRate = int.Parse(lines[1]);
                            break;
                        case "16":

                            break;
                        case "\\r\\n":
                            new_line_flag = lines[1].Equals("1") ? true : false;
                            break;
                        default:
                            dict.Add(lines[0].ToUpper(), lines[1]);
                            break;
                    }
                    Console.WriteLine(line);
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadKey();
                return false;
            }


        }
    }
}
