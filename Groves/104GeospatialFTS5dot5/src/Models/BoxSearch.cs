namespace GeospatialSearch.Models
{
    // tag::BoxSearch[]
    public class BoxSearch
    {
        public double LatitudeTopLeft { get; set; }
        public double LongitudeTopLeft { get; set; }
        public double LatitudeBottomRight { get; set; }
        public double LongitudeBottomRight { get; set; }
    }
    // end::BoxSearch[]
}