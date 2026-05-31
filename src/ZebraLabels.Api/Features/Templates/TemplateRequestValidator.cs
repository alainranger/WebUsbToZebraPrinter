using ZebraLabels.Api.Domain;

namespace ZebraLabels.Api.Features.Templates;

public static class TemplateRequestValidator
{
    public static Dictionary<string, string[]> ValidateCreateOrUpdate(TemplateEndpoints.CreateOrUpdateTemplateRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            errors["name"] = ["Name is required."];
        }

        if (string.IsNullOrWhiteSpace(request.RawContent))
        {
            errors["rawContent"] = ["Raw content is required."];
        }
        else
        {
            foreach (var error in ValidateContentMatchesLanguage(request.SourceLanguage, request.RawContent))
            {
                errors[error.Key] = error.Value;
            }
        }

        if (request.Dimensions is null)
        {
            errors["dimensions"] = ["Dimensions are required."];
        }
        else
        {
            try
            {
                _ = new LabelDimensions(request.Dimensions.WidthMm, request.Dimensions.HeightMm, request.Dimensions.Dpmm);
            }
            catch (ArgumentOutOfRangeException exception)
            {
                errors[$"dimensions.{exception.ParamName}"] = [exception.Message];
            }
        }

        var variables = request.Variables ?? [];
        var duplicateNames = variables
            .GroupBy(variable => variable.Name, StringComparer.OrdinalIgnoreCase)
            .Where(group => !string.IsNullOrWhiteSpace(group.Key) && group.Count() > 1)
            .Select(group => group.Key)
            .ToArray();

        if (duplicateNames.Length > 0)
        {
            errors["variables"] = [$"Variable names must be unique. Duplicates: {string.Join(", ", duplicateNames)}."];
        }

        return errors;
    }

    public static Dictionary<string, string[]> ValidateContentMatchesLanguage(PrintLanguage language, string content)
    {
        var errors = new Dictionary<string, string[]>();
        var normalizedContent = content.Trim();

        var looksLikeZpl = normalizedContent.Contains("^XA", StringComparison.OrdinalIgnoreCase)
            || normalizedContent.Contains("^XZ", StringComparison.OrdinalIgnoreCase)
            || normalizedContent.Contains("^FO", StringComparison.OrdinalIgnoreCase)
            || normalizedContent.Contains("^FD", StringComparison.OrdinalIgnoreCase)
            || normalizedContent.Contains("^FS", StringComparison.OrdinalIgnoreCase);

        if (language == PrintLanguage.Epl && looksLikeZpl)
        {
            errors["sourceLanguage"] = ["Source language is EPL, but content appears to be ZPL."];
        }

        return errors;
    }
}
