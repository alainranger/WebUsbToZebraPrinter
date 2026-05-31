<svelte:options runes={true} />

<script lang="ts">
  import { onMount } from 'svelte';
  import { apiClient } from '../../lib/api/client';
  import type { ApiProblem, PrintLanguage, PrinterProfile } from '../../lib/api/types';
  import ProblemAlert from '../../lib/ui/ProblemAlert.svelte';

  interface PrinterDraft {
    name: string;
    vendorId: number | null;
    productId: number | null;
    preferredLanguage: PrintLanguage;
    supportsEpl: boolean;
    supportsZpl: boolean;
    allowFallback: boolean;
    widthMm: number;
    heightMm: number;
    dpmm: number;
    notes: string;
  }

  let profiles = $state<PrinterProfile[]>([]);
  let problem = $state<ApiProblem | null>(null);
  let draft = $state<PrinterDraft>({
    name: 'Zebra GX430t',
    vendorId: 2655,
    productId: null,
    preferredLanguage: 'Zpl',
    supportsEpl: false,
    supportsZpl: true,
    allowFallback: false,
    widthMm: 102,
    heightMm: 152,
    dpmm: 8,
    notes: 'Primary shipping printer',
  });

  async function loadProfiles() {
    try {
      profiles = await apiClient.listPrinterProfiles();
    } catch (error) {
      problem = error as ApiProblem;
    }
  }

  async function createProfile() {
    problem = null;
    try {
      await apiClient.createPrinterProfile({
        name: draft.name,
        vendorId: draft.vendorId,
        productId: draft.productId,
        preferredLanguage: draft.preferredLanguage,
        capabilities: {
          supportsEpl: draft.supportsEpl,
          supportsZpl: draft.supportsZpl,
          allowEplToZplFallback: draft.allowFallback,
        },
        defaultDimensions: {
          widthMm: draft.widthMm,
          heightMm: draft.heightMm,
          dpmm: draft.dpmm,
        },
        notes: draft.notes,
      });

      await loadProfiles();
    } catch (error) {
      problem = error as ApiProblem;
    }
  }

  onMount(loadProfiles);
</script>

<section class="grid two">
  <div class="panel">
    <h2>Create printer profile</h2>
    <ProblemAlert {problem} />
    <div class="grid">
      <label>
        Name
        <input bind:value={draft.name} />
      </label>
      <div class="grid two">
        <label>
          Vendor ID
          <input type="number" bind:value={draft.vendorId} />
        </label>
        <label>
          Product ID
          <input type="number" bind:value={draft.productId} />
        </label>
      </div>
      <label>
        Preferred language
        <select bind:value={draft.preferredLanguage}>
          <option value="Epl">EPL</option>
          <option value="Zpl">ZPL</option>
        </select>
      </label>
      <div class="grid three">
        <label><input type="checkbox" bind:checked={draft.supportsEpl} /> Supports EPL</label>
        <label><input type="checkbox" bind:checked={draft.supportsZpl} /> Supports ZPL</label>
        <label
          ><input type="checkbox" bind:checked={draft.allowFallback} /> Allow EPL→ZPL fallback</label
        >
      </div>
      <div class="grid three">
        <label>
          Width (mm)
          <input type="number" bind:value={draft.widthMm} />
        </label>
        <label>
          Height (mm)
          <input type="number" bind:value={draft.heightMm} />
        </label>
        <label>
          DPMM
          <input type="number" bind:value={draft.dpmm} />
        </label>
      </div>
      <label>
        Notes
        <input bind:value={draft.notes} />
      </label>
      <button class="primary" onclick={createProfile}>Save profile</button>
    </div>
  </div>

  <div class="panel">
    <h2>Known printers</h2>
    <div class="list">
      {#each profiles as profile (profile.id)}
        <div class="card">
          <h3>{profile.name}</h3>
          <div class="row">
            <span class="badge">{profile.preferredLanguage}</span>
            <span class="muted">
              USB {profile.vendorId ?? 'n/a'}:{profile.productId ?? 'n/a'}
            </span>
          </div>
          <p class="muted">
            Capabilities: {profile.capabilities.supportsEpl ? 'EPL' : ''}
            {profile.capabilities.supportsZpl ? 'ZPL' : ''}
          </p>
          <p>{profile.notes}</p>
        </div>
      {/each}
    </div>
  </div>
</section>
