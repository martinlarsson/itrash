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
        //Sensors
        private static int SENSOR_IR_ID = 5;
        private static int SENSOR_WEIGHT_ID = 7;
        private static int SENSOR_LEVEL_ID = 4;
        private static int SENSOR_ROTATION_ID = 3;

        //LEDs
        private static int LED_RED = 0;
        private static int LED_GREEN = 3;
        private static int LED_PAPER = 2;
        private static int LED_PET = 6;
        private static int LED_BURN = 7;

        //Sleep
        private static int SLEEP_TIME_MS = 4000;
        private static int SHUTDOWN_TIME_MS = 500;

        //Servo
        private static double SERVO_START_POS = 115.00;
        private static double SERVO_END_POS = 200.00;

        //Thresholds
        private static double DISTANCE_THRESHOLD = 280.00;
        private static double LEVEL_THRESHOLD = 5.00;

        private InterfaceKit ifKit;
        private Servo servo;
        private Boolean open = false;
        private Boolean fullLock = false;
        private int currWeight = 0;

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
            Console.WriteLine("feed: can goes bananas.");
            Console.WriteLine("full: toggle 'full' status.");
            Console.WriteLine("exit: exit program.");
            String input = Console.ReadLine();
            while (!input.Equals("exit"))
            {
                if (input.Equals("feed"))
                {
                    feedMe();
                }
                else if (input.Equals("full"))
                {
                    toggleFull();
                }
                input = Console.ReadLine();
            }

            /*for (int i = 0; i < ifKit.outputs.Count; i++)
            {
                ifKit.outputs[i] = false;
            }*/

            Thread.Sleep(SLEEP_TIME_MS);
            closeCanOnExit();
            Thread.Sleep(SLEEP_TIME_MS);
            servo.close();
            ifKit.close();

            ifKit = null;
            servo = null;
        }

        private void toggleFull()
        {
            fullLock = !fullLock;
            ifKit.outputs[LED_GREEN] = !ifKit.outputs[LED_GREEN];
            ifKit.outputs[LED_RED] = !ifKit.outputs[LED_RED];
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

            // Update the trash type light (turn on the correct one)
            updateTrashLEDs(ifKit.sensors[SENSOR_ROTATION_ID].Value);
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

        /// <summary>
        /// Open can for SLEEP_TIME_MS ms and then automatically close.
        /// </summary>
        private void openCan()
        {
            if (servo.Attached)
            {
                servo.servos[0].Position = SERVO_END_POS;
            }
            Thread.Sleep(SLEEP_TIME_MS);
            if (servo.Attached)
            {
                servo.servos[0].Position = SERVO_START_POS;
            }
            Thread.Sleep(SLEEP_TIME_MS / 2);
            open = false;
        }

        /// <summary>
        /// Called when program exits to make sure can closes.
        /// </summary>
        private void closeCanOnExit()
        {
            open = false;
            ifKit.outputs[LED_GREEN] = false;
            ifKit.outputs[LED_RED] = false;
            ifKit.outputs[LED_PAPER] = false;
            ifKit.outputs[LED_PET] = false;
            ifKit.outputs[LED_BURN] = true;
            if (servo.Attached)
            {
                servo.servos[0].Position = SERVO_START_POS;
            }
        }

        /// <summary>
        /// The can calls for trash! Lights flashing, lid opening/closing repeatedly.
        /// </summary>
        private void feedMe()
        {
            ifKit.outputs[LED_GREEN] = true;
            ifKit.outputs[LED_RED] = false;
            for (int i = 0; i < 10; i++)
            {
                ifKit.outputs[LED_GREEN] = !ifKit.outputs[LED_GREEN];
                ifKit.outputs[LED_RED] = !ifKit.outputs[LED_RED];
                ifKit.outputs[LED_PAPER] = (i % 3 == 0) ? true : false;
                ifKit.outputs[LED_PET] = (i % 3 == 1) ? true : false;
                ifKit.outputs[LED_BURN] = (i % 3 == 2) ? false : true;
                servo.servos[0].Position = (i % 2 == 0) ? SERVO_START_POS : SERVO_END_POS;
                Thread.Sleep(300);
            }

            // Clean up
            /*ifKit.outputs[LED_GREEN] = true;
            ifKit.outputs[LED_RED] = false;
            ifKit.outputs[LED_PAPER] = true;
            ifKit.outputs[LED_PET] = false;
            ifKit.outputs[LED_BURN] = true;*/
            updateTrashLEDs(ifKit.sensors[SENSOR_ROTATION_ID].Value);
            servo.servos[0].Position = SERVO_START_POS;
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
        /// Lights up the green led and turns off the red led
        /// </summary>
        private void notFullCan()
        {
            ifKit.outputs[LED_GREEN] = true;
            ifKit.outputs[LED_RED] = false;
        }

        /// <summary>
        /// Light up the correct LED indicating trash type based on rotation value
        /// </summary>
        /// <param name="e">Event arguments</param>
        private void changeTrashType(SensorChangeEventArgs e)
        {
            updateTrashLEDs(e.Value);
        }

        private void updateTrashLEDs(int val)
        {
            if (val > 750)
            {
                ifKit.outputs[LED_PAPER] = true;
                ifKit.outputs[LED_PET] = false;
                ifKit.outputs[LED_BURN] = true;
            }
            else if (val < 500)
            {
                ifKit.outputs[LED_PAPER] = false;
                ifKit.outputs[LED_PET] = false;
                ifKit.outputs[LED_BURN] = false;
            }
            else
            {
                ifKit.outputs[LED_PAPER] = false;
                ifKit.outputs[LED_PET] = true;
                ifKit.outputs[LED_BURN] = true;
            }
        }

        /// <summary>
        /// Event handler for change in sensor reading from interface kit
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event arguments</param>
        private void ifKit_SensorChange(object sender, SensorChangeEventArgs e)
        {
            if (e.Index == SENSOR_WEIGHT_ID)
            {
                this.currWeight = e.Value;
            }
            else if (e.Index == SENSOR_IR_ID && e.Value > DISTANCE_THRESHOLD)
            {
                if (!open)
                {
                    open = true;
                    Thread lidThread = new Thread(new ThreadStart(this.openCan));
                    lidThread.Start();
                }
            } 
            else if (e.Index == SENSOR_LEVEL_ID && e.Value > LEVEL_THRESHOLD && !open && currWeight > 150)
            {
                fullCan();
            }
            else if (e.Index == SENSOR_ROTATION_ID)
            {
                changeTrashType(e);
            }

            else if (currWeight < 150 && !open && !fullLock)
            {
                notFullCan();
            }
        }

        /// <summary>
        /// Event handler for change in interface output
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event arguments</param>
        private void ifKit_OutputChange(object sender, OutputChangeEventArgs e)
        {
            //Console.WriteLine("Output index {0} value {1}", e.Index, e.Value.ToString());
        }

        /// <summary>
        /// Event handler for change in interface input
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event arguments</param>
        private void ifKit_InputChange(object sender, InputChangeEventArgs e)
        {
            //Console.WriteLine("Input index {0} value {1}", e.Index, e.Value.ToString());
        }

        /// <summary>
        /// Error handler for interface kit
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event arguments</param>
        private void ifKit_Error(object sender, ErrorEventArgs e)
        {
            Console.WriteLine(e.Description);
        }

        /// <summary>
        /// Detach event handler for interface kit
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event arguments</param>
        private void ifKit_Detach(object sender, DetachEventArgs e)
        {
            Console.WriteLine("InterfaceKit {0} detached!", e.Device.SerialNumber.ToString());
        }

        /// <summary>
        /// Attach event handler for interface kit
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event arguments</param>
        private void ifKit_Attach(object sender, AttachEventArgs e)
        {
            Console.WriteLine("InterfaceKit {0} attached!", e.Device.SerialNumber.ToString());
        }

        /// <summary>
        /// Position change handler for servo
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event arguments</param>
        private void servo_PositionChange(object sender, PositionChangeEventArgs e)
        {
            //Console.WriteLine("Servo {0} Position {1}", e.Index, e.Position);
        }

        /// <summary>
        /// Error event handler for servo
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event arguments</param>
        private void servo_Error(object sender, ErrorEventArgs e)
        {
            Console.WriteLine(e.Description);
        }

        /// <summary>
        /// Detach event handler for servo
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event arguments</param>
        private void servo_Detach(object sender, DetachEventArgs e)
        {
            Console.WriteLine("Detached" + e.Device);
        }

        /// <summary>
        /// Attach event handler for servo
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event arguments</param>
        private void servo_Attach(object sender, AttachEventArgs e)
        {
            Console.WriteLine("Attached " + e.Device);
        }
    }
}
