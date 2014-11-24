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
        char prevBackground = '3';

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
                    Console.WriteLine("MESSAGE IS: " + message[0]);
                    // Change color:
                    if (message[0] == '0' || message[0] == '1' || message[0] == '2')
                    {
                        char currColor = message[0];
                        //Console.WriteLine("msg: " + currColor);
                        if (currColor != prevColor)
                        {
                            prevColor = currColor;
                            drawController.changeColor((Colors)(int)currColor);
                        }
                        //Console.WriteLine(message);
                    }
                    // Change background:
                    else if (message[0] == '3' || message[0] == '4')
                    {
                        char currBackground = message[0];
                        if (currBackground != prevBackground)
                        {
                            prevBackground = currBackground;
                            int backgroundNum = currBackground - '3';
                            // need to subtract 3 to make it 0-indexed
                            drawController.ChangeBackground((Backgrounds)backgroundNum);
                        }

                    }
                }
                catch (Exception e)
                {
                }
            }
        }
    }
}
