import { Autocomplete, FormControl, InputLabel, MenuItem, Select, TextField } from "@mui/material";
import "./PlotArea.css";
import { useAppDispatch, useAppSelector } from "../..";
import { plotTypeActions } from "../../Redux/plotType/plotTypeActions";
import { plotTypes } from "../../Redux/plotType/plotTypeConstants";
import CustomPlot from "../CustomPlot/CustomPlot";
import { PlotTypeState } from "../../Redux/plotType/plotTypeReducer";
import { useEffect, useState } from "react";
import NetworkDto from "../../Models/NetworkDto";
import updateNetworks from "../../common/updateNetworks";
import { DemoContainer } from '@mui/x-date-pickers/internals/demo';
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { DateTimePicker } from '@mui/x-date-pickers/DateTimePicker';
import { renderTimeViewClock } from '@mui/x-date-pickers/timeViewRenderers';
import { dateFromActions } from "../../Redux/dateFrom/dateFromActions";
import { dateToActions } from "../../Redux/dateTo/dateToActions";
import { networkActions } from "../../Redux/network/networkActions";
import { DateFromState } from "../../Redux/dateFrom/dateFromReducer";
import { DateToState } from "../../Redux/dateTo/dateToReducer";
import { NetworkState } from "../../Redux/network/networkReducer";
import dayjs from "dayjs";
import { useDecodedToken } from "../App/App";


const textColor = process.env.REACT_APP_SELECT_PLOT_SWITCH_COLOR === undefined
        ? '#53B9EA'
        : `#${process.env.REACT_APP_SELECT_PLOT_SWITCH_COLOR}`;

export const defaultSx = {
    color: textColor,
    "&.MuiOutlinedInput-root": {
        "& fieldset": {
          borderColor: textColor,
        },
        "&:hover fieldset": {
          borderColor: "#53B9EA"
        },
        "& svg": {
          color: "#53B9EA"
        }
      },
      
};

const PlotArea = () => {
    const dispatch = useAppDispatch();
    const principal = useDecodedToken() ?? {Email: "test@test.test", IsAdmin: true, TG: "test", UserID: '12'};
    let plotTypeState: PlotTypeState = useAppSelector(state => state.plotType);
    let dateFromState: DateFromState = useAppSelector(state => state.dateFrom);
    let dateToState: DateToState = useAppSelector(state => state.dateTo);
    let networkState: NetworkState = useAppSelector(state => state.network);

    const [networks, setNetworks] = useState<NetworkDto[]>(
        [
            {
                networkID: 1,
                networkTitle: "network"
            },
            {
                networkID: 2,
                networkTitle: "net"
            },
            {
                networkID: 3,
                networkTitle: "Specific"
            },
        ]
    );

    const changePlotType = (type: string) => {
        dispatch({ 
            type: plotTypeActions.CHANGE,
            payload: { 
                newPlotType: type
            } 
        });
    }
    const changeDateFrom = (dateFrom: Date) => {
        dispatch({ 
            type: dateFromActions.SET,
            payload: dateFrom
        });
    }
    const changeDateTo = (dateTo: Date) => {
        dispatch({ 
            type: dateToActions.SET,
            payload: dateTo
        });
    }
    const changeNetwork = (network: string) => {
        dispatch({ 
            type: networkActions.SET,
            payload: network
        });
    }
    useEffect(() => {
        const interval = setInterval(() => {
            updateNetworks(setNetworks, principal.UserID);
        }, 5000);

        return () => {
            clearInterval(interval);
        }
    }, [principal.UserID]);

    useEffect(() => {
        updateNetworks<NetworkDto[]>(setNetworks, principal.UserID);
    }, [networks, principal.UserID])

    return (
        <div className="wrapper">
            <div className="select__holder">
                <FormControl fullWidth>
                    <InputLabel 
                        id="demo-simple-select-label" 
                        sx={defaultSx}
                    >
                        Тип зависимости
                    </InputLabel>
                    <Select
                        value={plotTypeState.plotType}
                        label="Тип зависимости"
                        sx={defaultSx}
                        onChange={(event) => { changePlotType(event.target.value) }}
                    >
                        <MenuItem value={plotTypes.choose}>Выберете тип зависимости</MenuItem>
                        <MenuItem value={plotTypes.f}>f(x)</MenuItem>
                        <MenuItem value={plotTypes.g}>g(x)</MenuItem>
                        <MenuItem value={plotTypes.h}>h(x)</MenuItem>
                    </Select>
                </FormControl>
                <Autocomplete
                        disablePortal
                        value={networkState.network}
                        className="network"
                        id="network"
                        options={networks.map(x => x.networkTitle)}
                        style={{color: defaultSx.color}}
                        onChange={(event, value) => { changeNetwork(value ?? "None") }}
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
                    <LocalizationProvider dateAdapter={AdapterDayjs}>
                        <DateTimePicker
                                value={dayjs(dateFromState.dateFrom)}
                                label="Дата с"
                                onChange={(date) => { changeDateFrom(date?.toDate() ?? new Date())}}
                                viewRenderers={{
                                    hours: renderTimeViewClock,
                                    minutes: renderTimeViewClock,
                                    seconds: renderTimeViewClock,
                                }}
                                sx={{
                                    "& .MuiInputBase-root": {
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
                                        }
                                    },
                                    "& .MuiFormLabel-root": {
                                            color: defaultSx.color,
                                    }
                                }}
                            />
                    </LocalizationProvider>
                    <LocalizationProvider dateAdapter={AdapterDayjs}>
                        <DateTimePicker
                                label="Дата по"
                                value={dayjs(dateToState.dateTo)}
                                onChange={(date) => { changeDateTo(date?.toDate() ?? new Date())}}
                                viewRenderers={{
                                    hours: renderTimeViewClock,
                                    minutes: renderTimeViewClock,
                                    seconds: renderTimeViewClock,
                                }}
                                sx={{
                                    "& .MuiInputBase-root": {
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
                                        }
                                    },
                                    "& .MuiFormLabel-root": {
                                            color: defaultSx.color,
                                    }
                                }}
                            />
                    </LocalizationProvider>
            </div>

            <div className="plot__holder">
                <CustomPlot />
            </div>
        </div>
    );
}

export default PlotArea;