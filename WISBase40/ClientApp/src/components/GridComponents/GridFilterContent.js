import React from 'react';
import { cellType } from '../Enums';
import { FilterCheckbox } from './GridFilterCheckbox';
import { FilterText } from './GridFilterText';

export function FilterContent(props) {
    switch (props.type) {
        case cellType.checkbox:
        case cellType.toggle:
            return <FilterCheckbox {...props} />;
        default:
            return <FilterText {...props} />;
    }
}