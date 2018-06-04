import * as React from 'react';
import { Link, RouteComponentProps } from 'react-router-dom';
import { connect } from 'react-redux';
import { ApplicationState }  from '../store';
import * as CounterStore from '../store/Counter';
import * as WeatherForecasts from '../store/WeatherForecasts';

type CounterProps =
    CounterStore.CounterState
    & typeof CounterStore.actionCreators
    & RouteComponentProps<{}>;

class Counter extends React.Component<CounterProps, {}> {
    public render() {
        return <div>
            <h1>Add a Photo</h1>

            <p>Upload one or more pictures to your collection</p>

            <form method="post" enctype="multipart/form-data" asp-controller="UploadFiles" asp-action="Index">
                <div class="form-group">
                    <div class="col-md-10">
                        <input type="file" name="files" multiple />
                    </div>
                </div>
                <div class="form-group">
                    <div class="col-md-10">
                        <input type="submit" value="Upload" />
                    </div>
                </div>
            </form>
        </div>;
    }
}

// Wire up the React component to the Redux store
export default connect(
    (state: ApplicationState) => state.counter, // Selects which state properties are merged into the component's props
    CounterStore.actionCreators                 // Selects which action creators are merged into the component's props
)(Counter) as typeof Counter;