import { Button, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle, TextField } from "@mui/material"
import { useState } from 'react'
import HowToRegIcon from '@mui/icons-material/HowToReg';
import Registration from "../Registration/Registration";
import { User, UserManager } from "oidc-client-ts";

interface LoginProps {
    userManager: UserManager
}

const Login = (props: LoginProps) => {
    const [isOpen, setIsOpen] = useState(false);
    const [isLogin, setIsLogin] = useState(true);

    const handleClickOpen = (): void => {
        setIsOpen(true);
    }

    const handleClose = (): void => {
        setIsOpen(false);
    };

    const handleSubmit = (event: React.FormEvent<HTMLFormElement>): void => {
        event.preventDefault();
        const formData = new FormData(event.currentTarget);
        const formJson = Object.fromEntries((formData as any).entries());
        const email = formJson.email;
        const password = formJson.password;
        console.log(email, password);
        handleClose();
    }

    const handleRegisterForm = () => {
        setIsOpen(false);
        setIsLogin(false);
    }

    if (isLogin) {
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
                </DialogContent>
    
                <DialogActions>
                    <Button variant="outlined" onClick={handleClose}>Отмена</Button>
                    <Button variant="outlined" type="submit">Войти</Button>
                    <Button variant="outlined" onClick={handleRegisterForm}>Зарегистрироваться</Button>
                </DialogActions>
            </Dialog>
            </>
    )
    } else {
        return (
            <Registration initIsOpen={true} userManager={props.userManager}/>
        )
    }
}

export default Login;