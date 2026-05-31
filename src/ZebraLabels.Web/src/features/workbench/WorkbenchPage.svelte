<svelte:options runes={true} />

<script lang="ts">
  import { onDestroy, onMount } from 'svelte';
  import { apiClient } from '../../lib/api/client';
  import type {
    ApiProblem,
    LabelTemplateDetails,
    PrinterProfile,
    PreviewOutputType,
    PreparedPrintPayload,
  } from '../../lib/api/types';
  import {
    browserSupportsWebUsb,
    closePrinterConnection,
    requestPrinterConnection,
    sendRawPayload,
    type PrinterConnection,
  } from '../../lib/webusb/printer';
  import ProblemAlert from '../../lib/ui/ProblemAlert.svelte';
  import PreviewPane from '../../lib/ui/PreviewPane.svelte';

  let templates = $state<LabelTemplateDetails[]>([]);
  let printerProfiles = $state<PrinterProfile[]>([]);
  let selectedTemplateId = $state<string>('');
  let selectedPrinterProfileId = $state<string>('');
  let outputType = $state<PreviewOutputType>('Png');
  let variableValues = $state<Record<string, string>>({ recipient: 'Ada Lovelace', city: 'Paris' });
  let previewUrl = $state<string | null>(null);
  let previewContentType = $state<string | null>(null);
  let problem = $state<ApiProblem | null>(null);
  let statusMessage = $state('Ready');
  let usbConnection = $state<PrinterConnection | null>(null);

  const webUsbSupported = browserSupportsWebUsb();

  const selectedTemplate = $derived(
    templates.find((template) => template.id === selectedTemplateId) ?? null,
  );
  const selectedPrinter = $derived(
    printerProfiles.find((profile) => profile.id === selectedPrinterProfileId) ?? null,
  );

  async function loadData() {
    problem = null;
    try {
      const summaries = await apiClient.listTemplates();
      templates = await Promise.all(
        summaries.map((template) => apiClient.getTemplate(template.id)),
      );
      printerProfiles = await apiClient.listPrinterProfiles();

      if (!selectedTemplateId && templates[0]) {
        selectedTemplateId = templates[0].id;
      }

      if (!selectedPrinterProfileId && printerProfiles[0]) {
        selectedPrinterProfileId = printerProfiles[0].id;
      }
    } catch (error) {
      problem = error as ApiProblem;
    }
  }

  function resetPreview() {
    if (previewUrl) {
      URL.revokeObjectURL(previewUrl);
    }

    previewUrl = null;
    previewContentType = null;
  }

  async function generatePreview() {
    if (!selectedTemplate) {
      return;
    }

    problem = null;
    statusMessage = 'Rendering preview…';
    resetPreview();

    try {
      const result = await apiClient.renderTemplate(
        selectedTemplate.id,
        outputType,
        variableValues,
      );
      previewContentType = result.contentType;
      previewUrl = URL.createObjectURL(result.blob);
      statusMessage = 'Preview rendered.';
    } catch (error) {
      problem = error as ApiProblem;
      statusMessage = 'Preview failed.';
    }
  }

  async function connectPrinter() {
    if (!selectedPrinter) {
      return;
    }

    try {
      usbConnection = await requestPrinterConnection(
        selectedPrinter.vendorId,
        selectedPrinter.productId,
      );
      statusMessage = `Connected to ${usbConnection.device.productName ?? selectedPrinter.name}.`;
    } catch (error) {
      statusMessage = 'Printer connection failed.';
      problem = {
        title: 'Printer connection failed',
        status: 400,
        detail: error instanceof Error ? error.message : 'Unknown WebUSB error.',
      };
    }
  }

  async function printSelectedTemplate() {
    if (!selectedTemplate || !selectedPrinter) {
      return;
    }

    if (!usbConnection) {
      await connectPrinter();
      if (!usbConnection) {
        return;
      }
    }

    let prepared: PreparedPrintPayload;

    try {
      statusMessage = 'Preparing raw print payload…';
      prepared = await apiClient.preparePrintJob({
        templateId: selectedTemplate.id,
        printerProfileId: selectedPrinter.id,
        variableValues,
      });

      statusMessage = 'Sending payload to printer…';
      await sendRawPayload(usbConnection, prepared.content);

      await apiClient.createPrintJob({
        templateId: selectedTemplate.id,
        printerProfileId: selectedPrinter.id,
        requestedLanguage: prepared.effectiveLanguage,
        content: prepared.content,
        checksum: prepared.checksum,
      });

      statusMessage = 'Payload sent and print job logged.';
    } catch (error) {
      statusMessage = 'Print flow failed.';
      problem = error as ApiProblem;
    }
  }

  onDestroy(() => {
    resetPreview();
    void closePrinterConnection(usbConnection);
  });

  onMount(loadData);
</script>

<section class="grid two">
  <div class="panel">
    <h2>Print workbench</h2>
    <ProblemAlert {problem} />
    <div class="grid">
      <div class="grid two">
        <label>
          Template
          <select bind:value={selectedTemplateId}>
            <option value="">Select a template</option>
            {#each templates as template (template.id)}
              <option value={template.id}>{template.name}</option>
            {/each}
          </select>
        </label>
        <label>
          Printer profile
          <select bind:value={selectedPrinterProfileId}>
            <option value="">Select a printer profile</option>
            {#each printerProfiles as profile (profile.id)}
              <option value={profile.id}>{profile.name}</option>
            {/each}
          </select>
        </label>
      </div>

      {#if selectedTemplate}
        <div class="grid">
          <h3>Variables</h3>
          {#each selectedTemplate.variables as variable (variable.name)}
            <label>
              {variable.displayName ?? variable.name}
              <input
                value={variableValues[variable.name] ?? variable.defaultValue ?? ''}
                oninput={(event) =>
                  (variableValues = {
                    ...variableValues,
                    [variable.name]: (event.currentTarget as HTMLInputElement).value,
                  })}
              />
            </label>
          {/each}
        </div>
      {/if}

      <div class="grid two">
        <label>
          Preview output
          <select bind:value={outputType}>
            <option value="Png">PNG</option>
            <option value="Pdf">PDF</option>
          </select>
        </label>
        <div class="card">
          <h3>USB status</h3>
          <p class:warning={!webUsbSupported}>
            {webUsbSupported
              ? 'WebUSB available in this browser.'
              : 'WebUSB unavailable in this browser.'}
          </p>
          {#if usbConnection}
            <p class="muted">
              Connected to {usbConnection.device.productName ?? 'selected USB device'}.
            </p>
          {/if}
        </div>
      </div>

      <div class="actions">
        <button class="secondary" onclick={generatePreview}>Render preview</button>
        <button class="secondary" onclick={connectPrinter}>Connect printer</button>
        <button class="primary" onclick={printSelectedTemplate}>Send to printer</button>
      </div>

      <p class="muted">{statusMessage}</p>
    </div>
  </div>

  <PreviewPane {previewUrl} contentType={previewContentType} />
</section>
