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
        private static int SENSOR_IR = 5;
        private static int SENSOR_WEIGHT = 7;
        private static int SENSOR_LEVEL = 6;
        private static int LED_RED = 1;
        private static int LED_GREEN = 3;
        private static int SLEEP_TIME_MS = 4000;

        private static double SERVO_START_POS = 115.00;
        private static double SERVO_END_POS = 200.00;
        private static double DISTANCE_THRESHOLD = 280.00;
        private static double LEVEL_THRESHOLD = 200.00;

        private InterfaceKit ifKit;
        private Servo servo;
        private Boolean open = false;

        /// <summary>
        /// Initialize servo and interface kit.
        /// </summary>
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

        /// <summary>
        /// Waits for input from keyboard to exit.
        /// </summary>
        private void keepAlive()
        {
            Console.WriteLine("Press any key to end...");
            Console.Read();

            for (int i = 0; i < ifKit.outputs.Count; i++)
            {
                ifKit.outputs[i] = false;
            }

            //closeCanOnExit();
            servo.close();
            ifKit.close();

            ifKit = null;
            servo = null;
        }

        /// <summary>
        /// Initialize interface kit
        /// </summary>
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
            ifKit.outputs[LED_GREEN] = true;
        }

        /// <summary>
        /// Initialize servo
        /// </summary>
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

        /// <summary>
        /// Open can for 4 seconds and then automatically close.
        /// </summary>
        private void openCan()
        {
            if (servo.Attached)
            {
                servo.servos[0].Position = SERVO_END_POS;
            }
            Console.WriteLine("Servo's position set to 210.00");
            Thread.Sleep(SLEEP_TIME_MS);
            if (servo.Attached)
            {
                servo.servos[0].Position = SERVO_START_POS;
            }
            open = false;
        }

        /// <summary>
        /// Called when program exits to make sure can closes.
        /// </summary>
        private void closeCanOnExit()
        {
            Thread.Sleep(SLEEP_TIME_MS);
            if (servo.Attached)
            {
                servo.servos[0].Position = SERVO_START_POS;
            }
            open = false;
        }

        /// <summary>
        /// Called when any sensor reads a new value
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">SensorChangeEventArgs</param>
        private void ifKit_SensorChange(object sender, SensorChangeEventArgs e)
        {
            if (e.Index == SENSOR_IR && e.Value > DISTANCE_THRESHOLD)
            {
                if (!open)
                {
                    open = true;
                    Thread lidThread = new Thread(new ThreadStart(this.openCan));
                    lidThread.Start();
                }
            } else if (e.Index == SENSOR_WEIGHT)
            {
                Console.WriteLine("Sensor index {0} value {1}", e.Index, e.Value.ToString());
            } else if (e.Index == SENSOR_LEVEL && e.Value > LEVEL_THRESHOLD)
            {
                fullCan();
            }
        }

        /// <summary>
        /// Lights up the red led and turns off the green led.
        /// </summary>
        private void fullCan()
        {
            ifKit.outputs[LED_GREEN] = false;
            ifKit.outputs[LED_RED] = true;
        }

        /// <summary>
        /// Lights up the green led and turns off the red led.
        /// </summary>
        private void emptyCan()
        {
            ifKit.outputs[LED_GREEN] = true;
            ifKit.outputs[LED_RED] = false;
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
