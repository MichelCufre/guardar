import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { Modal } from 'react-bootstrap';
import { PRE052LiberarOndaModal } from './PRE052LiberarOndaModal';
import { PRE052SelecLibOndaModal } from './PRE052SelecLibOndaModal';
import { PRE052CreatePrepLibreModal } from './PRE052CreatePrepLibreModal';
import { PRE052CreatePrepPedidoModal } from './PRE052CreatePrepPedidoModal';
import { PRE052AsociarPedidosModal } from './PRE052AsociarPedidosModal';
import { PRE052CreatePrepAdministrativoModal } from './PRE052CreatePrepAdministrativoModal';
import './PRE052.css';

export default function PRE052(props) {

    const { t } = useTranslation();

    const PermissionsCheck = useState({ Libre: 'PRE052CreatePrepLibre_Page_Access_Allow', Pedido: 'PRE052CreatePrepPedido_Page_Access_Allow', Asociar: 'PRE052AsociarPedidos_Page_Access_Allow', PrepAdm: "PRE052CreatePrepAdm_Page_Access_Allow" });

    const [showModal, setshowModal] = useState(false);

    const [showPopupLibOnda, setShowPopupLibOnda] = useState(false);
    const [showPopupSelectLibOnda, setShowPopupSelectLibOnda] = useState(false);

    const [showPopupCreatePrepLibre, setShowPopupCreatePrepLibre] = useState(false);
    const [showPopupCreatePrepPedido, setShowPopupCreatePrepPedido] = useState(false);
    const [showPopupAsociarPedidos, setShowPopupAsociarPedidos] = useState(false);
    const [showPopupCreatePrepAdm, setShowPopupCreatePrepAdm] = useState(false);

    const [disabledPrepLibre, setDisabledPrepLibre] = useState(false);
    const [disabledPrepPedido, setDisabledPrepPedido] = useState(false);
    const [disabledPrepAdm, setDisabledPrepAdm] = useState(false);
    const [keyPreparacion, setkeyPreparacion] = useState(null);
    const [keyEmpresa, setkeyEmpresa] = useState(null);

    const [infoFormulario, setInfoFormulario] = useState(null);

    const onAfterButtonAction = (data, nexus) => {
        if (data.buttonId === "btnAsociarPedidos") {
            setkeyPreparacion(data.row.cells.find(d => d.column === "NU_PREPARACION").value);
            setkeyEmpresa(data.row.cells.find(d => d.column === "CD_EMPRESA").value);
            //openFormAsociarPedDialog();
            setShowPopupAsociarPedidos(true);
        }
        else if (data.parameters.find(f => f.id === "PrepaFinalizada") != null) {
            nexus.getGrid("PRE052_grid_1").refresh();
        }
    }

    const onAfterPageLoad = (data) => {
        if (data.parameters.length > 0) {
            setDisabledPrepLibre(data.parameters.find(x => x.id === PermissionsCheck[0].Libre).value)
            setDisabledPrepPedido(data.parameters.find(x => x.id === PermissionsCheck[0].Pedido).value)
            setDisabledPrepAdm(data.parameters.find(x => x.id === PermissionsCheck[0].PrepAdm).value)
        }
    }

    const openFormDialog = () => {
        setShowPopupLibOnda(true);
        setshowModal(true);
    }
    const closeFormDialog = (formulario, atras, nexus) => {

        setShowPopupLibOnda(false);
        if (atras) {

            setInfoFormulario(formulario);
            setShowPopupSelectLibOnda(false);
            openFormDialog();

        } else if (formulario) {

            setInfoFormulario(formulario)

            openFormSelectLibOndaDialog();

        } else {
            setShowPopupSelectLibOnda(false);

            setshowModal(false);

            if (nexus)
                nexus.getGrid("PRE052_grid_1").refresh();
        }
    }
    const openFormSelectLibOndaDialog = () => {
        setShowPopupSelectLibOnda(true);
        setshowModal(true);
    }

    const openFormPrepLibreDialog = () => {
        setShowPopupCreatePrepLibre(true);
        setshowModal(true);
    }

    const closeFormPrepLibreDialog = (nexus) => {
        setShowPopupCreatePrepLibre(false);
        setshowModal(false);
        if (nexus)
            nexus.getGrid("PRE052_grid_1").refresh();
    }

    const openFormPrepPedidoDialog = () => {
        setShowPopupCreatePrepPedido(true);
        setshowModal(true);
    }
    const closeFormPrepPedidoDialog = (datos, nexus) => {
        setShowPopupCreatePrepPedido(false);

        if (datos) {
            setkeyPreparacion(datos.preparacion);
            setkeyEmpresa(datos.empresa)
            openFormAsociarPedDialog();
        } else { setshowModal(false); }
    }

    const openFormAsociarPedDialog = () => {
        setShowPopupAsociarPedidos(true);
        setshowModal(true);
    }
    const closeFormAsociarPedDialog = (nexus) => {
        setShowPopupAsociarPedidos(false);
        setshowModal(false);
        if (nexus)
            nexus.getGrid("PRE052_grid_1").refresh();
    }

    const openFormPrepAdmDialog = () => {
        setShowPopupCreatePrepAdm(true);
        setshowModal(true);
    }

    const closeFormPrepAdmDialog = (nexus) => {
        setShowPopupCreatePrepAdm(false);
        setshowModal(false);
        if (nexus)
            nexus.getGrid("PRE052_grid_1").refresh();
    }

    const handleBeforeButtonAction = (context, data, nexus) => {
        const preparacion = data.row.cells.find(d => d.column === "NU_PREPARACION").value;
        data.parameters = [{ id: "preparacion", value: preparacion }];
    }


    const showFormLiberarOnda = () => { return (<PRE052LiberarOndaModal show={showPopupLibOnda} onHide={closeFormDialog} formulario={infoFormulario} />); }
    const showFormSelectLibOnda = () => { return (<PRE052SelecLibOndaModal show={showPopupSelectLibOnda} onHide={closeFormDialog} formulario={infoFormulario} />); }

    return (

        <Page
            title={t("PRE052_Sec0_pageTitle_Titulo")}
            onAfterLoad={onAfterPageLoad}
            {...props}
        >
            <div style={{ textAlign: "center" }}>
                <button id="LiberarOnda" className="btn btn-primary" onClick={openFormDialog}>{t("PRE052_Sec0_btn_LiberacionOnda")}</button>
                &nbsp;
                <button id="PrepPedidos" className="btn btn-primary" disabled={disabledPrepPedido !== "True"} onClick={openFormPrepPedidoDialog}>{t("PRE052_Sec0_btn_CrearPrepManual")}</button>
                &nbsp;
                <button id="PrepLibre" className="btn btn-primary" disabled={disabledPrepLibre !== "True"} onClick={openFormPrepLibreDialog}>{t("PRE052_Sec0_btn_CrearPrepLibre")}</button>
                &nbsp;
                <button id="PrepAdm" className="btn btn-primary" disabled={disabledPrepAdm !== "True"} onClick={openFormPrepAdmDialog}>{t("PRE052CreatePrepAdm_Sec0_btn_PrepTitulo")}</button>
            </div>

            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="PRE052_grid_1" rowsToFetch={30} rowsToDisplay={15} enableExcelExport
                        onAfterButtonAction={onAfterButtonAction}
                        onBeforeButtonAction={handleBeforeButtonAction}
                    />
                </div>
            </div>

            <Modal show={showModal} onHide={closeFormDialog} dialogClassName="modal-90w" backdrop="static" >

                {showPopupLibOnda ? showFormLiberarOnda() : null}
                {showPopupSelectLibOnda ? showFormSelectLibOnda() : null}
            </Modal>
            <PRE052CreatePrepLibreModal show={showPopupCreatePrepLibre} onHide={closeFormPrepLibreDialog} />
            <PRE052CreatePrepPedidoModal show={showPopupCreatePrepPedido} onHide={closeFormPrepPedidoDialog} />
            <PRE052AsociarPedidosModal show={showPopupAsociarPedidos} onHide={closeFormAsociarPedDialog} preparacion={keyPreparacion} empresa={keyEmpresa} />
            <PRE052CreatePrepAdministrativoModal show={showPopupCreatePrepAdm} onHide={closeFormPrepAdmDialog} />
        </Page>
    );
}