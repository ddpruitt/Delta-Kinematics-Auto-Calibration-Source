using System;
using System.Linq;
using System.Threading;

namespace DeltaKinematics.Core
{
    public class Calibration
    {
        public ProbeHeight ProbeHeight { get; set; } = new ProbeHeight();

        public TowerRotation TowerRotation { get; set; } = new TowerRotation();

        public Iterations Iterations { get; } = new Iterations();

        public double PlateDiameter { get; set; }
        public double CenterHeight { get; set; }

        public double HorizontalRadius { get; set; }

        public double OffsetScalingMax { get; set; }

        public CartesianPoint Top { get; set; }
        public CartesianPoint Bottom { get; set; }

        public double valueZ;
        public double valueXYLarge;
        public double valueXYSmall;

        public string comboBoxZMinimumValue;
        public int zProbeSet = 0;

        public Calibration()
        {
            
        }

        public void InitiateCal()
        {
            //set gcode specifications
            valueZ = 0.482 * PlateDiameter;
            valueXYLarge = 0.417 * PlateDiameter;
            valueXYSmall = 0.241 * PlateDiameter;

            //set zprobe height to zero

            //if (comboBoxZMinimumValue == "Z-Probe" && Iterations.IterationNum == 0)
            //{
            //    zProbeSet = 1;
            //    Thread.Sleep(pauseTimeSet);
            //    _serialPort.WriteLine("G28");
            //    Thread.Sleep(pauseTimeSet);
            //    _serialPort.WriteLine("G30");

            //    double heightTime = (zMaxLength / zProbeSpeed) * 1000;
            //    Thread.Sleep(Convert.ToInt32(heightTime));
            //}

            //zero bed
            //Thread.Sleep(pauseTimeSet);
            //_serialPort.WriteLine("G28");
            //Thread.Sleep(pauseTimeSet);
            //_serialPort.WriteLine("G1 Z" + probingHeight.ToString() + " F5000");
            //Thread.Sleep(pauseTimeSet);
            //_serialPort.WriteLine("G30");
        }

        public void AnalyzeGeometry()
        {
            //calculates the tower angle at the top and bottom
            TowerRotation.X = TowerRotationCalculation(PlateDiameter, ProbeHeight.X, ProbeHeight.XOpp);
            TowerRotation.Y = TowerRotationCalculation(PlateDiameter, ProbeHeight.Y, ProbeHeight.YOpp);
            TowerRotation.Z = TowerRotationCalculation(PlateDiameter, ProbeHeight.Z, ProbeHeight.ZOpp);

            //Calculates the radii for each tower at the top and bottom of the towers
            var hypotenuse = new CartesianPoint
            {
                X = Hypotenuse(TowerRotation.X, CenterHeight),
                Y = Hypotenuse(TowerRotation.Y, CenterHeight),
                Z = Hypotenuse(TowerRotation.Z, CenterHeight)
            };

            var radiusSide = new CartesianPoint
            {
                X = RadiusSide(hypotenuse.X, CenterHeight),
                Y = RadiusSide(hypotenuse.Y, CenterHeight),
                Z = RadiusSide(hypotenuse.Z, CenterHeight)
            };

            //X
            Bottom.X = HorizontalRadius;
            Top.X = HorizontalRadius - radiusSide.X;


            //Y
            Bottom.Y = HorizontalRadius;
            Top.Y = HorizontalRadius - radiusSide.Y;

            //Z
            Bottom.Z = HorizontalRadius;
            Top.Z = HorizontalRadius - radiusSide.Z;

            //find max offset in Xy scaling with current tower offsets
            var aScaling = new[]
            {
                Math.Abs(90 - hypotenuse.X),
                Math.Abs(90 - hypotenuse.Y),
                Math.Abs(90 - hypotenuse.Z)
            }.Max();

            OffsetScalingMax = (Math.Sin(90)/Math.Sin(Math.PI - 90 - aScaling))*CenterHeight;
        }

        public double TowerRotationCalculation(double plateDiameter, double probeHeight, double probeHeightOpp)
        {
            var rotation =  Math.Acos((plateDiameter * 0.963) / Math.Sqrt(Math.Pow(Math.Abs(probeHeight - probeHeightOpp), 2) + Math.Pow((plateDiameter * 0.963), 2))) * 57.296 * 5;
            return probeHeight < probeHeightOpp ? 90 - rotation : 90 + rotation;
        }

        public double Hypotenuse(double rotation, double centerHeight)
        {
            return (Math.Sin(90)/Math.Sin(Math.PI - rotation - (180 - rotation)))*centerHeight;
        }

        public double RadiusSide(double hypotenuse, double centerHeight)
        {
            return Math.Sqrt(Math.Pow(hypotenuse, 2) - Math.Pow(centerHeight, 2));
        }
    }
}