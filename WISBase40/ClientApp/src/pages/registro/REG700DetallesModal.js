import React from 'react';
import { Button, Modal } from 'react-bootstrap';
import { useCustomTranslation } from '../../components/TranslationHook';
import { withPageContext } from '../../components/WithPageContext';
import { REG700DetallesRecorrido } from './REG700DetallesRecorrido';

function InternalREG700DetallesModal(props) {

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
                <Modal.Title>{t("REG700DetallesModal_Sec0_mdlCreate_Titulo")}</Modal.Title>
            </Modal.Header>

            <Modal.Body>

                <REG700DetallesRecorrido
                    id="REG700DetallesModal_grid_1"
                    onBeforeInitialize={addParameters}
                    onBeforeFetch={addParameters}
                    onBeforeFetchStats={addParameters}
                    onBeforeImportExcel={addParameters}
                    onBeforeExportExcel={addParameters}
                    onBeforeApplyFilter={addParameters}
                    onBeforeApplySort={addParameters}
                    rowsToDisplay={15}
                    isImportEnabled={props.isImportEnabled}
                />

            </Modal.Body>

            <Modal.Footer>
                <Button variant="btn btn-outline-secondary" onClick={handleClose}>{t("REG700DetallesModal_frm1_btn_CERRAR")}</Button>
            </Modal.Footer>
        </Modal>
    );
}

export const REG700DetallesModal = withPageContext(InternalREG700DetallesModal);