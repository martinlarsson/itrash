using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Phidgets;
using Phidgets.Events;
using System.Threading;

namespace Itrash
{
    class Itrash 
    {
        private InterfaceKit ifKit;
        private Servo servo;

        public Itrash()
        {
            try
            {
                servo = new Servo();
                servo.Attach += new AttachEventHandler(servo_Attach);
                servo.Detach += new DetachEventHandler(servo_Detach);
                servo.Error += new ErrorEventHandler(servo_Error);
                servo.PositionChange += new PositionChangeEventHandler(servo_PositionChange);
                servo.open();
                Console.WriteLine("Awaiting Servo attachment..");
                servo.waitForAttachment();
                if (servo.Attached)
                {
                    servo.servos[0].Position = 15.00; 
                }
                Console.WriteLine("Servo's position set to 0.00");
                Console.WriteLine("Press any key to continue...");
                Console.Read();
                if (servo.Attached)
                {
                    for (double i = 0.00; i < 110.00; i++)
                    {
                        Thread.Sleep(10);
                        servo.servos[0].Position = i;
                    }
                }
                servo.close();
                servo = null;

                /*
                ifKit = new InterfaceKit();
                ifKit.Attach += new AttachEventHandler(ifKit_Attach);
                ifKit.Detach += new DetachEventHandler(ifKit_Detach);
                ifKit.Error += new ErrorEventHandler(ifKit_Error);
                ifKit.InputChange += new InputChangeEventHandler(ifKit_InputChange);
                ifKit.OutputChange += new OutputChangeEventHandler(ifKit_OutputChange);
                ifKit.SensorChange += new SensorChangeEventHandler(ifKit_SensorChange);
                ifKit = null;
                */
            }
            catch (PhidgetException pex)
            {
                Console.WriteLine(pex.Description);
            }
        }

        private void servo_PositionChange(object sender, PositionChangeEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void servo_Error(object sender, ErrorEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void servo_Detach(object sender, DetachEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void servo_Attach(object sender, AttachEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void run()
        {
            ifKit.open();
            Console.WriteLine("Awaiting Interface Kit attachment...");
            ifKit.waitForAttachment();
            Console.WriteLine("Nr of sensors: " + ifKit.sensors.Count);
            ifKit.close();
            ifKit = null;
        }

        private void ifKit_SensorChange(object sender, SensorChangeEventArgs e)
        {
            Console.WriteLine("Sensor index {0} value {1}", e.Index, e.Index);
        }

        private void ifKit_OutputChange(object sender, OutputChangeEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ifKit_InputChange(object sender, InputChangeEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ifKit_Error(object sender, ErrorEventArgs e)
        {
            Console.WriteLine(e.Description);
        }

        private void ifKit_Detach(object sender, DetachEventArgs e)
        {
            Console.WriteLine("InterfaceKit {0} detached!", e.Device.SerialNumber.ToString());
        }

        private void ifKit_Attach(object sender, AttachEventArgs e)
        {
            Console.WriteLine("InterfaceKit {0} attached!", e.Device.SerialNumber.ToString());
        }
        
    }
}
