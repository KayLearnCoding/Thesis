using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Thesis
{
    public enum CarState
    {
        Vacant, InUse
    }
    public enum LampState
    {

    }
    public class GISVertex
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
    }
    class DemandPoint
    {
        public GISVertex location;
        public int CarNumber;
        public int CarID;

        public DemandPoint(GISVertex _location, int carNumber, int carID)
        {
            location = _location;
            CarNumber = carNumber;
            CarID = carID;
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

    }
    class FishNet
    {
        GISVertex bottomLeft;
        GISVertex upRight;

        public List<CellCenter> CellCenters = new List<CellCenter>();
        public FishNet(GISVertex _bottomleft, GISVertex _upright,double edgeLen)
        {
            bottomLeft = _bottomleft;
            upRight = _upright;

            int xEdgeCount = (int)((upRight.x - bottomLeft.x) / edgeLen);
            int yEdgeCount = (int)((upRight.y - bottomLeft.y) / edgeLen);
            //i -> row, j ->column
            for (int i = 0; i < xEdgeCount; i++)
            {
                //i = 0; j = 1
                for (int j = 0; j < yEdgeCount; j++)
                {
                    double x = bottomLeft.x + edgeLen * j  + edgeLen / 2;
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
    }
    public class OneJourney
    {
        int StartLineNum;
        int EndLineNum;
        string CarID;
        GISVertex StartLocation;
        GISVertex EndLocation;
        public void StartAt(GISVertex startPosition)
        {
            StartLocation = startPosition;
        }
        public void EndAt(GISVertex endPositon)
        {
            EndLocation = endPositon;
        }
    }
    class Journeys
    {
        List<OneJourney> journeys = new List<OneJourney>();
        public void addJourney(OneJourney oneJourney)
        {
            journeys.Add(oneJourney);
        }
    }
    class CellCenter
    {
        double x;
        double y;
        public CellCenter(double _x, double _y)
        {
            x = _x;
            y = _y;
        }

    }

    public class OnePoint
    {
        public GISVertex CarLocation;
        public CarState State;
        public string CarID;
        public string GPSTime;
        public OnePoint(GISVertex _location, CarState _State, string carID, string gpstime)
        {
            CarLocation = _location;
            State = _State;
            CarID= carID;
            GPSTime = gpstime;
        }

        public double DistanceTo(GISVertex anotherLocation)
        {
            double distance = CarLocation.DistanceTo(anotherLocation);
            return distance;
        }
    }

    public class TrackPoints
    {
        List<OnePoint> carTrack = new List<OnePoint>();

        public void addTrackPoint(OnePoint oneCar)
        {
            carTrack.Add(oneCar);
        }
        public string ShowTrackPoints()
        {
            String result = "";
            for (int i = 0; i < carTrack.Count; i++)
            {
                result += "ID:" + carTrack[i].CarID + " ,Location: " + carTrack[i].CarLocation.x + "," + carTrack[i].CarLocation.y;
            }
            return result;
        }
        public List<OnePoint> GetThisCarTrack()
        {
            return carTrack;
        }

        public int ShowCarTrackNum()
        {
            return carTrack.Count;
        }
    }
}
