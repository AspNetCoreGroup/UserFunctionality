import { networkActions } from "./networkActions"

export type NetworkState= {
    network: string
}

export type Action = {
    type: string,
    payload?: Date
}

export const initState: NetworkState= {
    network: ""
};

export default (state: NetworkState= initState, action: Action): any => {
    switch (action.type){
        case networkActions.SET:
            if (!action.payload)
                return { ...state};
            else{
                if (action.payload){
                    return { ...state, network: action.payload };
                }
                else
                    return { ...state};
            }
        default:
            return { ...state };
    }
}
