using System.Security.Cryptography;
using System.Text;
using ZebraLabels.Api.Domain;

namespace ZebraLabels.Api.Infrastructure.Rendering;

public sealed class TemplateRenderingService
{
    public (PrintPayload? Payload, Dictionary<string, string[]> Errors) TryRender(
        LabelTemplate template,
        IReadOnlyDictionary<string, string?> variableValues)
    {
        var errors = new Dictionary<string, string[]>();
        var substitutions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var variable in template.Variables.OrderBy(variable => variable.Order))
        {
            var hasExplicitValue = variableValues.TryGetValue(variable.Name, out var explicitValue) && !string.IsNullOrWhiteSpace(explicitValue);
            var effectiveValue = hasExplicitValue
                ? explicitValue!
                : variable.DefaultValue;

            if (variable.IsRequired && string.IsNullOrWhiteSpace(effectiveValue))
            {
                errors[variable.Name] = [$"A value is required for variable '{variable.Name}'."];
                continue;
            }

            substitutions[variable.Name] = effectiveValue ?? string.Empty;
        }

        if (errors.Count > 0)
        {
            return (null, errors);
        }

        var resolvedContent = template.RawContent;
        foreach (var substitution in substitutions)
        {
            resolvedContent = resolvedContent.Replace($"{{{{{substitution.Key}}}}}", substitution.Value, StringComparison.OrdinalIgnoreCase);
        }

        return (
            new PrintPayload(
                resolvedContent,
                Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(resolvedContent))),
                template.SourceLanguage),
            errors);
    }
}
