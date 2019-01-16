using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace P_median
{
    public enum CarState
    {
        Vacant, InUse
    }

    public enum LampState
    {

    }
    class GISVertex
    {
        public double x;
        public double y;
        public GISVertex(double _x, double _y)
        {
            x = _x;
            y = _y;
        }

        internal double DistanceTo(GISVertex anotherLocation)
        {
            return Math.Sqrt((x - anotherLocation.x) * (x - anotherLocation.x) + (y - anotherLocation.y) * (y - anotherLocation.y));
        }
        public override string ToString()
        {
            return "x,y: " + x.ToString() + "," + y.ToString();
        }
    }

    class DemandPoint
    {
        public GISVertex location;
        public int CarNumber;
        public int objectID;

        public DemandPoint(GISVertex _location, int carNumber, int objectid)
        {
            location = _location;
            CarNumber = carNumber;
            objectID = objectid;
        }

        public double DistanceTo(GISVertex anotherLocation)
        {
            double distance = location.DistanceTo(anotherLocation);
            return distance;
        }

        public double weightedDistanceTo(DemandPoint anotherDemandPoint)
        {
            double distance = location.DistanceTo(anotherDemandPoint.location);
            return anotherDemandPoint.CarNumber * distance;
        }

        public double noweightDistanceTo(DemandPoint demandPoint)
        {
            return location.DistanceTo(demandPoint.location);
        }
    }
    class FishNet
    {
        GISVertex bottomLeft;
        GISVertex upRight;

        public List<CellCenter> CellCenters = new List<CellCenter>();

        public FishNet(GISVertex _bottomleft, GISVertex _upright, double edgeLen)
        {
            bottomLeft = _bottomleft;
            upRight = _upright;

            int xEdgeCount = (int)Math.Ceiling((upRight.x - bottomLeft.x) / edgeLen);
            int yEdgeCount = (int)Math.Ceiling((upRight.y - bottomLeft.y) / edgeLen);
            //i -> row, j ->column
            for (int i = 0; i < xEdgeCount; i++)
            {
                //i = 0; j = 1
                for (int j = 0; j < yEdgeCount; j++)
                {
                    double x = bottomLeft.x + edgeLen * j + edgeLen / 2;
                    double y = bottomLeft.y + edgeLen * i + edgeLen / 2;
                    CellCenter oneCenter = new CellCenter(x, y);
                    CellCenters.Add(oneCenter);
                }
            }
        }

        public int CellNumber()
        {
            int number = CellCenters.Count;
            return number;
        }
        //FishNet fishNet = new FishNet(new GISVertex(0, 0), new GISVertex(80, 80), 30);
        //List<CellCenter> centers = fishNet.CellCenters;
        //    for (int i = 0; i<centers.Count; i++)
        //    {
        //        Console.WriteLine(centers[i].x + "," + centers[i].y);
        //    }
    }

    class CellCenter
    {
        public double x;
        public double y;
        public CellCenter(double _x, double _y)
        {
            x = _x;
            y = _y;
        }

    }

    class Car
    {
        public GISVertex CarLocation;
        public CarState State;
        public int CarID;
        public string GPSTime;
        public Car(GISVertex _location, CarState _State, int carID)
        {
            CarLocation = _location;
            State = _State;
            CarID = carID;
        }

        public double DistanceTo(GISVertex anotherLocation)
        {
            double distance = CarLocation.DistanceTo(anotherLocation);
            return distance;
        }



    }
}
