using EventStaf.Entities;
using EventStaf.Infra.Swagger;

namespace EventStaf.Models
{


	public class EventModel 
	{
        public int Id { get; set; }

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

	public class EventOperationMessage
	{
		public Guid OperationId { get; set; }
		public string OperationName { get; set; }
		public EventModel EventModel { get; set; }	
		public DateTime Timestamp { get; set; }
	}
}
