namespace Neighbor.Contract.Abstractions.Services;

public interface IDialogflowService
{
    Task<string> DetectIntentAsync(string sessionId, string text, string languageCode);
}
