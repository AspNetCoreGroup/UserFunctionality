import { dateToActions } from "./dateToActions"

export type DateToState= {
    dateTo: Date
}

export type Action = {
    type: string,
    payload?: Date
}

export const initState: DateToState= {
    dateTo: new Date()
};

export default (state: DateToState= initState, action: Action): any => {
    switch (action.type){
        case dateToActions.SET:
            if (!action.payload)
                return { ...state};
            else{
                if (action.payload){
                    return { ...state, dateTo: action.payload };
                }
                else
                    return { ...state};
            }
        default:
            return { ...state };
    }
}
