import { loginFormActions } from "./loginFormActions"

export type LoginFormState = {
    isLoginForm: boolean
}

export type Action = {
    type: string,
}

export const initStateLoginForm: LoginFormState = {
    isLoginForm: true,
};

export default (state: LoginFormState = initStateLoginForm, action: Action): any => {
    switch (action.type){
        case loginFormActions.SET_LOGIN:
            return { isLoginForm: true };
        case (loginFormActions.SET_REGISTRATION):
            return { isLoginForm: false };
        default:
            return { ...state };
    }
}
