using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;

namespace KinectColorApp
{
    class GalileoController
    {
        DrawController drawController;
        SerialPort port;
        Thread thread;
        string portName;
        int baudRate;
        bool _continue;
        char prevColor = '0';

        public GalileoController(DrawController dc, string pN, int bR)
        {
            drawController = dc;
            portName = pN;
            baudRate = bR;
            port = new SerialPort(portName, baudRate);
            port.Open();
            _continue = true;
            thread = new Thread(new ThreadStart(listenToSerial));
            thread.Start();
            
        }

        public void closePort()
        {
            _continue = false;
            thread.Join();
            port.Close();
        }

        public void listenToSerial()
        {
            Console.WriteLine("start thread");
            while (_continue)
            {
                try
                {
                    string message = port.ReadLine();
                    char currColor = message[0];
                    //Console.WriteLine("msg: " + currColor);
                    if (currColor != prevColor)
                    {
                        prevColor = currColor;
                        drawController.changeColor((Colors)(int)currColor);
                    }
                    //Console.WriteLine(message);
                }
                catch (Exception e)
                {
                }
            }
        }
    }
}
