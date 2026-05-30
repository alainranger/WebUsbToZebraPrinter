namespace ZebraLabels.Api.Domain;

public sealed class LanguageCapabilities
{
    private LanguageCapabilities()
    {
    }

    public LanguageCapabilities(bool supportsEpl, bool supportsZpl, bool allowEplToZplFallback)
    {
        SupportsEpl = supportsEpl;
        SupportsZpl = supportsZpl;
        AllowEplToZplFallback = allowEplToZplFallback;
    }

    public bool SupportsEpl { get; private set; }

    public bool SupportsZpl { get; private set; }

    public bool AllowEplToZplFallback { get; private set; }

    public bool Supports(PrintLanguage language) =>
        language switch
        {
            PrintLanguage.Epl => SupportsEpl,
            PrintLanguage.Zpl => SupportsZpl,
            _ => false
        };
}
