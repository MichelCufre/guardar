import React from 'react';
import { Button, Modal } from 'react-bootstrap';
import { Grid } from '../../components/GridComponents/Grid';
import { useCustomTranslation } from '../../components/TranslationHook';
import { withPageContext } from '../../components/WithPageContext';

function InternalREG700UbicacionesSinAsociarModal(props) {

    const { t } = useCustomTranslation();

    const handleClose = () => {
        props.onHide();
    };

    //--------------GRID--------------

    const addParameters = (context, data, nexus) => {

        var numeroRecorrido = props.numeroRecorrido;

        data.parameters = [{ id: "REG700_DETALLES_NU_RECORRIDO", value: numeroRecorrido }];
    };

    //----------------------------------------

    return (

        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-50w" backdrop="static">

            <Modal.Header closeButton>
                <Modal.Title>{t("REG700UbicacionesSinAsociar_Sec0_mdlCreate_Titulo")}</Modal.Title>
            </Modal.Header>

            <Modal.Body>

                <Grid
                    application="REG700UbicacionesSinAsociar"
                    id="REG700UbicacionesSinAsociar_grid_1"
                    rowsToFetch={30}
                    rowsToDisplay={15}
                    enableExcelExport
                    onBeforeInitialize={addParameters}
                    onBeforeFetch={addParameters}
                    onBeforeFetchStats={addParameters}
                    onBeforeImportExcel={addParameters}
                    onBeforeExportExcel={addParameters}
                    onBeforeApplyFilter={addParameters}
                    onBeforeApplySort={addParameters}
                />

            </Modal.Body>

            <Modal.Footer>
                <Button variant="btn btn-outline-secondary" onClick={handleClose}>{t("REG700DetallesModal_frm1_btn_CERRAR")}</Button>
            </Modal.Footer>
        </Modal>
    );
}

export const REG700UbicacionesSinAsociarModal = withPageContext(InternalREG700UbicacionesSinAsociarModal);