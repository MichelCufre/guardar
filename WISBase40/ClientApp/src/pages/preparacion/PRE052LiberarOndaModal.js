import React, { useState } from 'react';
import { Button, Col, Container, Modal, Row } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { Field, FieldCheckboxList, FieldSelect, FieldSelectAsync, FieldToggle, Form, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';
import { withPageContext } from '../../components/WithPageContext';

function InternalPRE052LiberarOndaModal(props) {

    const { t } = useTranslation();

    const [showModal, setshowModal] = useState(false);

    const [showPopupAdd, setShowPopupAdd] = useState(false);

    const [showBotonCreate, setShowBotonCreate] = useState(true);

    const [manejoDocumental, setManejoDocumental] = useState(true);

    const [opcionalHeight, setOpcionalHeight] = useState(87);

    const [itemsListCondicion, setItemsListCondicion] = useState([]);
    const [allSelectedItemsListCondicion, setAllSelectedItemsListCondicion] = useState(false);

    const [itemsListTipoDoc, setItemsListTipoDoc] = useState([]);
    const [itemsListDoc, setItemsListDoc] = useState([]);
    const [allSelecteditemsListTipoDoc, setAllSelecteditemsListTipoDoc] = useState(false);
    const [allSelecteditemsListDoc, setAllSelecteditemsListDoc] = useState(false);

    const validationSchema = {

        idEmpresa: Yup.string().required(),
        predio: Yup.string().required(),
        onda: Yup.string(),
        condicionLiberacion: Yup.string(),
        tipoDocumento: Yup.string(),
        documento: Yup.string(),
        //agrupacion: Yup.string(),
        ubicacionCompleta: Yup.string(),
        ubicacionIncompleta: Yup.string(),
        prepSoloCamion: Yup.string(),
        agrupPorCamion: Yup.string(),
        manejaVidaUtil: Yup.string(),
        priorizarDesborde: Yup.string(),
        usarSoloStkPicking: Yup.boolean(),
        stock: Yup.string(),
        pedidos: Yup.string(),
        repartirEscasez: Yup.string(),
        liberarPorUnidades: Yup.string(),
        stockDtmi: Yup.string(),
        respetaFifo: Yup.string(),
        ubicacionPicking2Fases: Yup.string(),
        descripcion: Yup.string().max(60, "Largo máximo 60 caracteres."),
        excluirUbicPicking: Yup.boolean(),
    };

    const initialValues = {

        ubicacionCompleta: "",
        ubicacionIncompleta: "",
        prepSoloCamion: "",
        agrupPorCamion: "",
        manejaVidaUtil: "",
        ubicacionPicking2Fases: "",
        priorizarDesborde: "",
        usarSoloStkPicking: "",
        stock: "",
        pedidos: "",
        repartirEscasez: "",
        liberarPorUnidades: "",
        stockDtmi: "",
        respetaFifo: "",
        idEmpresa: "",
        predio: "",
        onda: "",
        condicionLiberacion: "",
        tipoDocumento: "",
        documento: "",
        agrupacion: "",
        descripcion: "",
        excluirUbicPicking: ""
    }

    const handleChangeCondicion = (event, id) => {

        let itemsModificar = [...itemsListCondicion];

        let item = itemsModificar.find(d => d.id === id);

        item.selected = !item.selected;

        setItemsListCondicion(itemsModificar);
    };

    const handleChangeDoc = (event, id) => {

        let itemsModificar = [...itemsListDoc];

        console.log(itemsModificar);

        let item = itemsModificar.find(d => d.id === id);

        item.selected = !item.selected;

        setItemsListDoc(itemsModificar);
    };

    const handleChangeTipoDoc = (event, id) => {

        let itemsModificar = [...itemsListTipoDoc];

        console.log(itemsModificar);

        let item = itemsModificar.find(d => d.id === id);

        item.selected = !item.selected;

        setItemsListTipoDoc(itemsModificar);
    };

    const handleAfterInitialize = (context, form, query, nexus) => {
        var manejoDoc = query.parameters.find(w => w.id === "ManejoDocumental").value;

        let jsonCondiciones = query.parameters.find(w => w.id === "ListItemsCondicion").value;
        var arrayCondiciones = JSON.parse(jsonCondiciones.toString());

        let jsonTipoDoc = query.parameters.find(w => w.id === "ListItemsTipoDoc").value;
        var arrayTipoDoc = JSON.parse(jsonTipoDoc.toString());

        let jsonDoc = query.parameters.find(w => w.id === "ListItemsDoc").value;
        var arrayDoc = JSON.parse(jsonDoc.toString());

        setManejoDocumental(manejoDoc == "true");
        setItemsListCondicion(arrayCondiciones);
        setItemsListTipoDoc(arrayTipoDoc);
        setItemsListDoc(arrayDoc);
    };

    const handleFormAfterValidateField = (context, form, query, nexus) => {
        var paramManejoDoc = query.parameters.find(w => w.id === "ManejoDocumental");
        var paramTipoDoc = query.parameters.find(w => w.id === "ListItemsTipoDoc");
        var paramDoc = query.parameters.find(w => w.id === "ListItemsDoc");

        if (paramManejoDoc)
            setManejoDocumental(paramManejoDoc.value == "true");

        if (paramTipoDoc) {
            let jsonTipoDoc = paramTipoDoc.value;
            var arrayTipoDoc = JSON.parse(jsonTipoDoc.toString());

            setItemsListTipoDoc(arrayTipoDoc);
        }

        if (paramDoc) {
            let jsonDoc = paramDoc.value;
            var arrayDoc = JSON.parse(jsonDoc.toString());

            setItemsListDoc(arrayDoc);
        }
    };

    const handleBeforeInitialize = (context, form, query, nexus) => {
        if (props.formulario)
            query.parameters = props.formulario;
    }

    const handleFormAfterSubmit = (context, form, query, nexus) => {

        context.showErrorMessage = true;

        if (context.responseStatus === "OK") {
            if (query.buttonId == "btnSubmitConfirmar") {
                let parameters =
                    [
                        { id: "ubicacionCompleta", value: nexus.getForm("PRE052LiberacionOnda_form_1").getFieldValue("ubicacionCompleta") },
                        { id: "ubicacionIncompleta", value: nexus.getForm("PRE052LiberacionOnda_form_1").getFieldValue("ubicacionIncompleta") },
                        { id: "prepSoloCamion", value: nexus.getForm("PRE052LiberacionOnda_form_1").getFieldValue("prepSoloCamion") },
                        { id: "agrupPorCamion", value: nexus.getForm("PRE052LiberacionOnda_form_1").getFieldValue("agrupPorCamion") },
                        { id: "manejaVidaUtil", value: nexus.getForm("PRE052LiberacionOnda_form_1").getFieldValue("manejaVidaUtil") },
                        { id: "ubicacionPicking2Fases", value: nexus.getForm("PRE052LiberacionOnda_form_1").getFieldValue("ubicacionPicking2Fases") },
                        { id: "priorizarDesborde", value: nexus.getForm("PRE052LiberacionOnda_form_1").getFieldValue("priorizarDesborde") },
                        { id: "usarSoloStkPicking", value: nexus.getForm("PRE052LiberacionOnda_form_1").getFieldValue("usarSoloStkPicking") },
                        { id: "stock", value: nexus.getForm("PRE052LiberacionOnda_form_1").getFieldValue("stock") },
                        { id: "pedidos", value: nexus.getForm("PRE052LiberacionOnda_form_1").getFieldValue("pedidos") },
                        { id: "repartirEscasez", value: nexus.getForm("PRE052LiberacionOnda_form_1").getFieldValue("repartirEscasez") },
                        { id: "liberarPorUnidades", value: nexus.getForm("PRE052LiberacionOnda_form_1").getFieldValue("liberarPorUnidades") },
                        { id: "stockDtmi", value: nexus.getForm("PRE052LiberacionOnda_form_1").getFieldValue("stockDtmi") },
                        { id: "respetaFifo", value: nexus.getForm("PRE052LiberacionOnda_form_1").getFieldValue("respetaFifo") },
                        { id: "idEmpresa", value: nexus.getForm("PRE052LiberacionOnda_form_1").getFieldValue("idEmpresa") },
                        { id: "predio", value: nexus.getForm("PRE052LiberacionOnda_form_1").getFieldValue("predio") },
                        { id: "onda", value: nexus.getForm("PRE052LiberacionOnda_form_1").getFieldValue("onda") },
                        { id: "condicionLiberacion", value: nexus.getForm("PRE052LiberacionOnda_form_1").getFieldValue("condicionLiberacion") },
                        { id: "tipoDocumento", value: nexus.getForm("PRE052LiberacionOnda_form_1").getFieldValue("tipoDocumento") },
                        { id: "documento", value: nexus.getForm("PRE052LiberacionOnda_form_1").getFieldValue("documento") },
                        { id: "agrupacion", value: nexus.getForm("PRE052LiberacionOnda_form_1").getFieldValue("agrupacion") },
                        { id: "tiposDocumento", value: JSON.stringify(itemsListTipoDoc) },
                        { id: "documentos", value: JSON.stringify(itemsListDoc) },
                        { id: "condicionesLiberacion", value: JSON.stringify(itemsListCondicion) },
                        { id: "descripcion", value: nexus.getForm("PRE052LiberacionOnda_form_1").getFieldValue("descripcion") },
                        { id: "excluirUbicPicking", value: nexus.getForm("PRE052LiberacionOnda_form_1").getFieldValue("excluirUbicPicking") },

                    ];

                props.onHide(parameters);
            } else {
                props.onHide(null);
            }
        }
    }

    const handleSelectAllItemsCondicion = (selected) => {
        setAllSelectedItemsListCondicion(selected);

        setItemsListCondicion(itemsListCondicion.map(d => ({ ...d, selected: selected })));
    }

    const handleSelectAllItemsDoc = (selected) => {
        setAllSelecteditemsListDoc(selected);
        setItemsListDoc(itemsListDoc.map(d => ({ ...d, selected: selected })));
    }

    const handleSelectAllItemsTipoDoc = (selected) => {
        setAllSelecteditemsListTipoDoc(selected);
        setItemsListTipoDoc(itemsListTipoDoc.map(d => ({ ...d, selected: selected })));
    }

    const handleClose = () => {
        props.onHide(null, null, props.nexus);
    };

    const onBeforeSubmit = (context, form, query, nexus) => {
        query.parameters.push({ id: "isSubmit", value: true });
    }
    return (

        <Form
            application="PRE052LiberacionOnda"
            id="PRE052LiberacionOnda_form_1"
            initialValues={initialValues}
            validationSchema={validationSchema}
            onAfterValidateField={handleFormAfterValidateField}
            onAfterSubmit={handleFormAfterSubmit}
            onAfterInitialize={handleAfterInitialize}
            onBeforeSubmit={onBeforeSubmit}
        >

            <Modal.Header closeButton>
                <Modal.Title>{t("PRE052_Sec0_mdlLibOnda_Titulo")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Container fluid>
                    <Row>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="idEmpresa">{t("PRE052_frm1_lbl_idEmpresa")}</label>
                                <FieldSelectAsync name="idEmpresa" clereable={true} />
                                <StatusMessage for="idEmpresa" />
                            </div>
                            <div className="form-group" >
                                <label htmlFor="onda">{t("PRE052_frm1_lbl_onda")}</label>
                                <FieldSelectAsync name="onda" />
                                <StatusMessage for="onda" />
                            </div>
                        </Col>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="predio">{t("PRE052_frm1_lbl_predio")}</label>
                                <FieldSelect name="predio" />
                                <StatusMessage for="predio" />
                            </div>
                            <div className="form-group" >
                                <label htmlFor="agrupacion">{t("PRE052_frm1_lbl_agrupacion")}</label>
                                <FieldSelect name="agrupacion" />
                                <StatusMessage for="agrupacion" />
                            </div>
                        </Col>
                        <Col style={{ display: manejoDocumental ? 'block' : 'none' }}>
                            <div className="form-group" >
                                <label htmlFor="tipoDocumento">{t("PRE052_frm1_lbl_tipoDocumento")}</label>
                                <FieldCheckboxList name="tipoDocumento" items={itemsListTipoDoc} onChange={handleChangeTipoDoc} height={opcionalHeight}
                                    allSelected={allSelecteditemsListTipoDoc}
                                    onSelectAllChange={handleSelectAllItemsTipoDoc} />
                            </div>
                            <div className="form-group" >
                                <label htmlFor="documento">{t("PRE052_frm1_lbl_documento")}</label>
                                <FieldCheckboxList name="documento" items={itemsListDoc} onChange={handleChangeDoc} height={opcionalHeight}
                                    allSelected={allSelecteditemsListDoc}
                                    onSelectAllChange={handleSelectAllItemsDoc} />
                            </div>
                        </Col>
                    </Row>
                    <hr></hr>
                    <Row>
                        <Col>
                            <Row>
                                {/* Columna 1*/}
                                <Col>
                                    <Row>
                                        <Col>
                                            <div className="form-group" >
                                                <label htmlFor="ubicacionCompleta">{t("PRE052_frm1_lbl_ubicacionCompleta")}</label>
                                                <FieldSelect name="ubicacionCompleta" />
                                                <StatusMessage for="ubicacionCompleta" />
                                            </div>
                                        </Col>
                                        <Col>
                                            <div className="form-group" >
                                                <label htmlFor="ubicacionIncompleta">{t("PRE052_frm1_lbl_ubicacionIncompleta")}</label>
                                                <FieldSelect name="ubicacionIncompleta" />
                                                <StatusMessage for="ubicacionIncompleta" />
                                            </div>
                                        </Col>
                                    </Row>

                                    <Row>
                                        <Col>
                                            <div className="form-group" >
                                                <label htmlFor="prepSoloCamion">{t("PRE052_frm1_lbl_prepSoloCamion")}</label>
                                                <FieldSelect name="prepSoloCamion" />
                                                <StatusMessage for="prepSoloCamion" />
                                            </div>
                                        </Col>
                                        <Col>
                                            <div className="form-group" >
                                                <label htmlFor="agrupPorCamion">{t("PRE052_frm1_lbl_agrupPorCamion")}</label>
                                                <FieldSelect name="agrupPorCamion" />
                                                <StatusMessage for="agrupPorCamion" />
                                            </div>
                                        </Col>
                                    </Row>

                                    <Row>
                                        <Col>
                                            <div className="form-group" >
                                                <label htmlFor="manejaVidaUtil">{t("PRE052_frm1_lbl_manejaVidaUtil")}</label>
                                                <FieldSelect name="manejaVidaUtil" />
                                                <StatusMessage for="manejaVidaUtil" />
                                            </div>
                                        </Col>
                                        <Col>
                                            <div className="form-group" >
                                                <label htmlFor="priorizarDesborde">{t("PRE052_frm1_lbl_priorizarDesborde")}</label>
                                                <FieldSelect name="priorizarDesborde" />
                                                <StatusMessage for="priorizarDesborde" />
                                            </div>
                                        </Col>
                                    </Row>

                                    <Row>
                                        <Col>
                                            <div className="form-group" >
                                                <label htmlFor="descripcion">{t("PRE052_frm1_lbl_Libre_Descripcion")}</label>
                                                <Field name="descripcion" />
                                                <StatusMessage for="descripcion" />
                                            </div>
                                        </Col>
                                    </Row>

                                </Col>
                                {/* Columna 2*/}
                                <Col>

                                    <div className="form-group" >
                                        <label htmlFor="stock">{t("PRE052_frm1_lbl_stock")}</label>
                                        <FieldSelect name="stock" />
                                        <StatusMessage for="stock" />
                                    </div>
                                    <div className="form-group" >
                                        <label htmlFor="pedidos">{t("PRE052_frm1_lbl_pedidos")}</label>
                                        <FieldSelect name="pedidos" />
                                        <StatusMessage for="pedidos" />
                                    </div>
                                    <Row>
                                        <Col>
                                            <div className="form-group" >
                                                <label htmlFor="repartirEscasez">{t("PRE052_frm1_lbl_repartirEscasez")}</label>
                                                <FieldSelect name="repartirEscasez" />
                                                <StatusMessage for="repartirEscasez" />
                                            </div>
                                        </Col>
                                        <Col>
                                            <div className="form-group" >
                                                <label htmlFor="respetaFifo">{t("PRE052_frm1_lbl_respetaFifoLoteAuto")}</label>
                                                <FieldSelect name="respetaFifo" />
                                                <StatusMessage for="respetaFifo" />
                                            </div>
                                        </Col>
                                    </Row>

                                    <Row>
                                        <Col>
                                            <div className="form-group" >
                                                <label htmlFor="ubicacionPicking2Fases">{t("PRE052_frm1_lbl_UbicPicking2Faces")}</label>
                                                <FieldSelect name="ubicacionPicking2Fases" />
                                                <StatusMessage for="ubicacionPicking2Fases" />
                                            </div>
                                        </Col>
                                        <Col>
                                            <div className="form-group" >
                                                <label htmlFor="stockDtmi">{t("PRE052_frm1_lbl_ctrlStockDmti_fauca")}</label>
                                                <FieldSelect name="stockDtmi" />
                                                <StatusMessage for="stockDtmi" />
                                            </div>
                                        </Col>
                                    </Row>
                                </Col>
                                {/* Columna 3*/}
                                <Col>
                                    <Row>
                                        <Col>
                                            <div className="form-group" >
                                                <label htmlFor="liberarPorUnidades">{t("PRE052_frm1_lbl_liberarPorUnidades")}</label>
                                                <FieldSelect name="liberarPorUnidades" />
                                                <StatusMessage for="liberarPorUnidades" />
                                            </div>
                                        </Col>
                                        <Col>
                                            <div className="form-group" >
                                                <label htmlFor="liberarPorCurvas">{t("PRE052_frm1_lbl_liberarPorCurvas")}</label>
                                                <FieldSelect name="liberarPorCurvas" disabled={true} />
                                                <StatusMessage for="liberarPorCurvas" />
                                            </div>
                                        </Col>
                                    </Row>
                                    <div className="form-group" >
                                        <label htmlFor="condicionLiberacion">{t("PRE052_frm1_lbl_condicionLiberacion")}</label>
                                        <FieldCheckboxList
                                            name="condicionLiberacion"
                                            items={itemsListCondicion}
                                            onChange={handleChangeCondicion}
                                            height={opcionalHeight}
                                            allSelected={allSelectedItemsListCondicion}
                                            onSelectAllChange={handleSelectAllItemsCondicion}
                                        />
                                    </div>
                                     <Row>
                                        <Col>
                                            <div className="form-group">
                                                <FieldToggle name="excluirUbicPicking" label={t("PRE052_frm1_lbl_ExcluirUbicPicking")} />
                                            </div>
                                        </Col>
                                        <Col>
                                            <div className="form-group">
                                                <FieldToggle name="usarSoloStkPicking" label={t("PRE052_frm1_lbl_UsarSoloStkPicking")} />
                                            </div>
                                        </Col>
                                    </Row>
                                </Col>
                            </Row>
                        </Col>
                    </Row>
                </Container>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("PRE052_frm1_btn_cerrar")} </Button>
                <SubmitButton id="btnSubmitConfirmar" variant="primary" label="PRE052_frm1_btn_confirmar" />
            </Modal.Footer>
        </Form >

    );
}

export const PRE052LiberarOndaModal = withPageContext(InternalPRE052LiberarOndaModal);