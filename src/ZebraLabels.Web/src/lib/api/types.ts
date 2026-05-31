export type PrintLanguage = 'Epl' | 'Zpl';
export type PreviewOutputType = 'Png' | 'Pdf';
export type PrintJobStatus = 'Draft' | 'Submitted' | 'Rendered' | 'Sent' | 'Failed';

export interface LabelDimensions {
  widthMm: number;
  heightMm: number;
  dpmm: number;
}

export interface TemplateVariable {
  name: string;
  displayName?: string | null;
  isRequired: boolean;
  defaultValue?: string | null;
  exampleValue?: string | null;
  order: number;
}

export interface LabelTemplateSummary {
  id: string;
  name: string;
  sourceLanguage: PrintLanguage;
  version: number;
}

export interface LabelTemplateDetails extends LabelTemplateSummary {
  description?: string | null;
  rawContent: string;
  dimensions: LabelDimensions;
  variables: TemplateVariable[];
}

export interface LanguageCapabilities {
  supportsEpl: boolean;
  supportsZpl: boolean;
  allowEplToZplFallback: boolean;
}

export interface PrinterProfile {
  id: string;
  name: string;
  vendorId?: number | null;
  productId?: number | null;
  preferredLanguage: PrintLanguage;
  capabilities: LanguageCapabilities;
  defaultDimensions: LabelDimensions;
  notes?: string | null;
}

export interface PrintJob {
  id: string;
  templateId?: string | null;
  printerProfileId: string;
  requestedLanguage: PrintLanguage;
  effectiveLanguage: PrintLanguage;
  status: PrintJobStatus;
  failureReason?: string | null;
}

export interface PreparedPrintPayload {
  content: string;
  checksum: string;
  effectiveLanguage: PrintLanguage;
}

export interface ApiProblem {
  title: string;
  status: number;
  detail?: string;
  traceId?: string;
  errors?: Record<string, string[]>;
}
