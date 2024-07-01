import { loginUserActions } from "./loginUserActions"

export type LoginState = {
    isLogedIn: boolean
    username: string
}

export type Action = {
    type: string,
    payload?: any
}

export const initStateUser: LoginState = {
    isLogedIn: false,
    username: ""
};

export default (state: LoginState = initStateUser, action: Action): any => {
    switch (action.type){
        case loginUserActions.LOGIN:
            if (!action.payload)
                return { ...state};
            else{
                if (action.payload?.username){
                    return { isLogedIn: true, username: action.payload.username};
                }
                else
                    return { isLogedIn: true};
            }
        case (loginUserActions.LOGOUT):
            return { isLogedIn: false, username: "" };
        default:
            return { ...state };
    }
}
