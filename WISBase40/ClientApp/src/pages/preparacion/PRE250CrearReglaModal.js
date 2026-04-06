import React, { useState, useRef } from 'react';
import { Modal, Button, Row, Col, Tab, Tabs, Container } from 'react-bootstrap';
import { Form, Field, FieldSelect, StatusMessage, SubmitButton, FieldToggle, FieldDate } from '../../components/FormComponents/Form';
import { CheckboxList } from '../../components/CheckboxList';
import * as Yup from 'yup';
import { useTranslation } from 'react-i18next';
import { withPageContext } from '../../components/WithPageContext';
import { FieldTime } from '../../components/FormComponents/FormFieldTime';

function InternalPRE250CrearReglaModal(props) {

    const { t } = useTranslation();

    const [isCreate, setIsCreate] = useState(false);
    const [isUpdate, setIsUpdate] = useState(false);

    const [infoRegla, setInfoRegla] = useState("");

    const [opcionalHeight, setOpcionalHeight] = useState(87);

    const [itemsListDias, setItemsListDias] = useState([]);
    const [allSelectedItemsListDias, setAllSelectedItemsListDias] = useState(false);

    const validationSchema = {

        descripcion: Yup.string().required().max(200, "Largo maximo 200 caracteres."),
        nuOrden: Yup.string().required(),
        dtInicio: Yup.date().transform(value => (isNaN(value) ? undefined : value)).required(),
        dtFin: Yup.date().transform(value => (isNaN(value) ? undefined : value)).required(),
        horaInicio: Yup.string(),
        horaFin: Yup.string(),
        nuFrecuencia: Yup.string(),
        tpFrecuencia: Yup.string().required(),
        activa: Yup.boolean(),
        respetarIntervalos: Yup.boolean(),
        dias: Yup.string(),
    };

    const initialValues = {

        descripcion: "",
        nuOrden: "",
        dtInicio: "",
        dtFin: "",
        horaInicio: "",
        horaFin: "",
        nuFrecuencia: "",
        tpFrecuencia: "",
        activa: "",
        respetarIntervalos: "",
        dias: "",
    }

    const handleChangeDias = (event, id) => {

        //setItemsListModificar(itemsList);

        let itemsModificar = [...itemsListDias];

        let item = itemsModificar.find(d => d.id === id);

        item.selected = !item.selected;

        setItemsListDias(itemsModificar);
    };

    const handleAfterInitialize = (context, form, query, nexus) => {

        let jsonDias = query.parameters.find(w => w.id === "ListItemsDias").value;
        var arrayDias = JSON.parse(jsonDias.toString());

        setItemsListDias(arrayDias);

        if (props.regla) {
            setIsCreate(false)
            setIsUpdate(true)
            setInfoRegla(props.regla.find(a => a.id === "nuRegla").value);
        }
        else {
            setIsCreate(true)
            setIsUpdate(false)
        }
    };

    const handleBeforeInitialize = (context, form, query, nexus) => {
        let parameters = []

        if (props.regla)
            parameters = [{ id: "nuRegla", value: props.regla.find(a => a.id === "nuRegla").value }];

        if (props.formulario)
            props.formulario.forEach(element =>
                parameters.push(element)
            );

        query.parameters = parameters;
    }

    const handleFormAfterSubmit = (context, form, query, nexus) => {

        context.showErrorMessage = true;

        if (context.responseStatus === "OK") {
            if (query.buttonId == "btnSubmitConfirmar") {
                let parameters =
                    [
                        { id: "descripcion", value: nexus.getForm("PRE250CrearRegla_form_1").getFieldValue("descripcion") },
                        { id: "nuOrden", value: nexus.getForm("PRE250CrearRegla_form_1").getFieldValue("nuOrden") },
                        { id: "dtInicio", value: nexus.getForm("PRE250CrearRegla_form_1").getFieldValue("dtInicio") },
                        { id: "dtFin", value: nexus.getForm("PRE250CrearRegla_form_1").getFieldValue("dtFin") },
                        { id: "horaInicio", value: nexus.getForm("PRE250CrearRegla_form_1").getFieldValue("horaInicio") },
                        { id: "horaFin", value: nexus.getForm("PRE250CrearRegla_form_1").getFieldValue("horaFin") },
                        { id: "nuFrecuencia", value: nexus.getForm("PRE250CrearRegla_form_1").getFieldValue("nuFrecuencia") },
                        { id: "tpFrecuencia", value: nexus.getForm("PRE250CrearRegla_form_1").getFieldValue("tpFrecuencia") },
                        { id: "activa", value: nexus.getForm("PRE250CrearRegla_form_1").getFieldValue("activa") },
                        { id: "respetarIntervalos", value: nexus.getForm("PRE250CrearRegla_form_1").getFieldValue("respetarIntervalos") },
                        { id: "dias", value: JSON.stringify(itemsListDias) }
                    ];

                props.onHide(parameters, "irLiberacion");
            } else {
                props.onHide(null);
            }
        }
    }
    const handleFormBeforeSubmit = (context, form, query, nexus) => {

        if (props.regla)
            query.parameters = [{ id: "nuRegla", value: props.regla.find(a => a.id === "nuRegla").value }];
    }
    const handleSelectAllItemsDias = (selected) => {
        setAllSelectedItemsListDias(selected);

        setItemsListDias(itemsListDias.map(d => ({ ...d, selected: selected })));
    }
    const handleClose = () => {
        props.onHide(null, null, null, props.nexus);
    };

    const onBeforeValidateField = (context, form, query, nexus) => {
        if (props.regla)
            query.parameters = [{ id: "nuRegla", value: props.regla.find(a => a.id === "nuRegla").value }];
    }

    return (

        <Form
            application="PRE250CrearRegla"
            id="PRE250CrearRegla_form_1"
            initialValues={initialValues}
            validationSchema={validationSchema}
            onAfterSubmit={handleFormAfterSubmit}
            onBeforeSubmit={handleFormBeforeSubmit}
            onAfterInitialize={handleAfterInitialize}
            onBeforeInitialize={handleBeforeInitialize}
            onBeforeValidateField={onBeforeValidateField}
        >

            <Modal.Header closeButton>
                <Modal.Title>
                    <Container fluid style={{ display: isCreate ? 'block' : 'none' }} >
                        {t("PRE250_frm1_title_CrearRegla")}
                    </Container>
                    <Container fluid style={{ display: isUpdate ? 'block' : 'none' }} >
                        {t("PRE250_frm1_title_EditarRegla")} {`${infoRegla}`}
                    </Container>
                </Modal.Title>

            </Modal.Header>
            <Modal.Body>
                <Container fluid>
                    <Row>
                        {/* Columna 1*/}
                        <Col>
                            <Row>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="descripcion">{t("PRE250_frm1_lbl_descripcion")}</label>
                                        <Field name="descripcion" />
                                        <StatusMessage for="descripcion" />
                                    </div>
                                </Col>
                            </Row>
                        </Col>
                        {/* Columna 2*/}
                        <Col>
                            <Row>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="nuOrden">{t("PRE250_frm1_lbl_nuOrden")}</label>
                                        <Field name="nuOrden" />
                                        <StatusMessage for="nuOrden" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group" style={{ marginTop: "40px" }}>
                                        <FieldToggle name="activa" label="Activa" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group" style={{ marginTop: "40px" }}>
                                        <FieldToggle name="respetarIntervalos" label="Respetar intervalos" />
                                    </div>
                                </Col>
                            </Row>
                        </Col>
                    </Row>
                    <h5 className="form-title">{t("PRE250_frm1_lbl_legend")}</h5>
                    <Row>
                        {/* Columna 1*/}
                        <Col>

                            <Row>
                                <Col >
                                    <div className="form-group" >
                                        <label htmlFor="dtInicio">{t("PRE250_frm1_lbl_dtInicio")}</label>
                                        <FieldDate name="dtInicio" />
                                        <StatusMessage for="dtInicio" />
                                    </div>
                                </Col>
                                <Col >
                                    <div className="form-group" >
                                        <label htmlFor="dtFin">{t("PRE250_frm1_lbl_dtFin")}</label>
                                        <FieldDate name="dtFin" />
                                        <StatusMessage for="dtFin" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="tpFrecuencia">{t("PRE250_frm1_lbl_tpFrecuencia")}</label>
                                        <FieldSelect name="tpFrecuencia" />
                                        <StatusMessage for="tpFrecuencia" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="nuFrecuencia">{t("PRE250_frm1_lbl_nuFrecuencia")}</label>
                                        <Field name="nuFrecuencia" />
                                        <StatusMessage for="nuFrecuencia" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col md={6}>
                                    <div className="form-group">
                                        <label htmlFor="horaInicio">{t("PRE250_frm1_lbl_horaInicio")}</label>
                                        <FieldTime name="horaInicio" />
                                        <StatusMessage for="horaInicio" />
                                    </div>
                                </Col>
                                <Col md={6}>
                                    <div className="form-group">
                                        <label htmlFor="horaFin">{t("PRE250_frm1_lbl_horaFin")}</label>
                                        <FieldTime name="horaFin" />
                                        <StatusMessage for="horaFin" />
                                    </div>
                                </Col>
                            </Row>

                        </Col>
                        {/* Columna 3*/}
                        <Col>
                            <Row>
                                <Col>
                                    <div className="form-group col-8" >
                                        <label htmlFor="dias">{t("PRE250_frm1_lbl_dias")}</label>
                                        <CheckboxList
                                            items={itemsListDias}
                                            onChange={handleChangeDias}
                                            height={opcionalHeight}
                                            allSelected={allSelectedItemsListDias}
                                            onSelectAllChange={handleSelectAllItemsDias}
                                        />
                                    </div>
                                </Col>
                            </Row>
                        </Col>
                    </Row>

                </Container>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("PRE052_frm1_btn_cerrar")} </Button>
                <SubmitButton id="btnSubmitConfirmar" variant="primary" label="PRE250_frm1_btn_ConfirmarRegla" />
            </Modal.Footer>
        </Form >

    );
}

export const PRE250CrearReglaModal = withPageContext(InternalPRE250CrearReglaModal);