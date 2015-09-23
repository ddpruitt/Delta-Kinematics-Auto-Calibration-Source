using System;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using DeltaKinematics.Core;

namespace deltaKinematics
{
    public partial class Form1
    {

        //analyzes the geometry/accuracies of the printers frame
        private void AnalyzeGeometry()
        {
            //calculates the tower angle at the top and bottom
            TowerRotation.X = Math.Acos((plateDiameter * 0.963) / Math.Sqrt(Math.Pow(Math.Abs(ProbeHeight.X - ProbeHeight.XOpp), 2) + Math.Pow((plateDiameter * 0.963), 2))) * 57.296 * 5;
            TowerRotation.Y = Math.Acos((plateDiameter * 0.963) / Math.Sqrt(Math.Pow(Math.Abs(ProbeHeight.Y - ProbeHeight.YOpp), 2) + Math.Pow((plateDiameter * 0.963), 2))) * 57.296 * 5;
            TowerRotation.Z = Math.Acos((plateDiameter * 0.963) / Math.Sqrt(Math.Pow(Math.Abs(ProbeHeight.Z - ProbeHeight.ZOpp), 2) + Math.Pow((plateDiameter * 0.963), 2))) * 57.296 * 5;

            if (ProbeHeight.X < ProbeHeight.XOpp)
            {
                TowerRotation.X = 90 - TowerRotation.X;
            }
            else
            {
                TowerRotation.X = 90 + TowerRotation.X;
            }

            if (ProbeHeight.Y < ProbeHeight.YOpp)
            {
                TowerRotation.Y = 90 - TowerRotation.Y;
            }
            else
            {
                TowerRotation.Y = 90 + TowerRotation.Y;
            }

            if (ProbeHeight.Z < ProbeHeight.ZOpp)
            {
                TowerRotation.Z = 90 - TowerRotation.Z;
            }
            else
            {
                TowerRotation.Z = 90 + TowerRotation.Z;
            }

            //bottom
            Invoke((MethodInvoker)delegate { this.textXAngleTower.Text = TowerRotation.X.ToString(); });
            Invoke((MethodInvoker)delegate { this.textYAngleTower.Text = TowerRotation.Y.ToString(); });
            Invoke((MethodInvoker)delegate { this.textZAngleTower.Text = TowerRotation.Z.ToString(); });

            //top
            Invoke((MethodInvoker)delegate { this.textXAngleTop.Text = (180 - TowerRotation.X).ToString(); });
            Invoke((MethodInvoker)delegate { this.textYAngleTop.Text = (180 - TowerRotation.Y).ToString(); });
            Invoke((MethodInvoker)delegate { this.textZAngleTop.Text = (180 - TowerRotation.Z).ToString(); });

            //Calculates the radii for each tower at the top and bottom of the towers
            //X
            double hypotenuseX = (Math.Sin(90) / Math.Sin(Math.PI - TowerRotation.X - (180 - TowerRotation.X))) * centerHeight;
            double radiusSideX = Math.Sqrt(Math.Pow(hypotenuseX, 2) - Math.Pow(centerHeight, 2));
            double bottomX = HRad;
            double topX = HRad - radiusSideX;

            //Top
            Invoke((MethodInvoker)delegate { this.textXPlate.Text = bottomX.ToString(); });
            Invoke((MethodInvoker)delegate { this.textXPlateTop.Text = topX.ToString(); });

            //Y
            double hypotenuseY = (Math.Sin(90) / Math.Sin(Math.PI - TowerRotation.Y - (180 - TowerRotation.Y))) * centerHeight;
            double radiusSideY = Math.Sqrt(Math.Pow(hypotenuseX, 2) - Math.Pow(centerHeight, 2));
            double bottomY = HRad;
            double topY = HRad - radiusSideY;

            //Top
            Invoke((MethodInvoker)delegate { this.textYPlate.Text = bottomY.ToString(); });
            Invoke((MethodInvoker)delegate { this.textYPlateTop.Text = topY.ToString(); });

            //Z
            double hypotenuseZ = (Math.Sin(90) / Math.Sin(Math.PI - TowerRotation.Z - (180 - TowerRotation.Z))) * centerHeight;
            double radiusSideZ = Math.Sqrt(Math.Pow(hypotenuseX, 2) - Math.Pow(centerHeight, 2));
            double bottomZ = HRad;
            double topZ = HRad - radiusSideZ;

            //Top
            Invoke((MethodInvoker)delegate { this.textZPlate.Text = bottomZ.ToString(); });
            Invoke((MethodInvoker)delegate { this.textZPlateTop.Text = topZ.ToString(); });

            //find max offset in Xy scaling with current tower offsets
            double AScaling = Math.Max(Math.Max(Math.Abs(90 - hypotenuseX), Math.Abs(90 - hypotenuseY)), Math.Abs(90 - hypotenuseZ));
            double offsetScalingMax = (Math.Sin(90) / Math.Sin(Math.PI - 90 - AScaling)) * centerHeight;

            //set scaling offset
            Invoke((MethodInvoker)delegate { this.textScaleOffset.Text = offsetScalingMax.ToString(); });
        }

        private static string GetZeros(int zeroCount)
        {
            if (zeroCount < 0)
            {
                zeroCount = Math.Abs(zeroCount);
            }

            var sb = new StringBuilder();

            for (int i = 0; i < zeroCount; i++)
            {
                sb.Append("0");
            }

            return sb.ToString();
        }

        //uses long string instead of scientific notation
        public static string ToLongString(double input)
        {
            string str = input.ToString().ToUpper();

            // if string representation was collapsed from scientific notation, just return it:
            if (!str.Contains("E")) return str;

            bool negativeNumber = false;

            if (str[0] == '-')
            {
                str = str.Remove(0, 1);
                negativeNumber = true;
            }

            string sep = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            char decSeparator = sep.ToCharArray()[0];

            string[] exponentParts = str.Split('E');
            string[] decimalParts = exponentParts[0].Split(decSeparator);

            // fix missing decimal point:
            if (decimalParts.Length == 1) decimalParts = new string[] { exponentParts[0], "0" };

            int exponentValue = int.Parse(exponentParts[1]);

            string newNumber = decimalParts[0] + decimalParts[1];

            string result;

            if (exponentValue > 0)
            {
                result =
                    newNumber +
                    GetZeros(exponentValue - decimalParts[1].Length);
            }
            else // negative exponent
            {
                result =
                    "0" +
                    decSeparator +
                    GetZeros(exponentValue + decimalParts[0].Length) +
                    newNumber;

                result = result.TrimEnd('0');
            }

            if (negativeNumber)
                result = "-" + result;

            return result;
        }

        public void InitiateCal()
        {
            //set gcode specifications
            valueZ = 0.482 * plateDiameter;
            valueXYLarge = 0.417 * plateDiameter;
            valueXYSmall = 0.241 * plateDiameter;

            //set zprobe height to zero

            if (comboBoxZMinimumValue == "Z-Probe" && Iterations.IterationNum == 0)
            {
                zProbeSet = 1;
                Thread.Sleep(pauseTimeSet);
                _serialPort.WriteLine("G28");
                Thread.Sleep(pauseTimeSet);
                _serialPort.WriteLine("G30");

                double heightTime = (zMaxLength / zProbeSpeed) * 1000;
                Thread.Sleep(Convert.ToInt32(heightTime));
            }

            //zero bed
            Thread.Sleep(pauseTimeSet);
            _serialPort.WriteLine("G28");
            Thread.Sleep(pauseTimeSet);
            _serialPort.WriteLine("G1 Z" + probingHeight.ToString() + " F5000");
            Thread.Sleep(pauseTimeSet);
            _serialPort.WriteLine("G30");
        }

        public void CalibratePrinter()
        {
            double calculationTemp1 = 0;
            double HRadCorrection = 0;
            int t = 0;

            //check accuracy of current height-map and determine to end or procede
            if (ProbeHeight.X <= accuracy2 && ProbeHeight.X >= -accuracy2 && ProbeHeight.XOpp <= accuracy2 &&
                ProbeHeight.XOpp >= -accuracy2 && ProbeHeight.Y <= accuracy2 && ProbeHeight.Y >= -accuracy2 &&
                ProbeHeight.YOpp <= accuracy2 && ProbeHeight.YOpp >= -accuracy2 && ProbeHeight.Z <= accuracy2 &&
                ProbeHeight.Z >= -accuracy2 && ProbeHeight.ZOpp <= accuracy2 && ProbeHeight.ZOpp >= -accuracy2)
            {
                //fsr plate offset
                if (comboBoxZMinimumValue == "FSR")
                {
                    _serialPort.WriteLine("M206 T3 P153 X" + (centerHeight - Convert.ToDouble(textFSRPlateOffset.Text)));
                    LogConsole("Setting Z Max Length with adjustment for FSR\n");
                    Thread.Sleep(pauseTimeSet);
                }

                Thread.Sleep(pauseTimeSet);
                _serialPort.WriteLine("G28");
                LogConsole("Calibration Complete\n");
                //end code
            }
            else if (Iterations.IterationNum > Iterations.MaxIterations)
            {
                //max iterations hit
                LogConsole("Calibration Failed\n");
                LogConsole(
                    "Maximum number of iterations hit. Please restart application and run the advanced calibration.\n");
            }
            else
            {
                //logs current iteration number
                LogConsole("Current iteration: " + Iterations.IterationNum + "\n");

                //basic calibration
                if (calibrationState == 0)
                {
                    //////////////////////////////////////////////////////////////////////////////
                    //HRad is calibrated by increasing the outside edge of the glass by the average differences, this should balance the values with a central point of around zero
                    double HRadSA = ((ProbeHeight.X + ProbeHeight.XOpp + ProbeHeight.Y + ProbeHeight.YOpp +
                                      ProbeHeight.Z + ProbeHeight.ZOpp) / 6);

                    HRad = HRad + (HRadSA / HRadRatio);

                    AdjustProbeHeight(HRadSA);

                    LogConsole("HRad:" + HRad + "\n");

                    ////////////////////////////////////////////////////////////////////////////////
                    //Delta Radius Calibration******************************************************
                    DASA = ((ProbeHeight.X + ProbeHeight.XOpp) / 2);
                    DBSA = ((ProbeHeight.Y + ProbeHeight.YOpp) / 2);
                    DCSA = ((ProbeHeight.Z + ProbeHeight.ZOpp) / 2);

                    Towers.DA = Towers.DA + ((DASA) / HRadRatio);
                    Towers.DB = Towers.DB + ((DBSA) / HRadRatio);
                    Towers.DC = Towers.DC + ((DCSA) / HRadRatio);

                    ProbeHeight.X = ProbeHeight.X + ((DASA) / HRadRatio) * 0.5;
                    ProbeHeight.XOpp = ProbeHeight.XOpp + ((DASA) / HRadRatio) * 0.225;
                    ProbeHeight.Y = ProbeHeight.Y + ((DASA) / HRadRatio) * 0.1375;
                    ProbeHeight.YOpp = ProbeHeight.YOpp + ((DASA) / HRadRatio) * 0.1375;
                    ProbeHeight.Z = ProbeHeight.Z + ((DASA) / HRadRatio) * 0.1375;
                    ProbeHeight.ZOpp = ProbeHeight.ZOpp + ((DASA) / HRadRatio) * 0.1375;

                    ProbeHeight.X = ProbeHeight.X + ((DBSA) / HRadRatio) * 0.1375;
                    ProbeHeight.XOpp = ProbeHeight.XOpp + ((DBSA) / HRadRatio) * 0.1375;
                    ProbeHeight.Y = ProbeHeight.Y + ((DBSA) / HRadRatio) * 0.5;
                    ProbeHeight.YOpp = ProbeHeight.YOpp + ((DBSA) / HRadRatio) * 0.225;
                    ProbeHeight.Z = ProbeHeight.Z + ((DBSA) / HRadRatio) * 0.1375;
                    ProbeHeight.ZOpp = ProbeHeight.ZOpp + ((DBSA) / HRadRatio) * 0.1375;

                    ProbeHeight.X = ProbeHeight.X + ((DCSA) / HRadRatio) * 0.1375;
                    ProbeHeight.XOpp = ProbeHeight.XOpp + ((DCSA) / HRadRatio) * 0.1375;
                    ProbeHeight.Y = ProbeHeight.Y + ((DCSA) / HRadRatio) * 0.1375;
                    ProbeHeight.YOpp = ProbeHeight.YOpp + ((DCSA) / HRadRatio) * 0.1375;
                    ProbeHeight.Z = ProbeHeight.Z + ((DCSA) / HRadRatio) * 0.5;
                    ProbeHeight.ZOpp = ProbeHeight.ZOpp + ((DCSA) / HRadRatio) * 0.225;

                    WriteDeltaRadiiToEeprom();

                    //analyzes the printer geometry
                    if (analyzeCount == 0)
                    {
                        analyzeCount++;
                        AnalyzeGeometry();

                        LogConsole("Expect a slight inaccuracy in the geometry analysis; basic calibration.");
                    }

                    ////////////////////////////////////////////////////////////////////////////////
                    //Tower Offset Calibration******************************************************
                    int j = 0;
                    Temp2ProbeHeight.X = ProbeHeight.X;
                    Temp2ProbeHeight.XOpp = ProbeHeight.XOpp;
                    Temp2ProbeHeight.Y = ProbeHeight.Y;
                    Temp2ProbeHeight.YOpp = ProbeHeight.YOpp;
                    Temp2ProbeHeight.Z = ProbeHeight.Z;
                    Temp2ProbeHeight.ZOpp = ProbeHeight.ZOpp;

                    while (j < 100)
                    {
                        double theoryX = Offset.X + Temp2ProbeHeight.X * stepsPerMM *Offset.XCorrection;

                        //correction of one tower allows for XY dimensional accuracy
                        if (Temp2ProbeHeight.X > 0)
                        {
                            AdjustOffsetX();
                        }
                        else if (theoryX > 0 && Temp2ProbeHeight.X < 0)
                        {
                            //if x is negative and can be decreased
                            AdjustOffsetX();
                        }
                        else
                        {
                            //if tempX2 is negative
                            Offset.Y = Offset.Y - Temp2ProbeHeight.X * stepsPerMM *Offset.YCorrection * 2;
                            Offset.Z = Offset.Z - Temp2ProbeHeight.X * stepsPerMM *Offset.ZCorrection * 2;

                            Temp2ProbeHeight.YOpp = Temp2ProbeHeight.YOpp - (Temp2ProbeHeight.X * 2 *OffsetPercent.yyOppPerc);
                            Temp2ProbeHeight.X = Temp2ProbeHeight.X - (Temp2ProbeHeight.X * 2 *OffsetPercent.yxPerc);
                            Temp2ProbeHeight.Z = Temp2ProbeHeight.Z - (Temp2ProbeHeight.X * 2 *OffsetPercent.yxPerc);
                            Temp2ProbeHeight.XOpp = Temp2ProbeHeight.XOpp + (Temp2ProbeHeight.X * 2 *OffsetPercent.yxOppPerc);
                            Temp2ProbeHeight.ZOpp = Temp2ProbeHeight.ZOpp + (Temp2ProbeHeight.X * 2 *OffsetPercent.yxOppPerc);
                            Temp2ProbeHeight.Y = Temp2ProbeHeight.Y + Temp2ProbeHeight.X * 2;

                            Temp2ProbeHeight.ZOpp = Temp2ProbeHeight.ZOpp - (Temp2ProbeHeight.X * 2 *OffsetPercent.zzOppPerc);
                            Temp2ProbeHeight.X = Temp2ProbeHeight.X - (Temp2ProbeHeight.X * 2 *OffsetPercent.zxPerc);
                            Temp2ProbeHeight.Y = Temp2ProbeHeight.Y - (Temp2ProbeHeight.X * 2 *OffsetPercent.zyPerc);
                            Temp2ProbeHeight.XOpp = Temp2ProbeHeight.XOpp + (Temp2ProbeHeight.X * 2 *OffsetPercent.yxOppPerc);
                            Temp2ProbeHeight.YOpp = Temp2ProbeHeight.YOpp + (Temp2ProbeHeight.X * 2 *OffsetPercent.zyOppPerc);
                            Temp2ProbeHeight.Z = Temp2ProbeHeight.Z + Temp2ProbeHeight.X * 2;
                        }

                        double theoryY = Offset.Y + Temp2ProbeHeight.Y * stepsPerMM *Offset.YCorrection;

                        //Y
                        if (Temp2ProbeHeight.Y > 0)
                        {
                            AdjustOffsetY();
                        }
                        else if (theoryY > 0 && Temp2ProbeHeight.Y < 0)
                        {
                            AdjustOffsetY();
                        }
                        else
                        {
                            Offset.X = Offset.X - Temp2ProbeHeight.Y * stepsPerMM *Offset.XCorrection * 2;
                            Offset.Z = Offset.Z - Temp2ProbeHeight.Y * stepsPerMM *Offset.ZCorrection * 2;

                            Temp2ProbeHeight.XOpp = Temp2ProbeHeight.XOpp - (Temp2ProbeHeight.Y * 2 *OffsetPercent.xxOppPerc); //0.5
                            Temp2ProbeHeight.Z = Temp2ProbeHeight.Z - (Temp2ProbeHeight.Y * 2 *OffsetPercent.xzPerc); //0.25
                            Temp2ProbeHeight.Y = Temp2ProbeHeight.Y - (Temp2ProbeHeight.Y * 2 *OffsetPercent.xyPerc); //0.25
                            Temp2ProbeHeight.ZOpp = Temp2ProbeHeight.ZOpp + (Temp2ProbeHeight.Y * 2 *OffsetPercent.xzOppPerc); //0.25
                            Temp2ProbeHeight.YOpp = Temp2ProbeHeight.YOpp + (Temp2ProbeHeight.Y * 2 *OffsetPercent.xyOppPerc); //0.25
                            Temp2ProbeHeight.X = Temp2ProbeHeight.X + Temp2ProbeHeight.Y * 2;

                            Temp2ProbeHeight.ZOpp = Temp2ProbeHeight.ZOpp - (Temp2ProbeHeight.Y * 2 *OffsetPercent.zzOppPerc);
                            Temp2ProbeHeight.X = Temp2ProbeHeight.X - (Temp2ProbeHeight.Y * 2 *OffsetPercent.zxPerc);
                            Temp2ProbeHeight.Y = Temp2ProbeHeight.Y - (Temp2ProbeHeight.Y * 2 *OffsetPercent.zyPerc);
                            Temp2ProbeHeight.XOpp = Temp2ProbeHeight.XOpp + (Temp2ProbeHeight.Y * 2 *OffsetPercent.yxOppPerc);
                            Temp2ProbeHeight.YOpp = Temp2ProbeHeight.YOpp + (Temp2ProbeHeight.Y * 2 *OffsetPercent.zyOppPerc);
                            Temp2ProbeHeight.Z = Temp2ProbeHeight.Z + Temp2ProbeHeight.Y * 2;
                        }

                        double theoryZ = Offset.Z + Temp2ProbeHeight.Z * stepsPerMM *Offset.ZCorrection;

                        //Z
                        if (Temp2ProbeHeight.Z > 0)
                        {
                            AdjustOffsetZ();
                        }
                        else if (theoryZ > 0 && Temp2ProbeHeight.Z < 0)
                        {
                            AdjustOffsetZ();
                        }
                        else
                        {
                            Offset.Y = Offset.Y - Temp2ProbeHeight.Z * stepsPerMM *Offset.YCorrection * 2;
                            Offset.X = Offset.X - Temp2ProbeHeight.Z * stepsPerMM *Offset.XCorrection * 2;

                            Temp2ProbeHeight.XOpp = Temp2ProbeHeight.XOpp - (Temp2ProbeHeight.Z * 2 *OffsetPercent.xxOppPerc); //0.5
                            Temp2ProbeHeight.Z = Temp2ProbeHeight.Z - (Temp2ProbeHeight.Z * 2 *OffsetPercent.xzPerc); //0.25
                            Temp2ProbeHeight.Y = Temp2ProbeHeight.Y - (Temp2ProbeHeight.Z * 2 *OffsetPercent.xyPerc); //0.25
                            Temp2ProbeHeight.ZOpp = Temp2ProbeHeight.ZOpp + (Temp2ProbeHeight.Z * 2 *OffsetPercent.xzOppPerc); //0.25
                            Temp2ProbeHeight.YOpp = Temp2ProbeHeight.YOpp + (Temp2ProbeHeight.Z * 2 *OffsetPercent.xyOppPerc); //0.25
                            Temp2ProbeHeight.X = Temp2ProbeHeight.X + Temp2ProbeHeight.Z * 2;

                            Temp2ProbeHeight.YOpp = Temp2ProbeHeight.YOpp - (Temp2ProbeHeight.Z * 2 *OffsetPercent.yyOppPerc);
                            Temp2ProbeHeight.X = Temp2ProbeHeight.X - (Temp2ProbeHeight.Z * 2 *OffsetPercent.yxPerc);
                            Temp2ProbeHeight.Z = Temp2ProbeHeight.Z - (Temp2ProbeHeight.Z * 2 *OffsetPercent.yxPerc);
                            Temp2ProbeHeight.XOpp = Temp2ProbeHeight.XOpp + (Temp2ProbeHeight.Z * 2 *OffsetPercent.yxOppPerc);
                            Temp2ProbeHeight.ZOpp = Temp2ProbeHeight.ZOpp + (Temp2ProbeHeight.Z * 2 *OffsetPercent.yxOppPerc);
                            Temp2ProbeHeight.Y = Temp2ProbeHeight.Y + Temp2ProbeHeight.Z * 2;
                        }

                        Temp2ProbeHeight.X = CheckZero(Temp2ProbeHeight.X);
                        Temp2ProbeHeight.Y = CheckZero(Temp2ProbeHeight.Y);
                        Temp2ProbeHeight.Z = CheckZero(Temp2ProbeHeight.Z);
                        Temp2ProbeHeight.XOpp = CheckZero(Temp2ProbeHeight.XOpp);
                        Temp2ProbeHeight.YOpp = CheckZero(Temp2ProbeHeight.YOpp);
                        Temp2ProbeHeight.ZOpp = CheckZero(Temp2ProbeHeight.ZOpp);

                        LogConsole("XYZ Calc: " + Offset.X + " " + Offset.Y + " " + Offset.Z);
                        LogConsole("Height-Map: " + Temp2ProbeHeight.X + " " + Temp2ProbeHeight.XOpp + " " +
                                   Temp2ProbeHeight.Y + " " + Temp2ProbeHeight.YOpp + " " + Temp2ProbeHeight.Z + " " +
                                   Temp2ProbeHeight.ZOpp);

                        if (Temp2ProbeHeight.X < accuracy && Temp2ProbeHeight.X > -accuracy &&
                            Temp2ProbeHeight.Y < accuracy && Temp2ProbeHeight.Y > -accuracy &&
                            Temp2ProbeHeight.Z < accuracy && Temp2ProbeHeight.Z > -accuracy && Offset.X < 1000 && Offset.Y < 1000 && Offset.Z < 1000)
                        {
                            j = 100;
                        }
                        else if (j == 50)
                        {
                            //error protection
                            Temp2ProbeHeight.X = ProbeHeight.X;
                            Temp2ProbeHeight.XOpp = ProbeHeight.XOpp;
                            Temp2ProbeHeight.Y = ProbeHeight.Y;
                            Temp2ProbeHeight.YOpp = ProbeHeight.YOpp;
                            Temp2ProbeHeight.Z = ProbeHeight.Z;
                            Temp2ProbeHeight.ZOpp = ProbeHeight.ZOpp;

                            //X
                            Offset.XCorrection = 1.5;
                            OffsetPercent.xxOppPerc = 0.5;
                            OffsetPercent.xyPerc = 0.25;
                            OffsetPercent.xyOppPerc = 0.25;
                            OffsetPercent.xzPerc = 0.25;
                            OffsetPercent.xzOppPerc = 0.25;

                            //Y
                            Offset.YCorrection = 1.5;
                            OffsetPercent.yyOppPerc = 0.5;
                            OffsetPercent.yxPerc = 0.25;
                            OffsetPercent.yxOppPerc = 0.25;
                            OffsetPercent.yzPerc = 0.25;
                            OffsetPercent.yzOppPerc = 0.25;

                            //Z
                            Offset.ZCorrection = 1.5;
                            OffsetPercent.zzOppPerc = 0.5;
                            OffsetPercent.zxPerc = 0.25;
                            OffsetPercent.zxOppPerc = 0.25;
                            OffsetPercent.zyPerc = 0.25;
                            OffsetPercent.zyOppPerc = 0.25;

                            Offset.X = 0;
                            Offset.Y = 0;
                            Offset.Z = 0;

                            j++;
                        }
                        else
                        {
                            j++;
                        }
                    }

                    if (Offset.X > 1000 || Offset.Y > 1000 || Offset.Z > 1000)
                    {
                        LogConsole("XYZ offset calibration error, setting default values.");
                        LogConsole("XYZ offsets before damage prevention: X" + Offset.X + " Y" + Offset.Y + " Z" + Offset.Z +
                                   "\n");
                        Offset.X = 0;
                        Offset.Y = 0;
                        Offset.Z = 0;
                    }
                    else
                    {
                        ProbeHeight.X = Temp2ProbeHeight.X;
                        ProbeHeight.XOpp = Temp2ProbeHeight.XOpp;
                        ProbeHeight.Y = Temp2ProbeHeight.Y;
                        ProbeHeight.YOpp = Temp2ProbeHeight.YOpp;
                        ProbeHeight.Z = Temp2ProbeHeight.Z;
                        ProbeHeight.ZOpp = Temp2ProbeHeight.ZOpp;
                    }

                    //round to the nearest whole number
                    Offset.X = Math.Round(Offset.X);
                    Offset.Y = Math.Round(Offset.Y);
                    Offset.Z = Math.Round(Offset.Z);

                    LogConsole("XYZ:" + Offset.X + " " + Offset.Y + " " + Offset.Z + "\n");

                    //send data back to printer
                    _serialPort.WriteLine("M206 T1 P893 S" + Offset.X.ToString());
                    Thread.Sleep(pauseTimeSet);
                    _serialPort.WriteLine("M206 T1 P895 S" + Offset.Y.ToString());
                    Thread.Sleep(pauseTimeSet);
                    _serialPort.WriteLine("M206 T1 P897 S" + Offset.Z.ToString());
                    Thread.Sleep(pauseTimeSet);

                    ////////////////////////////////////////////////////////////////////////////////
                    //Alpha Rotation Calibration****************************************************

                    if (Offset.X != 0 && Offset.Y != 0 && Offset.Z != 0)
                    {
                        AlphaRotationCalibration();
                    }

                    ////////////////////////////////////////////////////////////////////////////////
                    //Diagonal Rod Calibration******************************************************
                    double diagChange = 1 /DiagonalRod.deltaOpp;
                    double towOppDiff = DiagonalRod.deltaTower /DiagonalRod.deltaOpp; //0.5


                    if (Offset.X != 0 && Offset.Y != 0 && Offset.Z != 0)
                    {
                        DiagonalRodCalibration(diagChange, towOppDiff);

                        if (diagonalRod < 500 && diagonalRod > 100)
                        {
                            //send obtained values back to the printer*************************************
                            LogConsole("Diagonal Rod:" + diagonalRod + "\n");
                            Thread.Sleep(pauseTimeSet);
                            _serialPort.WriteLine("M206 T3 P881 X" + diagonalRod.ToString());
                            LogConsole("Setting diagonal rod\n");
                        }
                        else
                        {
                            LogConsole("Diagonal rod not set\n");
                        }

                        if (HRad < 250 && HRad > 50)
                        {
                            //send obtained values back to the printer*************************************
                            LogConsole("HRad Recalibration:" + HRad + "\n");
                            _serialPort.WriteLine("M206 T3 P885 X" + CheckZero(HRad).ToString());
                            LogConsole("Setting Horizontal Radius\n");
                            Thread.Sleep(pauseTimeSet);
                        }
                        else
                        {
                            LogConsole("Horizontal radius not set\n");
                        }
                    }

                    //send obtained values back to the printer*************************************
                    Thread.Sleep(pauseTimeSet);
                    _serialPort.WriteLine("M206 T3 P901 X" + CheckZero(Towers.A));
                    LogConsole("Setting A Rotation\n");
                    Thread.Sleep(pauseTimeSet);
                    _serialPort.WriteLine("M206 T3 P905 X" + CheckZero(Towers.B));
                    LogConsole("Setting B Rotation\n");
                    Thread.Sleep(pauseTimeSet);
                    _serialPort.WriteLine("M206 T3 P909 X" + CheckZero(Towers.C).ToString());
                    LogConsole("Setting C Rotation\n");
                    Thread.Sleep(pauseTimeSet);

                    //rechecks calibration to either restart or finish
                    LogConsole("Checking height-map\n");
                    InitiateCal();
                }
                else
                {
                    //advanced calibration
                    double HRadSA;

                    if (calculationCount == 0)
                    {
                        //////////////////////////////////////////////////////////////////////////////
                        //HRad is calibrated by increasing the outside edge of the glass by the average differences, this should balance the values with an average of zero
                        if (calculationCheckCount == 0)
                        {
                            CalculationProbeHeight.X = ProbeHeight.X;
                            CalculationProbeHeight.XOpp = ProbeHeight.XOpp;
                            CalculationProbeHeight.Y = ProbeHeight.Y;
                            CalculationProbeHeight.YOpp = ProbeHeight.YOpp;
                            CalculationProbeHeight.Z = ProbeHeight.Z;
                            CalculationProbeHeight.ZOpp = ProbeHeight.ZOpp;

                            calculationTemp1 = ((ProbeHeight.X + ProbeHeight.XOpp + ProbeHeight.Y + ProbeHeight.YOpp +
                                                 ProbeHeight.Z + ProbeHeight.ZOpp) / 6);
                            HRadSA = ((ProbeHeight.X + ProbeHeight.XOpp + ProbeHeight.Y + ProbeHeight.YOpp +
                                       ProbeHeight.Z + ProbeHeight.ZOpp) / 6);
                            calculationCheckCount++;
                        }
                        else
                        {
                            HRadSA = ((ProbeHeight.X + ProbeHeight.XOpp + ProbeHeight.Y + ProbeHeight.YOpp +
                                       ProbeHeight.Z + ProbeHeight.ZOpp) / 6);

                            if (HRadCorrection != 0)
                            {
                                HRadCorrection = (HRadCorrection + (calculationTemp1 / (calculationTemp1 - HRadSA))) / 2;
                            }
                            else
                            {
                                HRadCorrection = calculationTemp1 / (calculationTemp1 - HRadSA);
                            }
                        } //end else

                        //check accuracy, if good, move to next step
                        if (HRadSA < accuracy2 && HRadSA > -accuracy2)
                        {
                            LogConsole("HRad:" + HRad + "\n");
                            LogConsole("HRad Correction:" + HRadCorrection + "\n");
                            LogConsole("HRad Average before calibration:" + calculationTemp1 + "\n");
                            LogConsole("HRad Average:" + HRadSA + "\n");
                            LogConsole("Horizontal Radius Calibration Success; Checking height-map.\n");
                            calculationCount++;
                            InitiateCal();
                            calculationCheckCount = 0;
                            calculationTemp1 = 0;
                        }
                        else
                        {
                            if (HRadCorrection == 0)
                            {
                                HRad = HRad + ((HRadSA) / HRadRatio);
                            }
                            else
                            {
                                HRad = HRad + ((HRadSA * HRadCorrection) / HRadRatio);
                            }

                            LogConsole("HRad Average before calibration:" + calculationTemp1 + "\n");

                            //remembers previous offset
                            calculationTemp1 = ((ProbeHeight.X + ProbeHeight.XOpp + ProbeHeight.Y + ProbeHeight.YOpp +
                                                 ProbeHeight.Z + ProbeHeight.ZOpp) / 6);

                            AdjustProbeHeight(HRadSA);

                            LogConsole("HRad:" + HRad + "\n");
                            LogConsole("HRad Correction:" + HRadCorrection + "\n");
                            LogConsole("HRad Average:" + HRadSA + "\n");

                            _serialPort.WriteLine("M206 T3 P885 X" + CheckZero(HRad).ToString());
                            LogConsole("Setting Horizontal Radius\n");
                            Thread.Sleep(pauseTimeSet);

                            LogConsole("Checking height-map\n");
                            InitiateCal();
                        } // end else
                    } // end horizontal radius calibration
                    else if (calculationCount == 1)
                    {
                        ////////////////////////////////////////////////////////////////////////////////
                        //Delta Radius Calibration******************************************************
                        double DASA = ((ProbeHeight.X + ProbeHeight.XOpp) / 2);
                        double DBSA = ((ProbeHeight.Y + ProbeHeight.YOpp) / 2);
                        double DCSA = ((ProbeHeight.Z + ProbeHeight.ZOpp) / 2);


                        if (DASA < accuracy2 && DASA > -accuracy2 && DBSA < accuracy2 && DBSA > -accuracy2 &&
                            DCSA < accuracy2 && DCSA > -accuracy2)
                        {
                            LogConsole("Delta Radius Calibration Success; Checking Height-Map");
                            calculationCount++;
                            InitiateCal();
                        }
                        else
                        {
                            Towers.DA = Towers.DA + ((DASA) / HRadRatio);
                            Towers.DB = Towers.DB + ((DBSA) / HRadRatio);
                            Towers.DC = Towers.DC + ((DCSA) / HRadRatio);

                            WriteDeltaRadiiToEeprom();

                            LogConsole("Checking height-map");
                            InitiateCal();
                        }
                    }
                    else if (calculationCount == 2)
                    {
                        if (analyzeCount == 0)
                        {
                            analyzeCount++;
                            AnalyzeGeometry();

                            LogConsole("Tower Rotation calculated, check XY Panel\n");
                        }

                        ////////////////////////////////////////////////////////////////////////////////
                        //Tower Offset Calibration******************************************************
                        double hTow2 = Math.Max(Math.Max(ProbeHeight.X, ProbeHeight.Y), ProbeHeight.Z);
                        double lTow2 = Math.Min(Math.Min(ProbeHeight.X, ProbeHeight.Y), ProbeHeight.Z);
                        double towDiff2 = hTow2 - lTow2;

                        var XYZAvg = (ProbeHeight.X + ProbeHeight.Y + ProbeHeight.Z) / 3;

                        double calculationXYZAvg = 0;

                        if (towDiff2 < 0.1 && towDiff2 > -0.1)
                        {
                            LogConsole("XYZ Offset Correction: " + calculationTemp1);
                            LogConsole("XYZ Offset Average Before Calibration: " + calculationXYZAvg);
                            LogConsole("XYZ Offset Average Afer Calibration: " + XYZAvg);
                            LogConsole("XYZ Offset Calibration Success; checking height-map\n");
                            //                        calculationCount++;
                            calculationCount++;
                            calculationCheckCount = 0;
                            InitiateCal();
                        }
                        else
                        {
                            //balance axes - retrieve data
                            double offsetXYZ;
                            if (calculationCheckCount == 0)
                            {
                                calculationXYZAvg = (ProbeHeight.X + ProbeHeight.Y + ProbeHeight.Z) / 3;
                                offsetXYZ = 1 / 0.7;

                                LogConsole("XYZ Offset Correction: " + calculationTemp1);
                                LogConsole("XYZ Offset Average Before Calibration: " + calculationXYZAvg);
                                LogConsole("XYZ Offset Average Afer Calibration: " + XYZAvg);
                                calculationCheckCount++;
                            }
                            else
                            {
                                calculationTemp1 = calculationXYZAvg / (calculationXYZAvg - XYZAvg);
                                offsetXYZ = (1 / 0.7) * calculationTemp1;

                                LogConsole("XYZ Offset Correction: " + calculationTemp1);
                                LogConsole("XYZ Offset: " + offsetXYZ);
                                LogConsole("XYZ Offset Average Before Calibration: " + calculationXYZAvg);
                                LogConsole("XYZ Offset Average Afer Calibration: " + XYZAvg);
                            }

                            double theoryX = Offset.X + ProbeHeight.X * stepsPerMM *Offset.XCorrection;
                            double theoryY = Offset.Y + ProbeHeight.Y * stepsPerMM *Offset.YCorrection;
                            double theoryZ = Offset.Z + ProbeHeight.Z * stepsPerMM *Offset.ZCorrection;

                            //correction of one tower allows for XY dimensional accuracy
                            if (ProbeHeight.X > 0)
                            {
                                //if x is positive
                                Offset.X = Offset.X + ProbeHeight.X * stepsPerMM *Offset.XCorrection;
                            }
                            else if (theoryX > 0 && ProbeHeight.X < 0)
                            {
                                //if x is negative and can be decreased
                                Offset.X = Offset.X + ProbeHeight.X * stepsPerMM *Offset.XCorrection;
                            }
                            else
                            {
                                //if X is negative
                                Offset.Y = Offset.Y - ProbeHeight.X * stepsPerMM *Offset.YCorrection * 2;
                                Offset.Z = Offset.Z - ProbeHeight.X * stepsPerMM *Offset.ZCorrection * 2;
                            }

                            //Y
                            if (ProbeHeight.Y > 0)
                            {
                                Offset.Y = Offset.Y + ProbeHeight.Y * stepsPerMM *Offset.YCorrection;
                            }
                            else if (theoryY > 0 && ProbeHeight.Y < 0)
                            {
                                Offset.Y = Offset.Y + ProbeHeight.Y * stepsPerMM *Offset.YCorrection;
                            }
                            else
                            {
                                Offset.X = Offset.X - ProbeHeight.Y * stepsPerMM *Offset.XCorrection * 2;
                                Offset.Z = Offset.Z - ProbeHeight.Y * stepsPerMM *Offset.ZCorrection * 2;
                            }

                            //Z
                            if (ProbeHeight.Z > 0)
                            {
                                Offset.Z = Offset.Z + ProbeHeight.Z * stepsPerMM *Offset.ZCorrection;
                            }
                            else if (theoryZ > 0 && ProbeHeight.Z < 0)
                            {
                                Offset.Z = Offset.Z + ProbeHeight.Z * stepsPerMM *Offset.ZCorrection;
                            }
                            else
                            {
                                Offset.Y = Offset.Y - ProbeHeight.Z * stepsPerMM *Offset.YCorrection * 2;
                                Offset.X = Offset.X - ProbeHeight.Z * stepsPerMM *Offset.XCorrection * 2;
                            }

                            //send data back to printer

                            Offset.X = Math.Round(Offset.X);
                            Offset.Y = Math.Round(Offset.Y);
                            Offset.Z = Math.Round(Offset.Z);

                            LogConsole("XYZ:" + ToLongString(Offset.X) + " " + ToLongString(Offset.Y) + " " +
                                       ToLongString(Offset.Z) + "\n");

                            _serialPort.WriteLine("M206 T1 P893 S" + ToLongString(Offset.X));
                            Thread.Sleep(pauseTimeSet);
                            _serialPort.WriteLine("M206 T1 P895 S" + ToLongString(Offset.Y));
                            Thread.Sleep(pauseTimeSet);
                            _serialPort.WriteLine("M206 T1 P897 S" + ToLongString(Offset.Z));
                            Thread.Sleep(pauseTimeSet);

                            LogConsole("Checking height-map\n");
                            InitiateCal();
                        } //end else 
                    } //end else if calculation count 1
                    else if (calculationCount == 3) //2
                    {
                        ////////////////////////////////////////////////////////////////////////////////
                        //Alpha Rotation Calibration****************************************************
                        double hTow1 = Math.Max(Math.Max(ProbeHeight.XOpp, ProbeHeight.YOpp), ProbeHeight.ZOpp);
                        double lTow1 = Math.Min(Math.Min(ProbeHeight.XOpp, ProbeHeight.YOpp), ProbeHeight.ZOpp);
                        double towDiff1 = hTow1 - lTow1;

                        if (towDiff1 < accuracy2 && towDiff1 > -accuracy2)
                        {
                            LogConsole("Alpha Rotation Calibration Success; checking height-map\n");

                            calculationCount++;
                            InitiateCal();
                        }
                        else
                        {
                            AlphaRotationCalibration();

                            LogConsole("Heights: X:" + ProbeHeight.X + ", XOpp:" + ProbeHeight.XOpp + ", Y:" +
                                       ProbeHeight.Y + ", YOpp:" + ProbeHeight.YOpp + ", Z:" + ProbeHeight.Z +
                                       ", and ZOpp:" + ProbeHeight.ZOpp + "\n");

                            _serialPort.WriteLine("M206 T3 P901 X" + CheckZero(Towers.A).ToString());
                            LogConsole("Setting A Rotation\n");
                            Thread.Sleep(pauseTimeSet);
                            _serialPort.WriteLine("M206 T3 P905 X" + CheckZero(Towers.B).ToString());
                            LogConsole("Setting B Rotation\n");
                            Thread.Sleep(pauseTimeSet);
                            _serialPort.WriteLine("M206 T3 P909 X" + CheckZero(Towers.C).ToString());
                            LogConsole("Setting C Rotation\n");
                            Thread.Sleep(pauseTimeSet);

                            LogConsole("Checking height-map\n");
                            InitiateCal();
                        } //end else
                    } //end alpha rotation calibration
                    else if (calculationCount == 4) //3
                    {
                        ////////////////////////////////////////////////////////////////////////////////
                        //Diagonal Rod Calibration******************************************************
                        double XYZ2 = (ProbeHeight.X + ProbeHeight.Y + ProbeHeight.Z) / 3;
                        double XYZOpp2 = (ProbeHeight.XOpp + ProbeHeight.YOpp + ProbeHeight.ZOpp) / 3;
                        XYZ2 = CheckZero(XYZ2);
                        XYZOpp2 = CheckZero(XYZOpp2);

                        if (XYZOpp2 < accuracy && XYZOpp2 > -accuracy && XYZ2 < accuracy && XYZ2 > -accuracy || t >= 2)
                        {
                            LogConsole("Diagonal Rod Calibration Success; checking height-map\n");
                            LogConsole("Calibration Complete; Homing Printer");
                            _serialPort.WriteLine("G28");

                            calculationCount = 0;
                            advancedCalibration = 0;
                            advancedCalCount = 0;
                        }
                        else
                        {

                            double diagChange = 1 /DiagonalRod.deltaOpp;
                            double towOppDiff = DiagonalRod.deltaTower /DiagonalRod.deltaOpp; //0.5

                            DiagonalRodCalibration(diagChange, towOppDiff);

                            LogConsole("Diagonal Rod:" + diagonalRod + "\n");
                            LogConsole("Heights: X:" + ProbeHeight.X + ", XOpp:" + ProbeHeight.XOpp + ", Y:" +
                                       ProbeHeight.Y + ", YOpp:" + ProbeHeight.YOpp + ", Z:" + ProbeHeight.Z +
                                       ", and ZOpp:" + ProbeHeight.ZOpp + "\n");

                            //send obtained values back to the printer*************************************
                            //Thread.Sleep(5000);
                            _serialPort.WriteLine("M206 T3 P881 X" + diagonalRod.ToString());
                            LogConsole("Setting diagonal rod\n");
                            Thread.Sleep(pauseTimeSet);
                            _serialPort.WriteLine("M206 T3 P885 X" + CheckZero(HRad).ToString());
                            LogConsole("Setting Horizontal Radius\n");
                            Thread.Sleep(pauseTimeSet);

                            //rechecks calibration to either restart or finish
                            LogConsole("Checking height-map\n");
                            t++;
                            InitiateCal();
                        } //end accuracy check
                    } // end diagonal rod calibration

                } // end else
            } // end advanced calibration
        }

        public void WriteDeltaRadiiToEeprom()
        {
            LogConsole("Delta Radii Offsets: " + Towers.DA.ToString() + ", " + Towers.DB.ToString() + ", " +
                       Towers.DC.ToString());

            _serialPort.WriteLine("M206 T3 P913 X" + ToLongString(Towers.DA));
            Thread.Sleep(pauseTimeSet);
            _serialPort.WriteLine("M206 T3 P917 X" + ToLongString(Towers.DB));
            Thread.Sleep(pauseTimeSet);
            _serialPort.WriteLine("M206 T3 P921 X" + ToLongString(Towers.DC));
            Thread.Sleep(pauseTimeSet);
        }

        private void AdjustOffsetZ()
        {
            Offset.Z = Offset.Z + Temp2ProbeHeight.Z*stepsPerMM*Offset.ZCorrection;

            Temp2ProbeHeight.ZOpp = Temp2ProbeHeight.ZOpp + (Temp2ProbeHeight.Z*OffsetPercent.zzOppPerc);
            Temp2ProbeHeight.X = Temp2ProbeHeight.X + (Temp2ProbeHeight.Z*OffsetPercent.zxPerc);
            Temp2ProbeHeight.Y = Temp2ProbeHeight.Y + (Temp2ProbeHeight.Z*OffsetPercent.zyPerc);
            Temp2ProbeHeight.XOpp = Temp2ProbeHeight.XOpp - (Temp2ProbeHeight.Z*OffsetPercent.yxOppPerc);
            Temp2ProbeHeight.YOpp = Temp2ProbeHeight.YOpp - (Temp2ProbeHeight.Z*OffsetPercent.zyOppPerc);
            Temp2ProbeHeight.Z = Temp2ProbeHeight.Z - Temp2ProbeHeight.Z;
        }

        public void AdjustOffsetY()
        {
            Offset.Y = Offset.Y + Temp2ProbeHeight.Y*stepsPerMM*Offset.YCorrection;

            Temp2ProbeHeight.YOpp = Temp2ProbeHeight.YOpp + (Temp2ProbeHeight.Y*OffsetPercent.yyOppPerc);
            Temp2ProbeHeight.X = Temp2ProbeHeight.X + (Temp2ProbeHeight.Y*OffsetPercent.yxPerc);
            Temp2ProbeHeight.Z = Temp2ProbeHeight.Z + (Temp2ProbeHeight.Y*OffsetPercent.yxPerc);
            Temp2ProbeHeight.XOpp = Temp2ProbeHeight.XOpp - (Temp2ProbeHeight.Y*OffsetPercent.yxOppPerc);
            Temp2ProbeHeight.ZOpp = Temp2ProbeHeight.ZOpp - (Temp2ProbeHeight.Y*OffsetPercent.yxOppPerc);
            Temp2ProbeHeight.Y = Temp2ProbeHeight.Y - Temp2ProbeHeight.Y;
        }

        public void AdjustOffsetX()
        {
//if x is positive
            Offset.X = Offset.X + Temp2ProbeHeight.X*stepsPerMM*Offset.XCorrection;

            Temp2ProbeHeight.XOpp = Temp2ProbeHeight.XOpp + (Temp2ProbeHeight.X*OffsetPercent.xxOppPerc); //0.5
            Temp2ProbeHeight.Z = Temp2ProbeHeight.Z + (Temp2ProbeHeight.X*OffsetPercent.xzPerc); //0.25
            Temp2ProbeHeight.Y = Temp2ProbeHeight.Y + (Temp2ProbeHeight.X*OffsetPercent.xyPerc); //0.25
            Temp2ProbeHeight.ZOpp = Temp2ProbeHeight.ZOpp - (Temp2ProbeHeight.X*OffsetPercent.xzOppPerc); //0.25
            Temp2ProbeHeight.YOpp = Temp2ProbeHeight.YOpp - (Temp2ProbeHeight.X*OffsetPercent.xyOppPerc); //0.25
            Temp2ProbeHeight.X = Temp2ProbeHeight.X - Temp2ProbeHeight.X;
        }

        private void AdjustProbeHeight(double HRadSA)
        {
            ProbeHeight.X = ProbeHeight.X - HRadSA;
            ProbeHeight.Y = ProbeHeight.Y - HRadSA;
            ProbeHeight.Z = ProbeHeight.Z - HRadSA;
            ProbeHeight.XOpp = ProbeHeight.XOpp - HRadSA;
            ProbeHeight.YOpp = ProbeHeight.YOpp - HRadSA;
            ProbeHeight.ZOpp = ProbeHeight.ZOpp - HRadSA;

            ProbeHeightCheckZero();
        }

        private void DiagonalRodCalibration(double diagChange, double towOppDiff)
        {
            int i = 0;
            while (i < 100)
            {
                double XYZOpp = (ProbeHeight.XOpp + ProbeHeight.YOpp + ProbeHeight.ZOpp)/3;
                diagonalRod = diagonalRod + (XYZOpp*diagChange);
                ProbeHeight.X = ProbeHeight.X - towOppDiff*XYZOpp;
                ProbeHeight.Y = ProbeHeight.Y - towOppDiff*XYZOpp;
                ProbeHeight.Z = ProbeHeight.Z - towOppDiff*XYZOpp;
                ProbeHeight.XOpp = ProbeHeight.XOpp - XYZOpp;
                ProbeHeight.YOpp = ProbeHeight.YOpp - XYZOpp;
                ProbeHeight.ZOpp = ProbeHeight.ZOpp - XYZOpp;
                XYZOpp = (ProbeHeight.XOpp + ProbeHeight.YOpp + ProbeHeight.ZOpp)/3;
                XYZOpp = CheckZero(XYZOpp);

                double XYZ = (ProbeHeight.X + ProbeHeight.Y + ProbeHeight.Z)/3;
                //hrad
                HRad = HRad + (XYZ/HRadRatio);

                if (XYZOpp >= 0)
                {
                    ProbeHeight.X = ProbeHeight.X - XYZ;
                    ProbeHeight.Y = ProbeHeight.Y - XYZ;
                    ProbeHeight.Z = ProbeHeight.Z - XYZ;
                    ProbeHeight.XOpp = ProbeHeight.XOpp - XYZ;
                    ProbeHeight.YOpp = ProbeHeight.YOpp - XYZ;
                    ProbeHeight.ZOpp = ProbeHeight.ZOpp - XYZ;
                }
                else
                {
                    ProbeHeight.X = ProbeHeight.X + XYZ;
                    ProbeHeight.Y = ProbeHeight.Y + XYZ;
                    ProbeHeight.Z = ProbeHeight.Z + XYZ;
                    ProbeHeight.XOpp = ProbeHeight.XOpp + XYZ;
                    ProbeHeight.YOpp = ProbeHeight.YOpp + XYZ;
                    ProbeHeight.ZOpp = ProbeHeight.ZOpp + XYZ;
                }

                ProbeHeightCheckZero();

                //XYZ is zero
                if (XYZOpp < accuracy && XYZOpp > -accuracy && XYZ < accuracy && XYZ > -accuracy)
                {
                    i = 100;
                    diagonalRod = CheckZero(diagonalRod);
                }
                else
                {
                    i++;
                }
            }
        }

        public void ProbeHeightCheckZero()
        {
            ProbeHeight.X = CheckZero(ProbeHeight.X);
            ProbeHeight.Y = CheckZero(ProbeHeight.Y);
            ProbeHeight.Z = CheckZero(ProbeHeight.Z);
            ProbeHeight.XOpp = CheckZero(ProbeHeight.XOpp);
            ProbeHeight.YOpp = CheckZero(ProbeHeight.YOpp);
            ProbeHeight.ZOpp = CheckZero(ProbeHeight.ZOpp);
        }

        private void AlphaRotationCalibration()
        {
            int k = 0;
            while (k < 100)
            {
                //X Alpha Rotation
                if (ProbeHeight.YOpp > ProbeHeight.ZOpp)
                {
                    double ZYOppAvg = (ProbeHeight.YOpp - ProbeHeight.ZOpp)/2;
                    Towers.A = Towers.A + (ZYOppAvg*AlphaRotationPercentage.X);
                    // (0.5/((diff y0 and z0 at X + 0.5)-(diff y0 and z0 at X = 0))) * 2 = 1.75
                    ProbeHeight.YOpp = ProbeHeight.YOpp - ZYOppAvg;
                    ProbeHeight.ZOpp = ProbeHeight.ZOpp + ZYOppAvg;
                }
                else if (ProbeHeight.YOpp < ProbeHeight.ZOpp)
                {
                    double ZYOppAvg = (ProbeHeight.ZOpp - ProbeHeight.YOpp)/2;

                    Towers.A = Towers.A - (ZYOppAvg*AlphaRotationPercentage.X);
                    ProbeHeight.YOpp = ProbeHeight.YOpp + ZYOppAvg;
                    ProbeHeight.ZOpp = ProbeHeight.ZOpp - ZYOppAvg;
                }

                //Y Alpha Rotation
                if (ProbeHeight.ZOpp > ProbeHeight.XOpp)
                {
                    double XZOppAvg = (ProbeHeight.ZOpp - ProbeHeight.XOpp)/2;
                    Towers.B = Towers.B + (XZOppAvg*AlphaRotationPercentage.Y);
                    ProbeHeight.ZOpp = ProbeHeight.ZOpp - XZOppAvg;
                    ProbeHeight.XOpp = ProbeHeight.XOpp + XZOppAvg;
                }
                else if (ProbeHeight.ZOpp < ProbeHeight.XOpp)
                {
                    double XZOppAvg = (ProbeHeight.XOpp - ProbeHeight.ZOpp)/2;

                    Towers.B = Towers.B - (XZOppAvg*AlphaRotationPercentage.Y);
                    ProbeHeight.ZOpp = ProbeHeight.ZOpp + XZOppAvg;
                    ProbeHeight.XOpp = ProbeHeight.XOpp - XZOppAvg;
                }
                //Z Alpha Rotation
                if (ProbeHeight.XOpp > ProbeHeight.YOpp)
                {
                    double YXOppAvg = (ProbeHeight.XOpp - ProbeHeight.YOpp)/2;
                    Towers.C = Towers.C + (YXOppAvg*AlphaRotationPercentage.Z);
                    ProbeHeight.XOpp = ProbeHeight.XOpp - YXOppAvg;
                    ProbeHeight.YOpp = ProbeHeight.YOpp + YXOppAvg;
                }
                else if (ProbeHeight.XOpp < ProbeHeight.YOpp)
                {
                    double YXOppAvg = (ProbeHeight.YOpp - ProbeHeight.XOpp)/2;

                    Towers.C = Towers.C - (YXOppAvg*AlphaRotationPercentage.Z);
                    ProbeHeight.XOpp = ProbeHeight.XOpp + YXOppAvg;
                    ProbeHeight.YOpp = ProbeHeight.YOpp - YXOppAvg;
                }

                //determine if value is close enough
                double hTow = Math.Max(Math.Max(ProbeHeight.XOpp, ProbeHeight.YOpp), ProbeHeight.ZOpp);
                double lTow = Math.Min(Math.Min(ProbeHeight.XOpp, ProbeHeight.YOpp), ProbeHeight.ZOpp);
                double towDiff = hTow - lTow;

                if (towDiff < accuracy && towDiff > -accuracy)
                {
                    k = 100;
                }
                else
                {
                    k++;
                }
            }

            //log
            LogConsole("ABC:" + Towers.A + " " + Towers.B + " " + Towers.C + "\n");
        }

//end calibrate

        //used in previous delta radii calibration
        private double[] linearRegression(double[] y, double[] x)
        {
            double[] lr = { };
            int n = y.Length;
            double sum_x = 0;
            double sum_y = 0;
            double sum_xy = 0;
            double sum_xx = 0;
            double sum_yy = 0;

            for (var i = 0; i < y.Length; i++)
            {
                sum_x += x[i];
                sum_y += y[i];
                sum_xy += (x[i] * y[i]);
                sum_xx += (x[i] * x[i]);
                sum_yy += (y[i] * y[i]);
            }

            lr[1] = (n * sum_xy - sum_x * sum_y) / (n * sum_xx - sum_x * sum_x); //slope
            lr[2] = (sum_y - lr[1] * sum_x) / n; //intercept
            lr[3] = Math.Pow((n * sum_xy - sum_x * sum_y) / Math.Sqrt((n * sum_xx - sum_x * sum_x) * (n * sum_yy - sum_y * sum_y)), 2);
            //r2

            return lr;
        }

        //check if values are close to zero, then set them to zero - avoids errors
        private double CheckZero(double value)
        {
            if (value > 0 && value < accuracy)
            {
                return 0;
            }
            else if (value < 0 && value > -accuracy)
            {
                return 0;
            }
            else
            {
                return value;
            }
        }

    }
}