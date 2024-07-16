import {  useAppSelector } from "../..";
import { LoginFormState } from "../../Redux/loginForm/loginFormReducer";
import Login from "../Login/Login";
import Registration from "../Registration/Registration";

const Authentificate = () => {
    let loginFormState: LoginFormState = useAppSelector(state => state.loginForm);
    
    if (loginFormState.isLoginForm) {
        return (
            <>
            <Login />
            </>
        )
    } else {
        return (
            <>
            <Registration initIsOpen={true}/>
            </>
        )
    }
    
}

export default Authentificate;