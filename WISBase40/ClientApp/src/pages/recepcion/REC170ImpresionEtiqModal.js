import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { useTranslation } from 'react-i18next';
import { withPageContext } from '../../components/WithPageContext';
import { Modal, Button } from 'react-bootstrap';

function InternalREC170ImpresionEtiqModal(props) {

    const { t } = useTranslation();

    const [impresoraModal, setImpresoraModal] = useState(false);

    const [rowSeleccionadasImprimir, setRowSeleccionadasImprimir] = useState(null);

    const OpenSeleccionImpresoraDialog = () => {
        setImpresoraModal(true);
    }

    const CloseSeleccionImpresoraDialog = () => {
        setImpresoraModal(false);
    }

    const onAfterInitialize = (context, form, query, nexus) => {

    }

    const HandleMenuButtonAction = (evt, data) => {

        props.nexus.getGrid("REC170ImpresionEtiq_grid_1").triggerMenuAction("btnImprimir", false, evt.ctrlKey);
    };

    const applyParameters = (context, data, nexus) => {
        if (props.agenda)
            data.parameters = [{ id: "agenda", value: props.agenda.find(x => x.id === "idAgenda").value }];
    };

    const GridOnAfterMenuItemAction = (context, data, nexus) => {
        let jsonAdded = data.parameters.find(w => w.id === "ListaFilasSeleccionadas").value;

        setRowSeleccionadasImprimir(jsonAdded);
        OpenSeleccionImpresoraDialog();
        props.onHide();

    }

    const handleClose = () => {
        props.onHide(props.nexus);
    };

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-90w" backdrop="static">
            <Modal.Header closeButton>
                <Modal.Title>{t("REC170ImpresionEtiq_Sec0_mdl_ImpresionEtiqModal_Titulo")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Grid
                    application="REC170ImpresionEtiq"
                    id="REC170ImpresionEtiq_grid_1"
                    rowsToFetch={30}
                    rowsToDisplay={15}
                    enableExcelExport
                    enableSelection
                    onBeforeInitialize={applyParameters}
                    onBeforeFetch={applyParameters}
                    onBeforeExportExcel={applyParameters}
                    onBeforeButtonAction={applyParameters}
                    onBeforeMenuItemAction={applyParameters}
                    onBeforeFetchStats={applyParameters}
                    onBeforeApplyFilter={applyParameters}
                    onBeforeApplySort={applyParameters}
                    onAfterValidateRow={applyParameters}
                    onAfterMenuItemAction={GridOnAfterMenuItemAction}
                />
            </Modal.Body>
            <Modal.Footer>
                <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("REC170ImpresionEtiq_frm1_btn_cerrar")} </Button>
                <Button variant="btn btn-outline-primary" onClick={HandleMenuButtonAction}> {t("REC170ImpresionEtiq_frm1_btn_confirmar")} </Button>
            </Modal.Footer>          
        </Modal>
    );
}

export const REC170ImpresionEtiqModal = withPageContext(InternalREC170ImpresionEtiqModal);
