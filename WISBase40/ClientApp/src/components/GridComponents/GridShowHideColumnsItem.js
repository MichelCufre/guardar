import React from 'react';
import { ListGroup } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { cellType } from '../Enums';

export function GridShowHideColumnsItem(props) {
    const { t } = useTranslation();

    const handleClick = (evt) => {
        evt.preventDefault();

        props.showHideEvent(props.columnId);        
    };

    if (!props.columnName) {
        let placeholder;

        switch (props.type) {
            case cellType.button:
                placeholder = "Botones";
                break;
            case cellType.itemList:
                placeholder = "Lista de items";
                break;
            case cellType.checkbox:
                placeholder = "Checkbox";
                break;
            default:
                placeholder = "Columna";
        }

        return (
            <ListGroup.Item className="gr-grid-showmenu-item" action onClick={handleClick}><span className="text-muted">{t(placeholder)}</span></ListGroup.Item>
        );
    }

    if (props.disabled) {
        return (
            <ListGroup.Item className="gr-grid-showmenu-item list-group-item-light" title="Columna no puede ocultarse">{t(props.columnName)}</ListGroup.Item>
        );
    }

    return (
        <ListGroup.Item className="gr-grid-showmenu-item" action onClick={handleClick}>{t(props.columnName)}</ListGroup.Item>
    );
}