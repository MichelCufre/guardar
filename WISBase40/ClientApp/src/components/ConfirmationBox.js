import React, { useState, useLayoutEffect, useEffect, useRef } from 'react';
import { Modal, Row, Button, Col } from 'react-bootstrap';
import { mapButtonVariant } from './Mapper';
import { useTranslation } from 'react-i18next';

export const ConfirmationBox = (props) => {
    const { t } = useTranslation();
    const [modalClass, setModalClass] = useState("");
    const [backdropClass, setBackdropClass] = useState("");
    const innerRef = useRef();

    useLayoutEffect(() => {
        const visibleBackdrop = document.querySelectorAll(".fade.modal-backdrop.show");

        if (visibleBackdrop && visibleBackdrop.length > 0) {
            setModalClass("modal-1070");
            setBackdropClass("modal-backdrop-1060");
        }
    }, [props.message]);

    useEffect(() => {
        if (innerRef.current && innerRef.current.focus()) {
            setTimeout(() => innerRef.current.focus(), 100);
        }
    }, [innerRef.current]);

    const handleClose = () => {
        props.close();
    };

    const handleAccept = () => {
        if (props.onAccept.current)
            props.onAccept.current();

        props.close();
    };

    const handleCancel = () => {
        if (props.onCancel.current)
            props.onCancel.current();

        props.close();
    };
    const translatedMessage = () => {
        let translatedMessage = t(props.message);

        if (props.argsMessage && props.argsMessage.length > 0) {
            props.argsMessage.forEach((arg, index) => {
                translatedMessage = translatedMessage.replace(new RegExp("\\{" + index + "\\}"), arg);
            });
        }
        return translatedMessage;
    };


    const acceptVariant = mapButtonVariant(props.acceptVariant) || "primary";
    const cancelVariant = mapButtonVariant(props.cancelVariant) || "secondary";

    const acceptLabel = props.acceptLabel || "General_Sec0_btn_CONFIRM_BOX_ACCEPT_DEFAULT";
    const cancelLabel = props.cancelLabel || "General_Sec0_btn_CONFIRM_BOX_CANCEL_DEFAULT";


    return (
        <Modal show={props.isOpen} size="lg" onHide={handleClose} backdropClassName={backdropClass} className={modalClass}>
            <Modal.Header closeButton>
                <Modal.Title>{t("General_Sec0_lbl_CONFIRM_BOX_TITLE")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Row>
                    <Col>
                        {translatedMessage()}
                    </Col>
                </Row>
            </Modal.Body>
            <Modal.Footer>
                <button type="button" className="btn btn-primary" autoFocus ref={innerRef}
                    onClick={handleAccept} >
                    {t(acceptLabel)}
                </button >
                <Button variant={cancelVariant} onClick={handleCancel}>
                    {t(cancelLabel)}
                </Button>
            </Modal.Footer>
        </Modal>
    );
};