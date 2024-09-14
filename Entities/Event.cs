using EventStaf.Infra.Swagger;
using MassTransit.Topology;

namespace EventStaf.Entities
{
	public class Event : EntityBase
	{
        [SwaggerProperty(SwaggerPropertyType.Name)]
        public string? EventCode { get; set; }


		[SwaggerProperty(SwaggerPropertyType.Name)]
		public string? Description { get; set; }

        public EventType EventType { get; set; }

        public PlaceType PlaceType { get; set; }

		public int PeopleCount { get; set; }

		[SwaggerProperty(SwaggerPropertyType.RegisterDate)]
		public DateTime EventStartDate { get; set; }

		[SwaggerProperty(SwaggerPropertyType.RegisterDate)]
		public DateTime EventEndDate { get; set; }

        public bool HasCatering { get; set; }

		public bool HasBeverages { get; set; }

        public int StaffCount { get; set; }
    }

    public enum EventType : int
    {
        Wedding,
        Birthday,
        Graduation,
        Corporate,
        Funeral,
        Other
    }

	public enum PlaceType : int
	{
		Saloon,
        Hotel,
        Garden,
        Beach,
        School,
        MetingRoom,
        Other    
	}
}
