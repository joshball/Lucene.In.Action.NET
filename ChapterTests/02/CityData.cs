namespace ChapterTests._02
{
    public class CityData
    {
        public string Id { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Notes { get; set; }

        public CityData()
        {
        }

        public CityData(string id, string city, string country, string notes)
        {
            Id = id;
            City = city;
            Country = country;
            Notes = notes;
        }
    }
}