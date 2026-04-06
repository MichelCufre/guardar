import React, { useState } from 'react';
import { Modal, Col, Row, Button, FormGroup } from 'react-bootstrap';
import { StatusMessage, Field, FieldSelectAsync, FieldCheckbox, FieldToggle, SubmitButton, Form, FieldDate, FieldSelect, FieldDateTime } from '../../components/FormComponents/Form';
import * as Yup from 'yup';
import { useTranslation } from 'react-i18next';
import { Page } from '../../components/Page';
import { FieldTime } from '../../components/FormComponents/FormFieldTime';
import { FormTab, FormTabStep } from '../../components/FormComponents/FormTab';

export function PAR401ModificarAtributoTipoLpn(props) {
    const { t } = useTranslation("translation", { useSuspense: false });

    const [isDominio, setisDominio] = useState(true);
    const [isSistema, setisSistema] = useState(true);
    const [isTexto, setIsisTexto] = useState(true);
    const [isHora, setisHora] = useState(true);
    const [isFecha, setisFecha] = useState(true);
    const [isNumerico, setisNumerico] = useState(true);
    const formShowDominio = isDominio ? "hidden" : "";
    const formShowTexto = isTexto ? "hidden" : "";
    const formShowHora = isHora ? "hidden" : "";
    const formShowFecha = isFecha ? "hidden" : "";
    const formShowNumerico = isNumerico ? "hidden" : "";
    const [isRequiredConsolidado, setisRequiredConsolidado] = useState(true);
    const formShowConsolidado = isRequiredConsolidado ? "hidden" : "";

    const initialValues = {

        CD_DOMINIO: "",
        TEXTO: "",
        HORA: "",
        FECHA: "",
        NUMERO: "",
        FL_REQUERIDO: "",
        ID_ESTADO_INICIAL: "",
        VL_VALIDO_INTERFAZ: "",
        ID_CONSOLIDACION_TIPO: "",
    };

    const validationSchema = {

        CD_DOMINIO: Yup.string().max(400),
        TEXTO: Yup.string().max(400),
        HORA: Yup.string().max(400),
        FECHA: Yup.string().max(400),
        NUMERO: Yup.string().max(400),
        FL_REQUERIDO: Yup.boolean(),
        VL_VALIDO_INTERFAZ: Yup.boolean(),
        ID_ESTADO_INICIAL: Yup.string(),
        ID_CONSOLIDACION_TIPO: Yup.string().max(400),
    };

    const handleClose = () => {
        props.onHide();
    };

    const handleFormAfterSubmit = (context, form, query, nexus) => {
        if (context.responseStatus === "OK") {
            props.onHide();
        }
    };

    const onAfterInitialize = (context, form, query, nexus) => {

        setisDominio(query.parameters.find(w => w.id === "isDominio").value === "F");
        setIsisTexto(query.parameters.find(w => w.id === "isTexto").value === "F");
        setisHora(query.parameters.find(w => w.id === "isHora").value === "F");
        setisFecha(query.parameters.find(w => w.id === "isFecha").value === "F");
        setisNumerico(query.parameters.find(w => w.id === "isNumerico").value === "F");
        setisRequiredConsolidado(query.parameters.find(w => w.id === "isRequiredConsolidado").value === "F");
    }

    const addParameters = (context, data, nexus) => {
        let parameters =
            [
                { id: "LpnTipo", value: props.LpnTipo },
                { id: "IdAtributo", value: props.IdAtributo },
            ];
        nexus.parameters = parameters;
    }

    return (
        <Page
            //title={t("REC170_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <Form
                id="PAR401_form_CreateAtributoTipoLpn"
                application="PAR401ModificarAtributoTipoLpn"
                initialValues={initialValues}
                validationSchema={validationSchema}
                onAfterSubmit={handleFormAfterSubmit}
                onBeforeInitialize={addParameters}
                onAfterInitialize={onAfterInitialize}
            >

                <Modal.Header>
                    <Modal.Title>{t("PAR401_Sec0_lbl_modalAtributoTitle_Titulo")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Row>
                        <Col md={12}>
                            <div className="form-group">
                                <label htmlFor="DS_ATRIBUTO">{t("PAR401_frm1_lbl_ID_ATRIBUTO")}</label>
                                <Field name="DS_ATRIBUTO" readOnly />
                                <StatusMessage for="DS_ATRIBUTO" />
                            </div>

                        </Col>
                    </Row>
                    <Row>
                        <Col md={12}>
                            <div className="form-group">
                                <label htmlFor="DS_ATRIBUTO_TIPO">{t("PAR401_frm1_lbl_DS_ATRIBUTO_TIPO")}</label>
                                <Field name="DS_ATRIBUTO_TIPO" readOnly />
                                <StatusMessage for="DS_ATRIBUTO_TIPO" />
                            </div>

                        </Col>
                    </Row>

                    <Row>
                        <Col>
                            <div className="form-group">
                                <label htmlFor="ID_ESTADO_INICIAL">{t("PAR401_frm1_lbl_ID_ESTADO_INICIAL")}</label>
                                <FieldSelect name="ID_ESTADO_INICIAL" />
                                <StatusMessage for="ID_ESTADO_INICIAL" />
                            </div>
                        </Col>
                    </Row>

                    <Row>
                        <Col md={12}>
                            <div className="form-group">
                                <div className={formShowDominio}>

                                    <FormGroup>

                                        <label htmlFor="CD_DOMINIO">{t("PAR401_frm1_lbl_CD_DOMINIO")}</label>
                                        <FieldSelect name="CD_DOMINIO" />
                                        <StatusMessage for="CD_DOMINIO" />

                                    </FormGroup>

                                </div>

                                <div className={formShowTexto}>

                                    <FormGroup>
                                        <label htmlFor="TEXTO">{t("PAR401_frm1_lbl_VL_DEFECTO")}</label>
                                        <Field name="TEXTO" />
                                        <StatusMessage for="TEXTO" />
                                    </FormGroup>

                                </div>
                                <div className={formShowHora}>

                                    <FormGroup>
                                        <label htmlFor="HORA">{t("PAR401_frm1_lbl_VL_DEFECTO")}</label>
                                        <FieldTime name="HORA" />
                                        <StatusMessage for="HORA" />
                                    </FormGroup>

                                </div>
                                <div className={formShowFecha}>

                                    <FormGroup>
                                        <label htmlFor="FECHA">{t("PAR401_frm1_lbl_VL_DEFECTO")}</label>
                                        <FieldDateTime name="FECHA" />
                                        <StatusMessage for="FECHA" />
                                    </FormGroup>

                                </div>
                                <div className={formShowNumerico}>

                                    <FormGroup>
                                        <label htmlFor="NUMERO">{t("PAR401_frm1_lbl_VL_DEFECTO")}</label>
                                        <Field name="NUMERO" />
                                        <StatusMessage for="NUMERO" />
                                    </FormGroup>

                                </div>
                            </div>
                        </Col>
                    </Row>

                    <div className={formShowConsolidado}>
                        <Row>
                            < Col md={12}>
                                <label htmlFor="ID_CONSOLIDACION_TIPO">{t("PAR401_frm1_lbl_ID_CONSOLIDACION_TIPO")}</label>
                                <FieldSelect name="ID_CONSOLIDACION_TIPO" />
                                <StatusMessage for="ID_CONSOLIDACION_TIPO" />
                            </Col>
                        </Row>
                        <br />
                    </div>

                    <Row>
                        <Col>
                            <div className="form-group">
                                <FieldToggle name="FL_REQUERIDO" label={t("PAR401_frm1_lbl_FL_REQUERIDO")} />
                                <StatusMessage for="FL_REQUERIDO" />
                            </div>
                        </Col>
                        <Col>
                            <div className="form-group">
                                <FieldToggle name="VL_VALIDO_INTERFAZ" label={t("PAR401_frm1_lbl_VL_VALIDO_INTERFAZ")} />
                                <StatusMessage for="VL_VALIDO_INTERFAZ" />
                            </div>
                        </Col>
                    </Row>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="outline-secondary" onClick={handleClose}>
                        {t("General_Sec0_btn_Cerrar")}
                    </Button>
                    <SubmitButton id="btnSubmitCreateTipoLpn" variant="primary" label="General_Sec0_btn_Confirmar" />
                </Modal.Footer>
                <Field name="ID_ATRIBUTO" hidden />
            </Form>
        </Page>
    );
}