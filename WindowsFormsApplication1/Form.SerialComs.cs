using System;
using System.Threading;
using DeltaKinematics.Core;

namespace deltaKinematics
{
    public partial class Form1
    {

        // The reader thread. Continue reading as long as _continue is true.
        public void Read()
        {
            while (_continue)
            {
                try
                {
                    string message = _serialPort.ReadLine();

                    if (!_initiatingCalibration)
                    {
                        LogMessage(message + "\n");
                    }
                    else
                    {
                        LogMessage(message + "\n");

                        double zProbe = 0;
                        if (message.Contains("Z-probe:"))
                        {
                            //Z-probe: 10.66 zCorr: 0

                            string[] parseInData = message.Split(new char[] { ' ', '\n' },
                                StringSplitOptions.RemoveEmptyEntries);
                            string[] parseFirstLine = parseInData[0].Split(new char[] { ':', '\n' },
                                StringSplitOptions.RemoveEmptyEntries);

                            //: 10.66 zCorr: 0
                            string[] parseZProbe = message.Split(new string[] { "Z-probe", "\n" },
                                StringSplitOptions.RemoveEmptyEntries);
                            string[] parseZProbeSpace = parseZProbe[0].Split(new char[] { ' ', '\n' },
                                StringSplitOptions.RemoveEmptyEntries);

                            double zProbeParse;

                            //check if there is a space between
                            if (parseZProbeSpace[0] == ":")
                            {
                                //Space
                                zProbeParse = double.Parse(parseZProbeSpace[1]);
                            }
                            else
                            {
                                //No space
                                zProbeParse = double.Parse(parseZProbeSpace[0].Substring(1));
                            }

                            //use returned probe height to calculate the actual z-Probe height
                            if (zProbeSet == 1)
                            {
                                LogConsole("Z-Probe length set to: " +
                                           (zMaxLength - Convert.ToDouble(parseFirstLine[1])) + "\n");
                                zProbe = zMaxLength - Convert.ToDouble(parseFirstLine[1]);
                                zProbeSet = 0;
                            }
                            else if (Iterations.CenterIterations == Iterations.IterationNum)
                            {
                                //LogMessage("Z-Probe Center Height: " + parseFirstLine[1] + "\n");
                                centerHeight = Convert.ToDouble(parseFirstLine[1]);

                                Iterations.CenterIterations++;

                                _serialPort.WriteLine("G1 Z" + probingHeight.ToString() + " X0 Y0");

                                Thread.Sleep(pauseTimeSet);
                                //X axis
                                _serialPort.WriteLine("G1 Z" + probingHeight.ToString() + " X-" +
                                                      valueXYLarge.ToString() + " Y-" + valueXYSmall.ToString());
                                Thread.Sleep(pauseTimeSet);
                                _serialPort.WriteLine("G30");
                            }
                            else if (Iterations.XIterations == Iterations.IterationNum)
                            {
                                //LogMessage("Z-Probe X Height: " + parseFirstLine[1] + "\n");
                                ProbeHeight.X = Convert.ToDouble(parseFirstLine[1]);

                                Iterations.XIterations++;

                                ProbeX0Y0();
                            }
                            else if (Iterations.XOppIterations == Iterations.IterationNum)
                            {
                                //LogMessage("Z-Probe X Opposite Height: " + parseFirstLine[1] + "\n");
                                ProbeHeight.XOpp = Convert.ToDouble(parseFirstLine[1]);

                                Iterations.XOppIterations++;

                                _serialPort.WriteLine("G1 Z" + probingHeight.ToString() + " X" + valueXYLarge.ToString() +
                                                      " Y" + valueXYSmall.ToString());
                                Thread.Sleep(pauseTimeSet);
                                _serialPort.WriteLine("G1 Z" + probingHeight.ToString() + " X0 Y0");
                                Thread.Sleep(pauseTimeSet);

                                //Y axis
                                _serialPort.WriteLine("G1 Z" + probingHeight.ToString() + " X" + valueXYLarge.ToString() +
                                                      " Y-" + valueXYSmall.ToString());
                                Thread.Sleep(pauseTimeSet);
                                _serialPort.WriteLine("G30");
                            }
                            else if (Iterations.YIterations == Iterations.IterationNum)
                            {
                                //LogMessage("Z-Probe Y Height: " + parseFirstLine[1] + "\n");
                                ProbeHeight.Y = Convert.ToDouble(parseFirstLine[1]);

                                Iterations.YIterations++;

                                ProbeX0Y0();
                            }
                            else if (Iterations.YOppIterations == Iterations.IterationNum)
                            {
                                //LogMessage("Z-Probe Y Opposite Height: " + parseFirstLine[1] + "\n");
                                ProbeHeight.YOpp = Convert.ToDouble(parseFirstLine[1]);

                                Iterations.YOppIterations++;

                                _serialPort.WriteLine("G1 Z" + probingHeight.ToString() + " X-" +
                                                      valueXYLarge.ToString() + " Y" + valueXYSmall.ToString());
                                Thread.Sleep(pauseTimeSet);
                                _serialPort.WriteLine("G1 Z" + probingHeight.ToString() + " X0 Y0");
                                Thread.Sleep(pauseTimeSet);

                                //Z axis
                                _serialPort.WriteLine("G1 Z" + probingHeight.ToString() + " Y" + valueZ.ToString());
                                Thread.Sleep(pauseTimeSet);
                                _serialPort.WriteLine("G30");
                            }
                            else if (Iterations.ZIterations == Iterations.IterationNum)
                            {
                                //LogMessage("Z-Probe Z Height: " + parseFirstLine[1] + "\n");
                                ProbeHeight.Z = Convert.ToDouble(parseFirstLine[1]);

                                Iterations.ZIterations++;

                                _serialPort.WriteLine("G1 Z" + probingHeight.ToString() + " Y" + valueZ.ToString());
                                Thread.Sleep(pauseTimeSet);
                                _serialPort.WriteLine("G1 Z" + probingHeight.ToString() + " X0 Y0");
                                Thread.Sleep(pauseTimeSet);
                                _serialPort.WriteLine("G1 Z" + probingHeight.ToString() + " Y-" + valueZ.ToString());
                                Thread.Sleep(pauseTimeSet);
                                _serialPort.WriteLine("G30");
                            }
                            else if (Iterations.ZOppIterations == Iterations.IterationNum)
                            {
                                //LogMessage("Z-Probe Z Opposite Height: " + parseFirstLine[1] + "\n");
                                ProbeHeight.ZOpp = Convert.ToDouble(parseFirstLine[1]);

                                _serialPort.WriteLine("G1 Z" + probingHeight.ToString() + " Y-" + valueZ.ToString());
                                Thread.Sleep(pauseTimeSet);
                                _serialPort.WriteLine("G1 Z" + probingHeight.ToString() + " X0 Y0");
                                Thread.Sleep(pauseTimeSet);

                                /*
                                //prevents double homing
                                if (advancedCalibration != 1)
                                {
                                    _serialPort.WriteLine("G28");
                                }
                                */

                                centerHeight = zMaxLength - probingHeight + centerHeight;
                                ProbeHeight.X = centerHeight - (zMaxLength - probingHeight + ProbeHeight.X);
                                ProbeHeight.XOpp = centerHeight - (zMaxLength - probingHeight + ProbeHeight.XOpp);
                                ProbeHeight.Y = centerHeight - (zMaxLength - probingHeight + ProbeHeight.Y);
                                ProbeHeight.YOpp = centerHeight - (zMaxLength - probingHeight + ProbeHeight.YOpp);
                                ProbeHeight.Z = centerHeight - (zMaxLength - probingHeight + ProbeHeight.Z);
                                ProbeHeight.ZOpp = centerHeight - (zMaxLength - probingHeight + ProbeHeight.ZOpp);
                                //centerHeight = 0;

                                //invert values
                                ProbeHeight.X = -ProbeHeight.X;
                                ProbeHeight.XOpp = -ProbeHeight.XOpp;
                                ProbeHeight.Y = -ProbeHeight.Y;
                                ProbeHeight.YOpp = -ProbeHeight.YOpp;
                                ProbeHeight.Z = -ProbeHeight.Z;
                                ProbeHeight.ZOpp = -ProbeHeight.ZOpp;

                                // Sets height-maps in separate function
                                SetHeights();

                                _serialPort.WriteLine("M206 T3 P153 X" + centerHeight);
                                LogConsole("Setting Z Max Length\n");
                                Thread.Sleep(pauseTimeSet);

                                zMaxLength = centerHeight;

                                Iterations.ZOppIterations++;

                                if (advancedCalibration == 1)
                                {
                                    //find base heights
                                    //find heights with each value increased by 1 - HRad, tower offset 1-3, diagonal rod

                                    if (advancedCalCount == 0)
                                    {
                                        //start
                                        if (_serialPort.IsOpen)
                                        {
                                            //set diagonal rod +1
                                            _serialPort.WriteLine("M206 T3 P881 X" + (diagonalRod + 1).ToString());
                                            LogConsole("Setting diagonal rod to: " + (diagonalRod + 1).ToString() + "\n");
                                            Thread.Sleep(pauseTimeSet);
                                        }

                                        InitiateCal();

                                        advancedCalCount++;
                                    }
                                    else if (advancedCalCount == 1)
                                    {
                                        //get diagonal rod percentages

                                        DiagonalRod.deltaTower = ((TempProbeHeight.X - ProbeHeight.X) +
                                                      (TempProbeHeight.Y - ProbeHeight.Y) +
                                                      (TempProbeHeight.Z - ProbeHeight.Z)) / 3;
                                        DiagonalRod.deltaOpp = ((TempProbeHeight.XOpp - ProbeHeight.XOpp) +
                                                    (TempProbeHeight.YOpp - ProbeHeight.YOpp) +
                                                    (TempProbeHeight.ZOpp - ProbeHeight.ZOpp)) / 3;

                                        if (_serialPort.IsOpen)
                                        {
                                            //reset diagonal rod
                                            _serialPort.WriteLine("M206 T3 P881 X" + (diagonalRod).ToString());
                                            LogConsole("Setting diagonal rod to: " + (diagonalRod).ToString() + "\n");
                                            Thread.Sleep(pauseTimeSet);

                                            //set Hrad +1
                                            _serialPort.WriteLine("M206 T3 P885 X" + (HRad + 1).ToString());
                                            LogConsole("Setting Horizontal Radius to: " + (HRad + 1).ToString() + "\n");
                                            Thread.Sleep(pauseTimeSet);
                                        }

                                        InitiateCal();

                                        advancedCalCount++;
                                    }
                                    else if (advancedCalCount == 2)
                                    {
                                        //get HRad percentages
                                        HRadRatio =
                                            -(Math.Abs((ProbeHeight.X - TempProbeHeight.X) +
                                                       (ProbeHeight.Y - TempProbeHeight.Y) +
                                                       (ProbeHeight.Z - TempProbeHeight.Z) +
                                                       (ProbeHeight.XOpp - TempProbeHeight.XOpp) +
                                                       (ProbeHeight.YOpp - TempProbeHeight.YOpp) +
                                                       (ProbeHeight.ZOpp - TempProbeHeight.ZOpp))) / 6;

                                        if (_serialPort.IsOpen)
                                        {
                                            //reset horizontal radius
                                            _serialPort.WriteLine("M206 T3 P885 X" + (HRad).ToString());
                                            LogConsole("Setting Horizontal Radius to: " + (HRad).ToString() + "\n");
                                            Thread.Sleep(pauseTimeSet);

                                            //set X offset
                                            _serialPort.WriteLine("M206 T1 P893 S" + (Offset.X + 80).ToString());
                                            LogConsole("Setting offset X to: " + (Offset.X + 80).ToString() + "\n");
                                            Thread.Sleep(pauseTimeSet);
                                        }

                                        InitiateCal();

                                        advancedCalCount++;
                                    }
                                    else if (advancedCalCount == 3)
                                    {
                                        //get X offset percentages

                                        Offset.XCorrection = Math.Abs(1 / (ProbeHeight.X - TempProbeHeight.X));
                                        OffsetPercent.xxOppPerc =
                                            Math.Abs((ProbeHeight.XOpp - TempProbeHeight.XOpp) /
                                                     (ProbeHeight.X - TempProbeHeight.X));
                                        OffsetPercent.xyPerc =
                                            Math.Abs((ProbeHeight.Y - TempProbeHeight.Y) /
                                                     (ProbeHeight.X - TempProbeHeight.X));
                                        OffsetPercent.xyOppPerc =
                                            Math.Abs((ProbeHeight.YOpp - TempProbeHeight.YOpp) /
                                                     (ProbeHeight.X - TempProbeHeight.X));
                                        OffsetPercent.xzPerc =
                                            Math.Abs((ProbeHeight.Z - TempProbeHeight.Z) /
                                                     (ProbeHeight.X - TempProbeHeight.X));
                                        OffsetPercent.xzOppPerc =
                                            Math.Abs((ProbeHeight.ZOpp - TempProbeHeight.ZOpp) /
                                                     (ProbeHeight.X - TempProbeHeight.X));

                                        if (_serialPort.IsOpen)
                                        {
                                            //reset X offset
                                            _serialPort.WriteLine("M206 T1 P893 S" + (Offset.X).ToString());
                                            LogConsole("Setting offset X to: " + (Offset.X).ToString() + "\n");
                                            Thread.Sleep(pauseTimeSet);

                                            //set Y offset
                                            _serialPort.WriteLine("M206 T1 P895 S" + (Offset.Y + 80).ToString());
                                            LogConsole("Setting offset Y to: " + (Offset.Y + 80).ToString() + "\n");
                                            Thread.Sleep(pauseTimeSet);
                                        }

                                        InitiateCal();

                                        advancedCalCount++;
                                    }
                                    else if (advancedCalCount == 4)
                                    {
                                        //get Y offset percentages

                                        Offset.YCorrection = Math.Abs(1 / (ProbeHeight.Y - TempProbeHeight.Y));
                                        OffsetPercent.yyOppPerc =
                                            Math.Abs((ProbeHeight.YOpp - TempProbeHeight.YOpp) /
                                                     (ProbeHeight.Y - TempProbeHeight.Y));
                                        OffsetPercent.yxPerc =
                                            Math.Abs((ProbeHeight.X - TempProbeHeight.X) /
                                                     (ProbeHeight.Y - TempProbeHeight.Y));
                                        OffsetPercent.yxOppPerc =
                                            Math.Abs((ProbeHeight.XOpp - TempProbeHeight.XOpp) /
                                                     (ProbeHeight.Y - TempProbeHeight.Y));
                                        OffsetPercent.yzPerc =
                                            Math.Abs((ProbeHeight.Z - TempProbeHeight.Z) /
                                                     (ProbeHeight.Y - TempProbeHeight.Y));
                                        OffsetPercent.yzOppPerc =
                                            Math.Abs((ProbeHeight.ZOpp - TempProbeHeight.ZOpp) /
                                                     (ProbeHeight.Y - TempProbeHeight.Y));

                                        if (_serialPort.IsOpen)
                                        {
                                            //reset Y offset
                                            _serialPort.WriteLine("M206 T1 P895 S" + (Offset.Y).ToString());
                                            LogConsole("Setting offset Y to: " + (Offset.Y).ToString() + "\n");
                                            Thread.Sleep(pauseTimeSet);

                                            //set Z offset
                                            _serialPort.WriteLine("M206 T1 P897 S" + (Offset.Z + 80).ToString());
                                            LogConsole("Setting offset Z to: " + (Offset.Z + 80).ToString() + "\n");
                                            Thread.Sleep(pauseTimeSet);
                                        }

                                        InitiateCal();

                                        advancedCalCount++;
                                    }
                                    else if (advancedCalCount == 5)
                                    {
                                        //get Z offset percentages

                                        Offset.ZCorrection = Math.Abs(1 / (ProbeHeight.Z - TempProbeHeight.Z));
                                        OffsetPercent.zzOppPerc =
                                            Math.Abs((ProbeHeight.ZOpp - TempProbeHeight.ZOpp) /
                                                     (ProbeHeight.Z - TempProbeHeight.Z));
                                        OffsetPercent.zxPerc =
                                            Math.Abs((ProbeHeight.X - TempProbeHeight.X) /
                                                     (ProbeHeight.Z - TempProbeHeight.Z));
                                        OffsetPercent.zxOppPerc =
                                            Math.Abs((ProbeHeight.XOpp - TempProbeHeight.XOpp) /
                                                     (ProbeHeight.Z - TempProbeHeight.Z));
                                        OffsetPercent.zyPerc =
                                            Math.Abs((ProbeHeight.Y - TempProbeHeight.Y) /
                                                     (ProbeHeight.Z - TempProbeHeight.Z));
                                        OffsetPercent.zyOppPerc =
                                            Math.Abs((ProbeHeight.YOpp - TempProbeHeight.YOpp) /
                                                     (ProbeHeight.Z - TempProbeHeight.Z));

                                        if (_serialPort.IsOpen)
                                        {
                                            //set Z offset
                                            _serialPort.WriteLine("M206 T1 P897 S" + (Offset.Z).ToString());
                                            LogConsole("Setting offset Z to: " + (Offset.Z).ToString() + "\n");
                                            Thread.Sleep(pauseTimeSet);

                                            //set alpha rotation offset perc X
                                            _serialPort.WriteLine("M206 T3 P901 X" + (Towers.A + 1).ToString());
                                            LogConsole("Setting Alpha A to: " + (Towers.A + 1).ToString() + "\n");
                                            Thread.Sleep(pauseTimeSet);
                                        }

                                        InitiateCal();

                                        advancedCalCount++;

                                    }
                                    else if (advancedCalCount == 6) //6
                                    {
                                        //get A alpha rotation

                                        AlphaRotationPercentage.X = (2 /
                                                                    Math.Abs((ProbeHeight.YOpp - ProbeHeight.ZOpp) -
                                                                             (TempProbeHeight.YOpp -
                                                                              TempProbeHeight.ZOpp)));

                                        if (_serialPort.IsOpen)
                                        {
                                            //set alpha rotation offset perc X
                                            _serialPort.WriteLine("M206 T3 P901 X" + (Towers.A).ToString());
                                            LogConsole("Setting Alpha A to: " + (Towers.A).ToString() + "\n");
                                            Thread.Sleep(pauseTimeSet);

                                            //set alpha rotation offset perc Y
                                            _serialPort.WriteLine("M206 T3 P905 X" + (Towers.B + 1).ToString());
                                            LogConsole("Setting Alpha B to: " + (Towers.B + 1).ToString() + "\n");
                                            Thread.Sleep(pauseTimeSet);
                                        }

                                        InitiateCal();

                                        advancedCalCount++;
                                    }
                                    else if (advancedCalCount == 7) //7
                                    {
                                        //get B alpha rotation

                                        AlphaRotationPercentage.Y = (2 /
                                                                    Math.Abs((ProbeHeight.ZOpp - ProbeHeight.XOpp) -
                                                                             (TempProbeHeight.ZOpp -
                                                                              TempProbeHeight.XOpp)));

                                        if (_serialPort.IsOpen)
                                        {
                                            //set alpha rotation offset perc Y
                                            _serialPort.WriteLine("M206 T3 P905 X" + (Towers.B).ToString());
                                            LogConsole("Setting Alpha B to: " + (Towers.B).ToString() + "\n");
                                            Thread.Sleep(pauseTimeSet);

                                            //set alpha rotation offset perc Z
                                            _serialPort.WriteLine("M206 T3 P909 X" + (Towers.C + 1).ToString());
                                            LogConsole("Setting Alpha C to: " + (Towers.C + 1).ToString() + "\n");
                                            Thread.Sleep(pauseTimeSet);
                                        }

                                        InitiateCal();

                                        advancedCalCount++;
                                    }
                                    else if (advancedCalCount == 8) //8
                                    {
                                        //get C alpha rotation

                                        AlphaRotationPercentage.Z = (2 /
                                                                    Math.Abs((ProbeHeight.XOpp - ProbeHeight.YOpp) -
                                                                             (TempProbeHeight.XOpp -
                                                                              TempProbeHeight.YOpp)));

                                        if (_serialPort.IsOpen)
                                        {
                                            //set alpha rotation offset perc Z
                                            _serialPort.WriteLine("M206 T3 P909 X" + (Towers.C).ToString());
                                            LogConsole("Setting Alpha C to: " + (Towers.C).ToString() + "\n");
                                            Thread.Sleep(pauseTimeSet);

                                        }

                                        LogConsole("Alpha offset percentages: " + AlphaRotationPercentage.X + ", " + AlphaRotationPercentage.Y + ", and" + AlphaRotationPercentage.Z + "\n");

                                        advancedCalibration = 0;
                                        advancedCalCount = 0;

                                        InitiateCal();

                                        SetAdvancedCalVars();
                                    }

                                    Iterations.IterationNum++;
                                }
                                else
                                {
                                    Iterations.IterationNum++;

                                    CalibratePrinter();
                                }
                            }
                        }

                        //parse EEProm
                        if (message.Contains("EPR"))
                        {
                            string[] parseEPR = message.Split(new string[] { "EPR", "\n" },
                                StringSplitOptions.RemoveEmptyEntries);
                            string[] parseEPRSpace;

                            if (parseEPR.Length > 1)
                            {
                                parseEPRSpace = parseEPR[1].Split(new char[] { ' ', '\n' },
                                    StringSplitOptions.RemoveEmptyEntries);
                            }
                            else
                            {
                                parseEPRSpace = parseEPR[0].Split(new char[] { ' ', '\n' },
                                    StringSplitOptions.RemoveEmptyEntries);
                            }

                            int intParse;
                            double doubleParse2;

                            //check if there is a space between
                            if (parseEPRSpace[0] == ":")
                            {
                                //Space
                                intParse = int.Parse(parseEPRSpace[2]);
                                doubleParse2 = double.Parse(parseEPRSpace[3]);
                            }
                            else
                            {
                                //No space
                                intParse = int.Parse(parseEPRSpace[1]);
                                doubleParse2 = double.Parse(parseEPRSpace[2]);
                            }


                            if (intParse == 11)
                            {
                                LogConsole("EEProm capture initiated\n");
                                stepsPerMM = doubleParse2;
                            }
                            else if (intParse == 145)
                            {
                                xMaxLength = doubleParse2;
                            }
                            else if (intParse == 149)
                            {
                                yMaxLength = doubleParse2;
                            }
                            else if (intParse == 153)
                            {
                                zMaxLength = doubleParse2;
                            }
                            else if (intParse == 808)
                            {
                                zProbe = doubleParse2;
                            }
                            else if (intParse == 881)
                            {
                                diagonalRod = doubleParse2;
                            }
                            else if (intParse == 885)
                            {
                                HRad = doubleParse2;
                            }
                            else if (intParse == 893)
                            {
                                Offset.X = doubleParse2;
                            }
                            else if (intParse == 895)
                            {
                                Offset.Y = doubleParse2;
                            }
                            else if (intParse == 897)
                            {
                                Offset.Z = doubleParse2;
                            }
                            else if (intParse == 901)
                            {
                                Towers.A = doubleParse2;
                            }
                            else if (intParse == 905)
                            {
                                Towers.B = doubleParse2;
                            }
                            else if (intParse == 909)
                            {
                                Towers.C = doubleParse2;
                            }
                            else if (intParse == 913)
                            {
                                Towers.DA = doubleParse2;
                            }
                            else if (intParse == 917)
                            {
                                Towers.DB = doubleParse2;
                            }
                            else if (intParse == 921)
                            {
                                Towers.DC = doubleParse2;

                                LogConsole("EEProm:  Steps:" + stepsPerMM + ", X Max:" + xMaxLength + ", Y Max:" +
                                           yMaxLength + ", Z Max:" + zMaxLength + ", Z-Probe Offset:" + zProbe +
                                           ", Diagonal Rod:" + diagonalRod + ", Horizontal Radius:" + HRad +
                                           ", X Offset:" + Offset.X + ", Y Offset:" + Offset.Y + ", Z Offset:" + Offset.Z +
                                           ", Alpha A:" + Towers.A + ", Alpha B:" + Towers.B + ",  Alpha C:" + Towers.C + ", Delta A:" + Towers.DA +
                                           ", Delta B:" + Towers.DB + ", and Delta C:" + Towers.DC + "\n");
                                LogConsole("EEProm captured, beginning calibration.");

                                // Once the program has the EEProm stored, calibration initiates
                                InitiateCal();
                            }
                        } //end EEProm capture
                    }
                }

                catch (TimeoutException)
                {
                }
            }
        }

        public void ProbeX0Y0()
        {
            _serialPort.WriteLine("G1 Z" + probingHeight.ToString() + " X-" +
                                  valueXYLarge.ToString() + " Y-" + valueXYSmall.ToString());
            Thread.Sleep(pauseTimeSet);
            _serialPort.WriteLine("G1 Z" + probingHeight.ToString() + " X0 Y0");
            Thread.Sleep(pauseTimeSet);
            _serialPort.WriteLine("G1 Z" + probingHeight.ToString() + " X" + valueXYLarge.ToString() +
                                  " Y" + valueXYSmall.ToString());
            Thread.Sleep(pauseTimeSet);
            _serialPort.WriteLine("G30");
        }
    }
}