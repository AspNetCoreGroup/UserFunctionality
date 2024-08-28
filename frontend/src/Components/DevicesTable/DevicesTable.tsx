import { Table, TableBody, TableCell, TableContainer, TableHead, TableRow } from "@mui/material";
import Paper from '@mui/material/Paper';
import DeviceDto from "../../Models/DeviceDto";
import './DevicesTable.css';
import { defaultSx } from "../PlotArea/PlotArea";

export interface TableProps {
    columns: string[];
    rows: DeviceDto[];
}


const DevicesTable = (props: TableProps) => {
    return (
        <div className="table">
            <TableContainer component={Paper} sx={{maxWidth: 850}}>
                <Table sx={{ minWidth: 850, maxWidth: 850, backgroundColor: "#cdecfa" }} aria-label="simple table">
                    <TableHead>
                    <TableRow>
                        {props.columns.map(col => <TableCell align="center" sx={{fontWeight: "bold"}}>{col}</TableCell>)}
                    </TableRow>
                    </TableHead>
                    <TableBody>
                    {props.rows.map((row) => (
                        <TableRow
                        key={row.deviceID}
                        sx={{ '&:last-child td, &:last-child th': { border: 0 } }}
                        >
                        <TableCell component="th" scope="row" align="center" >
                            {row.deviceID}
                        </TableCell>
                        <TableCell align="center">{row.deviceCustomTitle}</TableCell>
                        <TableCell align="center">{row.deviceCode}</TableCell>
                        </TableRow>
                    ))}
                    </TableBody>
                </Table>
            </TableContainer>
        </div>
    )
};

export default DevicesTable;