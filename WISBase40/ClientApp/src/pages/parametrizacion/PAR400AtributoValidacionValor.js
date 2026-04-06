import React, { useState, useEffect } from 'react';
import { Col, Container, FormGroup, Modal, Row } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { Field, FieldDate, Form, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';
import { FieldTime } from '../../components/FormComponents/FormFieldTime';
import { Page } from '../../components/Page';
export function PAR400AtributoValidacionValor(props) {
    const { t } = useTranslation("translation", { useSuspense: false });

    const [isTexto, setIsTexto] = useState(true);
    const [isHora, setIsHora] = useState(true);
    const [isFecha, setIsFecha] = useState(true);
    const [isNumerico, setIsNumerico] = useState(true);
    const [isURL, setIsURL] = useState(true);
    const [isDecimal, setIsDecimal] = useState(true);
    const [nexus, setNexus] = useState(null);
    const [labeltxt, setLabeltxt] = useState("");
    const [idValidacion, setIdValidacion] = useState("");
    const [nmValidacion, setNmValidacion] = useState("");

    const formShowTexto = isTexto ? "hidden" : "";
    const formShowHora = isHora ? "hidden" : "";
    const formShowFecha = isFecha ? "hidden" : "";
    const formShowNumerico = isNumerico ? "hidden" : "";
    const formShowDecimal = isDecimal ? "hidden" : "";
    const formShowURL = isURL ? "hidden" : "";

    useEffect(() => {
        if (nexus && props.listValidaciones) {
            nexus.getForm("PAR400AtributoValidacionValor_form_1").reset();
        }
    }, [props.listValidaciones]);

    const initialValues = {
        URL: "",
        TEXTO: "",
        HORA: "",
        FECHA: "",
        NUMERO: "",
        DECIMAL: "",
    };

    const validationSchema = {

        URL: Yup.string().max(1000),
        TEXTO: Yup.string().max(1000),
        HORA: Yup.string().max(1000),
        FECHA: Yup.string().max(1000),
        NUMERO: Yup.string().max(1000),
        DECIMAL: Yup.string().max(1000),
    };

    const handleFormAfterInitialize = (context, form, query, nexus) => {
        setIdValidacion(query.parameters.find(w => w.id === "idValidacion").value);
        setNmValidacion(query.parameters.find(w => w.id === "nmValidacion").value);

        setNexus(nexus);

        setIsURL(query.parameters.find(w => w.id === "isURL").value === "F");
        setIsTexto(query.parameters.find(w => w.id === "isTexto").value === "F");
        setIsHora(query.parameters.find(w => w.id === "isHora").value === "F");
        setIsFecha(query.parameters.find(w => w.id === "isFecha").value === "F");
        setIsNumerico(query.parameters.find(w => w.id === "isNumerico").value === "F");
        setIsDecimal(query.parameters.find(w => w.id === "isDecimal").value === "F");
        setLabeltxt(query.parameters.find(w => w.id === "labeltxt").value)
    }

    const handleFormBeforeInitialize = (context, form, query, nexus) => {
        setNexus(nexus);
        if (props.listValidaciones) {
            query.parameters = [
                { id: "listValidaciones", value: props.listValidaciones },
                { id: "tipoAtributo", value: props.tipoAtributo },
                { id: "nmAtributo", value: props.nmAtributo },
                { id: "codigoAtributo", value: props.codigoAtributo }
            ];
        }
    };

    const handleFormAfterSubmit = (context, form, query, nexus) => {
        setNexus(nexus);
        context.showErrorMessage = true;

        if (context.responseStatus === "OK") {
            props.onHide(query.parameters.find(w => w.id === "ListValidacion").value);
        }
    }

    const handleFormBeforeSubmit = (context, form, query, nexus) => {
        setNexus(nexus);
        query.parameters = [
            { id: "listValidaciones", value: props.listValidaciones },
            { id: "tipoAtributo", value: props.tipoAtributo },
            { id: "nmAtributo", value: props.nmAtributo },
            { id: "codigoAtributo", value: props.codigoAtributo }
        ];
    }

    const handleBeforeValidate = (context, form, query, nexus) => {
        setNexus(nexus);
        query.parameters = [
            { id: "listValidaciones", value: props.listValidaciones },
            { id: "tipoAtributo", value: props.tipoAtributo },
            { id: "nmAtributo", value: props.nmAtributo },
            { id: "codigoAtributo", value: props.codigoAtributo }
        ];
    };

    return (

        <Page
            {...props}
            application="PAR400AtributoValidacionValor"
        >
            <Form
                application="PAR400AtributoValidacionValor"
                id="PAR400AtributoValidacionValor_form_1"
                onBeforeInitialize={handleFormBeforeInitialize}
                onAfterInitialize={handleFormAfterInitialize}
                onAfterSubmit={handleFormAfterSubmit}
                onBeforeSubmit={handleFormBeforeSubmit}
                initialValues={initialValues}
                validationSchema={validationSchema}
                onBeforeValidateField={handleBeforeValidate}
            >
                <Container fluid>
                    <Modal.Header closeButton>
                        <Modal.Title>{t("PAR400Validacion_Sec0_modalTitle_Titulo")}</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        <Row>
                            <Col md={6}>
                                <div className="form-group">
                                    <label htmlFor="ID_VALIDACION">{t("PAR401_frm1_lbl_ID_VALIDACION")}</label>
                                    <Field name="ID_VALIDACION" value={idValidacion} readOnly />
                                    <StatusMessage for="ID_VALIDACION" />
                                </div>
                            </Col>
                            <Col md={6}>
                                <div className="form-group">
                                    <label htmlFor="NM_VALIDACION">{t("PAR401_frm1_lbl_NM_VALIDACION")}</label>
                                    <Field name="NM_VALIDACION" value={nmValidacion} readOnly />
                                    <StatusMessage for="NM_VALIDACION" />
                                </div>
                            </Col>
                        </Row>

                        <Row>
                            <Col md={12}>
                                <div className="form-group">

                                    <div className={formShowURL}>

                                        <FormGroup>
                                            <label htmlFor="URL">{t(labeltxt)}</label>
                                            <Field name="URL" />
                                            <StatusMessage for="URL" />
                                        </FormGroup>

                                    </div>
                                    <div className={formShowTexto}>

                                        <FormGroup>
                                            <label htmlFor="TEXTO">{t(labeltxt)}</label>
                                            <Field name="TEXTO" />
                                            <StatusMessage for="TEXTO" />
                                        </FormGroup>

                                    </div>
                                    <div className={formShowHora}>

                                        <FormGroup>
                                            <label htmlFor="HORA">{t(labeltxt)}</label>
                                            <FieldTime name="HORA" />
                                            <StatusMessage for="HORA" />
                                        </FormGroup>

                                    </div>
                                    <div className={formShowFecha}>

                                        <FormGroup>
                                            <label htmlFor="FECHA">{t(labeltxt)}</label>
                                            <FieldDate name="FECHA" />
                                            <StatusMessage for="FECHA" />
                                        </FormGroup>

                                    </div>
                                    <div className={formShowNumerico}>

                                        <FormGroup>
                                            <label htmlFor="NUMERO">{t(labeltxt)}</label>
                                            <Field name="NUMERO" />
                                            <StatusMessage for="NUMERO" />
                                        </FormGroup>

                                    </div>
                                    <div className={formShowDecimal}>

                                        <FormGroup>
                                            <label htmlFor="DECIMAL">{t(labeltxt)}</label>
                                            <Field name="DECIMAL" />
                                            <StatusMessage for="DECIMAL" />
                                        </FormGroup>

                                    </div>
                                </div>
                            </Col>
                        </Row>
                    </Modal.Body>
                    <Modal.Footer>
                        <SubmitButton id="btnSubmitConfirmar" variant="primary" label="FAC251_frm1_btn_confirmar" />
                    </Modal.Footer>
                </Container>
            </Form>
        </Page>
    );
}