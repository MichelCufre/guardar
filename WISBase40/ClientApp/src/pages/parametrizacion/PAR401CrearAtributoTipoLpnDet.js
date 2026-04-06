import React, { useState } from 'react';
import { Modal, Col, Row, Button, FormGroup } from 'react-bootstrap';
import { StatusMessage, Field, FieldSelectAsync, FieldCheckbox, FieldToggle, SubmitButton, Form, FieldDate, FieldSelect, FieldDateTime} from '../../components/FormComponents/Form';
import * as Yup from 'yup';
import { useTranslation } from 'react-i18next';
import { Page } from '../../components/Page';
import { FieldTime } from '../../components/FormComponents/FormFieldTime';

export function PAR401CrearAtributoTipoLpnDet(props) {
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


    const initialValues = {
        ID_ATRIBUTO: "",
        CD_DOMINIO: "",
        TEXTO: "",
        HORA: "",
        FECHA: "",
        NUMERO: "",
        FL_REQUERIDO: "",
        ID_ESTADO_INICIAL: "",
        VL_VALIDO_INTERFAZ: "",
    };

    const validationSchema = {
        ID_ATRIBUTO: Yup.string().max(10).required(),
        CD_DOMINIO: Yup.string().max(400),
        TEXTO: Yup.string().max(400),
        HORA: Yup.string().max(400),
        FECHA: Yup.string().max(400),
        NUMERO: Yup.string().max(400),
        FL_REQUERIDO: Yup.boolean(),
        VL_VALIDO_INTERFAZ: Yup.boolean(),
        ID_ESTADO_INICIAL: Yup.string(),
    };

    const handleClose = () => {
        props.onHide();
    };

    const handleFormAfterSubmit = (context, form, query, nexus) => {
        if (context.responseStatus === "OK") {
            props.onHide();
        }
    };

    const handleFormAfterValidateField = (context, form, query, nexus) => {
        if (query.fieldId === "ID_ATRIBUTO" && form.fields.find(w => w.id === "ID_ATRIBUTO").value !== "") {
            setisDominio(query.parameters.find(w => w.id === "isDominio").value === "F");
            setIsisTexto(query.parameters.find(w => w.id === "isTexto").value === "F");
            setisHora(query.parameters.find(w => w.id === "isHora").value === "F");
            setisFecha(query.parameters.find(w => w.id === "isFecha").value === "F");
            setisNumerico(query.parameters.find(w => w.id === "isNumerico").value === "F");
        }
    };


    return (
        <Page
            //title={t("REC170_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <Form
                id="PAR401_form_CreateAtributoTipoLpn"
                application="PAR401CreateAtributoTipoLpnDet"
                initialValues={initialValues}
                validationSchema={validationSchema}
                onAfterSubmit={handleFormAfterSubmit}
                onAfterValidateField={handleFormAfterValidateField}
            >

                <Modal.Header>
                    <Modal.Title>{t("PAR401_Sec0_lbl_modalAtributoTitle_Titulo")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Row>
                        <Col md={12}>
                            <div className="form-group">
                                <label htmlFor="ID_ATRIBUTO">{t("PAR401_frm1_lbl_ID_ATRIBUTO")}</label>
                                <FieldSelect name="ID_ATRIBUTO" />
                                <StatusMessage for="ID_ATRIBUTO" />
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
                                        <FieldDate name="FECHA" />
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

            </Form>
        </Page>
    );
}