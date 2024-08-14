import { Autocomplete, InputLabel, makeStyles, Paper, TextField } from "@mui/material";
import NetworkDto from "../../Models/NetworkDto";
import { useState } from "react";
import { defaultSx } from "../PlotArea/PlotArea";
import updateNetworks from "../../common/updateNetworks";
import "./States.css";

const States = () => {
    const [networks, setNetworks] = useState<NetworkDto[]>([
        {
            NetworkID: 1,
            NetworkTitle: "Network"
        },
        {
            NetworkID: 2,
            NetworkTitle: "Net"
        },
        {
            NetworkID: 3,
            NetworkTitle: "Specific"
        },
    ]);

    updateNetworks<NetworkDto[]>(setNetworks);

    return (
        <Autocomplete
            disablePortal
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
        onSelect={() => {/*тут отображение девайсов*/}}
        />
    )
};

export default States;