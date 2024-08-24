import { Button } from '@mui/material';
import './Logout.css'
import { useAppDispatch } from '../..';

interface LogoutType {
    Email: String | undefined
}

const Logout = (props: LogoutType) => {
    const deleteAllCookies = () => {
        const cookies = document.cookie.split(";");
        for (let i = 0; i < cookies.length; i++) {
            const cookie = cookies[i];
            const eqPos = cookie.indexOf("=");
            const name = eqPos > -1 ? cookie.substr(0, eqPos) : cookie;
            document.cookie = name + "=;expires=Thu, 01 Jan 1970 00:00:00 GMT";
        }
    }

    const dispatch = useAppDispatch();

    const handleLogout = (email: String | undefined) => {
        fetch(`/api/users/Logout?email=${email}`,{
            method: 'PATCH',
            cache: 'no-cache',
            headers: {
                "Content-Type": "application/json",
            },
            credentials: 'include'
        })
        .then(_ => {
            deleteAllCookies();
            window.location.reload();
        })
        .catch((e) => console.log());
    }

    return (
        <div className="logout__button">
            <Button variant="outlined" onClick={e => {handleLogout(props.Email)}}>Выйти</Button>
        </div>
    )
}

export default Logout;