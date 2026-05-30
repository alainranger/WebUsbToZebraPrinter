<svelte:options runes={true} />

<script lang="ts">
	import TemplatesPage from './features/templates/TemplatesPage.svelte';
	import WorkbenchPage from './features/workbench/WorkbenchPage.svelte';
	import PrintersPage from './features/printers/PrintersPage.svelte';
	import HistoryPage from './features/print-jobs/HistoryPage.svelte';

	type Page = 'workbench' | 'templates' | 'printers' | 'history';

	let page = $state<Page>('workbench');

	const navItems: Array<{ value: Page; label: string }> = [
		{ value: 'workbench', label: 'Workbench' },
		{ value: 'templates', label: 'Templates' },
		{ value: 'printers', label: 'Printers' },
		{ value: 'history', label: 'Print jobs' }
	];
</script>

<svelte:head>
	<title>Zebra Labels</title>
</svelte:head>

<div class="shell">
	<header class="hero">
		<div>
			<p class="eyebrow">WebUSB + ASP.NET + Aspire</p>
			<h1>Zebra label workbench</h1>
			<p class="subtitle">
				Create EPL/ZPL templates, render previews through Labelize, and send raw payloads to Zebra
				printers from the browser.
			</p>
		</div>
	</header>

	<nav class="nav">
		{#each navItems as item (item.value)}
			<button class:active={page === item.value} onclick={() => (page = item.value)}>
				{item.label}
			</button>
		{/each}
	</nav>

	<main class="content">
		{#if page === 'workbench'}
			<WorkbenchPage />
		{:else if page === 'templates'}
			<TemplatesPage />
		{:else if page === 'printers'}
			<PrintersPage />
		{:else}
			<HistoryPage />
		{/if}
	</main>
</div>
