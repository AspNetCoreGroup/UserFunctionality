import { Button, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle, TextField } from "@mui/material"
import { useState } from 'react'
import HowToRegIcon from '@mui/icons-material/HowToReg';

type RegistrationProps = {
    initIsOpen: boolean;
}

const Registration = (props: RegistrationProps) => {
    const [isOpen, setIsOpen] = useState(props.initIsOpen);

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
        const telegramm = formJson.telegramm
        console.log(email, password, telegramm);
        handleClose();
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