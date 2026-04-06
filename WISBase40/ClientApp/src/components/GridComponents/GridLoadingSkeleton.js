import React, { Component } from 'react';

export class LoadingSkeleton extends Component {
    constructor(props) {
        super(props);

        this.state = {
            show: true
        };
    }

    handleTransitionEnd = () => {
        if (!this.props.isInitializing) {
            this.setState({
                show: false
            });
        }
    }

    getClassName() {
        return "gr-loading-skeleton" + (this.props.isInitializing ? " display" : " hidden");
    }
    getStyle() {
        return {
            height: this.props.height
        };
    }    

    render() {
        if (!this.state.show)
            return null;

        return (
            <div className={this.getClassName()} style={this.getStyle()} onTransitionEnd={this.handleTransitionEnd}>                
                <div className="gr-skeleton">
                    <div className="gr-toolbar-skeleton">
                        <div className="gr-action-skeleton">
                            <div className="gr-button-skeleton" />
                            <div className="gr-divider-skeleton" />
                            <div className="gr-action-skeleton-padding" />
                        </div>
                        <div className="gr-filter-skeleton">
                            <div className="gr-button-skeleton" />
                            <div className="gr-button-skeleton" />
                            <div className="gr-divider-skeleton" />
                            <div className="gr-text-skeleton" />
                        </div>
                    </div>
                    <div className="gr-header-skeleton" />
                    <div className="gr-body-skeleton" />
                </div>
            </div>
        );
    }
}