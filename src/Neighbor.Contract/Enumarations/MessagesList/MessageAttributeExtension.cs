namespace Neighbor.Contract.Enumarations.MessagesList;

[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
sealed class MessageAttribute(string message, string code) : Attribute
{
    public string Message { get; } = message;
    public string Code { get; } = code;
}

public static class MessageAttributeExtension
{
    public static (string Message, string Code) GetMessage(this MessagesList messageCode)
    {
        var type = typeof(MessagesList);
        var field = type.GetField(messageCode.ToString());

        var attribute = (MessageAttribute?)field?.GetCustomAttributes(typeof(MessageAttribute), false).FirstOrDefault();
        return attribute != null ? (attribute.Message, attribute.Code) : (string.Empty, string.Empty);
    }
}
