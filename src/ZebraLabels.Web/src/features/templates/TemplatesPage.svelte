<svelte:options runes={true} />

<script lang="ts">
  import { onMount } from 'svelte';
  import { apiClient } from '../../lib/api/client';
  import type { ApiProblem, LabelTemplateDetails, LabelTemplateSummary } from '../../lib/api/types';
  import TemplateForm from './TemplateForm.svelte';
  import type { TemplateDraft } from './TemplateForm.svelte';

  // panel mode: null = closed, 'create' = new template, 'edit' = edit existing
  type PanelMode = 'create' | 'edit' | null;

  let templates = $state<LabelTemplateSummary[]>([]);
  let panelMode = $state<PanelMode>(null);
  let editingTemplate = $state<LabelTemplateDetails | null>(null);
  let formProblem = $state<ApiProblem | null>(null);
  let listProblem = $state<ApiProblem | null>(null);
  let busy = $state(false);
  let saving = $state(false);
  let deletingTemplateId = $state<string | null>(null);
  let pendingDeletionTemplate = $state<LabelTemplateSummary | null>(null);

  async function loadTemplates() {
    busy = true;
    listProblem = null;
    try {
      templates = await apiClient.listTemplates();
    } catch (error) {
      listProblem = error as ApiProblem;
    } finally {
      busy = false;
    }
  }

  function openCreate() {
    editingTemplate = null;
    formProblem = null;
    panelMode = 'create';
  }

  async function openEdit(templateId: string) {
    formProblem = null;
    try {
      editingTemplate = await apiClient.getTemplate(templateId);
      panelMode = 'edit';
    } catch (error) {
      listProblem = error as ApiProblem;
    }
  }

  function closePanel() {
    panelMode = null;
    editingTemplate = null;
    formProblem = null;
  }

  async function handleSave(draft: TemplateDraft) {
    saving = true;
    formProblem = null;
    try {
      if (panelMode === 'edit' && editingTemplate) {
        await apiClient.updateTemplate(editingTemplate.id, {
          name: draft.name,
          description: draft.description,
          sourceLanguage: draft.sourceLanguage,
          rawContent: draft.rawContent,
          dimensions: { widthMm: draft.widthMm, heightMm: draft.heightMm, dpmm: draft.dpmm },
          variables: editingTemplate.variables,
        });
      } else {
        await apiClient.createTemplate({
          name: draft.name,
          description: draft.description,
          sourceLanguage: draft.sourceLanguage,
          rawContent: draft.rawContent,
          dimensions: { widthMm: draft.widthMm, heightMm: draft.heightMm, dpmm: draft.dpmm },
          variables: [
            {
              name: 'recipient',
              displayName: 'Recipient',
              isRequired: true,
              defaultValue: '',
              exampleValue: 'Ada Lovelace',
              order: 1,
            },
            {
              name: 'city',
              displayName: 'City',
              isRequired: false,
              defaultValue: 'Paris',
              exampleValue: 'Paris',
              order: 2,
            },
          ],
        });
      }
      await loadTemplates();
      closePanel();
    } catch (error) {
      formProblem = error as ApiProblem;
    } finally {
      saving = false;
    }
  }

  function requestDelete(templateId: string) {
    pendingDeletionTemplate = templates.find((t) => t.id === templateId) ?? null;
  }

  function cancelDelete() {
    if (deletingTemplateId !== null) return;
    pendingDeletionTemplate = null;
  }

  function handleDeleteBackdropKeydown(event: KeyboardEvent) {
    if (event.key === 'Escape' || event.key === 'Enter' || event.key === ' ') {
      event.preventDefault();
      cancelDelete();
    }
  }

  async function confirmDelete() {
    if (!pendingDeletionTemplate) return;
    const templateId = pendingDeletionTemplate.id;
    listProblem = null;
    deletingTemplateId = templateId;
    try {
      await apiClient.deleteTemplate(templateId);
      await loadTemplates();
      if (editingTemplate?.id === templateId) closePanel();
      pendingDeletionTemplate = null;
    } catch (error) {
      listProblem = error as ApiProblem;
    } finally {
      deletingTemplateId = null;
    }
  }

  onMount(loadTemplates);
</script>

<div class="master-details" class:has-panel={panelMode !== null}>
  <!-- Master: template list -->
  <section class="panel master">
    <div class="master-header">
      <h2>Templates</h2>
      <button class="primary" type="button" onclick={openCreate}>+ Add</button>
    </div>

    {#if listProblem}
      <div class="problem">
        <strong>{listProblem.title}</strong>
        {#if listProblem.detail}<div>{listProblem.detail}</div>{/if}
      </div>
    {/if}

    {#if busy}
      <p class="muted">Loading templates…</p>
    {:else if templates.length === 0}
      <p class="muted">No templates yet. Click <strong>+ Add</strong> to create one.</p>
    {:else}
      <table class="template-table">
        <thead>
          <tr>
            <th>Name</th>
            <th>Language</th>
            <th>Version</th>
            <th class="col-actions">Actions</th>
          </tr>
        </thead>
        <tbody>
          {#each templates as template (template.id)}
            <tr class:active-row={editingTemplate?.id === template.id}>
              <td class="col-name">{template.name}</td>
              <td><span class="badge">{template.sourceLanguage}</span></td>
              <td class="muted">v{template.version}</td>
              <td class="col-actions">
                <button
                  class="secondary"
                  type="button"
                  onclick={() => openEdit(template.id)}
                  disabled={deletingTemplateId === template.id}
                >
                  Edit
                </button>
                <button
                  class="danger"
                  type="button"
                  disabled={deletingTemplateId === template.id}
                  onclick={() => requestDelete(template.id)}
                >
                  {deletingTemplateId === template.id ? 'Deleting…' : 'Delete'}
                </button>
              </td>
            </tr>
          {/each}
        </tbody>
      </table>
    {/if}
  </section>

  <!-- Details: form panel -->
  {#if panelMode !== null}
    <TemplateForm
      initialData={panelMode === 'edit' ? editingTemplate : null}
      problem={formProblem}
      {saving}
      onSave={handleSave}
      onCancel={closePanel}
    />
  {/if}
</div>

<!-- Delete confirmation modal -->
{#if pendingDeletionTemplate}
  <div
    class="modal-backdrop"
    role="button"
    tabindex="0"
    aria-label="Close delete confirmation dialog"
    onclick={cancelDelete}
    onkeydown={handleDeleteBackdropKeydown}
  ></div>
  <div class="modal-layer">
    <div
      class="modal panel"
      role="dialog"
      tabindex="-1"
      aria-modal="true"
      aria-labelledby="delete-template-title"
    >
      <h3 id="delete-template-title">Delete template?</h3>
      <p>
        This action will permanently remove
        <strong>{pendingDeletionTemplate.name}</strong>.
      </p>
      <div class="actions modal-actions">
        <button type="button" onclick={cancelDelete} disabled={deletingTemplateId !== null}>
          Cancel
        </button>
        <button
          class="danger"
          type="button"
          onclick={confirmDelete}
          disabled={deletingTemplateId !== null}
        >
          {deletingTemplateId !== null ? 'Deleting…' : 'Delete template'}
        </button>
      </div>
    </div>
  </div>
{/if}

<style>
  .master-details {
    display: grid;
    grid-template-columns: 1fr;
    gap: 1.25rem;
  }

  .master-details.has-panel {
    grid-template-columns: 1fr 1fr;
  }

  .master-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: 1rem;
  }

  .master-header h2 {
    margin: 0;
  }

  .template-table {
    width: 100%;
    border-collapse: collapse;
    font-size: 0.9rem;
  }

  .template-table th,
  .template-table td {
    padding: 0.5rem 0.75rem;
    text-align: left;
    border-bottom: 1px solid rgba(255, 255, 255, 0.07);
  }

  .template-table th {
    font-weight: 600;
    color: var(--color-muted, #888);
    font-size: 0.8rem;
    text-transform: uppercase;
    letter-spacing: 0.04em;
  }

  .template-table tbody tr:last-child td {
    border-bottom: none;
  }

  .template-table tbody tr:hover td {
    background: rgba(255, 255, 255, 0.03);
  }

  .active-row td {
    background: rgba(99, 179, 237, 0.08);
  }

  .col-name {
    font-weight: 500;
  }

  .col-actions {
    text-align: right;
    white-space: nowrap;
  }

  .col-actions button + button {
    margin-left: 0.5rem;
  }

  .modal-backdrop {
    position: fixed;
    inset: 0;
    display: grid;
    place-items: center;
    padding: 1.25rem;
    background: rgba(0, 0, 0, 0.45);
    backdrop-filter: blur(2px);
    z-index: 20;
  }

  .modal {
    width: min(32rem, 100%);
    margin: 0;
  }

  .modal-layer {
    position: fixed;
    inset: 0;
    display: grid;
    place-items: center;
    padding: 1.25rem;
    z-index: 21;
    pointer-events: none;
  }

  .modal-layer .modal {
    pointer-events: auto;
  }

  .modal p {
    margin: 0.75rem 0 0;
  }

  .modal-actions {
    justify-content: flex-end;
    margin-top: 1rem;
  }
</style>
