import DeviceDto from "./DeviceDto";
import NetworkDto from "./NetworkDto";

export default interface NetworkDeviceDto {
    NetworkDeviceID: number;
    NetworkID: number;
    DeviceID: number;
    NetworkDto: NetworkDto | undefined | null;
    DeviceDto: DeviceDto | undefined | null
}