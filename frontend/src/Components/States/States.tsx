import { Autocomplete, InputLabel, makeStyles, Paper, TextField } from "@mui/material";
import NetworkDto from "../../Models/NetworkDto";
import { useEffect, useState } from "react";
import { defaultSx } from "../PlotArea/PlotArea";
import updateNetworks from "../../common/updateNetworks";
import "./States.css";
import NetworkDeviceDto from "../../Models/NetworkDeviceDto";
import updateDevices from "../../common/updateDevices";
import CustomTable from "../DevicesTable/DevicesTable";
import DevicesTable from "../DevicesTable/DevicesTable";
import DeviceDto from "../../Models/DeviceDto";

const States = () => {
    const [networks, setNetworks] = useState<NetworkDto[]>([
        {
            NetworkID: 1,
            NetworkTitle: "Network 1"
        },
        {
            NetworkID: 2,
            NetworkTitle: "Network 2"
        },
        {
            NetworkID: 3,
            NetworkTitle: "Network 3"
        },
    ]);

    const [network, setNetwork] = useState<NetworkDto>(networks[0]);

    const [devices, setDevices] = useState<DeviceDto[]>([]);

    useEffect(() => {
        const interval = setInterval(() => {
            updateNetworks<NetworkDto[]>(setNetworks);
            updateDevices<DeviceDto[]>(setDevices, network.NetworkID);
        }, 5000);

        return () => {
            clearInterval(interval);
        }
    }, []);

    useEffect(() => {
        updateNetworks<NetworkDto[]>(setNetworks);
        updateDevices<DeviceDto[]>(setDevices, network.NetworkID);
    }, [network])
    
    return (
        <div className="state__wrapper">
            <Autocomplete
                disablePortal
                onChange={(e, v, r, d) => {
                    setNetwork(networks.find(n => n.NetworkTitle === v) ?? networks[0]);
                    updateDevices<DeviceDto[]>(setDevices, network.NetworkID);
                }}
                
                className="networks"
                id="networks"
                options={networks.map(x => x.NetworkTitle)}
                style={{color: defaultSx.color, borderColor: "#53B9EA", }}
                renderInput={(params) => <TextField {...params} 
                                    label="Сеть" 
                                    InputProps={{ ...params.InputProps, 
                                            style: {color: defaultSx.color},
                                        }
                                    }
                                    variant="outlined"
                                    InputLabelProps={{...params.InputLabelProps, style: {color: defaultSx.color}}}
                            />}
                sx={{
                    ...defaultSx,
                    "&.MuiAutocomplete-root": {
                        "&:hover fieldset": {
                            borderColor: defaultSx.color,
                        },
                        "& fieldset": {
                            borderColor: defaultSx.color,
                        },
                        "& button": {
                            color: defaultSx.color,
                        }
                    }
                }}
            />
            <DevicesTable columns={['ID', 'Наименование', 'Серийный номер']} rows={devices} />
        </div>
    )
};

export default States;