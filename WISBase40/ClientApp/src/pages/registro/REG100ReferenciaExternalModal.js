import React from 'react';
import { Modal, Row } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { Grid } from '../../components/GridComponents/Grid';
import { withPageContext } from '../../components/WithPageContext';


function InternalREG100ReferenciaExternaUpdate(props) {
    const { t } = useTranslation();

    // const handleClose = () => {
    //     props.onHide();
    // }

    const addParameters = (context, data, nexus) => {
        data.parameters = props.empresa;

        //data.parameters = [
        //    { id: "idUsuario", value: props.usuario.find(x => x.id === "idUsuario").value }

        //];
    }


    return (
        <Modal dialogClassName="modal-90w" show={props.show} onHide={props.onHide} >
            <Modal.Header closeButton>
                <Modal.Title>{t("REG100_Sec0_mdlUpdate_Referencia_Titulo")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <div className="w-100">
                    <Grid
                        application="REG100UpdateRef"
                        id="REG100UpdateRef_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        onBeforeInitialize={addParameters}
                        onBeforeFetch={addParameters}
                        onBeforeExportExcel={addParameters}
                        onBeforeFetchStats={addParameters}
                        onBeforeApplySort={addParameters}
                        onBeforeApplyFilter={addParameters}
                        onBeforeCommit={addParameters}
                        onAfterMenuItemAction={addParameters}
                    />
                </div>
            </Modal.Body>
        </Modal>
    );
}

export const REG100UpdateReferenciaExternaModal = withPageContext(InternalREG100ReferenciaExternaUpdate);