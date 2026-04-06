import React, { useState } from 'react';
import { Modal, Button, Row, Col, Tab, Tabs, Container } from 'react-bootstrap';
import { Grid } from '../../components/GridComponents/Grid';
import { useTranslation } from 'react-i18next';
import { Page } from '../../components/Page';
import { withPageContext } from '../../components/WithPageContext';

function InternalREG040PrediosModal(props) {
    const { t } = useTranslation();

    const handleClose = () => {
        props.onHide();
    };

    const handleBeforeButtonAction = (context, data, nexus) => {

        if (data.buttonId === "btnEnviarTracking") {
            data.parameters = [
                { id: "predio", value: data.row.cells.find(w => w.column == "NU_PREDIO").value },
            ];
        }
    };

    const handleAfterButtonAction = (context, data, nexus) => {

        if (nexus)
            nexus.getGrid("REG705_grid_1").refresh();

        data.getGrid("REG705_grid_1").refresh();

    };
    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-90w" backdrop="static">
            <Modal.Header closeButton>
                <Modal.Title>{t("REG040_SecPredios_pageTitle_Titulo")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Page
                    application="REG705"
                    {...props}
                >
                    <div className="row mb-4">
                        <div className="col-12">
                            <Grid
                                id="REG705_grid_1"
                                application="REG705"
                                rowsToFetch={30}
                                rowsToDisplay={15}
                                enableExcelExport
                                onBeforeButtonAction={handleBeforeButtonAction}
                                onAfterButtonAction={handleAfterButtonAction}
                            />
                        </div>
                    </div>
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

export const REG040PrediosModal = withPageContext(InternalREG040PrediosModal);