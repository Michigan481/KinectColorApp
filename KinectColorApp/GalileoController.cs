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
        int prevBackground = 0;
        int maxBackground = 3;

        DateTime lastTime;

        public GalileoController(DrawController dc, string pN, int bR)
        {
            lastTime = DateTime.Now;
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
                    if (message[0] == '0' || message[0] == '1' || message[0] == '2' || message[0] == '4')
                    {
                        char currColor = (message[0] == '4' ? '3' : message[0]);
                        //Console.WriteLine("msg: " + currColor);
                        if (currColor != prevColor)
                        {
                            prevColor = currColor;
                            int colorNum = currColor - '0';
                            drawController.changeColor((Colors)colorNum);
                        }
                        //Console.WriteLine(message);
                    }
                    // Change background:
                    else if (message[0] == '3')
                    {
                        TimeSpan timeDiff = DateTime.Now - lastTime;

                        if (timeDiff.TotalMilliseconds > 1000)
                        {
                            int currBackground = prevBackground + 1;
                            if (currBackground == maxBackground)
                            {
                                currBackground = 0;
                            }

                            prevBackground = currBackground;
                            this.drawController.setBackgroundFlag((Backgrounds)currBackground);
                            lastTime = DateTime.Now;
                        }
                        //Thread.Sleep(2000);

                    }
                }
                catch (Exception e)
                {
                }
            }
        }
    }
}
