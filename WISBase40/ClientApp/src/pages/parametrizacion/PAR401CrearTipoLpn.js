import React, { useState} from 'react';
import { Modal, Col, Row, Button, Tab, Tabs, Container } from 'react-bootstrap';
import { StatusMessage, Field, FieldSelectAsync, FieldCheckbox, FieldToggle, SubmitButton, Form, FieldDate, FieldSelect } from '../../components/FormComponents/Form';
import * as Yup from 'yup';
import { useTranslation } from 'react-i18next';
import { Page } from '../../components/Page';

export function PAR401CrearTipoLpn(props) {

    const { t } = useTranslation("translation", { useSuspense: false });
    const [keyTab, setKeyTab] = useState("infoPrincipal");


    const initialValues = {
        TP_LPN_TIPO: "",
        NM_LPN_TIPO: "",
        DS_LPN_TIPO: "",
        NU_COMPONENTE: "",
        NU_TEMPLATE_ETIQUETA: "",
        FL_PERMITE_CONSOLIDAR: "",
        FL_PERMITE_AGREGAR_LINEAS: "",
        FL_MULTIPRODUCTO: "",
        FL_MULTI_LOTE: "",
        FL_CONTENEDOR_LPN: "",
        FL_PERMITE_GENERAR: "",
        NU_SEQ_LPN: "",
        FL_INGRESO_RECEPCION_ATRIBUTO: "",
        VL_PREFIJO: "",
    };

    const validationSchema = {
        TP_LPN_TIPO: Yup.string().required().max(10),
        NM_LPN_TIPO: Yup.string().required().max(30),
        DS_LPN_TIPO: Yup.string().required().max(400),
        NU_SEQ_LPN: Yup.string().required().max(15),
        NU_COMPONENTE: Yup.string().max(10),
        NU_TEMPLATE_ETIQUETA: Yup.string().required().max(15),
        FL_PERMITE_CONSOLIDAR: Yup.boolean(),
        FL_PERMITE_AGREGAR_LINEAS: Yup.boolean(),
        FL_MULTIPRODUCTO: Yup.boolean(),
        FL_MULTI_LOTE: Yup.boolean(),
        FL_CONTENEDOR_LPN: Yup.boolean(),
        FL_PERMITE_GENERAR: Yup.boolean(),
        FL_INGRESO_RECEPCION_ATRIBUTO: Yup.boolean(),
        VL_PREFIJO: Yup.string().required().max(3), 
      
    };
    const handleClose = () => {
        props.onHide();
    };

    const handleFormAfterSubmit = (context, form, query, nexus) => {
        if (context.responseStatus === "OK") {
            props.onHide();
        }
    };

    const handleChange = (event, id) => {
        setFieldValue(event.target.value.toUpperCase());
    };

    const handleInput= e => {
        e.target.value = ("" + e.target.value).toUpperCase();
    };

    return (
        <Page
            application="PAR401CreateTipoLpn"
            //title={t("REC170_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <Form
                id="PAR401_form_CreateTipoLpn"
                application="PAR401CreateTipoLpn"
                initialValues={initialValues}
                validationSchema={validationSchema}
                onAfterSubmit={handleFormAfterSubmit}

           
            >
                <Modal.Header closeButton>
                    <Modal.Title>{t("PAR401_Sec0_modalTitle_Titulo")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Container fluid>
                        <Tabs defaultActiveKey="infoPrincipal" transition={false} id="noanim-tab-example"
                                activeKey={keyTab}
                                onSelect={(k) => setKeyTab(k)}
                         >

                            <Tab eventKey="infoPrincipal" title={t("PAR401_frm1_tab_infoPrin")}>
                                <br></br>
                                <h5 className='form-title'>{t("PAR401_frm_subT_General")}</h5>
                                <Row>
                                    <Col md={4}>
                                        <div className="form-group">
                                            <label htmlFor="TP_LPN_TIPO">{t("PAR401_frm1_lbl_TP_LPN_TIPO")}</label>
                                            <Field name="TP_LPN_TIPO"  />
                                            <StatusMessage for="TP_LPN_TIPO" />
                                        </div>
                                    </Col>
                                    <Col md={4}>
                                        <div className="form-group">
                                            <label htmlFor="NM_LPN_TIPO">{t("PAR401_frm1_lbl_NM_LPN_TIPO")}</label>
                                            <Field name="NM_LPN_TIPO" />
                                            <StatusMessage for="NM_LPN_TIPO" />
                                            </div>                       
                                    </Col>
                                    <Col md={4}>
                                        <div className="form-group">
                                            <label htmlFor="NU_TEMPLATE_ETIQUETA">{t("PAR401_frm1_lbl_NU_TEMPLATE_ETIQUETA")}</label>
                                            <FieldSelect name="NU_TEMPLATE_ETIQUETA" />
                                            <StatusMessage for="NU_TEMPLATE_ETIQUETA" />
                                        </div>                            
                                    </Col>
                                </Row>
                                <Row>
                                    <Col md={4}>
                                        <div className="form-group">
                                            <label htmlFor="DS_LPN_TIPO">{t("PAR401_frm1_lbl_DS_LPN_TIPO")}</label>
                                            <Field name="DS_LPN_TIPO" />
                                            <StatusMessage for="DS_LPN_TIPO" />
                                        </div>
                                    </Col>
                                    <Col md={4}>
                                        <div className="form-group">
                                            <label htmlFor="NU_SEQ_LPN">{t("PAR401_frm1_lbl_NU_SEQ_LPN")}</label>
                                            <Field name="NU_SEQ_LPN" />
                                            <StatusMessage for="NU_SEQ_LPN" />
                                        </div>
                                    </Col>
                                    <Col md={4}>
                                        <div className="form-group">
                                            <label htmlFor="VL_PREFIJO">{t("PAR401_frm1_lbl_VL_PREFIJO")}</label>
                                            <Field name="VL_PREFIJO" />
                                            <StatusMessage for="VL_PREFIJO" />
                                        </div>
                                    </Col>
                                </Row>
                                <Row>
                                    <Col md={4}>
                                        <div className="form-group">
                                            <FieldToggle name="FL_PERMITE_CONSOLIDAR" label={t("PAR401_frm1_lbl_FL_PERMITE_CONSOLIDAR")} />
                                            <StatusMessage for="FL_PERMITE_CONSOLIDAR" />
                                        </div>
                                    </Col>
                                    <Col md={4}>
                                        <div className="form-group">
                                            <FieldToggle name="FL_PERMITE_AGREGAR_LINEAS" label={t("PAR401_frm1_lbl_FL_PERMITE_AGREGAR_LINEAS")} />
                                            <StatusMessage for="FL_PERMITE_AGREGAR_LINEAS" />
                                        </div>
                                    </Col>
                                    <Col md={4}>
                                        <div className="form-group">
                                            <FieldToggle name="FL_PERMITE_GENERAR" label={t("PAR401_frm1_lbl_FL_PERMITE_GENERAR")} />
                                            <StatusMessage for="FL_PERMITE_GENERAR" />
                                        </div>
                                    </Col>
                                </Row>
                                <Row>
                                   
                                    <Col md={4}>
                                        <div className="form-group">
                                            <FieldToggle name="FL_MULTIPRODUCTO" label={t("PAR401_frm1_lbl_FL_MULTIPRODUCTO")} />
                                            <StatusMessage for="FL_MULTIPRODUCTO" />
                                        </div>
                                    </Col>
                                    <Col md={4}>
                                        <div className="form-group">
                                            <FieldToggle name="FL_MULTI_LOTE" label={t("PAR401_frm1_lbl_FL_MULTI_LOTE")} />
                                            <StatusMessage for="FL_MULTI_LOTE" />
                                        </div>
                                    </Col>
                                    <Col md={4}>
                                        <div className="form-group">
                                            <FieldToggle name="FL_INGRESO_RECEPCION_ATRIBUTO" label={t("PAR401_frm1_lbl_FL_INGRESO_RECEPCION_ATRIBUTO")} />
                                            <StatusMessage for="FL_INGRESO_RECEPCION_ATRIBUTO" />
                                        </div>
                                    </Col>
                                </Row>
                                <Row>
                                    <Col md={4}>
                                        <div className="form-group">
                                            <FieldToggle name="FL_PERMITE_DESTRUIR_ALM" label={t("PAR401_frm1_lbl_FL_PERMITE_DESTRUIR_ALM")} />
                                            <StatusMessage for="FL_PERMITE_DESTRUIR_ALM" />
                                        </div>
                                    </Col>
                                    <Col md={4}>
                                        <div className="form-group">
                                            <FieldToggle name="FL_CONTENEDOR_LPN" label={t("PAR401_frm1_lbl_FL_CONTENEDOR_LPN")} />
                                            <StatusMessage for="FL_CONTENEDOR_LPN" />
                                        </div>
                                    </Col>
                                </Row>
                                <h5 className='form-title'>{t("PAR401_frm_subT_Facturacion")}</h5>
                                <Row>
                                    <Col md={4}>
                                        <div className="form-group">
                                            <label htmlFor="NU_COMPONENTE">{t("PAR401_frm1_lbl_NU_COMPONENTE")}</label>
                                            <Field
                                                onChange={handleChange}
                                                onInput={handleInput}
                                                name="NU_COMPONENTE" />
                                            <StatusMessage for="NU_COMPONENTE" />
                                        </div>
                                    </Col>
                                </Row>
                            </Tab>
                        </Tabs>
                    </Container>
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