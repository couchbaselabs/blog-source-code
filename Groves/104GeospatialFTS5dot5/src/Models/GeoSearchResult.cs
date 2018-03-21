namespace GeospatialSearch.Models
{
    // tag::GeoSearchResult[]
    public class GeoSearchResult
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public InfoWindow InfoWindow { get; set; }
    }

    public class InfoWindow
    {
        public string Content { get; set; }
    }
    // end::GeoSearchResult[]
}