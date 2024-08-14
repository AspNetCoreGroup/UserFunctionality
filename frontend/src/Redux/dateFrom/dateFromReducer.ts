import { dateFromActions } from "./dateFromActions"

export type DateFromState = {
    dateFrom: Date
}

export type Action = {
    type: string,
    payload?: Date
}

export const initState: DateFromState = {
    dateFrom: new Date(new Date().setHours(new Date().getHours() - 1))
};

export default (state: DateFromState = initState, action: Action): any => {
    switch (action.type){
        case dateFromActions.SET:
            if (!action.payload)
                return { ...state};
            else{
                if (action.payload){
                    return { ...state, dateFrom: action.payload };
                }
                else
                    return { ...state};
            }
        default:
            return { ...state };
    }
}
