import React, { useState, useRef } from 'react';
import { Modal, Button, Row, Col, Tab, Tabs, Container } from 'react-bootstrap';
import { Grid } from '../../components/GridComponents/Grid';
import { useCustomTranslation } from '../../components/TranslationHook';
import { Page } from '../../components/Page';
import { withPageContext } from '../../components/WithPageContext';

function InternalINT050AdministrarBloqueosModal(props) {

    const { t } = useCustomTranslation();

    const handleClose = () => {
        props.onHide();
    };

    const onBeforeMenuItemAction = (context, data, nexus) => {
        data.parameters = [           
        ];
    };


    const onAfterMenuItemAction = (context, data, nexus) => {
        nexus.getGrid("INT050Bloqueos_grid_1").refresh();
    };

    return (
        <Modal dialogClassName="modal-50w" show={props.show} onHide={props.onHide} >
            <Modal.Header closeButton>
                <Modal.Title>{t("INT050_Sec0_Title_AdministrarBloqueos")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Page
                    load
                    {...props}
                >
                    <Grid
                        application="INT050Bloqueos"
                        id="INT050Bloqueos_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableSelection
                        onBeforeMenuItemAction={onBeforeMenuItemAction}
                        onAfterMenuItemAction={onAfterMenuItemAction}
                    />
                </Page>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="outline-secondary" onClick={handleClose}>
                    {t("General_Sec0_btn_Cerrar")}
                </Button>
            </Modal.Footer>
        </Modal>
    );
}

export const INT050AdministrarBloqueosModal = withPageContext(InternalINT050AdministrarBloqueosModal);