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
        public Boolean open = false;

        public Itrash()
        {
            try
            {
                initServo();
                initInterFaceKit();
                keepAlive();
            }
            catch (PhidgetException pex)
            {
                Console.WriteLine(pex.Description);
            }
        }

        private void keepAlive()
        {
            // Wait for an InterfaceKit phidget to be attached
            Console.WriteLine("Press any key to end...");
            Console.Read();

            for (int i = 0; i < ifKit.outputs.Count; i++)
            {
                ifKit.outputs[i] = false;
            }

            // User input was read so we'll terminate the program, so close the object
            ifKit.close();

            // Set the object to null to get it out of memory
            ifKit = null;
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

            // Turn on the green status-light on the front side of the iTrash
            Thread.Sleep(500);
            ifKit.outputs[3] = true;
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
        }

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

        public void openCan()
        {
            if (servo.Attached)
            {
                servo.servos[0].Position = 200.00;
            }
            Console.WriteLine("Servo's position set to 210.00");
            Thread.Sleep(4000);
            if (servo.Attached)
            {
                servo.servos[0].Position = 115.00;
            }
            open = false;

        }

        private void ifKit_SensorChange(object sender, SensorChangeEventArgs e)
        {

            if (e.Index == 5 && e.Value > 280.00)
            {
                if (!open)
                {
                    open = true;
                    Thread openThread = new Thread(new ThreadStart(this.openCan));
                    openThread.Start();
               
                }
            
                
            }
            if (e.Index==4)
            {
                Console.WriteLine("Sensor index {0} value {1}", e.Index, e.Value.ToString());
            }





            if (e.Index == 6 && e.Value > 200)
            {
                
                fullCan();
            }
            //Console.WriteLine("Sensor index {0} value {1}", e.Index, e.Value.ToString());
        }

        private void fullCan()
        {
            ifKit.outputs[3] = false;
            ifKit.outputs[1] = true;
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
