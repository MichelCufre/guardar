import React from 'react';
import { Button, Col, Modal, Row } from 'react-bootstrap';
import * as Yup from 'yup';
import { Field, FieldNumber, FieldSelect, FieldToggle, Form, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';
import { FormWarningMessage } from '../../components/FormComponents/FormWarningMessage';
import { useCustomTranslation } from '../../components/TranslationHook';
import { withPageContext } from '../../components/WithPageContext';

function InternalPRE811CreatePreferenciaModal(props) {
    const { t } = useCustomTranslation();

    const initialValues = {
        dsPreferencia: "",
        predio: "",
        bloqueMin: "",
        bloqueMax: "",
        calleMin: "",
        calleMax: "",
        columnaMin: "",
        columnaMax: "",
        alturaMin: "",
        alturaMax: "",
        pesoMax: "",
        volumenMax: "",
        clientesSimultaneos: "",
        pedidosSimultaneos: "",
        maxPickeos: "",
        maxUnidades: "",
        habilitarControlAcceso: "",
        habilitarEmpresa: "",
        habilitarCliente: "",
        habilitarRuta: "",
        habilitarZona: "",
        habilitarLiberacion: "",
        habilitarTpPedido: "",
        habilitarTpExpedicion: "",
        habilitarClase: "",
        habilitarFamilia: "",
        habilitarLiberadoCompleto: "",
        habilitarPedCompleto: "",
    };

    const validationSchema = {
        dsPreferencia: Yup.string(),
        predio: Yup.string().required(),
        bloqueMin: Yup.string(),
        bloqueMax: Yup.string(),
        calleMin: Yup.string(),
        calleMax: Yup.string(),
        columnaMin: Yup.string(),
        columnaMax: Yup.string(),
        alturaMin: Yup.string(),
        alturaMax: Yup.string(),
        pesoMax: Yup.string(),
        volumenMax: Yup.string(),
        clientesSimultaneos: Yup.string(),
        pedidosSimultaneos: Yup.string(),
        maxPickeos: Yup.string(),
        maxUnidades: Yup.string(),
        habilitarControlAcceso: Yup.string(),
        habilitarEmpresa: Yup.string(),
        habilitarCliente: Yup.string(),
        habilitarRuta: Yup.string(),
        habilitarZona: Yup.string(),
        habilitarLiberacion: Yup.string(),
        habilitarTpPedido: Yup.string(),
        habilitarTpExpedicion: Yup.string(),
        habilitarClase: Yup.string(),
        habilitarFamilia: Yup.string(),
        habilitarLiberadoCompleto: Yup.string(),
        habilitarPedCompleto: Yup.string(),
    };

    const handleClose = () => {
        props.onHide();
    };

    const handleFormBeforeSubmit = (context, form, query, nexus) => {
        query.parameters = [
            { id: "isSubmit", value: true }
        ];
    }

    const handleFormAfterSubmit = (context, form, query, nexus) => {
        if (context.responseStatus === "OK") {

            if (query.buttonId == "btnSubmitConfiguracion") {
                props.onHide(query.parameters.find(a => a.id === "nuPreferencia").value);
            } else {
                nexus.getGrid("PRE811_grid_1").refresh();
                props.onHide(null);
            }
        }
    }

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-70w" backdrop="static">
            <Form
                application="PRE811CreatePreferencia"
                id="PRE811_form_1"
                initialValues={initialValues}
                validationSchema={validationSchema}
                onBeforeSubmit={handleFormBeforeSubmit}
                onAfterSubmit={handleFormAfterSubmit}
            >
                <Modal.Header closeButton>
                    <Modal.Title>{t("PRE811_Sec0_mdlCreate_Titulo")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Row>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="predio">{t("PRE811_frm1_lbl_predio")}</label>
                                <FieldSelect name="predio" />
                                <StatusMessage for="predio" />
                            </div>
                        </Col>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="dsPreferencia">{t("PRE811_frm1_lbl_dsPreferencia")}</label>
                                <Field name="dsPreferencia" />
                                <StatusMessage for="dsPreferencia" />
                            </div>
                        </Col>
                    </Row>
                    <br />
                    <Row>
                        <Col className="col-4">
                            <Row>
                                <div className="form-group" >
                                    <label><h5>{t("PRE811_mdlCreate_Titulo_Ubic")}</h5></label>
                                </div>
                            </Row>
                            <Row>
                                <Col className="col-6">
                                    <div className="form-group" >
                                        <label htmlFor="bloqueMin">{t("PRE811_frm1_lbl_bloqueMin")}</label>
                                        <Field name="bloqueMin" />
                                        <StatusMessage for="bloqueMin" />
                                    </div>
                                </Col>
                                <Col className="col-6">
                                    <div className="form-group" >
                                        <label htmlFor="bloqueMax">{t("PRE811_frm1_lbl_bloqueMax")}</label>
                                        <Field name="bloqueMax" />
                                        <StatusMessage for="bloqueMax" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col className="col-6">
                                    <div className="form-group" >
                                        <label htmlFor="calleMin">{t("PRE811_frm1_lbl_calleMin")}</label>
                                        <Field name="calleMin" />
                                        <StatusMessage for="calleMin" />
                                    </div>
                                </Col>
                                <Col className="col-6">
                                    <div className="form-group" >
                                        <label htmlFor="calleMax">{t("PRE811_frm1_lbl_calleMax")}</label>
                                        <Field name="calleMax" />
                                        <StatusMessage for="calleMax" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col className="col-6">
                                    <div className="form-group" >
                                        <label htmlFor="columnaMin">{t("PRE811_frm1_lbl_columnaMin")}</label>
                                        <FieldNumber name="columnaMin" />
                                        <StatusMessage for="columnaMin" />
                                    </div>
                                </Col>
                                <Col className="col-6">
                                    <div className="form-group" >
                                        <label htmlFor="columnaMax">{t("PRE811_frm1_lbl_columnaMax")}</label>
                                        <FieldNumber name="columnaMax" />
                                        <StatusMessage for="columnaMax" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col className="col-6">
                                    <div className="form-group" >
                                        <label htmlFor="alturaMin">{t("PRE811_frm1_lbl_alturaMin")}</label>
                                        <FieldNumber name="alturaMin" />
                                        <StatusMessage for="alturaMin" />
                                    </div>
                                </Col>
                                <Col className="col-6">
                                    <div className="form-group" >
                                        <label htmlFor="alturaMax">{t("PRE811_frm1_lbl_alturaMax")}</label>
                                        <FieldNumber name="alturaMax" />
                                        <StatusMessage for="alturaMax" />
                                    </div>
                                </Col>
                            </Row>
                        </Col>
                        <Col className="col-8">
                            <Row>
                                <Col>
                                    <div className="form-group">
                                        <label><h5>{t("PRE811_mdlCreate_Titulo_Restric")}</h5></label>
                                    </div>
                                </Col>
                            </Row>
                            <Row className="mb-1">
                                <Col>
                                    <div className="form-group">
                                        <div className="alert alert-warning" align="center">{t("PRE811_frm1_lbl_VL_MAX_PERMITIDO")}</div>
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col className="col-6">
                                    <div className="form-group">
                                        <label htmlFor="pesoMax">{t("PRE811_frm1_lbl_pesoMax")}</label>
                                        <Field name="pesoMax" />
                                        <StatusMessage for="pesoMax" />
                                    </div>
                                </Col>
                                <Col className="col-6">
                                    <div className="form-group">
                                        <label htmlFor="volumenMax">{t("PRE811_frm1_lbl_volumenMax")}</label>
                                        <Field name="volumenMax" />
                                        <StatusMessage for="volumenMax" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col className="col-6">
                                    <div className="form-group">
                                        <label htmlFor="clientesSimultaneos">{t("PRE811_frm1_lbl_clientesSimultaneos")}</label>
                                        <FieldNumber name="clientesSimultaneos" />
                                        <StatusMessage for="clientesSimultaneos" />
                                    </div>
                                </Col>
                                <Col className="col-6">
                                    <div className="form-group">
                                        <label htmlFor="pedidosSimultaneos">{t("PRE811_frm1_lbl_pedidosSimultaneos")}</label>
                                        <FieldNumber name="pedidosSimultaneos" />
                                        <StatusMessage for="pedidosSimultaneos" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col className="col-6">
                                    <div className="form-group">
                                        <label htmlFor="maxPickeos">{t("PRE811_frm1_lbl_maxPickeos")}</label>
                                        <FieldNumber name="maxPickeos" />
                                        <StatusMessage for="maxPickeos" />
                                    </div>
                                </Col>
                                <Col className="col-6">
                                    <div className="form-group">
                                        <label htmlFor="maxUnidades">{t("PRE811_frm1_lbl_maxUnidades")}</label>
                                        <FieldNumber name="maxUnidades" />
                                        <StatusMessage for="maxUnidades" />
                                    </div>
                                </Col>
                            </Row>
                        </Col>
                    </Row>
                    <hr />
                    <Row>
                        <Row>
                            <Col className="col-3">
                                <div className="form-group" >
                                    <label><h5>{t("PRE811_frm1_lbl_ParametrosDeConfiguración")}</h5></label>
                                </div>
                            </Col>
                            <Col className="col-9">
                                <FormWarningMessage message={t("PRE811_frm1_lbl_InfoControlAcceso")} show/>
                            </Col>
                        </Row>
                        <Row>
                            <Col>
                                <div className="form-group">
                                    <FieldToggle name="habilitarControlAcceso" label={t("PRE811_frm1_lbl_HabilitarControlAcceso")} />
                                    <StatusMessage for="habilitarControlAcceso" />
                                </div>
                                <div class="form-group">
                                    <FieldToggle name="habilitarPedCompleto" label={t("PRE811CoAc_frm1_lbl_FlagPedCompleto")} />
                                    <StatusMessage for="habilitarPedCompleto" />
                                </div>
                                <div className="form-group">
                                    <FieldToggle name="habilitarLiberadoCompleto" label={t("PRE811_frm1_lbl_habilitarLiberadoCompleto")} />
                                    <StatusMessage for="habilitarLiberadoCompleto" />
                                </div>
                                <div className="form-group">
                                    <FieldToggle name="habilitarLiberacion" label={t("PRE811_frm1_lbl_HabilitarCondLiberacion")} />
                                    <StatusMessage for="habilitarLiberacion" />
                                </div>
                            </Col>
                            <Col>
                                <div className="form-group">
                                    <FieldToggle name="habilitarClase" label={t("PRE811_frm1_lbl_HabilitarClases")} />
                                    <StatusMessage for="habilitarClase" />
                                </div>
                                <div className="form-group">
                                    <FieldToggle name="habilitarFamilia" label={t("PRE811_frm1_lbl_HabilitarFamilia")} />
                                    <StatusMessage for="habilitarFamilia" />
                                </div>
                                <div className="form-group">
                                    <FieldToggle name="habilitarRuta" label={t("PRE811_frm1_lbl_HabilitarRuta")} />
                                    <StatusMessage for="habilitarRuta" />
                                </div>
                                <div className="form-group">
                                    <FieldToggle name="habilitarZona" label={t("PRE811_frm1_lbl_HabilitarZona")} />
                                    <StatusMessage for="habilitarZona" />
                                </div>
                            </Col>
                            <Col>
                                <div className="form-group">
                                    <FieldToggle name="habilitarEmpresa" label={t("PRE811_frm1_lbl_HabilitarEmpresa")} />
                                    <StatusMessage for="habilitarEmpresa" />
                                </div>
                                <div className="form-group">
                                    <FieldToggle name="habilitarCliente" label={t("PRE811_frm1_lbl_HabilitarCliente")} />
                                    <StatusMessage for="habilitarCliente" />
                                </div>
                                <div className="form-group">
                                    <FieldToggle name="habilitarTpPedido" label={t("PRE811_frm1_lbl_HabilitarTipoPedido")} />
                                    <StatusMessage for="habilitarTpPedido" />
                                </div>
                                <div className="form-group">
                                    <FieldToggle name="habilitarTpExpedicion" label={t("PRE811_frm1_lbl_HabilitarTipoExpedicion")} />
                                    <StatusMessage for="habilitarTpExpedicion" />
                                </div>
                            </Col>
                        </Row>
                    </Row>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("PRE811_frm1_btn_CANCELAR")} </Button>
                    <SubmitButton id="btnSubmitCreatePreferencia" variant="primary" label="PRE811_frm1_btn_CREAR" />
                    <SubmitButton id="btnSubmitConfiguracion" variant="primary" label="PRE811_frm1_btn_IrConfiguracion" />
                </Modal.Footer>
            </Form>
        </Modal >
    );
}

export const PRE811CreatePreferenciaModal = withPageContext(InternalPRE811CreatePreferenciaModal);