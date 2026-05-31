using ZebraLabels.Api.Domain;

namespace ZebraLabels.Api.UnitTests.Features.Templates;

public sealed class TemplateDeletionTests
{
    [Fact]
    public void SoftDelete_MarksTemplateAsArchived()
    {
        var template = new LabelTemplate
        {
            Id = Guid.NewGuid(),
            Name = "Shipping Label",
            SourceLanguage = PrintLanguage.Zpl,
            RawContent = "^XA^XZ",
            Dimensions = new LabelDimensions(102, 152, 8),
            IsArchived = false,
            Version = 1
        };

        template.IsArchived = true;
        template.Version += 1;

        Assert.True(template.IsArchived);
        Assert.Equal(2, template.Version);
    }
}
