import 'gasparesganga-jquery-loading-overlay';
import React, { useState } from 'react';
import { Button, Modal } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { withPageContext } from '../../components/WithPageContext';
import { PRE812AsignarFuncCola } from './PRE812AsignarFuncCola';

function InternalPRE812ZoominModal(props) {
    const { t } = useTranslation("translation", { useSuspense: false });
    const [rowSeleccionadas, setRowSeleccionadas] = useState(null);

    const [showAsignarFuncPopupAdd, setShowAsignarFuncPopupAdd] = useState(false);

    const closeAsignarFuncModal = () => {
        setShowAsignarFuncPopupAdd(false);
        setShowRearModal(true);
    }

    const gridOnBeforeButtonAction = (context, data, nexus) => {
        data.parameters = [
            { id: "nuPedido", value: props.datosKey.find(a => a.id === "nuPedido").value },
            { id: "cdEmpresa", value: props.datosKey.find(a => a.id === "cdEmpresa").value },
            { id: "cdCliente", value: props.datosKey.find(a => a.id === "cdCliente").value }
        ];
    }

    const handleClose = () => {
        props.onHide();
    };

    const addParameters = (context, data, nexus) => {
        data.parameters = [
            { id: "nuPedido", value: props.datosKey.find(a => a.id === "nuPedido").value },
            { id: "cdEmpresa", value: props.datosKey.find(a => a.id === "cdEmpresa").value },
            { id: "cdCliente", value: props.datosKey.find(a => a.id === "cdCliente").value }
        ];
    }

    const onAfterMenuItemAction = (context, data, nexus) => {
        if (data.buttonId === "btnAsociar" && data.parameters.find(w => w.id === "ListaFilasSeleccionadas")) {
            let jsonAdded = data.parameters.find(w => w.id === "ListaFilasSeleccionadas").value;
            setRowSeleccionadas(jsonAdded);
            setShowAsignarFuncPopupAdd(true);
            setShowRearModal(false);
        }
        else
            nexus.getGrid("PRE812Zoo_grid_1").refresh();
    }

    const [showRearModal, setShowRearModal] = useState(true);

    return (
        <Modal show={props.show} style={{ display: showRearModal ? "block" : "none" }} onHide={handleClose} dialogClassName="modal-90w" backdrop="static">
            <Modal.Header closeButton>
                <Modal.Title>{t("PRE812Zoo_Sec0_modalTitle_Titulo")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Page
                    {...props}
                >
                    <div className="row mb-4">
                        <div className="col-12">
                            <Grid
                                id="PRE812Zoo_grid_1"
                                application="PRE812ZoominModal"
                                rowsToFetch={30}
                                rowsToDisplay={15}
                                enableExcelExport
                                enableSelection
                                onBeforeMenuItemAction={gridOnBeforeButtonAction}
                                onAfterMenuItemAction={onAfterMenuItemAction}
                                onBeforeInitialize={addParameters}
                                onBeforeSelectSearch={addParameters}
                                onBeforeExportExcel={addParameters}
                                onBeforeFetch={addParameters}
                                onBeforeFetchStats={addParameters}
                                onBeforeApplySort={addParameters}
                                onBeforeApplyFilter={addParameters}
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
            <PRE812AsignarFuncCola show={showAsignarFuncPopupAdd} onHide={closeAsignarFuncModal} rowSeleccionadas={rowSeleccionadas} />
        </Modal>
    );
}

export const PRE812ZoominModal = withPageContext(InternalPRE812ZoominModal);