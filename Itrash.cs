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
                initInterFaceKit();
                //initServo();
            }
            catch (PhidgetException pex)
            {
                Console.WriteLine(pex.Description);
            }
        }

        private void initInterFaceKit()
        {
            // Initialize the InterfaceKit object
            ifKit = new InterfaceKit();

            // Hook the basic event-handlers
            ifKit.Attach += new AttachEventHandler(ifKit_Attach);
            ifKit.Detach += new DetachEventHandler(ifKit_Detach);
            ifKit.Error += new ErrorEventHandler(ifKit_Error);

            // Hook the InterfaceKit event-handlers
            ifKit.InputChange += new InputChangeEventHandler(ifKit_InputChange);
            ifKit.OutputChange += new OutputChangeEventHandler(ifKit_OutputChange);
            ifKit.SensorChange += new SensorChangeEventHandler(ifKit_SensorChange);

            // Open the object for device connections
            ifKit.open();

            // Wait for an InterfaceKit phidget to be attached
            Console.WriteLine("Press any key to end...");
            Console.Read();

            // User input was read so we'll terminate the program, so close the object
            ifKit.close();

            // Set the object to null to get it out of memory
            ifKit = null;
        }

        private void initServo()
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
            Console.WriteLine("Servo's position set to 15.00");
            Console.WriteLine("Press any key to continue...");
            Console.Read();
            if (servo.Attached)
            {
                servo.servos[0].Position = 115;
            }



            servo.close();
            servo = null;
        }
        /// <summary>
        /// Du kan va en summary!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void servo_PositionChange(object sender, PositionChangeEventArgs e)
        {
            Console.WriteLine("Servo {0} Position {1}", e.Index, e.Position);
        }

        private void servo_Error(object sender, ErrorEventArgs e)
        {
            Console.WriteLine(e.Description);
        }

        private void servo_Detach(object sender, DetachEventArgs e)
        {
            Console.WriteLine("Detached"+e.Device);
        }

        private void servo_Attach(object sender, AttachEventArgs e)
        {
            Console.WriteLine("Attached "+e.Device);
        }

        //public void run()
        //{
           // ifKit.open();
            //Console.WriteLine("Awaiting Interface Kit attachment...");
            //ifKit.waitForAttachment();
            //Console.WriteLine("Nr of sensors: " + ifKit.sensors.Count);
            //ifKit.close();
            //ifKit = null;
        //}

        private void ifKit_SensorChange(object sender, SensorChangeEventArgs e)
        {
            Console.WriteLine("Sensor index {0} value {1}", e.Index, e.Value.ToString());
        }

        private void ifKit_OutputChange(object sender, OutputChangeEventArgs e)
        {
            Console.WriteLine("Output index {0} value {1}", e.Index, e.Value.ToString());
        }

        private void ifKit_InputChange(object sender, InputChangeEventArgs e)
        {
            Console.WriteLine("Input index {0} value {1}", e.Index, e.Value.ToString());
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
