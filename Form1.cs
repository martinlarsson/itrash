using Phidgets;
using Phidgets.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Itrash
{
    public partial class Form1Test : Form
    {
        private InterfaceKit ifKit;

        public Form1Test()
        {
            InitializeComponent();
        }

        // Init device
        private void Form_Load(object sender, EventArgs e)
        {
            try
            {
                ifKit = new InterfaceKit();

                ifKit.Attach += new AttachEventHandler(ifKit_Attach);
                ifKit.Detach += new DetachEventHandler(ifKit_Detach);
                ifKit.Error += new ErrorEventHandler(ifKit_Error);

                ifKit.InputChange += new InputChangeEventHandler(ifKit_InputChange);
                ifKit.OutputChange += new OutputChangeEventHandler(ifKit_OutputChange);
                ifKit.SensorChange += new SensorChangeEventHandler(ifKit_SensorChange);
            }
            catch (PhidgetException ignored) {}
        }

        private void ifKit_SensorChange(object sender, SensorChangeEventArgs e)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        private void ifKit_Detach(object sender, DetachEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ifKit_Attach(object sender, AttachEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
