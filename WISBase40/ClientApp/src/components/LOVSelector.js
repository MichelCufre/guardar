import React, { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Button, Col, Modal, Row } from 'react-bootstrap';
import Select from 'react-select';

export function LOVSelector(props) {
    const { t } = useTranslation();

    const [selectedValue, setSelectedValue] = useState(null);

    useEffect(() => {
        if (!selectedValue && props.items.length > 0) {
            setSelectedValue(props.items[0].Id);
        }
    });

    const handleClose = () => {
        setSelectedValue(null);
        props.onHide();
    };

    const handleSelect = () => {
        props.onSelect(selectedValue);
        setSelectedValue(null);
    };

    const handleChange = (option) => {
        setSelectedValue(option.value);
    };

    if (props.items && props.items.length > 0) {
        const options = props.items.map(i => ({ value: i.Id, label: [i.Id, i.Value].join(" - ") }));
        const title = props.title || t("General_Sec0_modalTitle_LOVSelector");

        return (
            <Modal show={props.show} onHide={handleClose} dialogClassName="modal-25w" backdrop="static">
                <Modal.Header closeButton>
                    <Modal.Title>{title}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Row>
                        <Col>
                            <Select defaultValue={options[0]} onChange={handleChange} options={options} />
                        </Col>
                    </Row>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="outline-secondary" onClick={handleClose}>
                        {t("General_Sec0_btn_Cerrar")}
                    </Button>
                    <Button variant="primary" onClick={handleSelect}>
                        {t("General_Sec0_btn_Confirmar")}
                    </Button>
                </Modal.Footer>
            </Modal >
        );
    }

    return null;
}