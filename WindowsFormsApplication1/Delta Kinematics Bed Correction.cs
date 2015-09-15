using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using DeltaKinematics.Core;

namespace deltaKinematics
{
    public partial class Form1 : Form
    {
        public Iterations Iterations { get; } = new Iterations();

        public ProbeHeight ProbeHeight { get; } = new ProbeHeight();

        public ProbeHeight TempProbeHeight { get; } = new ProbeHeight();

        public ProbeHeight Temp2ProbeHeight { get; } = new ProbeHeight();

        public ProbeHeight CalculationProbeHeight { get; } = new ProbeHeight();

        private string versionState = "2.0.4PA";
        private string wait = "wait";

        //////////////////////////
        private double centerHeight;
        private int zProbeSet = 0;
        private int advancedCalibration = 0;
        private int advancedCalCount = 0;
        private int calculationCount = 0;
        private int calculationCheckCount = 0;
        private int pauseTimeSet = 1000;
        private int j = 0;

        private double calculationTemp1;
        private double HRadCorrection;
        private double plateDiameter;
        private double valueZ;
        private double valueXYLarge;
        private double valueXYSmall;
        private double stepsPerMM;
        private double xMaxLength;
        private double yMaxLength;
        private double zMaxLength;
        private double diagonalRod;
        private double HRad;
        private double HRadSA;

        private double offsetX;
        private double offsetY;
        private double offsetZ;

        private double offsetXCorrection = 1/0.7;
        private double offsetYCorrection = 1/0.7;
        private double offsetZCorrection = 1/0.7;

        private double xxPerc;
        private double yyPerc;
        private double zzPerc;

        private double A;
        private double B;
        private double C;

        private double DA;
        private double DB;
        private double DC;

        private double zProbe;

        private double probingHeight = 100;

        private double HRadRatio = -0.5;

        private double accuracy = 0.001;
        private double accuracy2 = 0.025;

        private double XYZAvg;

        private double calculationXYZAvg;
        private double offsetXYZ;

        private int t = 0;

        private string comboBoxZMinimumValue;
        private double towerXRotation;
        private double towerYRotation;
        private double towerZRotation;

        //XYZ offset
        //X
        private double xxOppPerc = 0.5;
        private double xyPerc = 0.25;
        private double xyOppPerc = 0.25;
        private double xzPerc = 0.25;
        private double xzOppPerc = 0.25;

        //Y
        private double yyOppPerc = 0.5;
        private double yxPerc = 0.25;
        private double yxOppPerc = 0.25;
        private double yzPerc = 0.25;
        private double yzOppPerc = 0.25;

        //Z
        private double zzOppPerc = 0.5;
        private double zxPerc = 0.25;
        private double zxOppPerc = 0.25;
        private double zyPerc = 0.25;
        private double zyOppPerc = 0.25;

        //diagonal rod
        private double deltaTower = 0.13083;
        private double deltaOpp = 0.21083;

        //alpha rotation
        private double alphaRotationPercentageX = 1.725;
        private double alphaRotationPercentageY = 1.725;
        private double alphaRotationPercentageZ = 1.725;

        private double zProbeSpeed;
        private int analyzeCount = 0;
        private int calibrationState;

        //delta radii
        private double DASA;
        private double DBSA;
        private double DCSA;

        //////////////////////////
        private static SerialPort _serialPort;
        private static bool _continue;
        private Thread readThread;
        private string message;
        private Boolean _initiatingCalibration = false;
        private string _eepromString;


        private void setVariables_Click(object sender, EventArgs e)
        {
            SetVariablesAll();
        }

        public Form1()
        {
            InitializeComponent();
            Init();
        }

        // Initialize the application.
        //
        private void Init()
        {
            readThread = new Thread(Read);
            _serialPort = new SerialPort();

            printerConsoleTextBox.Text = "";
            printerConsoleTextBox.ScrollBars = ScrollBars.Vertical;

            consoleTextBox.Text = "";
            consoleTextBox.ScrollBars = ScrollBars.Vertical;

            String[] zMinArray = {"FSR", "Z-Probe"};
            comboZMin.DataSource = zMinArray;

            // Build the combobox of available ports.
            string[] ports = SerialPort.GetPortNames();

            if (ports.Length >= 1)
            {
                Dictionary<string, string> comboSource =
                    new Dictionary<string, string>();

                int count = 0;

                foreach (string element in ports)
                {
                    comboSource.Add(ports[count], ports[count]);
                    count++;
                }

                portComboBox.DataSource = new BindingSource(comboSource, null);
                portComboBox.DisplayMember = "Key";
                portComboBox.ValueMember = "Value";
            }
            else
            {
                LogConsole("No ports available\n");
            }
            // Basic set of standard baud rates.
            cboBaudRate.Items.Add("250000");
            cboBaudRate.Items.Add("115200");
            cboBaudRate.Items.Add("57600");
            cboBaudRate.Items.Add("38400");
            cboBaudRate.Items.Add("19200");
            cboBaudRate.Items.Add("9600");
            cboBaudRate.Text = "250000"; // This is the default for most RAMBo controllers.
        }


 

    }
}

/*
//REMOVED CODE
//FIRST TOWER ANALYSIS
            towerXRotation = Math.Atan((plateDiameter * 0.963) / ((centerHeight + X) - (centerHeight + XOpp)));
            towerYRotation = Math.Atan((plateDiameter * 0.963) / ((centerHeight + Y) - (centerHeight + YOpp)));
            towerZRotation = Math.Atan((plateDiameter * 0.963) / ((centerHeight + Z) - (centerHeight + ZOpp)));


//SECOND TOWER ANALYSIS
double XPlateRadCalc = Math.Sqrt(Math.Pow(Math.Abs(X - XOpp), 2) + Math.Pow((270 * 0.963), 2));
double hypX = Math.Sqrt(Math.Pow(XPlateRadCalc, 2) + Math.Pow((centerHeight - XOpp), 2));
double XAngleCompl = Math.Acos(Math.Pow(XPlateRadCalc, 2) + Math.Pow((centerHeight - XOpp), 2) - Math.Pow(hypX, 2)) / (2 * XPlateRadCalc * (centerHeight - XOpp));

//use complimentary angle of angle found
towerXRotation = 180 - XAngleCompl * 57.296;

double YPlateRadCalc = Math.Sqrt(Math.Pow(Math.Abs(Y - YOpp), 2) + Math.Pow((270 * 0.963), 2));
double hypY = Math.Sqrt(Math.Pow(YPlateRadCalc, 2) + Math.Pow((centerHeight - YOpp), 2));
double YAngleCompl = Math.Acos(Math.Pow(YPlateRadCalc, 2) + Math.Pow((centerHeight - YOpp), 2) - Math.Pow(hypY, 2)) / (2 * YPlateRadCalc * (centerHeight - YOpp));

//use complimentary angle of angle found
towerYRotation = 180 - YAngleCompl * 57.296;

double ZPlateRadCalc = Math.Sqrt(Math.Pow(Math.Abs(Z - ZOpp), 2) + Math.Pow((270 * 0.963), 2));
double hypZ = Math.Sqrt(Math.Pow(ZPlateRadCalc, 2) + Math.Pow((centerHeight - ZOpp), 2));
double ZAngleCompl = Math.Acos(Math.Pow(ZPlateRadCalc, 2) + Math.Pow((centerHeight - ZOpp), 2) - Math.Pow(hypZ, 2)) / (2 * ZPlateRadCalc * (centerHeight - ZOpp));

//use complimentary angle of angle found
towerZRotation = 180 - ZAngleCompl * 57.296;
cos A = (b2 + c2 − a2) / 2bc

a = centerheight
b = Math.Sqrt(Math.Pow(centerHeight, 2) + Math.Pow(plateDiameter * 0.963))
c = plate build diameter

A = 

if (X < XOpp)
{
    LogConsole("X inverted");
    towerXRotation = (90 - towerXRotation) + 90;
}
if (Y < YOpp)
{
    LogConsole("Y inverted");
    towerYRotation = (90 - towerYRotation) + 90;
}
if (Z < ZOpp)
{
    LogConsole("Z inverted");
    towerZRotation = (90 - towerZRotation) + 90;
}

//THIRD DELTA RADII
                        //should not have large input values, or should even equal zero

                        // lr.slope
                        // lr.intercept
                        // lr.r2
                        //DA = X / -0.5 + DA;

                        //slope,intercept,r2
                    double[] known_yDA = { X, tempX };
                    double[] known_xDA = { 0, 1 };
                    double[] lrDA = linearRegression(known_yDA, known_xDA);
                    double DATemp = DA;
                    double DOpposingX = 1;
                    double DOpposingXL = 1;
                    double DOpposingXR = 1;

                    double[] known_yDB = { Y, tempY };
                    double[] known_xDB = { 0, 1 };
                    double[] lrDB = linearRegression(known_yDB, known_xDB);
                    double DBTemp = DB;
                    double DOpposingY = 1;
                    double DOpposingYL = 1;
                    double DOpposingYR = 1;

                    double[] known_yDC = { Z, tempZ };
                    double[] known_xDC = { 0, 1 };
                    double[] lrDC = linearRegression(known_yDC, known_xDC);
                    double DCTemp = DC;
                    double DOpposingZ = 1;
                    double DOpposingZL = 1;
                    double DOpposingZR = 1;

                    double hTow3 = Math.Max(Math.Max(X, Y), Z);
                    double lTow3 = Math.Min(Math.Min(X, Y), Z);
                    double towDiff3 = hTow3 - lTow3;

                    XYZAvg = (X + Y + Z) / 3;

                    if (towDiff3 < 0.1 && towDiff3 > -0.1)
                    {
                        LogConsole("Delta Radius Calibration Success; checking height-map\n");
                        calculationCount++;
                        initiateCal();
                    }
                    else
                    {
                        j = 0;

                        while (j < 0)
                        {
                            /////////////////////////////////////X
                            LogConsole(lrDA[2].ToString());

                            DA = lrDA[2] * -2 + DA;
                            DA = checkZero(DA);
                            XOpp = ((lrDA[2] * DOpposingX) - XOpp) * -1;
                            YOpp = ((lrDA[2] * DOpposingXL) - YOpp) * -1;
                            ZOpp = ((lrDA[2] * DOpposingXR) - ZOpp) * -1;
                            X = lrDA[2] - X;
                            X = checkZero(X);

                            /////////////////////////////////////Y
                            LogConsole(lrDB[2].ToString());

                            DB = lrDB[2] * -2 + DB + (lrDB[2] * 0.125);
                            DB = checkZero(DB);
                            XOpp = ((lrDB[2] * DOpposingYR) - XOpp) * -1;
                            YOpp = ((lrDB[2] * DOpposingY) - YOpp) * -1;
                            ZOpp = ((lrDB[2] * DOpposingYL) - ZOpp) * -1;
                            Y = lrDB[2] - Y;
                            Y = checkZero(Y);

                            /////////////////////////////////////Z
                            LogConsole(lrDC[2].ToString());

                            DC = lrDC[2] * -2 + DC + (lrDC[2] * 0.25);
                            DC = checkZero(DC);
                            XOpp = ((lrDC[2] * DOpposingZL) - XOpp) * -1;
                            YOpp = ((lrDC[2] * DOpposingZR) - YOpp) * -1;
                            ZOpp = ((lrDC[2] * DOpposingZ) - ZOpp) * -1;
                            Z = lrDC[2] - Z;
                            Z = checkZero(Z);

                            if (X < accuracy && X > -accuracy && Y < accuracy && Y > -accuracy && Z < accuracy && Z > -accuracy)
                            {
                                j = 1;
                            }
                        }
                    }

    
                            while (j != 1)
                            {
                                //XYZ offset
                                xxPerc = 1;
                                yyPerc = 1;
                                zzPerc = 1;

                                //X
                                xxOppPerc = 0.5;
                                xyPerc = 0.25;
                                xyOppPerc = 0.25;
                                xzPerc = 0.25;
                                xzOppPerc = 0.25;

                                //Y
                                yyOppPerc = 0.5;
                                yxPerc = 0.25;
                                yxOppPerc = 0.25;
                                yzPerc = 0.25;
                                yzOppPerc = 0.25;

                                //Z
                                zzOppPerc = 0.5;
                                zxPerc = 0.25;
                                zxOppPerc = 0.25;
                                zyPerc = 0.25;
                                zyOppPerc = 0.25;

                                //correction
                                offsetXCorrection = 1 / 1.4;
                                offsetYCorrection = 1 / 1.4;
                                offsetZCorrection = 1 / 1.4;

                                double theoryX = offsetX + X * stepsPerMM * offsetXCorrection;

                                //correction of one tower allows for XY dimensional accuracy
                                if (X > 0)
                                {
                                    //if x is positive
                                    offsetX = offsetX + X * stepsPerMM * offsetXCorrection;

                                    XOpp = XOpp + (X * xxOppPerc);//0.5
                                    Z = Z + (X * xzPerc);//0.25
                                    Y = Y + (X * xyPerc);//0.25
                                    ZOpp = ZOpp - (X * xzOppPerc);//0.25
                                    YOpp = YOpp - (X * xyOppPerc);//0.25
                                    X = X - X;
                                }
                                else if (theoryX > 0 && X < 0)
                                {
                                    //if x is negative and can be decreased
                                    offsetX = offsetX + X * stepsPerMM * offsetXCorrection;

                                    XOpp = XOpp + (X * xxOppPerc);//0.5
                                    Z = Z + (X * xzPerc);//0.25
                                    Y = Y + (X * xyPerc);//0.25
                                    ZOpp = ZOpp - (X * xzOppPerc);//0.25
                                    YOpp = YOpp - (X * xyOppPerc);//0.25
                                    X = X - X;
                                }
                                else
                                {
                                    //if X is negative
                                    offsetY = offsetY - X * stepsPerMM * offsetYCorrection * 2;
                                    offsetZ = offsetZ - X * stepsPerMM * offsetZCorrection * 2;

                                    YOpp = YOpp - (X * 2 * yyOppPerc);
                                    X = X - (X * 2 * yxPerc);
                                    Z = Z - (X * 2 * yxPerc);
                                    XOpp = XOpp + (X * 2 * yxOppPerc);
                                    ZOpp = ZOpp + (X * 2 * yxOppPerc);
                                    Y = Y + X * 2;

                                    ZOpp = ZOpp - (X * 2 * zzOppPerc);
                                    X = X - (X * 2 * zxPerc);
                                    Y = Y - (X * 2 * zyPerc);
                                    XOpp = XOpp + (X * 2 * yxOppPerc);
                                    YOpp = YOpp + (X * 2 * zyOppPerc);
                                    Z = Z + X * 2;
                                }

                                double theoryY = offsetY + Y * stepsPerMM * offsetYCorrection;

                                //Y
                                if (Y > 0)
                                {
                                    offsetY = offsetY + Y * stepsPerMM * offsetYCorrection;

                                    YOpp = YOpp + (Y * yyOppPerc);
                                    X = X + (Y * yxPerc);
                                    Z = Z + (Y * yxPerc);
                                    XOpp = XOpp - (Y * yxOppPerc);
                                    ZOpp = ZOpp - (Y * yxOppPerc);
                                    Y = Y - Y;
                                }
                                else if (theoryY > 0 && Y < 0)
                                {
                                    offsetY = offsetY + Y * stepsPerMM * offsetYCorrection;

                                    YOpp = YOpp + (Y * yyOppPerc);
                                    X = X + (Y * yxPerc);
                                    Z = Z + (Y * yxPerc);
                                    XOpp = XOpp - (Y * yxOppPerc);
                                    ZOpp = ZOpp - (Y * yxOppPerc);
                                    Y = Y - Y;
                                }
                                else
                                {
                                    offsetX = offsetX - Y * stepsPerMM * offsetXCorrection * 2;
                                    offsetZ = offsetZ - Y * stepsPerMM * offsetZCorrection * 2;

                                    XOpp = XOpp - (Y * 2 * xxOppPerc);//0.5
                                    Z = Z - (Y * 2 * xzPerc);//0.25
                                    Y = Y - (Y * 2 * xyPerc);//0.25
                                    ZOpp = ZOpp + (Y * 2 * xzOppPerc);//0.25
                                    YOpp = YOpp + (Y * 2 * xyOppPerc);//0.25
                                    X = X + Y * 2;

                                    ZOpp = ZOpp - (Y * 2 * zzOppPerc);
                                    X = X - (Y * 2 * zxPerc);
                                    Y = Y - (Y * 2 * zyPerc);
                                    XOpp = XOpp + (Y * 2 * yxOppPerc);
                                    YOpp = YOpp + (Y * 2 * zyOppPerc);
                                    Z = Z + Y * 2;
                                }

                                double theoryZ = offsetZ + Z * stepsPerMM * offsetZCorrection;

                                //Z
                                if (Z > 0)
                                {
                                    offsetZ = offsetZ + Z * stepsPerMM * offsetZCorrection;

                                    ZOpp = ZOpp + (Z * zzOppPerc);
                                    X = X + (Z * zxPerc);
                                    Y = Y + (Z * zyPerc);
                                    XOpp = XOpp - (Z * yxOppPerc);
                                    YOpp = YOpp - (Z * zyOppPerc);
                                    Z = Z - Z;
                                }
                                else if (theoryZ > 0 && Z < 0)
                                {
                                    offsetZ = offsetZ + Z * stepsPerMM * offsetZCorrection;

                                    ZOpp = ZOpp + (Z * zzOppPerc);
                                    X = X + (Z * zxPerc);
                                    Y = Y + (Z * zyPerc);
                                    XOpp = XOpp - (Z * yxOppPerc);
                                    YOpp = YOpp - (Z * zyOppPerc);
                                    Z = Z - Z;
                                }
                                else
                                {
                                    offsetY = offsetY - Z * stepsPerMM * offsetYCorrection * 2;
                                    offsetX = offsetX - Z * stepsPerMM * offsetXCorrection * 2;

                                    XOpp = XOpp - (Z * 2 * xxOppPerc);//0.5
                                    Z = Z - (Z * 2 * xzPerc);//0.25
                                    Y = Y - (Z * 2 * xyPerc);//0.25
                                    ZOpp = ZOpp + (Z * 2 * xzOppPerc);//0.25
                                    YOpp = YOpp + (Z * 2 * xyOppPerc);//0.25
                                    X = X + Z * 2;

                                    YOpp = YOpp - (Z * 2 * yyOppPerc);
                                    X = X - (Z * 2 * yxPerc);
                                    Z = Z - (Z * 2 * yxPerc);
                                    XOpp = XOpp + (Z * 2 * yxOppPerc);
                                    ZOpp = ZOpp + (Z * 2 * yxOppPerc);
                                    Y = Y + Z * 2;
                                }

                                X = checkZero(X);
                                Y = checkZero(Y);
                                Z = checkZero(Z);
                                XOpp = checkZero(XOpp);
                                YOpp = checkZero(YOpp);
                                ZOpp = checkZero(ZOpp);

                                if (X < accuracy && X > -accuracy && Y < accuracy && Y > -accuracy && Z < accuracy && Z > -accuracy)
                                {
                                    j = 1;
                                }
                                else
                                {
                                    LogConsole("Calculation XYZ:" + ToLongString(offsetX) + " " + ToLongString(offsetY) + " " + ToLongString(offsetZ) + "\n");
                                }
                            }
*/
