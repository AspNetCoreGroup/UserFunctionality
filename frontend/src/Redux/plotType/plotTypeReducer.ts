import { plotTypeActions } from "./plotTypeActions"
import { plotTypes } from "./plotTypeConstants"

export type PlotTypeState= {
    plotType: string
}

export type Action = {
    type: string,
    payload?: any
}

export const initState: PlotTypeState= {
    plotType: plotTypes.choose
};

export default (state: PlotTypeState= initState, action: Action): any => {
    switch (action.type){
        case plotTypeActions.CHANGE:
            if (!action.payload)
                return { ...state};
            else{
                if (action.payload?.newPlotType){
                    return { ...state, plotType: action.payload.newPlotType };
                }
                else
                    return { ...state};
            }
        default:
            return { ...state };
    }
}
