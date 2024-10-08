import Box from '@mui/material/Box';
import Tabs from '@mui/material/Tabs';
import Tab from '@mui/material/Tab';
import Typography from '@mui/material/Typography';
import NavigationButton from "../NavigationButton/NavigationButton";
import {
    MemoryRouter,
    Route,
    Routes,
    Link,
    matchPath,
    useLocation,
} from 'react-router-dom';
import { StaticRouter } from 'react-router-dom/server';
import SsidChartRoundedIcon from '@mui/icons-material/SsidChartRounded';
import PlayCircleFilledWhiteRoundedIcon from '@mui/icons-material/PlayCircleFilledWhiteRounded';
import DisplaySettingsRoundedIcon from '@mui/icons-material/DisplaySettingsRounded';
import HomeRoundedIcon from '@mui/icons-material/HomeRounded';
import ManageAccountsIcon from '@mui/icons-material/ManageAccounts';
import { useEffect, useState } from "react";
import { route } from '../../Redux/route/routeConstants';
import { useAppDispatch, useAppSelector } from '../..';
import { routeActions } from '../../Redux/route/routeActions';
import checkJwtExists from '../../common/JWT/checkJwtExists';
import { useDecodedToken } from '../App/App';

const routes = route;

const useRouteMatch = (patterns: readonly string[]) => {
    const { pathname } = useLocation();

    for (let pattern in patterns) {
        const possibleMatch = matchPath(pattern, pathname);
        if (possibleMatch !== null) {
        return possibleMatch;
        }
    }

    return null;
}

const Router = (props: { children?: React.ReactNode }) => {
    const { children } = props;
    if (typeof window === 'undefined') {
        return <StaticRouter location={routes.home}>{children}</StaticRouter>;
    }

    return (
        <MemoryRouter initialEntries={[routes.home]} initialIndex={0}>
            {children}
        </MemoryRouter>
    );
}

const CurrentRoute = () => {
    const location = useLocation();
  
    return (
      <Typography variant="body2" sx={{ pb: 2 }} color="text.secondary">
      </Typography>
    );
  }

interface CustomTabsProps {
    isAdmin: boolean;
}

const CustomTabs = (props: CustomTabsProps) => {
    const routeMatch = useRouteMatch([routes.home, routes.launchSimulation, routes.monitoring, routes.states]);

    const textColor = process.env.REACT_APP_NAVIGATION_TAB_COLOR === undefined
        ? '#53B9EA'
        : `#${process.env.REACT_APP_NAVIGATION_TAB_COLOR}`;

    const [tabState, setTabState] = useState(routes.home);

    const handleTabs = (value: string) => {
        setTabState(value);
    }

    const dispatch = useAppDispatch();
    let route = useAppSelector(state => state.route);

    const changeRoute = (selectedRoute: string) => {
        dispatch({ 
            type: routeActions.CHANGE,
            payload: { 
                newRoute: selectedRoute
            } 
        });
    }

    let location = useLocation();

    useEffect(() => {
        changeRoute(location.pathname);
        setTabState(location.pathname);
    }, [location.pathname])

    const tabs = [<Tab 
        icon={<HomeRoundedIcon color="secondary"/>}
        iconPosition="start"
        value={routes.home}
        sx={{alignSelf:'start'}}
        label={
            <NavigationButton color={textColor} label="Домашняя страница"/>
        }
        to={routes.home}
        component={Link}
    />,
    <Tab 
        icon={<SsidChartRoundedIcon color="secondary" />}
        iconPosition="start"
        sx={{alignSelf:'start'}}
        value={routes.monitoring}
        label={
            <NavigationButton color={textColor} label="Мониторинг"/>
        } 
        to={routes.monitoring}
        component={Link} 
        />,
    <Tab 
        icon={<DisplaySettingsRoundedIcon color="secondary"/>}
        iconPosition="start"
        sx={{alignSelf:'start'}}
        value={routes.states}
        label={
            <NavigationButton color={textColor} label="Состояния"/>
        }
        to={routes.states}
        component={Link} 
        />,
    ];

    if (props.isAdmin) {
        tabs.push(
            <Tab 
                icon={<DisplaySettingsRoundedIcon color="secondary"/>}
                iconPosition="start"
                sx={{alignSelf:'start'}}
                value={routes.administration}
                label={
                    <NavigationButton color={textColor} label="Администрирование"/>
                }
                to={routes.administration}
                component={Link} 
                />)
    }
  
    return (
        <Tabs 
            orientation="vertical" 
            textColor="secondary"
            onChange={(event, value) => { handleTabs(value) }}
            value={tabState}
            >
            {tabs}
        </Tabs>
    );
}

const Navigation = () => {
    const token = useDecodedToken();

    const routesJsx = [
        <Route path="*" element={<CurrentRoute />} />,
        <Route path={routes.home} element={<CurrentRoute />} />,
        <Route path={routes.monitoring} element={<CurrentRoute />} />,
        <Route path={routes.states} element={<CurrentRoute />} />,
    ];

    if (token?.IsAdmin) {
        routesJsx.push(
            <Route path={routes.administration} element={<CurrentRoute />} />
        );
    }

    return (
        <Router>
            <Routes>
                {routesJsx}
            </Routes>
            <Box>
                <CustomTabs isAdmin={token?.IsAdmin ?? false}/>
            </Box>
        </Router>
    )
}

export default Navigation;