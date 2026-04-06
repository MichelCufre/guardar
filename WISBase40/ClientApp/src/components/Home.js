import React, { Component } from 'react';
import { Page } from './Page';
import './Home.css';

export default class Home extends Component {
    displayName = Home.name

    render() {
        const backgroundImageStyle = {
            backgroundImage: `url(${window.location.origin + "/api/Image/Get?id=background"})`
        };
    
        return (
            <Page>
                <div className="main-background" style={backgroundImageStyle}>
                    <div className="clear_both"></div>
                </div>
            </Page>
        );
    }
}
