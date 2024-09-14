using EventStaf.Models;
using MassTransit;

namespace EventStaf.Infra.MessageQue
{
	public interface IEventStafPublisher
	{
		Task PublishEventOperation(EventModel eventModel, string operationType);
	}

	public class EventStafPublisher : IEventStafPublisher
	{
		private readonly IPublishEndpoint _publishEndpoint;

		public EventStafPublisher(IPublishEndpoint publishEndpoint)
		{
			_publishEndpoint = publishEndpoint;
		}

		public async Task PublishEventOperation(EventModel eventModel, string operationType)
		{
			await _publishEndpoint.Publish(new EventOperationMessage
			{
				OperationId = Guid.NewGuid(),
				OperationName = operationType,
				EventModel = eventModel,
				Timestamp = DateTime.UtcNow
			});
		}
	}
}