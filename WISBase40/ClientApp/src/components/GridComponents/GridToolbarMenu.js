import React, { Component } from 'react';
import { MenuItem } from './GridMenuItem';

export class ToolbarMenu extends Component {
    constructor(props) {
        super(props);

        this.state = {
            show: false
        };
    }

    handleItemClick = (evt) => {
        this.setState({
            show: false
        });

        this.props.performMenuItemAction(evt.target.id, false, evt.ctrlKey);
    }
    handleClick = () => {
        this.setState(prevState => ({
            show: !prevState.show
        }));
    }
    handleMenuLeave = () => {
        this.setState({
            show: false
        });
    }

    getItems = () => {
        if (this.props.items) {
            var cosa = this.props.items.map((d, index) => (
                <MenuItem
                    key={d.id}
                    type={d.itemType}
                    id={d.id}
                    index={index}
                    label={d.label}
                    className={d.cssClass}
                    onClick={this.handleItemClick}
                />
            ));

            return cosa;
        }

        return null;
    }
    getStyle = () => {
        return {
            display: this.state.show ? "block" : "none"
        };
    }

    render() {
        if (this.props.items && this.props.items.length > 0) {
            return (
                <div className="dropdown">
                    <button className="gr-toolbar-btn" onClick={this.handleClick} title={this.props.label}>
                        <i className={this.props.icon} />
                    </button>
                    <div className="dropdown-menu" style={this.getStyle()} onMouseLeave={this.handleMenuLeave}>
                        {this.getItems()}
                    </div>
                </div>
            );
        }

        return (
            <div className="dropdown">
                <button className="gr-toolbar-btn disabled" title={this.props.label}>
                    <i className={this.props.icon} />
                </button>
            </div>
        );        
    }
}