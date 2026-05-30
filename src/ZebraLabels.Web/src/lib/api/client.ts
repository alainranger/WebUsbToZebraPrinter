import type {
	ApiProblem,
	LabelTemplateDetails,
	LabelTemplateSummary,
	PreparedPrintPayload,
	PreviewOutputType,
	PrintJob,
	PrintLanguage,
	PrinterProfile
} from './types';

async function readProblem(response: Response): Promise<ApiProblem> {
	try {
		return (await response.json()) as ApiProblem;
	} catch {
		return {
			title: response.statusText || 'Request failed',
			status: response.status
		};
	}
}

async function requestJson<T>(path: string, init?: RequestInit): Promise<T> {
	const response = await fetch(path, {
		...init,
		headers: {
			'content-type': 'application/json',
			...(init?.headers ?? {})
		}
	});

	if (!response.ok) {
		throw await readProblem(response);
	}

	return (await response.json()) as T;
}

async function requestBinary(path: string, init?: RequestInit): Promise<{ blob: Blob; contentType: string }> {
	const response = await fetch(path, init);
	if (!response.ok) {
		throw await readProblem(response);
	}

	return {
		blob: await response.blob(),
		contentType: response.headers.get('content-type') ?? 'application/octet-stream'
	};
}

export const apiClient = {
	listTemplates(): Promise<LabelTemplateSummary[]> {
		return requestJson<LabelTemplateSummary[]>('/api/templates');
	},

	getTemplate(templateId: string): Promise<LabelTemplateDetails> {
		return requestJson<LabelTemplateDetails>(`/api/templates/${templateId}`);
	},

	createTemplate(payload: Omit<LabelTemplateDetails, 'id' | 'version'>): Promise<LabelTemplateDetails> {
		return requestJson<LabelTemplateDetails>('/api/templates', {
			method: 'POST',
			body: JSON.stringify(payload)
		});
	},

	updateTemplate(templateId: string, payload: Omit<LabelTemplateDetails, 'id' | 'version'>): Promise<LabelTemplateDetails> {
		return requestJson<LabelTemplateDetails>(`/api/templates/${templateId}`, {
			method: 'PUT',
			body: JSON.stringify(payload)
		});
	},

	async deleteTemplate(templateId: string): Promise<void> {
		const response = await fetch(`/api/templates/${templateId}`, {
			method: 'DELETE'
		});

		if (!response.ok) {
			throw await readProblem(response);
		}
	},

	renderTemplate(templateId: string, outputType: PreviewOutputType, variableValues: Record<string, string>) {
		return requestBinary(`/api/templates/${templateId}/render`, {
			method: 'POST',
			headers: { 'content-type': 'application/json' },
			body: JSON.stringify({ outputType, variableValues })
		});
	},

	renderRaw(payload: {
		language: PrintLanguage;
		content: string;
		dimensions: { widthMm: number; heightMm: number; dpmm: number };
		outputType: PreviewOutputType;
	}) {
		return requestBinary('/api/preview/render', {
			method: 'POST',
			headers: { 'content-type': 'application/json' },
			body: JSON.stringify(payload)
		});
	},

	listPrinterProfiles(): Promise<PrinterProfile[]> {
		return requestJson<PrinterProfile[]>('/api/printer-profiles');
	},

	createPrinterProfile(payload: Omit<PrinterProfile, 'id'>): Promise<PrinterProfile> {
		return requestJson<PrinterProfile>('/api/printer-profiles', {
			method: 'POST',
			body: JSON.stringify(payload)
		});
	},

	listPrintJobs(): Promise<PrintJob[]> {
		return requestJson<PrintJob[]>('/api/print-jobs');
	},

	preparePrintJob(payload: {
		templateId: string;
		printerProfileId: string;
		variableValues: Record<string, string>;
	}): Promise<PreparedPrintPayload> {
		return requestJson<PreparedPrintPayload>('/api/print-jobs/prepare', {
			method: 'POST',
			body: JSON.stringify(payload)
		});
	},

	createPrintJob(payload: {
		templateId?: string;
		printerProfileId: string;
		requestedLanguage: PrintLanguage;
		content: string;
		checksum: string;
	}): Promise<PrintJob> {
		return requestJson<PrintJob>('/api/print-jobs', {
			method: 'POST',
			body: JSON.stringify(payload)
		});
	}
};
