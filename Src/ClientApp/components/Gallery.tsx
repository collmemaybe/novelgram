import * as React from 'react';
import { Link, RouteComponentProps } from 'react-router-dom';
import { connect } from 'react-redux';
import { ApplicationState } from '../store';
import * as GalleryState from '../store/Gallery';

type GalleryProps =
    GalleryState.GalleryState
    & typeof GalleryState.actionCreators
    & RouteComponentProps<{ userId: string }>;

class Gallery extends React.Component<GalleryProps, {}> {
    componentWillMount() {
        const userId = this.props.match.params.userId || "";
        this.props.requestDetails(userId);
    }

    componentWillReceiveProps(nextProps: GalleryProps) {
        const userId = this.props.match.params.userId || "";
        this.props.requestDetails(userId);
    }

    public render() {
        return <div>
            <h1>Gallery</h1>
            {this.renderTable()}
        </div>;
    }

    private renderTable() {
        return <table className='table'>
                   <tbody>
                   {this.props.details.map(d => <tr><td>{d.key}</td></tr>)}
                   </tbody>
               </table>;
    }
}

export default connect(
    (state: ApplicationState) => state.gallery, // Selects which state properties are merged into the component's props
    GalleryState.actionCreators                 // Selects which action creators are merged into the component's props
)(Gallery) as typeof Gallery;