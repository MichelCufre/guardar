import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { PAR400CreacionAtributoModal } from './PAR400CreacionAtributoModal';
import { PAR400ModificacionAtributoModal } from './PAR400ModificacionAtributoModal';
import { PAR400AtributoValidacion } from './PAR400AtributoValidacion';
import { Modal } from 'react-bootstrap';

export default function PAR400(props) {

    const [showModal, setShowModal] = useState(false);
    const [showPopupCrearAtributo, setShowPopupCrearAtributo] = useState(false);
    const [showPopupModificarAtributo, setShowPopupModificarAtributo] = useState(false);
    const [codigoAtributo, setCodigoAtributo] = useState(null);
    const [tipoAtributo, setTipoAtributo] = useState(null);
    const [NmAtributo, setNmAtributo] = useState(null);

    const [showAtributoValidacionesModal, setShowAtributoValidacionesModal] = useState(false);
    const [showAtributoValidaciones, setShowAtributoValidaciones] = useState(false);

    const { t } = useTranslation();

    const openFormDialog = () => {
        setShowPopupCrearAtributo(true);
        setShowModal(true);
    }

    const openFormEditarAtributo = () => {
        setShowPopupModificarAtributo(true);
        setShowModal(true);
    }
    const openFormAtributoValidacion = () => {
        setShowAtributoValidacionesModal(true);
        setShowAtributoValidaciones(true);
    }
    const closeFormDialog = (nexus) => {
        setShowPopupCrearAtributo(false);
        setShowPopupModificarAtributo(false);
        setShowModal(false);

        if (nexus) {
            nexus.getGrid("PAR400_grid_1").refresh();
        }
    }

    const GridOnBeforeButtonAction = (context, data, nexus) => {
        if (data.buttonId === "btnEditar") {
            context.abortServerCall = true;
            setCodigoAtributo(data.row.cells.find(w => w.column == "ID_ATRIBUTO").value);
            setTipoAtributo(data.row.cells.find(w => w.column == "ID_ATRIBUTO_TIPO").value);
            openFormEditarAtributo();
        }
        if (data.buttonId === "btnAsociar") {
            context.abortServerCall = true;
            setCodigoAtributo(data.row.cells.find(w => w.column == "ID_ATRIBUTO").value);
            setTipoAtributo(data.row.cells.find(w => w.column == "ID_ATRIBUTO_TIPO").value);
            setNmAtributo(data.row.cells.find(w => w.column == "NM_ATRIBUTO").value)
            openFormAtributoValidacion();
        }
    }

    const closeFormDialogAtributoValidaciones = () => {
        setShowAtributoValidaciones(false);
        setShowAtributoValidacionesModal(false);
    };

    const showFormCrearAtributo = () => { return (<PAR400CreacionAtributoModal show={showPopupCrearAtributo} onHide={closeFormDialog} />); }
    const showFormModificarAtributo = () => {
        return (<PAR400ModificacionAtributoModal show={showPopupModificarAtributo} onHide={closeFormDialog} codigoAtributo={codigoAtributo} tipoAtributo={tipoAtributo} />);
    }

    return (
        <Page
            title={t("PAR400_Sec0_pageTitle_Titulo")}
        >
            <div style={{ textAlign: "center" }}>
                <button className="btn btn-primary" onClick={openFormDialog}>{t("PAR200_Sec0_btn_NuevoAtributo")}</button>
            </div>

            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="PAR400_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                        onBeforeButtonAction={GridOnBeforeButtonAction}
                    />
                </div>
            </div>

            <Modal show={showModal} onHide={closeFormDialog} dialogClassName={"modal-50w"} backdrop="static" >
                {showPopupCrearAtributo ? showFormCrearAtributo() : null}
                {showPopupModificarAtributo ? showFormModificarAtributo() : null}
            </Modal>

            <Modal show={showAtributoValidacionesModal} onHide={closeFormDialogAtributoValidaciones} dialogClassName="modal-90w" backdrop="static" >
                <PAR400AtributoValidacion show={showAtributoValidaciones} onHide={closeFormDialogAtributoValidaciones} codigoAtributo={codigoAtributo} tipoAtributo={tipoAtributo} nmAtributo={NmAtributo}/>
            </Modal>

        </Page>
    );
}