import React, { useState, useEffect } from 'react';
import { Col, FormGroup, Modal, Row, Button } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { Field, FieldDateTime, Form, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';
import { FieldTime } from '../../components/FormComponents/FormFieldTime';
import { Page } from '../../components/Page';

export default function PRE100DefinicionAtributoLpn(props) {
    const { t } = useTranslation();
    const [nexus, setNexus] = useState(null);

    const [idAtributo, setIdAtributo] = useState("");
    const [nombreAtributo, setNombreAtributo] = useState("");
    const [labelInput, setLabelInput] = useState("");

    const [isField, setIsField] = useState(true);
    const [isFieldTime, setIsFieldTime] = useState(true);
    const [isFieldSelect, setIsFieldSelect] = useState(true);
    const [isFieldDateTime, setIsFieldDateTime] = useState(true);

    const showField = !isField ? "hidden" : "";
    const showFieldTime = !isFieldTime ? "hidden" : "";
    const showFieldSelect = !isFieldSelect ? "hidden" : "";
    const showFieldDateTime = !isFieldDateTime ? "hidden" : "";

    const initialValues = {
        inputField: "",
        inputFieldTime: "",
        inputFieldSelect: "",
        inputFieldDateTime: "",
    };

    const validationSchema = {
        inputField: Yup.string(),
        inputFieldTime: Yup.string(),
        inputFieldSelect: Yup.string(),
        inputFieldDateTime: Yup.string()
    };

    useEffect(() => {
        if (nexus && props.listAtributos && nexus.getForm("PRE100DefinicionAtributoLpn_form_1")) {
            nexus.getForm("PRE100DefinicionAtributoLpn_form_1").reset();
        }
    }, [props.listAtributos]);

    const handleFormBeforeInitialize = (context, form, query, nexus) => {
        if (props.listAtributos) {
            query.parameters = [
                { id: "listAtributos", value: props.listAtributos },
                { id: "pedido", value: props.datos.find(d => d.id === "pedido").value },
                { id: "cliente", value: props.datos.find(d => d.id === "cliente").value },
                { id: "empresa", value: props.datos.find(d => d.id === "empresa").value },
                { id: "producto", value: props.datos.find(d => d.id === "producto").value },
                { id: "faixa", value: props.datos.find(d => d.id === "faixa").value },
                { id: "identificador", value: props.datos.find(d => d.id === "identificador").value },
                { id: "idEspecificaIdentificador", value: props.datos.find(d => d.id === "idEspecificaIdentificador").value },
                { id: "tipoLpn", value: props.datos.find(d => d.id === "tipoLpn").value },
                { id: "idExternoLpn", value: props.datos.find(d => d.id === "idExternoLpn").value },
                { id: "idConfiguracion", value: props.datos.find(d => d.column === "idConfiguracion")?.value },
                { id: "cantidad", value: props.datos.find(d => d.column === "cantidad")?.value },
                { id: "update", value: props.datos.find(d => d.id === "update")?.value }
            ];
        }

        setNexus(nexus);
    };

    const handleFormAfterInitialize = (context, form, query, nexus) => {
        setIdAtributo(query.parameters.find(w => w.id === "idAtributo").value);
        setNombreAtributo(query.parameters.find(w => w.id === "nombreAtributo").value);

        setNexus(nexus);

        setLabelInput(query.parameters.find(w => w.id === "labelInput").value)

        setIsField(query.parameters.find(w => w.id === "isField").value === "S");
        setIsFieldTime(query.parameters.find(w => w.id === "isFieldTime").value === "S");
        setIsFieldSelect(query.parameters.find(w => w.id === "isFieldSelect").value === "S");
        setIsFieldDateTime(query.parameters.find(w => w.id === "isFieldDateTime").value === "S");
    }

    const handleFormBeforeSubmit = (context, form, query, nexus) => {
        query.parameters = [
            { id: "listAtributos", value: props.listAtributos },
            { id: "pedido", value: props.datos.find(d => d.id === "pedido").value },
            { id: "cliente", value: props.datos.find(d => d.id === "cliente").value },
            { id: "empresa", value: props.datos.find(d => d.id === "empresa").value },
            { id: "producto", value: props.datos.find(d => d.id === "producto").value },
            { id: "faixa", value: props.datos.find(d => d.id === "faixa").value },
            { id: "identificador", value: props.datos.find(d => d.id === "identificador").value },
            { id: "idEspecificaIdentificador", value: props.datos.find(d => d.id === "idEspecificaIdentificador").value },
            { id: "tipoLpn", value: props.datos.find(d => d.id === "tipoLpn").value },
            { id: "idExternoLpn", value: props.datos.find(d => d.id === "idExternoLpn").value },
            { id: "idConfiguracion", value: props.datos.find(d => d.column === "idConfiguracion")?.value },
            { id: "cantidad", value: props.datos.find(d => d.column === "cantidad")?.value },
            { id: "update", value: props.datos.find(d => d.id === "update")?.value }
        ];
        setNexus(nexus);
    }

    const handleFormAfterSubmit = (context, form, query, nexus) => {
        setNexus(nexus);
        context.showErrorMessage = true;

        if (context.responseStatus === "OK") {
            props.onHide(query.parameters.find(w => w.id === "listAtributos").value);
        }
    }

    const handleBeforeValidate = (context, form, query, nexus) => {
        setNexus(nexus);
        query.parameters = [
            { id: "listAtributos", value: props.listAtributos },
            { id: "pedido", value: props.datos.find(d => d.id === "pedido").value },
            { id: "cliente", value: props.datos.find(d => d.id === "cliente").value },
            { id: "empresa", value: props.datos.find(d => d.id === "empresa").value },
            { id: "producto", value: props.datos.find(d => d.id === "producto").value },
            { id: "faixa", value: props.datos.find(d => d.id === "faixa").value },
            { id: "identificador", value: props.datos.find(d => d.id === "identificador").value },
            { id: "idEspecificaIdentificador", value: props.datos.find(d => d.id === "idEspecificaIdentificador").value },
            { id: "tipoLpn", value: props.datos.find(d => d.id === "tipoLpn").value },
            { id: "idExternoLpn", value: props.datos.find(d => d.id === "idExternoLpn").value },
            { id: "idConfiguracion", value: props.datos.find(d => d.column === "idConfiguracion")?.value },
            { id: "cantidad", value: props.datos.find(d => d.column === "cantidad")?.value },
            { id: "update", value: props.datos.find(d => d.id === "update")?.value }
        ];
    };

    const handleClose = () => {
        props.onHide();
    };

    return (
        <Modal show={props.show} onHide={handleClose} backdrop="static">

            <Page
                {...props}
                application="PRE100DefinicionAtributoLpn"
            /*onBeforeLoad={onBeforeLoad}*/
            >
                <Form
                    application="PRE100DefinicionAtributoLpn"
                    id="PRE100DefinicionAtributoLpn_form_1"
                    initialValues={initialValues}
                    validationSchema={validationSchema}
                    onBeforeInitialize={handleFormBeforeInitialize}
                    onAfterInitialize={handleFormAfterInitialize}
                    onBeforeValidateField={handleBeforeValidate}
                    onBeforeSubmit={handleFormBeforeSubmit}
                    onAfterSubmit={handleFormAfterSubmit}
                >
                    <Modal.Header closeButton>
                        <Modal.Title>{t("PRE100DefinicionAtributo_Sec0_modalTitle_Titulo")}</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        <Row>
                            <Col md={6}>
                                <div className="form-group">
                                    <label htmlFor="idAtributo">{t("PRE100DefinicionAtributo_frm1_lbl_IdAtributo")}</label>
                                    <Field name="idAtributo" value={idAtributo} readOnly />
                                    <StatusMessage for="idAtributo" />
                                </div>
                            </Col>
                            <Col md={6}>
                                <div className="form-group">
                                    <label htmlFor="nombreAtributo">{t("PRE100DefinicionAtributo_frm1_lbl_NombreAtributo")}</label>
                                    <Field name="nombreAtributo" value={nombreAtributo} readOnly />
                                    <StatusMessage for="nombreAtributo" />
                                </div>
                            </Col>
                        </Row>
                        <Row>
                            <Col md={12}>
                                <div className="form-group">

                                    <div className={showField}>
                                        <FormGroup>
                                            <label htmlFor="inputField">{t(labelInput)}</label>
                                            <Field name="inputField" />
                                            <StatusMessage for="inputField" />
                                        </FormGroup>
                                    </div>
                                    <div className={showFieldTime}>
                                        <FormGroup>
                                            <label htmlFor="inputFieldTime">{t(labelInput)}</label>
                                            <FieldTime name="inputFieldTime" />
                                            <StatusMessage for="inputFieldTime" />
                                        </FormGroup>
                                    </div>
                                    <div className={showFieldDateTime}>
                                        <FormGroup>
                                            <label htmlFor="inputFieldDateTime">{t(labelInput)}</label>
                                            <FieldDateTime name="inputFieldDateTime" />
                                            <StatusMessage for="inputFieldDateTime" />
                                        </FormGroup>
                                    </div>
                                    <div className={showFieldSelect}>
                                        <FormGroup>
                                            <label htmlFor="inputFieldSelect">{t(labelInput)}</label>
                                            <Field name="inputFieldSelect" />
                                            <StatusMessage for="inputFieldSelect" />
                                        </FormGroup>
                                    </div>
                                </div>
                            </Col>
                        </Row>
                    </Modal.Body>
                    <Modal.Footer>
                        <Button variant="outline-secondary" onClick={handleClose}>
                            {t("General_Sec0_btn_Cerrar")}
                        </Button>
                        <SubmitButton id="btnConfirmarValorAtributo" variant="primary" label="General_Sec0_btn_Confirmar" />
                    </Modal.Footer>
                </Form>
            </Page>
        </Modal>
    );
}