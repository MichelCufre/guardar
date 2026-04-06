import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { useTranslation } from 'react-i18next';
import { Modal, Button} from 'react-bootstrap';

export function VerErroresAutomatismoModal(props) {

    const { t } = useTranslation();

    const [automatismoEjecucion, setAutomatismoEjecucion] = useState(null);

    const handleClose = () => {
        props.onHide();
    };

    const onBeforeInitialize = (context, data, nexus) => {
        setAutomatismoEjecucion(props.automatismoEjecucion);

        data.parameters.push({ id: "NU_AUT_EJECUCION", value: props.automatismoEjecucion });
    };

    const addParameters = (context, data, nexus) => {
        data.parameters = [{ id: "NU_AUT_EJECUCION", value: automatismoEjecucion }];
    };

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-70w" backdrop="static">
            <Modal.Header closeButton>
                <Modal.Title> {t("AUT101VerErroresModal_Sec0_modalTitle_Titulo")} </Modal.Title>
            </Modal.Header>

            <Modal.Body>
                <Grid
                    application="AUT101VerErrores"
                    id="AUT101VerErrores_grid_1"
                    onBeforeInitialize={onBeforeInitialize}
                    onBeforeFetch={addParameters}
                    onBeforeFetchStats={addParameters}
                    onBeforeExportExcel={addParameters}
                    onBeforeApplyFilter={addParameters}
                    onBeforeApplySort={addParameters}
                    rowsToFetch={30}
                    rowsToDisplay={15}
                    enableExcelExport
                />
            </Modal.Body>

            <Modal.Footer>
                <Button variant="outline-secondary" onClick={handleClose}>
                    {t("General_Sec0_btn_Cerrar")}
                </Button>
            </Modal.Footer>

        </Modal>
    );
}