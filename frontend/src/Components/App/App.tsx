import './App.css';
import Navigation from "../Navigation/Navigation";
import { useAppSelector } from '../..';
import Content from '../Content/Content';
import Authentificate from '../Authentificate/Authentificate';
import { LoginState } from '../../Redux/loginUser/loginUserReducer';
import { State } from '../../Redux/route/routeReducer';
import Welcome from '../Welcome/Welcome';
import { useJwt } from 'react-jwt'
import Logout from '../Logout/Logout';
import Principal from '../../common/JWT/principal';
import UserProfile from '../UserProfile/UserProfile';

const logo = require('../../Images/logo.png')

interface AppProps {
    isAuthorized: boolean;
    email: string
}

export const useDecodedToken = (): Principal | null => {
    const token = document.cookie.split(';').find(c => c.includes('token'))?.split('=')[1] ?? "";
    const tokenDecoded = useJwt<Principal>(token).decodedToken;

    return tokenDecoded;
}


const App = (props: AppProps) => {
    let selectedRoute: State = useAppSelector(state => state.route);
    let loginState: LoginState = useAppSelector(state => state.login);

    let email: String | undefined = '';
    const claims: Principal | null = useDecodedToken();
    
    if (props.isAuthorized) {
        email = claims?.Email;
    }

    return (
        <div className="app">
            <div className="left__column">
                <div className="credentials">
                    <a href="https://otus.ru/lessons/asp-net/">
                        <img src={logo} className='logo' />
                    </a>
                </div>

                <div className="navigation">
                    <Navigation />
                </div>
            </div>

            <div className="right__column">
            <div className="additional__info">
                {props.isAuthorized
                    ? 
                    <div className="authorized">
                        <UserProfile />
                        <Logout Email={email}/>
                    </div>
                    :
                    <div className="login">
                        <Authentificate />
                    </div>
                }
            </div>
                
                <Content currRoute={selectedRoute} />
            </div>

        </div>
    );
}

export default App;
