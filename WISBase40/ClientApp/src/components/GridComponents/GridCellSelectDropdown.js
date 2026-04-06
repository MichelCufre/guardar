import React from 'react';
import { GridCellSelectMenu } from './GridCellSelectMenu';

export function Dropdown(props) {
    return (
        <React.Fragment>
            {props.target}
            {props.isOpen ? <GridCellSelectMenu siblingsWidth={props.siblingsWidth} parentWidth={props.parentWidth} positionLeft={props.left} positionTop={props.bottom}>{props.children}</GridCellSelectMenu> : null}
            {props.isOpen ? <div className="gr-cell-select-blanket" onClick={props.onClose} /> : null}
        </React.Fragment>
    );
}