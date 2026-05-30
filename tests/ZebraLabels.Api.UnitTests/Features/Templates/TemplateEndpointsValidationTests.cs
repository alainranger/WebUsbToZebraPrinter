using ZebraLabels.Api.Domain;
using ZebraLabels.Api.Features.Templates;

namespace ZebraLabels.Api.UnitTests.Features.Templates;

public sealed class TemplateEndpointsValidationTests
{
	[Fact]
	public void ValidateRequest_ReturnsError_WhenEplTemplateContainsZplCommands()
	{
		var request = new TemplateEndpoints.CreateOrUpdateTemplateRequest(
			"Shipping Label",
			null,
			PrintLanguage.Epl,
			"^XA\n^FO50,50^FDHello^FS\n^XZ",
			new TemplateEndpoints.LabelDimensionsDto(102, 152, 8),
			[]);

		var errors = TemplateRequestValidator.ValidateCreateOrUpdate(request);

		Assert.True(errors.ContainsKey("sourceLanguage"));
		Assert.Contains("appears to be ZPL", errors["sourceLanguage"][0]);
	}

	[Fact]
	public void ValidateRequest_DoesNotReturnLanguageError_WhenZplTemplateContainsZplCommands()
	{
		var request = new TemplateEndpoints.CreateOrUpdateTemplateRequest(
			"Shipping Label",
			null,
			PrintLanguage.Zpl,
			"^XA\n^FO50,50^FDHello^FS\n^XZ",
			new TemplateEndpoints.LabelDimensionsDto(102, 152, 8),
			[]);

		var errors = TemplateRequestValidator.ValidateCreateOrUpdate(request);

		Assert.False(errors.ContainsKey("sourceLanguage"));
	}
}
