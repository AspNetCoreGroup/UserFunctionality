import { Autocomplete, Button, Link, TextField } from "@mui/material";
import { useState } from "react";
import NetworkDto from "../../Models/NetworkDto";
import { defaultSx } from "../PlotArea/PlotArea";
import React from "react";
import DeviceDto from "../../Models/DeviceDto";
import NetworkDeviceDto from "../../Models/NetworkDeviceDto";
import './Administration.css';

const Administration = () => {
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

    const [id, setId] = useState<number>(0);
    const [code, setCode] = useState<number>(0);
    const [title, setTitle] = useState<string>("");

    const handleSubmit = (e: React.ChangeEvent<HTMLFormElement>) => {
        e.preventDefault();
        const formData = new FormData(e.currentTarget);
        const formJson = Object.fromEntries((formData as any).entries());
        const device: DeviceDto = Object.create(formJson);
        const networkDevice: NetworkDeviceDto = {
            DeviceDto: device,
            NetworkDto: network,
            DeviceID: device.DeviceID,
            NetworkDeviceID: Math.floor(Math.random() * 1e16),
            NetworkID: network.NetworkID
        };

        const backendServerUri = `${process.env.REACT_APP_BACKEND_SERVER_URI}`;

        fetch(`${backendServerUri}/backend/networks/${network.NetworkID}/devices`,{
            method: 'POST',
            cache: 'no-cache',
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(networkDevice),
            credentials: 'include'
        })
        .then(response => response.json())
        .catch(() => console.log('Error fetching'));
    }

    const fieldSx = {mb: 3, maxWidth: 950,
        "&.MuiFormControl-root": {
            bolderColor: defaultSx.color,
                "&:hover fieldset": {
                    borderColor: defaultSx.color,
                },
                "& fieldset": {
                    borderColor: defaultSx.color,
                },
                "& button": {
                    color: defaultSx.color,
                },
                "& input": {
                    color: defaultSx.color,
                },
                "& label": {
                    color: defaultSx.color,
                }
            }
    };

    return (
        <div className="administration__wrapper">
            <Autocomplete
                disablePortal
                onChange={(e, v, r, d) => {
                    setNetwork(networks.find(n => n.NetworkTitle === v) ?? networks[0]);
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
            <React.Fragment>
                <form autoComplete="off" onSubmit={handleSubmit} className="form">
                        <TextField 
                            label="ID"
                            onChange={e => setId(Number.parseInt(e.target.value))}
                            required
                            variant="outlined"
                            type="input"
                            sx={fieldSx}
                            fullWidth
                            value={id}
                        />
                        <TextField 
                            label="Серийный номер"
                            onChange={e => setCode(Number.parseInt(e.target.value))}
                            required
                            variant="outlined"
                            type="input"
                            value={code}
                            fullWidth
                            sx={fieldSx}
                        />
                        <TextField 
                            label="Наименование"
                            onChange={e => setTitle(e.target.value)}
                            required
                            variant="outlined"
                            type="input"
                            value={title}
                            fullWidth
                            sx={fieldSx}
                        />
                        <Button variant="outlined" color="secondary" type="submit" sx={fieldSx}>Добавить устройство</Button>
                </form>
            </React.Fragment>
        </div>
    )
};

export default Administration;