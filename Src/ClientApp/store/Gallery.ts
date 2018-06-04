import { fetch, addTask } from 'domain-task';
import { Action, Reducer, ActionCreator } from 'redux';
import { AppThunkAction } from './';

// -----------------
// STATE - This defines the type of data maintained in the Redux store.

export interface GalleryState {
    userId?: string;
    details: PhotoDetail[];
}

export interface PhotoDetail {
    key: string;
}

// -----------------
// ACTIONS - These are serializable (hence replayable) descriptions of state transitions.
// They do not themselves have any side-effects; they just describe something that is going to happen.

interface RequestDetailsAction {
    type: 'REQUEST_DETAILS';
    userId: string;
}

interface ReceiveDetailsAction {
    type: 'RECEIVE_DETAILS';
    userId: string;
    details: PhotoDetail[];
}

// Declare a 'discriminated union' type. This guarantees that all references to 'type' properties contain one of the
// declared type strings (and not any other arbitrary string).
type KnownAction = RequestDetailsAction | ReceiveDetailsAction;

// ----------------
// ACTION CREATORS - These are functions exposed to UI components that will trigger a state transition.
// They don't directly mutate state, but they can have external side-effects (such as loading data).

export const actionCreators = {
    requestDetails: (userId: string): AppThunkAction<KnownAction> => (dispatch, getState) => {
        let fetchTask = fetch(`api/user/gallery?userId=${userId}`, {method: 'GET', credentials: 'include'})
            .then(response => response.json() as Promise<PhotoDetail[]>)
            .then(data => {
                dispatch({ type: 'RECEIVE_DETAILS', userId: userId, details: data });
            });

        addTask(fetchTask); // Ensure server-side prerendering waits for this to complete
        dispatch({ type: 'REQUEST_DETAILS', userId: userId });
    }
};

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

const unloadedState: GalleryState = { details: [] };

export const reducer: Reducer<GalleryState> = (state: GalleryState, incomingAction: Action) => {
    const action = incomingAction as KnownAction;
    switch (action.type) {
        case 'REQUEST_DETAILS':
            return {
                userId: action.userId,
                details: state.details
            };
        case 'RECEIVE_DETAILS':
            return {
                userId: action.userId,
                details: action.details
            };
        default:
            // The following line guarantees that every action in the KnownAction union has been covered by a case above
            const exhaustiveCheck: never = action;
    }

    return state || unloadedState;
};
