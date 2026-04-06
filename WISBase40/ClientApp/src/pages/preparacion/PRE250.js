import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { Modal } from 'react-bootstrap';
import { PRE250CrearReglaModal } from './PRE250CrearReglaModal';
import { PRE250ConfLiberacionModal } from './PRE250ConfLiberacionModal';
import { PRE250AsignarAgentesModal } from './PRE250AsignarAgentesModal';

export default function PRE250(props) {
    const { t } = useTranslation();

    const [showModal, setshowModal] = useState(false);

    const [showPopupRegla, setShowPopupRegla] = useState(false);
    const [showPopupLiberacion, setShowPopupLiberacion] = useState(false);

    const [showModalAsignarAgentes, setshowModalAsignarAgentes] = useState(false);
    const [showPopupAsignarAgentes, setShowPopupAsignarAgentes] = useState(false);

    const [keyRegla, setkeyRegla] = useState(null);

    const [infoFormulario, setInfoFormulario] = useState(null);


    const openFormDialog = () => {
        setShowPopupRegla(true);
        setshowModal(true);
    }

    const openAsignarAgentes = () => {
        setShowPopupAsignarAgentes(true);
        setshowModalAsignarAgentes(true);
    }

    const closeAsignarAgentes = (nexus) => {

        setkeyRegla(null);
        setshowModalAsignarAgentes(false);
        setShowPopupAsignarAgentes(false);

        if (nexus)
            nexus.getGrid("PRE250_grid_1").refresh();
    }

    const closeFormDialog = (formulario, ir, atras, nexus) => {

        setShowPopupRegla(false);
        if (atras) {

            setInfoFormulario(formulario);
            if (atras == "btnVolverRegla") {
                setShowPopupLiberacion(false);
                openFormDialog();
            }
        } else if (ir) {

            setInfoFormulario(formulario)
            if (ir == "irLiberacion") {
                openFormLiberacionDialog();
            }

        } else {
            setShowPopupLiberacion(false);
            setshowModal(false);

            setInfoFormulario(null)
            setkeyRegla(null);

            if (nexus)
                nexus.getGrid("PRE250_grid_1").refresh();
        }
    }
    const openFormLiberacionDialog = () => {
        setShowPopupLiberacion(true);
        setshowModal(true);
    }

    const onBeforeButtonAction = (context, data, nexus) => {

        if (data.buttonId === "btnEditar") {
            context.abortServerCall = true;
            const nuRegla = data.row.cells.find(d => d.column === "NU_REGLA").value;
            setkeyRegla([{ id: "nuRegla", value: nuRegla }]);
            data.parameters = [{ id: "nuRegla", value: nuRegla }];

            openFormDialog();
        }
        else if (data.buttonId === "btnAsignarAgentes") {

            context.abortServerCall = true;

            const nuRegla = data.row.cells.find(d => d.column === "NU_REGLA").value;
            setkeyRegla([
                { id: "nuRegla", value: nuRegla },
                { id: "empresa", value: data.row.cells.find(d => d.column === "CD_EMPRESA").value },
                { id: "tpAgente", value: data.row.cells.find(d => d.column === "TP_AGENTE").value }
            ]);
            data.parameters = [
                { id: "nuRegla", value: nuRegla },
                { id: "empresa", value: data.row.cells.find(d => d.column === "CD_EMPRESA").value },
                { id: "tpAgente", value: data.row.cells.find(d => d.column === "TP_AGENTE").value },
            ];

            openAsignarAgentes();
        }
    };

    const showFormRegla = () => { return (<PRE250CrearReglaModal show={showPopupRegla} onHide={closeFormDialog} formulario={infoFormulario} regla={keyRegla} />); }
    const showFormLiberacion = () => { return (<PRE250ConfLiberacionModal show={showPopupLiberacion} onHide={closeFormDialog} formulario={infoFormulario} regla={keyRegla} />); }

    const showAsignarAgentes = () => { return (<PRE250AsignarAgentesModal show={showPopupAsignarAgentes} onHide={closeAsignarAgentes} formulario={infoFormulario} regla={keyRegla} />); }

    return (

        <Page
            title={t("PRE250_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div style={{ textAlign: "center" }}>
                <button id="CrearRegla" className="btn btn-primary" onClick={openFormDialog}>{t("PRE250_Sec0_btn_CrearRegla")}</button>
            </div>

            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="PRE250_grid_1" rowsToFetch={30} rowsToDisplay={15} enableExcelExport
                        onBeforeButtonAction={onBeforeButtonAction}
                    />
                </div>
            </div>

            <Modal show={showModal} onHide={closeFormDialog} dialogClassName="modal-90w" backdrop="static" >

                {showPopupRegla ? showFormRegla() : null}
                {showPopupLiberacion ? showFormLiberacion() : null}
            </Modal>
            <Modal show={showModalAsignarAgentes} onHide={closeAsignarAgentes} dialogClassName="modal-90w" backdrop="static" >
                {showPopupAsignarAgentes ? showAsignarAgentes() : null}
            </Modal>
        </Page>
    );
}