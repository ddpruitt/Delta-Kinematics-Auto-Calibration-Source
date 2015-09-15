using System;
using System.ComponentModel;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;

namespace deltaKinematics
{
    public partial class Form1
    {
        // Connect to printer.
        private void connectButton_Click(object sender, EventArgs e)
        {
            if (_serialPort.IsOpen)
            {
                LogConsole("Already Connected\n");
            }
            else
            {
                try
                {
                    // Opens a new thread if there has been a previous thread that has closed.
                    if (readThread.IsAlive == false)
                    {
                        readThread = new Thread(Read);
                        _serialPort = new SerialPort();
                    }

                    _serialPort.PortName = portComboBox.Text;
                    _serialPort.BaudRate = int.Parse(cboBaudRate.Text);

                    // Set the read/write timeouts.
                    _serialPort.ReadTimeout = 500;
                    _serialPort.WriteTimeout = 500;

                    // Open the serial port and start reading on a reader thread.
                    // _continue is a flag used to terminate the app.

                    if (_serialPort.BaudRate != 0 && _serialPort.PortName != "")
                    {
                        _serialPort.Open();
                        _continue = true;

                        readThread.Start();
                        LogConsole("Connected\n");
                    }
                    else
                    {
                        LogConsole("Please fill all text boxes above\n");
                    }
                }
                catch (Exception e1)
                {
                    _continue = false;
                    readThread.Join();
                    _serialPort.Close();
                    LogConsole(e1.Message + "\n");
                }
            }
        }

        // Disconnect from printer.
        private void disconnectButton_Click(object sender, EventArgs e)
        {
            if (_serialPort.IsOpen)
            {
                try
                {
                    _continue = false;
                    readThread.Join();
                    _serialPort.Close();
                    LogConsole("Disconnected\n");
                }
                catch (Exception e1)
                {
                    LogConsole(e1.Message + "\n");
                }
            }
            else
            {
                LogConsole("Not Connected\n");
            }
        }

        // Send gcode to printer.
        private void sendGCodeButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (_serialPort.IsOpen)
            {
                string text = textBox1.Text.ToString().ToUpper();
                _serialPort.WriteLine(text + "\n");
            }
            else
            {
                LogConsole("Not Connected\n");
            }
        }

        // Clear logs.
        private void clearLogsButton_Click(object sender, EventArgs e)
        {
            printerConsoleTextBox.Text = "";
            consoleTextBox.Text = "";
        }

        // Calibrate.
        private void calibrateButton_Click(object sender, EventArgs e)
        {
            if (_serialPort.IsOpen)
            {
                SetVariablesAll();

                calibrationState = 0;
                advancedCalibration = 0;

                //fetches EEProm
                FetchEeProm();
                //LogMessage (_eepromString);
            }
            else
            {
                LogConsole("Not Connected\n");
            }
        }

        // Reset printer.
        private void resetPrinterButton_Click(object sender, EventArgs e)
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.WriteLine("M112");
            }
            else
            {
                LogConsole("Not Connected\n");
            }
        }

        // Donate.
        private void donateButton_Click(object sender, EventArgs e)
        {
            string url = "";

            string business = "steventrowland@gmail.com"; // your paypal email
            string description = "Donation"; // '%20' represents a space. remember HTML!
            string country = "US"; // AU, US, etc.
            string currency = "USD"; // AUD, USD, etc.

            url += "https://www.paypal.com/cgi-bin/webscr" +
                   "?cmd=" + "_donations" +
                   "&business=" + business +
                   "&lc=" + country +
                   "&item_name=" + description +
                   "&currency_code=" + currency +
                   "&bn=" + "PP%2dDonationsBF";

            System.Diagnostics.Process.Start(url);
        }

        // Contact.
        private void contactButton_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = "mailto:steventrowland@gmail.com";
            proc.Start();
        }

        // Version information.
        private void versionButton_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("Version: " + versionState +
                                                 "\n\nCreated by Steven T. Rowland\nsteventrowland@gmail.com\n");
        }

        // Open advanced panel.
        private void openAdvancedPanelButton_Click(object sender, EventArgs e)
        {
            //System.Windows.Forms.MessageBox.Show("Advanced settings will be available in Version: 2.0.1\nFeatures:\n-Accuracy control\n-Max iterations\n-Delta radius offset percentages\n-Horizontal Radius offset percentages\n-And more");

            if (advancedPanel.Visible == false)
            {
                advancedPanel.Visible = true;
            }
            else
            {
                advancedPanel.Visible = false;
                panelAdvancedMore.Visible = false;
                XYPanel1.Visible = false;
            }
        }

        //starts basic offset learning calibration
        private void basicCalibration_Click(object sender, EventArgs e)
        {
            if (_serialPort.IsOpen)
            {
                SetVariablesAll();

                calibrationState = 0;
                advancedCalibration = 1;
                //fetches EEProm
                FetchEeProm();
            }
            else
            {
                LogConsole("Not Connected\n");
            }
        }

        // Start heuristic calibration.
        private void advancedCalibrationButton_Click(object sender, EventArgs e)
        {
            if (_serialPort.IsOpen)
            {
                SetVariablesAll();

                calibrationState = 1;
                advancedCalibration = 1;
                //fetches EEProm
                FetchEeProm();
            }
            else
            {
                LogConsole("Not Connected\n");
            }
        }

        // Open "more" panel.
        private void openMorePanelButton_Click(object sender, EventArgs e)
        {
            if (panelAdvancedMore.Visible == false)
            {
                panelAdvancedMore.Visible = true;
            }
            else
            {
                panelAdvancedMore.Visible = false;
                XYPanel1.Visible = false;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        //
        private void Form1_Load(object sender, EventArgs e)
        {

        }


        //
        private void Form1_Load_1(object sender, EventArgs e)
        {

        }

        //
        private void textMaxIterations_TextChanged(object sender, EventArgs e)
        {

        }

        private void consoleTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void scalingXYDisplay_Click(object sender, EventArgs e)
        {
            if (XYPanel1.Visible == false)
            {
                XYPanel1.Visible = true;
            }
            else
            {
                XYPanel1.Visible = false;
            }
        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void label21_Click(object sender, EventArgs e)
        {

        }

        private void cboBaudRate_Validating(object sender, CancelEventArgs e)
        {
            if (!cboBaudRate.Items.Contains(cboBaudRate.Text))
            {
                LogConsole("Invalid baud rate selected!\n");
                e.Cancel = false; // if this is true, the user can't leave the control.
            }
        }

        /*
        //
        private void displaydata_event(object sender, EventArgs e)
        {
            string testInData = message.ToString();

            if (testInData != wait)
            {
                //printerConsoleTextBox.AppendText(inData + "\n");
            }
        }
        */

    }
}