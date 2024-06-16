import './App.css';
import Navigation from "../Navigation/Navigation";
import { useAppSelector } from '../..';
import Content from '../Content/Content';
import Registration from '../Registration/Registration';
import Authentificate from '../Authentificate/Authentificate';

const logo = require('../../Images/logo.png')

const App = () => {
    let selectedRoute = useAppSelector(state => state.route);

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
                    <Authentificate />
                </div>
                
                <Content currRoute={selectedRoute} />
                
            </div>

        </div>
    );
}

export default App;
