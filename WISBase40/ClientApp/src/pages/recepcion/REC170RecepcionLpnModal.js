import React from 'react';
import { Button, Modal } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { Grid } from '../../components/GridComponents/Grid';
import { withPageContext } from '../../components/WithPageContext';

function InternalREC170RecepcionLpnModal(props) {
    const { t } = useTranslation();

    const applyParameters = (context, data, nexus) => {
        data.parameters = [
            { id: "keyAgenda", value: props.agenda?.find(a => a.id === "idAgenda").value }
        ];
    };

    const handleClose = () => {
        props.onHide(null, props.nexus);
    };

    return (

        <Modal
            show={props.show} onHide={handleClose} dialogClassName="modal-90w" backdrop="static"
        >
            <Modal.Header closeButton>
                <Modal.Title>{t("REC170RecepcionLpn_Sec0_lbl_Titulo")}</Modal.Title>
            </Modal.Header>

            <Modal.Body>
                <Grid
                    application="REC170RecepcionLpn"
                    id="REC170RecepcionLpn_grid_1"
                    rowsToFetch={30}
                    rowsToDisplay={15}
                    onBeforeFetch={applyParameters}
                    onBeforeInitialize={applyParameters}
                    onBeforeExportExcel={applyParameters}
                    onBeforeFetchStats={applyParameters}
                    onBeforeApplyFilter={applyParameters}
                    onBeforeApplySort={applyParameters}
                    enableExcelExport
                />
            </Modal.Body>

            <Modal.Footer>
                <Button onClick={handleClose} id="btnCerrar" variant="outline-secondary"> {t("General_Sec0_btn_Cerrar")} </Button>
            </Modal.Footer>
        </Modal >
    );
}
export const REC170RecepcionLpnModal = withPageContext(InternalREC170RecepcionLpnModal);
