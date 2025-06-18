using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyModbus;

namespace ComartTracking
{
    internal class Conveyor
    {
        public string port = "COM4";
        public int baudRate = 9600;

        ModbusClient client;
        public Conveyor(string port, int baudRate)
        {
            try
            {
                this.port = port;
                this.baudRate = baudRate;
                client = new ModbusClient(port);
                client.Baudrate = baudRate;
                client.Parity = Parity.None;
                client.StopBits = StopBits.One;
                client.Connect();
            }
            catch (Exception ex )
            {
                MessageBox.Show("Error connecting to conveyor: " + ex.Message);
            }
            
        }

        public void turnOn()
        {
            try
            {
                client.WriteSingleRegister(8192, 1); // Assuming register 0 controls the conveyor
                Console.WriteLine("Conveyor turned on.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error turning on conveyor: " + e.Message);
            }

        }

        public void turnOff()
        {
            try
            {
                client.WriteSingleRegister(8192, 5); // Assuming register 0 controls the conveyor
                Console.WriteLine("Conveyor turned off.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error turning off conveyor: " + e.Message);
            }
        }
    }
}
