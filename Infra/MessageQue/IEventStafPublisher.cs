using EventStaf.Models;
using MassTransit;
using System.Diagnostics;

namespace EventStaf.Infra.MessageQue
{
	public interface IEventStafPublisher
	{
		Task PublishEventOperation(EventModel eventModel, string operationType);
	}


    //public abstract class MessagePublisher
    //{
    //    private readonly IPublishEndpoint _publishEndpoint;
    //    protected MessagePublisher(IPublishEndpoint publishEndpoint)
    //    {
    //        _publishEndpoint = publishEndpoint;
    //    }

    //    private static readonly ActivitySource _activitySource = new("messaging.masstransit");

    //    public async Task PublishMessage<T>(T message, string operationCode) where T : class
    //    {
    //        var activity = _activitySource.StartActivity(
    //            $"Publish:{typeof(T).Name}",
    //            ActivityKind.Producer);

    //        try
    //        {
    //            await _publishEndpoint.Publish(message, context =>
    //            {
    //                if (activity != null)
    //                {
    //                    // Explicitly add trace context
    //                    context.Headers.Set("traceparent", activity.Id);

    //                    // Add custom properties
    //                    activity.SetTag("messaging.system", "rabbitmq");
    //                    activity.SetTag("messaging.destination_kind", "queue");
    //                    activity.SetTag("messaging.operation", operationCode);
    //                    activity.SetTag("messaging.message_type", typeof(T).Name);
    //                }
    //            });

    //            activity?.SetStatus(ActivityStatusCode.Ok);
    //        }
    //        catch (Exception ex)
    //        {
    //            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
    //            throw ex;
    //        }
    //    }
    //}


    public class EventStafPublisher : IEventStafPublisher
	{
		private readonly IPublishEndpoint _publishEndpoint;

		public EventStafPublisher(IPublishEndpoint publishEndpoint) 
		{
			_publishEndpoint = publishEndpoint;
		}

		public async Task PublishEventOperation(EventModel eventModel, string operationType)
		{
            //await base.PublishMessage(eventModel, operationType);
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