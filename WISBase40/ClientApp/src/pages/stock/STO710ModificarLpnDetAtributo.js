import React, { useState } from 'react';
import { Modal, Col, Row, Button, FormGroup } from 'react-bootstrap';
import { StatusMessage, Field, FieldSelectAsync, FieldCheckbox, FieldToggle, SubmitButton, Form, FieldDate, FieldSelect, FieldDateTime } from '../../components/FormComponents/Form';
import * as Yup from 'yup';
import { useTranslation } from 'react-i18next';
import { Page } from '../../components/Page';
import { FieldTime } from '../../components/FormComponents/FormFieldTime';
import { FormTab, FormTabStep } from '../../components/FormComponents/FormTab';
export function STO710ModificarLpnDetAtributo(props) {
    const { t } = useTranslation("translation", { useSuspense: false });

    const [isDominio, setisDominio] = useState(true);
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

        CD_DOMINIO: "",
        TEXTO: "",
        HORA: "",
        FECHA: "",
        NUMERO: "",
    };

    const validationSchema = {

        CD_DOMINIO: Yup.string().max(400),
        TEXTO: Yup.string().max(400),
        HORA: Yup.string().max(400),
        FECHA: Yup.string().max(400),
        NUMERO: Yup.string().max(400),
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
    }

    const addParameters = (context, data, nexus) => {

        let parameters =
            [
                { id: "NuLpn", value: props.NuLpn },
                { id: "LpnTipo", value: props.LpnTipo },
                { id: "IdAtributo", value: props.IdAtributo },
                { id: "VlLpnAtributo", value: props.VlLpnAtributo },

                 { id: "CdProduto", value: props.CdProduto },
                { id: "CdEmpresa", value: props.CdEmpresa },
                { id: "NuIdentificador", value: props.NuIdentificador },
                { id: "IdLpnDet", value: props.IdLpnDet }

            ];
        nexus.parameters = parameters;
    }
    const onBeforeValidateField = (context, form, query, nexus) => {
        query.parameters = [
            { id: "NuLpn", value: props.NuLpn },
            { id: "LpnTipo", value: props.LpnTipo },
            { id: "IdAtributo", value: props.IdAtributo },
            { id: "VlLpnAtributo", value: props.VlLpnAtributo },
            { id: "CdProduto", value: props.CdProduto },
            { id: "CdEmpresa", value: props.CdEmpresa },
            { id: "NuIdentificador", value: props.NuIdentificador },
            { id: "IdLpnDet", value: props.IdLpnDet }
        ];
    }
    return (
        <Page
            //title={t("REC170_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <Form
                id="STO710_form_ModificarAtributoLpn"
                application="STO710ModificarLpnDetAtributo"
                initialValues={initialValues}
                validationSchema={validationSchema}
                onAfterSubmit={handleFormAfterSubmit}
                onBeforeSubmit={addParameters}
                onBeforeInitialize={addParameters}
                onAfterInitialize={onAfterInitialize}
                onBeforeValidateField={onBeforeValidateField}
            >

                <Modal.Header>
                    <Modal.Title>{t("STO710_Sec0_lbl_modalAtributoTitle_Titulo")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Row>
                        <Col md={12}>
                            <div className="form-group">
                                <label htmlFor="DS_ATRIBUTO">{t("STO710_frm1_lbl_ID_ATRIBUTO")}</label>
                                <Field name="DS_ATRIBUTO" readOnly />
                                <StatusMessage for="DS_ATRIBUTO" />
                            </div>

                        </Col>
                    </Row>
                    <Row>
                        <Col md={12}>
                            <div className="form-group">
                                <label htmlFor="DS_ATRIBUTO_TIPO">{t("STO710_frm1_lbl_DS_ATRIBUTO_TIPO")}</label>
                                <Field name="DS_ATRIBUTO_TIPO" readOnly />
                                <StatusMessage for="DS_ATRIBUTO_TIPO" />
                            </div>

                        </Col>
                    </Row>
                    <Row>
                        <Col md={12}>
                            <div className="form-group">
                                <div className={formShowDominio}>

                                    <FormGroup>

                                        <label htmlFor="CD_DOMINIO">{t("STO710_frm1_lbl_CD_DOMINIO")}</label>
                                        <FieldSelect name="CD_DOMINIO" />
                                        <StatusMessage for="CD_DOMINIO" />

                                    </FormGroup>

                                </div>

                                <div className={formShowTexto}>

                                    <FormGroup>
                                        <label htmlFor="TEXTO">{t("STO710_frm1_lbl_VL_LPN_ATRIBUTO")}</label>
                                        <Field name="TEXTO" />
                                        <StatusMessage for="TEXTO" />
                                    </FormGroup>

                                </div>
                                <div className={formShowHora}>

                                    <FormGroup>
                                        <label htmlFor="HORA">{t("STO710_frm1_lbl_VL_LPN_ATRIBUTO")}</label>
                                        <FieldTime name="HORA" />
                                        <StatusMessage for="HORA" />
                                    </FormGroup>

                                </div>
                                <div className={formShowFecha}>

                                    <FormGroup>
                                        <label htmlFor="FECHA">{t("STO710_frm1_lbl_VL_LPN_ATRIBUTO")}</label>
                                        <FieldDateTime name="FECHA" />
                                        <StatusMessage for="FECHA" />
                                    </FormGroup>

                                </div>
                                <div className={formShowNumerico}>

                                    <FormGroup>
                                        <label htmlFor="NUMERO">{t("STO710_frm1_lbl_VL_LPN_ATRIBUTO")}</label>
                                        <Field name="NUMERO" />
                                        <StatusMessage for="NUMERO" />
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
                    <SubmitButton id="btnSubmitCreateTipoLpn" variant="primary" label="General_Sec0_btn_Confirmar" />
                </Modal.Footer>
            </Form>
        </Page>
    );
}