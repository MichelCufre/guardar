import React, { useState } from 'react';
import { Col, Modal, Row } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { FieldToggle, Form, StatusMessage } from '../../components/FormComponents/Form';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { STO700CrearLPNModal } from './STO700CrearLPNModal';
import { STO700ImpresionLpnModal } from './STO700ImpresionLpnModal';
import { STO700LpnCodigosBarraModal } from './STO700LpnCodigosBarraModal';

export default function STO700(props) {

    const { t } = useTranslation();
    const [rowSeleccionadasImprimir, setRowSeleccionadasImprimir] = useState(null);
    const [showModal, setShowModal] = useState(false);
    const [showPopupCrearLpn, setShowPopupCrearLpn] = useState(false);
    const [showPopupImprimir, setShowPopupImprimir] = useState(false);
    const [showPopupLpnCodigosBarra, setShowPopupLpnCodigosBarra] = useState(false);
    const [numeroLpn, setNumeroLpn] = useState("");
    const [lpnsActivos, setLpnsActivos] = useState(true);

    const initialValues = {
        lpnsActivos: true,
    };

    const validationSchema = {
        lpnsActivos: Yup.string()
    };

    const openFormDialog = () => {
        setShowPopupCrearLpn(true);
        setShowModal(true);
    }

    const openFormLpnCodigosBarras = () => {
        setShowPopupLpnCodigosBarra(true);
        setShowModal(true);
    }

    const closeLpnCodigosBarra = () => {
        setShowPopupLpnCodigosBarra(false);
    }

    const openImprimirDialog = () => {
        setShowPopupImprimir(true);
    }

    const closeImprimirDialog = () => {
        setShowPopupImprimir(false);
    }

    const closeFormDialog = (nexus) => {
        setShowPopupCrearLpn(false);
        setShowModal(false);

        if (nexus) {
            nexus.getGrid("STO700_grid_1").refresh();
        }
    }

    const onAfterMenuItemAction = (context, data, nexus) => {

        let jsonAdded = data.parameters.find(w => w.id === "ListaFilasSeleccionadas").value;

        setRowSeleccionadasImprimir(jsonAdded);

        openImprimirDialog();
    }

    const onBeforeButtonAction = (context, data, nexus) => {
        if (data.buttonId === "btnCodBarras") {
            context.abortServerCall = true;
            setNumeroLpn(data.row.cells.find(w => w.column == "NU_LPN").value);
            openFormLpnCodigosBarras();
        }
    }

    const onAfterButtonAction = (context, data, nexus) => {
        if (data.buttonId === "btnFinalizar") {
            nexus.getGrid("STO700_grid_1").refresh();
        }
    }

    const addParameters = (context, data, nexus) => {

        data.parameters.push({ id: "lpnsActivos", value: lpnsActivos });
    }

    const onBeforeValidateField = (context, form, query, nexus) => {
        var lpnsActivos = nexus.getForm("STO700_form_1").getFieldValue("lpnsActivos");
        setLpnsActivos(lpnsActivos);

        query.parameters.push({ id: "lpnsActivos", value: lpnsActivos });

        nexus.getGrid("STO700_grid_1").refresh();
    };

    return (
        <Page
            title={t("STO700_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <Row>
                <Col sm={5}>
                    <Form id="STO700_form_1"
                        initialValues={initialValues}
                        validationSchema={validationSchema}
                        onBeforeValidateField={onBeforeValidateField}
                    >
                        <div className="form-group">
                            <FieldToggle name="lpnsActivos" label={t("STO700_frm1_lbl_activos")} />
                            <StatusMessage for="lpnsActivos" />
                        </div>
                    </Form>
                </Col>
                <Col sm={7} className="d-flex justify-content-left">
                    <button className="btn btn-primary" style={{ marginLeft: '7%' }} onClick={openFormDialog}>{t("STO700_Sec0_btn_Nuevolpn")}</button>
                </Col>
            </Row>
            <br />
            <div className="row mb-4">
                <div className="col-12">
                    <Grid
                        id="STO700_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableSelection
                        enableExcelExport
                        onBeforeInitialize={addParameters}
                        onBeforeFetch={addParameters}
                        onBeforeFetchStats={addParameters}
                        onBeforeExportExcel={addParameters}
                        onBeforeApplyFilter={addParameters}
                        onBeforeApplySort={addParameters}
                        onBeforeMenuItemAction={addParameters}
                        onAfterMenuItemAction={onAfterMenuItemAction}
                        onBeforeButtonAction={onBeforeButtonAction}
                        onAfterButtonAction={onAfterButtonAction}
                    />
                </div>
            </div>


            <Modal show={showPopupCrearLpn} onHide={closeFormDialog} dialogClassName="modal-50w" backdrop="static" >
                <STO700CrearLPNModal show={showPopupCrearLpn} onHide={closeFormDialog} />
            </Modal>
            <Modal show={showPopupLpnCodigosBarra} onHide={closeLpnCodigosBarra} dialogClassName="modal-50w" backdrop="static" >
                <STO700LpnCodigosBarraModal show={showPopupLpnCodigosBarra} onHide={closeLpnCodigosBarra} numeroLpn={numeroLpn} />
            </Modal>
            <STO700ImpresionLpnModal show={showPopupImprimir} onHide={closeImprimirDialog} rowSeleccionadas={rowSeleccionadasImprimir} hideLoadError={true} />

        </Page>
    );
}