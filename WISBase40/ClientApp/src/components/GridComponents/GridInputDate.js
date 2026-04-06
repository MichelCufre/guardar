import React, { Component } from 'react';
import Flatpickr from 'react-flatpickr';

export default class InputDate extends Component {
    componentDidMount() {
        if (this.reference) {
            this.reference.focus();
        }
    }

    render() {
        return (
            <Flatpickr
                data-enable-time
                options={{
                    defaultDate: this.props.value,
                    altInput: true,
                    altInputClass: "gr-cell-content-input",
                    altFormat: "d/m/Y H:i:s"
                }}
                onBlur={this.props.onBlur}
                onKeyUp={this.props.onKeyUp}
                ref={reactFlatPicker => { this.reference = reactFlatPicker ? reactFlatPicker.flatpickr.altInput : null; }}
            />
        );
    }
}