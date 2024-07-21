import { Button, createTheme, Dialog, DialogActions, DialogContent, DialogTitle, Divider, List, ListItem, ListItemText, styled, TextField } from "@mui/material";
import './UserProfile.css'
import { useDecodedToken } from "../App/App";
import { useState } from "react";
import ManageAccountsIcon from '@mui/icons-material/ManageAccounts';
import UpdateClaimsModel from "../../Models/UpdateClaimsModel";


const UserProfile = () => {
    const principal = useDecodedToken();

    const identityServerUri = `${process.env.REACT_APP_IDENTITY_SERVER_URI}`;
    
    const [isOpen, setIsOpen] = useState(false);

    const handleClickOpen = (): void => {
        setIsOpen(true);
    }

    const handleClose = (): void => {
        setIsOpen(false);
    };

    const handleUpdateClaims = (event: React.FormEvent<HTMLFormElement>): void => {
        event.preventDefault();
        const formData = new FormData(event.currentTarget);
        const formJson = Object.fromEntries((formData as any).entries());

        const body: UpdateClaimsModel = Object.create(formJson);
        body.Email = principal?.Email;
        body.Telegramm = body.Telegramm === '' ? principal?.TG : body.Telegramm; 
        
        fetch(`${identityServerUri}/api/users/Claims/Update`,{
            method: 'PATCH',
            cache: 'no-cache',
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(body),
            credentials: 'include'
        })
        .then(response => response.json())
        .then((json) => {
            handleClose()
        })
        .catch((e) => console.log())
        .finally(() => window.location.reload());
    }

    return (
        <div className="user-profile__button">
            <Button variant="contained" onClick={handleClickOpen} startIcon={<ManageAccountsIcon />}>
                Профиль
            </Button>
            
            <Dialog
                open={isOpen}
                PaperProps={{
                    component: 'form',
                    onSubmit: handleUpdateClaims,
                    }}
            >
                <DialogTitle>Параметры профиля</DialogTitle>
                <DialogContent>
                    <ListItem>
                        <ListItemText primary={principal?.IsAdmin ? "Администратор системы" : "Пользователь"}  className="is-admin"/>
                    </ListItem>

                    <ListItem>
                        <ListItemText primary={principal?.Email}  className="email"/>
                    </ListItem>

                    <ListItem>
                        <ListItemText primary="Telegramm" className="text-label"/>
                        <TextField
                            margin="dense"
                            id="Telegramm"
                            name="Telegramm"
                            label={'@' + principal?.TG}
                            type="text"
                            fullWidth
                            variant="outlined"
                        />
                        
                    </ListItem>

                </DialogContent>

                <DialogActions>
                    <Button variant="outlined" onClick={handleClose}>ОК</Button>
                    <Button variant="outlined" type="submit">Изменить параметры</Button>
                </DialogActions>
            </Dialog>
        </div>
        
    );
}

export default UserProfile;