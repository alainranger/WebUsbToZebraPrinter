using ZebraLabels.Api.Domain;

namespace ZebraLabels.Api.Infrastructure.Printing;

public sealed class PayloadTransformationService(ILogger<PayloadTransformationService> logger)
{
    public (PrintPayload? Payload, Dictionary<string, string[]> Errors) Resolve(
        PrinterProfile printerProfile,
        PrintPayload payload)
    {
        if (printerProfile.Capabilities.Supports(payload.Language))
        {
            return (payload, new Dictionary<string, string[]>());
        }

        if (payload.Language == PrintLanguage.Epl
            && printerProfile.Capabilities.SupportsZpl
            && printerProfile.Capabilities.AllowEplToZplFallback)
        {
            logger.LogWarning(
                "Printer profile {PrinterProfileId} allows EPL->ZPL fallback, but transformation is not implemented.",
                printerProfile.Id);

            return (null, new Dictionary<string, string[]>
            {
                ["requestedLanguage"] = ["EPL to ZPL fallback is configured but not implemented yet."]
            });
        }

        return (null, new Dictionary<string, string[]>
        {
            ["requestedLanguage"] = [$"Printer profile '{printerProfile.Name}' does not support {payload.Language}."]
        });
    }
}
