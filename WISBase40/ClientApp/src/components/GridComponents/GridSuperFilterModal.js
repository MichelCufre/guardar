import React, { useState } from 'react';
import { Modal, Button } from 'react-bootstrap';
import { filterStatus } from '../Enums';
import ContentEditable from 'react-contenteditable';
import { useTranslation } from 'react-i18next';
import 'react-autocomplete-input/dist/bundle.css';

export function GridSuperFilterModal(props) {
    const [value, setValue] = useState("");
    const { t } = useTranslation();

    const handleClose = (evt) => {
        props.closeSuperFilterBar();
    };

    const handleApply = (evt) => {
        evt.preventDefault();

        const regex = /<mark data-column="[a-zA-Z0-9_-]+">[^<>]+<\/mark>/g;

        const processedValue = value.replace(regex, (x) => ":" + /<mark data-column="([a-zA-Z0-9_-]+)">.+<\/mark>/g.exec(x)[1]).replace(/<\/div>/g, ' ').replace(/<div>/g, '').replace(/<br\/?>/g, ' ');

        console.log(processedValue);

        props.applySuperFilter(processedValue);
        
        props.closeSuperFilterBar();
    };

    const handleChange = (evt) => {
        setValue(evt.target.value);
    };

    const isSuperFilterOpen = props.superFilterStatus === filterStatus.open;

    const options = props.columns.map(d => t(d.name));

    const content = value.replace(/:[a-zA-Z0-9_-]+ /g, (x) => {
        const column = props.columns.find(d => d.id === x.substring(1, x.length - 1));

        if(column)
            return '<mark data-column="' + column.id + '">' + column.name + "</mark> ";

        return "";
    });

    const style = {
        wordBreak: 'break-word',
        whiteSpace: 'normal',
        height: 'auto',
        minHeight: '10rem'
    };

    return (
        <Modal show={isSuperFilterOpen} onHide={handleClose}>
            <Modal.Header closeButton>
                <Modal.Title>Filtro avanzado</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <ContentEditable
                    className="form-control"
                    style={style}
                    onChange={handleChange}
                    html={content}
                />
            </Modal.Body>
            <Modal.Footer>
                <Button variant="danger" onClick={handleClose}>
                    Cancelar
                </Button>
                <Button variant="primary" onClick={handleApply}>
                    Aplicar
                </Button>
            </Modal.Footer>
        </Modal>
    );
}