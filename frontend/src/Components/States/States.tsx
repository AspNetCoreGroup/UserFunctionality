import { Autocomplete, InputLabel, makeStyles, Paper, TextField } from "@mui/material";
import NetworkDto from "../../Models/NetworkDto";
import { useState } from "react";
import { defaultSx } from "../PlotArea/PlotArea";

const States = () => {
    const backendServerUri = `${process.env.REACT_APP_BACKEND_SERVER_URI}`;
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

    const updateNetworks = () => {
        fetch(`${backendServerUri}/backend/networks`,{
            method: 'GET',
            cache: 'no-cache',
            headers: {
                "Content-Type": "application/json",
            }
        })
        .then(response => response.json())
        .then((json) => {
            setNetworks(json as NetworkDto[]);
        })
        .catch(() => console.log('Error fetching'));
    };

    return (
        <Autocomplete
            disablePortal
            className="networks"
            id="networks"
            options={networks.map(x => x.NetworkTitle)}
            onMouseMove={updateNetworks}
            style={{color: defaultSx.color}}
            renderInput={(params) => <TextField {...params} 
                                label="Сеть" 
                                InputProps={{ ...params.InputProps, 
                                        style: {color: defaultSx.color},
                                    }
                                }
                                variant="outlined"
                                InputLabelProps={{...params.InputLabelProps, style: {color: defaultSx.color}}}
                        />}
            sx={{...defaultSx, "&.MuiAutocomplete-root": {
        "& fieldset": {
          borderColor: defaultSx.color,
        },
      }}}
        onSelect={() => {/*тут отображение девайсов*/}}
        />
    )
};

export default States;