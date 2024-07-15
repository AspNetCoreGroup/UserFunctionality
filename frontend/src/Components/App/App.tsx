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

const logo = require('../../Images/logo.png')

interface AppProps {
    isAuthorized: boolean;
    email: string
}

interface ClaimsType {
    TG: string;
    IsAdmin: boolean;
    Email: string;
}

const App = (props: AppProps) => {
    let selectedRoute: State = useAppSelector(state => state.route);
    let loginState: LoginState = useAppSelector(state => state.login);
    const token = document.cookie.split(';').find(c => c.includes('token'))?.split('=')[1] ?? "";

    let email: String | undefined = '';
    const claims: ClaimsType | null = useJwt<ClaimsType>(token).decodedToken;
    
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
                        <Welcome username={email} />
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
