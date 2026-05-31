<svelte:options runes={true} />

<script lang="ts">
  import { onMount } from 'svelte';
  import { apiClient } from '../../lib/api/client';
  import type { ApiProblem, PrintJob } from '../../lib/api/types';
  import ProblemAlert from '../../lib/ui/ProblemAlert.svelte';

  let jobs = $state<PrintJob[]>([]);
  let problem = $state<ApiProblem | null>(null);

  async function loadJobs() {
    try {
      jobs = await apiClient.listPrintJobs();
    } catch (error) {
      problem = error as ApiProblem;
    }
  }

  onMount(loadJobs);
</script>

<section class="panel">
  <h2>Print history</h2>
  <ProblemAlert {problem} />
  {#if jobs.length === 0}
    <p class="muted">No print jobs have been logged yet.</p>
  {:else}
    <table>
      <thead>
        <tr>
          <th>Status</th>
          <th>Requested</th>
          <th>Effective</th>
          <th>Template</th>
          <th>Printer profile</th>
        </tr>
      </thead>
      <tbody>
        {#each jobs as job (job.id)}
          <tr>
            <td>{job.status}</td>
            <td>{job.requestedLanguage}</td>
            <td>{job.effectiveLanguage}</td>
            <td>{job.templateId ?? 'n/a'}</td>
            <td>{job.printerProfileId}</td>
          </tr>
        {/each}
      </tbody>
    </table>
  {/if}
</section>
