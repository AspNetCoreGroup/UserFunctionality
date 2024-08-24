import { Button, Checkbox, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle, FormControlLabel, TextField } from "@mui/material"
import { useState } from 'react'
import HowToRegIcon from '@mui/icons-material/HowToReg';
import Registration from "../Registration/Registration";
import LoginModel from "../../Models/LoginModel";
import { loginUserActions } from "../../Redux/loginUser/loginUserActions";
import { useAppDispatch } from "../..";
import { loginFormActions } from "../../Redux/loginForm/loginFormActions";

const Login = () => {
    const [isOpen, setIsOpen] = useState(false);

    const handleClickOpen = (): void => {
        setIsOpen(true);
    }

    const handleClose = (): void => {
        setIsOpen(false);
    };

    const dispatch = useAppDispatch();
    const login = (username: string) => {
        dispatch({ 
            type: loginUserActions.LOGIN,
            payload: { 
                username: username
            } 
        });
    }

    const handleSubmit = (event: React.FormEvent<HTMLFormElement>): void => {
        event.preventDefault();
        const formData = new FormData(event.currentTarget);
        const formJson = Object.fromEntries((formData as any).entries());
        const body: LoginModel = Object.create(formJson);
        
        fetch(`/api/users/Login`,{
            method: 'PATCH',
            cache: 'no-cache',
            mode: 'no-cors',
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(formJson),
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

    const handleRegisterForm = () => {
        setIsOpen(false);
        dispatch({
            type: loginFormActions.SET_REGISTRATION
        });
    }

    return (
        <>
        <Button variant="outlined" onClick={handleClickOpen} startIcon={<HowToRegIcon />}>
            Авторизация
        </Button>
        <Dialog
            open={isOpen}
            onClose={handleClose}
            PaperProps={{
                component: 'form',
                onSubmit: handleSubmit,
                }}
        >
            <DialogTitle>Авторизоваться</DialogTitle>
            <DialogContent>
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
            <FormControlLabel 
                control={<Checkbox />}
                label="Запомнить меня"
                id="rememberMe"
                name="rememberMe"
            />
            </DialogContent>

            <DialogActions>
                <Button variant="outlined" onClick={handleClose}>Отмена</Button>
                <Button variant="outlined" type="submit">Войти</Button>
                <Button variant="outlined" onClick={handleRegisterForm}>Зарегистрироваться</Button>
            </DialogActions>
        </Dialog>
        </>
    )
}

export default Login;
