<svelte:options runes={true} />

<script lang="ts">
  let { previewUrl, contentType } = $props<{
    previewUrl: string | null;
    contentType: string | null;
  }>();
</script>

<section class="panel">
  <h2>Preview</h2>
  {#if previewUrl}
    {#if contentType === 'application/pdf'}
      <iframe class="preview-frame" src={previewUrl} title="Label preview"></iframe>
    {:else}
      <img class="preview-frame" src={previewUrl} alt="Label preview" />
    {/if}
  {:else}
    <p class="muted">Generate a preview to inspect the current label rendering.</p>
  {/if}
</section>
