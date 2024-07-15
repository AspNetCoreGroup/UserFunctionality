import ReactDOM from 'react-dom/client';
import App from './Components/App/App';
import plotTypeReducer from './Redux/plotType/plotTypeReducer';
import { Provider, useDispatch, useSelector } from 'react-redux';
import { configureStore } from "@reduxjs/toolkit"
import routeReducer from './Redux/route/routeReducer';
import loginUserReducer from './Redux/loginUser/loginUserReducer';
import { AuthProvider } from 'react-oidc-context';


export const storage = configureStore(
  {
    reducer: {
      plotType: plotTypeReducer,
      route: routeReducer,
      login: loginUserReducer
    }
  }
);
const state = storage.getState();
export const useAppDispatch = useDispatch.withTypes<typeof storage.dispatch>();
export const useAppSelector = useSelector.withTypes<typeof state>();

const root = ReactDOM.createRoot(
  document.getElementById('root') as HTMLElement
);
root.render(
  <Provider store={storage}>
      <AuthProvider >
        <App isAuthorized={!!document.cookie.split(';').find(c => c.includes('token'))} email={state.login}/>
      </AuthProvider>
  </Provider>
);

