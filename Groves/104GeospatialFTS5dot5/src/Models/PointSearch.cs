namespace GeospatialSearch.Models
{
    public class PointSearch
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Distance { get; set; }
        // miles is being assumed as the unit
        public string DistanceWithUnits => Distance + "mi";
    }
}