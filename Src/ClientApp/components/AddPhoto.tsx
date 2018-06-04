import * as React from 'react';
import { RouteComponentProps } from 'react-router-dom';

export default class AddPhoto extends React.Component<RouteComponentProps<{}>, {}> {
    public render() {
        return <div>
            <h1>Add a Photo</h1>

            <p>Upload one or more pictures to your collection</p>

            <form method="post" id="theform" action="/Photo/Upload">
                <div className="form-group">
                    <div className="col-md-10">
                        <input type="file" name="formFile" />
                    </div>
                </div>
                <div className="form-group">
                    <div className="col-md-10">
                        <input type="submit" value="Upload" onClick={(e: any) => {
                            e.preventDefault();
                            const f = document.getElementById("theform") as HTMLFormElement;
                            f.enctype = "multipart/form-data";
                            f.submit();
                        } } />
                    </div>
                </div>
            </form>
        </div>;
    }
}