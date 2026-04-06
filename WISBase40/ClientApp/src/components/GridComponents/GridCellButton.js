import React, { Component } from 'react';
import { withKeyNavigation } from './WithKeyNavigation';
import { withTranslation } from 'react-i18next';

class InternalCellButton extends Component {
    constructor(props) {
        super(props);

        this.cellRef = React.createRef();

        this.state = {
            disabled: false
        };
    }

    /*shouldComponentUpdate(nextProps) {
        return this.props.content.value !== nextProps.content.value
            || this.props.disabledButtons.length !== nextProps.disabledButtons.length
            || this.props.content.old !== this.props.content.value
            || nextProps.content.old !== nextProps.content.value;
    }*/

    handleClick = (evt) => {
        evt.preventDefault();

        const buttonId = evt.currentTarget.id;
        const ctrlKey = evt.ctrlKey;

        this.setState({
            disabled: true
        }, () => {
                this.props.performButtonAction(this.props.rowId, this.props.column.id, buttonId, false, ctrlKey).then(d => {
                this.setState({
                    disabled: false
                });
            });
        });
    }

    handleKeydown = (evt) => {
        switch (evt.which) {
            case 37:
                this.props.navigation.handleLeft(evt.target);
                evt.preventDefault();
                break;
            case 39:
                this.props.navigation.handleRight(evt.target);
                evt.preventDefault();
                break;
            case 38:
                this.props.navigation.handleUp(evt.target);
                evt.preventDefault();
                break;
            case 40:
                this.props.navigation.handleDown(evt.target);
                evt.preventDefault();
                break;
            case 13:
                this.props.navigation.handleDown(evt.target);
                evt.preventDefault();
                break;
            case 9:
                this.props.navigation.handleTab(evt.target, evt.shiftKey);
                evt.preventDefault();
                break;
        }
    }

    addHighlight = (evt) => {
        this.props.toggleCellHighlight(evt.shiftKey);
    }

    getButtonContent = (btn) => {
        if (btn.cssClass) {
            return (
                <i className={btn.cssClass} />
            );
        }

        return btn.label;
    }    
    getButtons = () => {
        if (this.props.rowIsNew)
            return null;

        if (!this.props.column.buttons) {
            return null;
        }

        return this.props.column.buttons.map(btn => {            
            if (this.props.disabledButtons.indexOf(btn.id) === -1) {
                if (this.state.disabled) {
                    return (
                        <button
                            key={btn.id}
                            className="gr-btn disabled"
                            value={btn.id}
                            id={btn.id}
                            onClick={this.handleClick}
                            title={this.props.t(btn.label)}
                            onFocus={this.addHighlight}
                        >
                            {this.getButtonContent(btn)}
                        </button>
                    );
                }

                return (
                    <button
                        key={btn.id}
                        className="gr-btn"
                        value={btn.id}
                        id={btn.id}
                        onClick={this.handleClick}
                        title={this.props.t(btn.label)}
                        onFocus={this.addHighlight}
                    >
                        {this.getButtonContent(btn)}
                    </button>
                );
            }
        });
    }

    render() {
        return (
            <div
                ref={this.cellRef}
                className="gr-btn-list"
                onKeyDown={this.handleKeydown}
                onFocus={this.addHighlight}
                tabIndex={0}
            >
                {this.getButtons()}
            </div>
        );
    }
}

export const CellButton = withTranslation()(withKeyNavigation(InternalCellButton));