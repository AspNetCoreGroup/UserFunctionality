import { UserManager } from "oidc-client-ts";
import Login from "../Login/Login";

interface AuthentificateProps {
    userManager: UserManager
}

const Authentificate = (props: AuthentificateProps) => {

    return (
        <>
        <Login userManager={props.userManager}/>
        </>
    )
}

export default Authentificate;