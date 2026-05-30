export interface PrinterConnection {
	device: USBDevice;
	interfaceNumber: number;
	endpointNumber: number;
}

function getZebraLikeFilter(vendorId?: number | null, productId?: number | null): USBDeviceFilter[] {
	if (vendorId && productId) {
		return [{ vendorId, productId }];
	}

	if (vendorId) {
		return [{ vendorId }];
	}

	return [{ vendorId: 0x0a5f }];
}

export function browserSupportsWebUsb(): boolean {
	return typeof navigator !== 'undefined' && 'usb' in navigator;
}

export async function requestPrinterConnection(vendorId?: number | null, productId?: number | null): Promise<PrinterConnection> {
	if (!browserSupportsWebUsb()) {
		throw new Error('WebUSB is not available in this browser.');
	}

	const device = await navigator.usb.requestDevice({ filters: getZebraLikeFilter(vendorId, productId) });
	if (!device.opened) {
		await device.open();
	}

	if (!device.configuration) {
		await device.selectConfiguration(1);
	}

	const configuration = device.configuration;
	if (!configuration) {
		throw new Error('The selected USB device does not expose a configuration.');
	}

	const selectedInterface = configuration.interfaces.find((item) =>
		item.alternates.some((alternate) => alternate.endpoints.some((endpoint) => endpoint.direction === 'out'))
	);

	if (!selectedInterface) {
		throw new Error('No bulk OUT endpoint was found on the selected USB device.');
	}

	await device.claimInterface(selectedInterface.interfaceNumber);

	const endpointNumber =
		selectedInterface.alternates
			.flatMap((alternate) => alternate.endpoints)
			.find((endpoint) => endpoint.direction === 'out')?.endpointNumber ?? 0;

	return {
		device,
		interfaceNumber: selectedInterface.interfaceNumber,
		endpointNumber
	};
}

export async function sendRawPayload(connection: PrinterConnection, content: string): Promise<void> {
	const encoder = new TextEncoder();
	const result = await connection.device.transferOut(connection.endpointNumber, encoder.encode(content));
	if (result.status !== 'ok') {
		throw new Error(`USB transfer failed with status '${result.status}'.`);
	}
}

export async function closePrinterConnection(connection: PrinterConnection | null): Promise<void> {
	if (!connection) {
		return;
	}

	try {
		await connection.device.releaseInterface(connection.interfaceNumber);
	} catch {
		// Ignore release errors during cleanup when the device was already detached.
	}

	if (connection.device.opened) {
		await connection.device.close();
	}
}
