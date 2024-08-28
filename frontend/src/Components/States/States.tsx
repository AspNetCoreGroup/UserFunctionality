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
import { useDecodedToken } from "../App/App";

const States = () => {
    const principal = useDecodedToken() ?? {Email: "test@test.test", IsAdmin: true, TG: "test", UserID: '12'};
    const [networks, setNetworks] = useState<NetworkDto[]>([
        {
            networkID: 1,
            networkTitle: "network 1"
        },
        {
            networkID: 2,
            networkTitle: "network 2"
        },
        {
            networkID: 3,
            networkTitle: "network 3"
        },
    ]);

    const [network, setNetwork] = useState<NetworkDto>(networks[0]);

    const [devices, setDevices] = useState<DeviceDto[]>([]);

    useEffect(() => {
        const interval = setInterval(() => {
            updateNetworks<NetworkDto[]>(setNetworks, principal.UserID);
        }, 5000);

        return () => {
            clearInterval(interval);
        }
    }, [principal.UserID]);

    useEffect(() => {
        updateNetworks<NetworkDto[]>(setNetworks, principal.UserID);
    }, [network, principal.UserID])
    console.log(networks);
    return (
        <div className="state__wrapper">
            <Autocomplete
                disablePortal
                onChange={(e, v, r, d) => {
                    setNetwork(networks.find(n => n.networkTitle === v) ?? networks[0]);
                    updateDevices<DeviceDto[]>(setDevices, network.networkID, principal?.UserID);
                }}
                
                className="networks"
                id="networks"
                options={networks.map(x => x.networkTitle)}
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