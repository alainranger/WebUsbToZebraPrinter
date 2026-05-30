<svelte:options runes={true} />

<script lang="ts">
  import type { ApiProblem, LabelTemplateDetails, PrintLanguage } from "../../lib/api/types";
  import ProblemAlert from "../../lib/ui/ProblemAlert.svelte";

  export interface TemplateDraft {
	name: string;
	description: string;
	sourceLanguage: PrintLanguage;
	rawContent: string;
	widthMm: number;
	heightMm: number;
	dpmm: number;
  }

  let {
	initialData = null,
	problem = null,
	saving = false,
	onSave,
	onCancel,
  }: {
	initialData?: LabelTemplateDetails | null;
	problem?: ApiProblem | null;
	saving?: boolean;
	onSave: (draft: TemplateDraft) => void;
	onCancel: () => void;
  } = $props();

  const isEditing = $derived(initialData !== null);

  let draft = $state<TemplateDraft>(
	initialData
	  ? {
		  name: initialData.name,
		  description: initialData.description ?? "",
		  sourceLanguage: initialData.sourceLanguage,
		  rawContent: initialData.rawContent,
		  widthMm: initialData.dimensions.widthMm,
		  heightMm: initialData.dimensions.heightMm,
		  dpmm: initialData.dimensions.dpmm,
		}
	  : {
		  name: "",
		  description: "",
		  sourceLanguage: "Zpl",
		  rawContent: "^XA\n^FO30,30^A0N,30,30^FD{{recipient}}^FS\n^FO30,80^A0N,26,26^FD{{city}}^FS\n^XZ",
		  widthMm: 102,
		  heightMm: 152,
		  dpmm: 8,
		}
  );
</script>

<div class="panel template-form">
  <h2>{isEditing ? "Edit template" : "New template"}</h2>
  <ProblemAlert {problem} />
  <div class="grid">
	<label>
	  Name
	  <input bind:value={draft.name} disabled={saving} />
	</label>
	<label>
	  Description
	  <input bind:value={draft.description} disabled={saving} />
	</label>
	<label>
	  Source language
	  <select bind:value={draft.sourceLanguage} disabled={saving}>
		<option value="Epl">EPL</option>
		<option value="Zpl">ZPL</option>
	  </select>
	</label>
	<div class="grid three">
	  <label>
		Width (mm)
		<input type="number" bind:value={draft.widthMm} disabled={saving} />
	  </label>
	  <label>
		Height (mm)
		<input type="number" bind:value={draft.heightMm} disabled={saving} />
	  </label>
	  <label>
		DPMM
		<input type="number" bind:value={draft.dpmm} disabled={saving} />
	  </label>
	</div>
	<label>
	  Raw content
	  <textarea bind:value={draft.rawContent} disabled={saving}></textarea>
	</label>
	<div class="actions">
	  <button type="button" onclick={onCancel} disabled={saving}>Cancel</button>
	  <button
		class="primary"
		type="button"
		onclick={() => onSave(draft)}
		disabled={saving}
	  >
		{saving ? "Saving…" : isEditing ? "Update template" : "Create template"}
	  </button>
	</div>
  </div>
</div>

<style>
  .template-form textarea {
	min-height: 10rem;
	font-family: monospace;
	font-size: 0.85rem;
  }
</style>
