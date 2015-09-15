using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DeltaKinematics.Core;

namespace deltaKinematics
{
    public partial class Form1
    {
        public void SetVariablesAll()
        {
            if (_serialPort.IsOpen)
            {
                accuracy = Convert.ToDouble(textAccuracy.Text);
                accuracy2 = Convert.ToDouble(textAccuracy2.Text);

                Iterations.MaxIterations = int.Parse(textMaxIterations.Text);
                pauseTimeSet = int.Parse(textPauseTimeSet.Text);
                probingHeight = int.Parse(textProbingHeight.Text);

                HRadRatio = Convert.ToDouble(textHRadRatio.Text);

                //XYZ offset
                //X
                xxOppPerc = Convert.ToDouble(textxxOppPerc.Text);
                xyPerc = Convert.ToDouble(textxyPerc.Text);
                xyOppPerc = Convert.ToDouble(textxyOppPerc.Text);
                xzPerc = Convert.ToDouble(textxzPerc.Text);
                xzOppPerc = Convert.ToDouble(textxzOppPerc.Text);

                //Y
                yyOppPerc = Convert.ToDouble(textyyOppPerc.Text);
                yxPerc = Convert.ToDouble(textyxPerc.Text);
                yxOppPerc = Convert.ToDouble(textyxOppPerc.Text);
                yzPerc = Convert.ToDouble(textyzPerc.Text);
                yzOppPerc = Convert.ToDouble(textyzOppPerc.Text);

                //Z
                zzOppPerc = Convert.ToDouble(textzzOppPerc.Text);
                zxPerc = Convert.ToDouble(textzxPerc.Text);
                zxOppPerc = Convert.ToDouble(textzxOppPerc.Text);
                zyPerc = Convert.ToDouble(textzyPerc.Text);
                zyOppPerc = Convert.ToDouble(textzyOppPerc.Text);

                //diagonal rod
                deltaTower = Convert.ToDouble(textDeltaTower.Text);
                deltaOpp = Convert.ToDouble(textDeltaOpp.Text);

                zProbeSpeed = int.Parse(textProbingSpeed.Text);

                _serialPort.WriteLine("M206 T3 P812 X" + textProbingSpeed.Text.ToString());
                _serialPort.WriteLine("M206 T3 808 X" + textZProbeHeight.Text.ToString());

                LogConsole("Setting Z-Probe Speed\n");
                LogConsole("Setting Z-Probe Height\n");
                Thread.Sleep(pauseTimeSet);

                LogConsole("Variables set\n");
            }
            else
            {
                LogConsole("Not Connected\n");
            }
        }

        public void SetHeights()
        {
            //set base heights for advanced calibration comparison
            if (Iterations.IterationNum == 0)
            {
                Invoke((MethodInvoker)delegate { this.textXTemp.Text = Math.Round(ProbeHeight.X, 3).ToString(); });
                Invoke((MethodInvoker)delegate { this.textXOppTemp.Text = Math.Round(ProbeHeight.XOpp, 3).ToString(); });
                Invoke((MethodInvoker)delegate { this.textYTemp.Text = Math.Round(ProbeHeight.Y, 3).ToString(); });
                Invoke((MethodInvoker)delegate { this.textYOppTemp.Text = Math.Round(ProbeHeight.YOpp, 3).ToString(); });
                Invoke((MethodInvoker)delegate { this.textZTemp.Text = Math.Round(ProbeHeight.Z, 3).ToString(); });
                Invoke((MethodInvoker)delegate { this.textZOppTemp.Text = Math.Round(ProbeHeight.ZOpp, 3).ToString(); });

                //calculate parameters
                TempProbeHeight.X = ProbeHeight.X;
                TempProbeHeight.XOpp = ProbeHeight.XOpp;
                TempProbeHeight.Y = ProbeHeight.Y;
                TempProbeHeight.YOpp = ProbeHeight.YOpp;
                TempProbeHeight.Z = ProbeHeight.Z;
                TempProbeHeight.ZOpp = ProbeHeight.ZOpp;
            }
            else
            {
                Invoke((MethodInvoker)delegate { this.textX.Text = Math.Round(ProbeHeight.X, 3).ToString(); });
                Invoke((MethodInvoker)delegate { this.textXOpp.Text = Math.Round(ProbeHeight.XOpp, 3).ToString(); });
                Invoke((MethodInvoker)delegate { this.textY.Text = Math.Round(ProbeHeight.Y, 3).ToString(); });
                Invoke((MethodInvoker)delegate { this.textYOpp.Text = Math.Round(ProbeHeight.YOpp, 3).ToString(); });
                Invoke((MethodInvoker)delegate { this.textZ.Text = Math.Round(ProbeHeight.Z, 3).ToString(); });
                Invoke((MethodInvoker)delegate { this.textZOpp.Text = Math.Round(ProbeHeight.ZOpp, 3).ToString(); });
            }
        }

        public void SetAdvancedCalVars()
        {
            Invoke((MethodInvoker)delegate { this.textDeltaTower.Text = Math.Round(deltaTower, 3).ToString(); });
            Invoke((MethodInvoker)delegate { this.textDeltaOpp.Text = Math.Round(deltaOpp, 3).ToString(); });
            Invoke((MethodInvoker)delegate { this.textHRadRatio.Text = Math.Round(HRadRatio, 3).ToString(); });

            Invoke((MethodInvoker)delegate { this.textxxPerc.Text = Math.Round(offsetXCorrection, 3).ToString(); });
            Invoke((MethodInvoker)delegate { this.textxxOppPerc.Text = Math.Round(xxOppPerc, 3).ToString(); });
            Invoke((MethodInvoker)delegate { this.textxyPerc.Text = Math.Round(xyPerc, 3).ToString(); });
            Invoke((MethodInvoker)delegate { this.textxyOppPerc.Text = Math.Round(xyOppPerc, 3).ToString(); });
            Invoke((MethodInvoker)delegate { this.textxzPerc.Text = Math.Round(xzPerc, 3).ToString(); });
            Invoke((MethodInvoker)delegate { this.textxzOppPerc.Text = Math.Round(xzOppPerc, 3).ToString(); });

            Invoke((MethodInvoker)delegate { this.textyyPerc.Text = Math.Round(offsetYCorrection, 3).ToString(); });
            Invoke((MethodInvoker)delegate { this.textyyOppPerc.Text = Math.Round(yyOppPerc, 3).ToString(); });
            Invoke((MethodInvoker)delegate { this.textyxPerc.Text = Math.Round(yxPerc, 3).ToString(); });
            Invoke((MethodInvoker)delegate { this.textyxOppPerc.Text = Math.Round(yxOppPerc, 3).ToString(); });
            Invoke((MethodInvoker)delegate { this.textyzPerc.Text = Math.Round(yzPerc, 3).ToString(); });
            Invoke((MethodInvoker)delegate { this.textyzOppPerc.Text = Math.Round(yzOppPerc, 3).ToString(); });

            Invoke((MethodInvoker)delegate { this.textzzPerc.Text = Math.Round(offsetZCorrection, 3).ToString(); });
            Invoke((MethodInvoker)delegate { this.textzzOppPerc.Text = Math.Round(zzOppPerc, 3).ToString(); });
            Invoke((MethodInvoker)delegate { this.textzxPerc.Text = Math.Round(zxPerc, 3).ToString(); });
            Invoke((MethodInvoker)delegate { this.textzxOppPerc.Text = Math.Round(zxOppPerc, 3).ToString(); });
            Invoke((MethodInvoker)delegate { this.textzyPerc.Text = Math.Round(zyPerc, 3).ToString(); });
            Invoke((MethodInvoker)delegate { this.textzyOppPerc.Text = Math.Round(zyOppPerc, 3).ToString(); });
        }

        //prints to printer console
        public void LogMessage(string value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(LogMessage), new object[] { value });
                return;
            }
            printerConsoleTextBox.AppendText(value + "\n");
        }

        //prints to console
        public void LogConsole(string value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(LogConsole), new object[] { value });
                return;
            }
            consoleTextBox.AppendText(value + "\n");
        }

        private void FetchEeProm()
        {
            if (int.Parse(textBox4.Text) > 50)
            {
                // TODO: make sure the user has entered a plate diameter!
                plateDiameter = int.Parse(textBox4.Text);

                // Replace later
                comboBoxZMinimumValue = comboZMin.SelectedItem.ToString();

                // Read EEPROM
                _serialPort.WriteLine("M205");
                LogConsole("Request to read EEPROM sent\n");
                _initiatingCalibration = true;
            }
            else
            {
                LogConsole("Please enter your build plate diameter and try again\n");
            }
        }
    }
}
