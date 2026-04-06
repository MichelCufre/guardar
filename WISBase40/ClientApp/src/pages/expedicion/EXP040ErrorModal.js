import React, { useState } from 'react';
import { Modal, Button, Row, Col, Tab, Tabs } from 'react-bootstrap';
import { useCustomTranslation } from '../../components/TranslationHook';

export function EXP040ErrorModal(props) {
    const { t } = useCustomTranslation("translation", { useSuspense: false });

    const handleClose = () => {
        props.onHide();
    }

    console.log(props.errores);

    const content = !props.errores ? null : props.errores.map(error => {
        const datos = error.Datos.map(e => (
            <li>{e}</li>
        ));

        return (
            <Row>
                <Col>
                    <strong>{error.Message}</strong>
                    <ul>{datos}</ul>
                </Col>
            </Row>
        )
    });

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-50w" backdrop="static">
            <Modal.Header closeButton>
                <Modal.Title>{t("EXP040_lbl_Title_ErrorValidacion")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                {content}
            </Modal.Body>
            <Modal.Footer>
                <Button variant="btn btn-outline-secondary" onClick={handleClose}>
                    {t("General_Sec0_btn_Cerrar")}
                </Button>
            </Modal.Footer>
        </Modal>
    );
}