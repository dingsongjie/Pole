namespace Pole.EventBus.RabbitMQ
{
    public interface IRabbitMQClient
    {
        ModelWrapper PullModel();
    }
}
