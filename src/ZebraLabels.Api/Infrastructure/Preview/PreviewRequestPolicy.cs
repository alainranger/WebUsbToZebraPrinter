using Microsoft.Extensions.Options;
using ZebraLabels.Api.Domain;

namespace ZebraLabels.Api.Infrastructure.Preview;

public sealed class PreviewRequestPolicy(IOptions<LabelizeOptions> options)
{
    public Dictionary<string, string[]> Validate(string content, LabelDimensions dimensions)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(content))
        {
            errors["content"] = ["Content is required."];
        }

        if (content.Length > options.Value.MaxContentLength)
        {
            errors["content"] = [$"Content must be {options.Value.MaxContentLength} characters or less."];
        }

        if (dimensions.WidthMm <= 0)
        {
            errors["dimensions.widthMm"] = ["Width must be positive."];
        }

        if (dimensions.HeightMm <= 0)
        {
            errors["dimensions.heightMm"] = ["Height must be positive."];
        }

        return errors;
    }
}
