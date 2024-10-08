import { Button, Checkbox, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle, FormControlLabel, TextField } from "@mui/material"
import { useEffect, useState } from 'react'
import HowToRegIcon from '@mui/icons-material/HowToReg';
import { useAppDispatch, useAppSelector } from "../..";
import { loginUserActions } from "../../Redux/loginUser/loginUserActions";
import { loginFormActions } from "../../Redux/loginForm/loginFormActions";
import RegisterModel from "../../Models/RegisterModel";

type RegistrationProps = {
    initIsOpen: boolean;
}

const Registration = (props: RegistrationProps) => {
    const dispatch = useAppDispatch();
    const login = (username: string) => {
        dispatch({ 
            type: loginUserActions.LOGIN,
            payload: { 
                username: username
            } 
        });
    }

    const [isOpen, setIsOpen] = useState(props.initIsOpen);

    const handleClickOpen = (): void => {
        setIsOpen(true);
    }

    const handleClose = (): void => {
        setIsOpen(false);
        dispatch({
            type: loginFormActions.SET_LOGIN
        });
    };

    const handleSubmit = (event: React.FormEvent<HTMLFormElement>): void => {
        event.preventDefault();
        const formData = new FormData(event.currentTarget);
        const formJson = Object.fromEntries((formData as any).entries());

        const body: RegisterModel = {
            userName: formJson.username,
            email: formJson.email,
            password: formJson.password,
            telegramm: formJson.telegramm,
            isAdmin: formJson.isadmin == 'On'
        };

        fetch(`/api/users/Register`,{
            method: 'POST',
            cache: 'no-cache',
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(body),
            credentials: 'include'
        })
        .then(response => response.json())
        .then((json) => {
            login(json.userName);
            handleClose()
        })
        .catch((e) => console.log())
        //.finally(() => window.location.reload());
    }

    return (
        <>
        <Button variant="outlined" onClick={handleClickOpen} startIcon={<HowToRegIcon />}>
            Зарегистрироваться
        </Button>
        <Dialog
            open={isOpen}
            onClose={handleClose}
            PaperProps={{
                component: 'form',
                onSubmit: handleSubmit,
            }}
        >
            <DialogTitle>Регистрация</DialogTitle>
                <DialogContent>
                    <TextField
                        autoFocus
                        required
                        margin="dense"
                        id="username"
                        name="username"
                        label="Имя пользователя"
                        type="name"
                        fullWidth
                        variant="standard"
                    />
                    <TextField
                        autoFocus
                        required
                        margin="dense"
                        id="email"
                        name="email"
                        label="Email"
                        type="email"
                        fullWidth
                        variant="standard"
                    />
                    <TextField
                        required
                        margin="dense"
                        id="password"
                        name="password"
                        label="Пароль"
                        type="password"
                        fullWidth
                        variant="standard"
                    />
                    <TextField
                        margin="dense"
                        id="telegramm"
                        name="telegramm"
                        label="Telegramm"
                        type="text"
                        fullWidth
                        variant="standard"
                    />
                    <FormControlLabel 
                        control={<Checkbox />}
                        label="Регистрация как администратор"
                        id="isadmin"
                        name="isadmin"
                    />
                </DialogContent>

                <DialogActions>
                <Button onClick={handleClose}>Отмена</Button>
                <Button type="submit">Зарегистрироваться</Button>
            </DialogActions>
        </Dialog>
        </>
    )
}

export default Registration;