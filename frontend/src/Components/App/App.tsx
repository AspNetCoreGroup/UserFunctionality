import './App.css';
import Navigation from "../Navigation/Navigation";
import { useAppSelector } from '../..';
import Content from '../Content/Content';
import Authentificate from '../Authentificate/Authentificate';
import { LoginState } from '../../Redux/loginUser/loginUserReducer';
import { State } from '../../Redux/route/routeReducer';
import Cookies from 'js-cookie'
import { useAuth } from 'react-oidc-context';
import Welcome from '../Welcome/Welcome';

const logo = require('../../Images/logo.png')
const App = () => {
    let selectedRoute: State = useAppSelector(state => state.route);
    let logedInUsername: LoginState = useAppSelector(state => state.login);

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
                {false
                    ? 
                    <Welcome username={"aa"} />
                    :
                    <div className="additional__info">
                        <Authentificate />
                    </div>
                }
                
                
                <Content currRoute={selectedRoute} />
                
            </div>

        </div>
    );
}

export default App;
