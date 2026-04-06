import React from 'react';
import { cellType } from '../Enums';
import { CellText } from './GridCellText';
import { CellCheckbox } from './GridCellCheckbox';
import { CellProgress } from './GridCellProgress';
import { CellDateTime } from './GridCellDateTime';
import { CellDate } from './GridCellDate';
import { CellButton } from './GridCellButton';
import { CellItemList } from './GridCellItemList';
import { CellSelect } from './GridCellSelect';
import { CellSelectAsync } from './GridCellSelectAsync';
import { CellToggle } from './GridCellToggle';

export function CellContent(props) {
    switch (props.type) {
        case cellType.text:
            return <CellText {...props} />;
        case cellType.checkbox:
            return <CellCheckbox {...props} />;
        case cellType.progress:
            return <CellProgress {...props} />;
        case cellType.dateTime:
            return <CellDateTime {...props} />;
        case cellType.date:
            return <CellDate {...props} />;
        case cellType.button:
            return <CellButton {...props} />;
        case cellType.itemList:
            return <CellItemList {...props} />;
        case cellType.select:
            return <CellSelect {...props} />;
        case cellType.selectAsync:
            return <CellSelectAsync {...props} />;
        case cellType.toggle:
            return <CellToggle {...props} />;
        default:
            return props.content.value;
    }
}