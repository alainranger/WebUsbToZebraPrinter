<svelte:options runes={true} />

<script lang="ts">
	import type { ApiProblem } from '../api/types';

	let { problem } = $props<{ problem: ApiProblem | null }>();
</script>

{#if problem}
	<div class="problem">
		<strong>{problem.title}</strong>
		{#if problem.detail}
			<div>{problem.detail}</div>
		{/if}
		{#if problem.errors}
			<ul>
				{#each Object.entries(problem.errors) as [key, messages] (key)}
					<li><strong>{key}</strong>: {(messages as string[]).join(', ')}</li>
				{/each}
			</ul>
		{/if}
	</div>
{/if}
