interface USBEndpoint {
	direction: 'in' | 'out';
	endpointNumber: number;
}

interface USBAlternateInterface {
	endpoints: USBEndpoint[];
}

interface USBInterface {
	interfaceNumber: number;
	alternates: USBAlternateInterface[];
}

interface USBConfiguration {
	interfaces: USBInterface[];
}

interface USBOutTransferResult {
	status: 'ok' | 'stall' | 'babble';
}

interface USBDevice {
	opened: boolean;
	productName?: string;
	configuration: USBConfiguration | null;
	open(): Promise<void>;
	close(): Promise<void>;
	selectConfiguration(configurationValue: number): Promise<void>;
	claimInterface(interfaceNumber: number): Promise<void>;
	releaseInterface(interfaceNumber: number): Promise<void>;
	transferOut(endpointNumber: number, data: BufferSource): Promise<USBOutTransferResult>;
}

interface USBDeviceFilter {
	vendorId?: number;
	productId?: number;
}

interface USB {
	requestDevice(options: { filters: USBDeviceFilter[] }): Promise<USBDevice>;
}

interface Navigator {
	usb: USB;
}
