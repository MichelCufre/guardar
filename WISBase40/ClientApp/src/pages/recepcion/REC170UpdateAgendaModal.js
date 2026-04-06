import React, { useState } from 'react';
import { Page } from '../../components/Page';
import { Modal, Button, Row, Col, Tab, Tabs, Container } from 'react-bootstrap';
import { useCustomTranslation } from '../../components/TranslationHook';
import { Form, Field, FieldSelect, FieldSelectAsync, FieldTextArea, StatusMessage, SubmitButton, FieldDate, FieldCheckbox } from '../../components/FormComponents/Form';
import { CheckboxList } from '../../components/CheckboxList';
import * as Yup from 'yup';
import { withPageContext } from '../../components/WithPageContext';
import { useToaster } from '../../components/ToasterHook';


function InternalREC170UpdateAgendaModal(props) {

    const { t } = useCustomTranslation();
    const [infoAgenda, setInfoAgenda] = useState(null);

    const toaster = useToaster();

    const [referenciaLibre, setReferenciaLibre] = useState(false);
    const [referenciaUnica, setReferenciaUnica] = useState(false);
    const [referenciaMultiple, setReferenciaMultiple] = useState(false);

    const [itemsListSeleccionados, setItemsListSeleccionados] = useState([]);

    const initialValues = {

        numeroPredio: "",
        idEmpresa: "",
        tipoRecepcionExterno: "",
        codigoInternoAgente: "",
        autoCargarDetalle: "",
        referenciaLibre: "",
        referencia: "",
        fechaEntrega: "",
        funcionarioAsignado: "",
        placa: "",
        anexo1: "",
        anexo2: "",
        anexo3: "",
        anexo4: ""

    };

    const validationSchema = {

        numeroPredio: Yup.string().required(),
        idEmpresa: Yup.string().required(),
        tipoRecepcionExterno: Yup.string().required(),
        codigoInternoAgente: Yup.string().required(),

        autoCargarDetalle: Yup.string(),
        referenciaLibre: Yup.string(),
        referencia: Yup.string(),
        fechaEntrega: Yup.string(),
        funcionarioAsignado: Yup.string(),
        placa: Yup.string(),
        anexo1: Yup.string(),
        anexo2: Yup.string(),
        anexo3: Yup.string(),
        anexo4: Yup.string()
    };

    const handleFormBeforeInitialize = (context, form, query, nexus) => {

        query.parameters = [

            { id: "keyAgenda", value: props.agenda.find(a => a.id === "idAgenda").value }
        ];

    }

    const handleFormAfterInitialize = (context, form, query, nexus) => {

        if (query.parameters.find(x => x.id === "infoAgenda") != null) {
            setInfoAgenda(query.parameters.find(x => x.id === "infoAgenda").value);
        }


        const tipo = query.parameters.find(p => p.id === "tipoSeleccion");

        if (tipo.value == "Fail" || tipo.value == "FACTURA" || tipo.value == "BOLSA" ) {

            setReferenciaLibre(false);
            setReferenciaUnica(false);
            setReferenciaMultiple(false);

        } else if (tipo.value == "LIBRE" || tipo.value == "LPN") {

            setReferenciaLibre(true);
            setReferenciaUnica(false);
            setReferenciaMultiple(false);

        } else if (tipo.value == "MONO") {

            setReferenciaLibre(false);
            setReferenciaUnica(true);
            setReferenciaMultiple(false);

        } else if (tipo.value == "MULTIPLE") {

            setReferenciaLibre(false);
            setReferenciaUnica(false);
            setReferenciaMultiple(true);

            // cargar las listas

            let jsonAsociadas = query.parameters.find(w => w.id === "referenciasAsociadas").value;
            var arrayAsociadas = JSON.parse(jsonAsociadas.toString());

            setItemsListSeleccionados(arrayAsociadas);
        }

    };


    const handleFormBeforeSubmit = (context, form, query, nexus) => {

        query.parameters = [

            { id: "keyAgenda", value: props.agenda.find(a => a.id === "idAgenda").value }
        ];

    }

    const handleFormAfterSubmit = (context, form, query, nexus) => {

        context.showErrorMessage = true;

        if (context.responseStatus === "OK") {

            props.nexus.getGrid("REC170_grid_1").refresh();
            props.onHide(null, null);
        }
    }

    const handleChangeSeleccionados = (event, id) => {

    };

    const handleSelectAllSeleccionados = (selected) => {

    }

    const handleClose = () => {
        props.onHide();
    };

    const styleCheckList = {
        height: 200,
        maxHeight: 200
    }


    if (!props.show) {
        return null;
    }


    return (        
        <Page
            {...props}
        >
            <Form
                application="REC170Update"
                id="REC170Update_form_1"
                initialValues={initialValues}
                validationSchema={validationSchema}
                onBeforeInitialize={handleFormBeforeInitialize}
                onAfterInitialize={handleFormAfterInitialize}
                onBeforeSubmit={handleFormBeforeSubmit}
                onAfterSubmit={handleFormAfterSubmit}

            >
                <Modal.Header >
                    <Modal.Title>{t("REC170_Sec0_mdlUpdate_Titulo")} {`${props.agenda.find(a => a.id === "idAgenda").value}`}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Container fluid>

                        <Tabs defaultActiveKey="Datos" transition={false} id="noanim-tab-example">
                            <Tab eventKey="Datos" title={t("REC170_frm1_tab_datos")} >
                                <br></br>
                                <Row>
                                    <Col lg={6}>
                                        <Row className="mb-2">
                                            <Col>
                                                <div className="form-group" >
                                                    <label htmlFor="idEmpresa">{t("REC170_frm1_lbl_idEmpresa")}</label>
                                                    <FieldSelectAsync name="idEmpresa" readOnly />
                                                    <StatusMessage for="idEmpresa" />
                                                </div>
                                            </Col>
                                            <Col>
                                                <div className="form-group" >
                                                    <label htmlFor="codigoInternoAgente">{t("REC170_frm1_lbl_codigoInternoAgente")}</label>
                                                    <FieldSelectAsync name="codigoInternoAgente" readOnly />
                                                    <StatusMessage for="codigoInternoAgente" />
                                                </div>
                                            </Col>
                                        </Row>
                                        <Row className="mb-2">
                                            <Col>
                                                <div className="form-group" >
                                                    <label htmlFor="numeroPredio">{t("REC170_frm1_lbl_numeroPredio")}</label>
                                                    <FieldSelect name="numeroPredio" readOnly />
                                                    <StatusMessage for="numeroPredio" />
                                                </div>
                                            </Col>
                                            <Col>
                                                <div className="form-group" >
                                                    <label htmlFor="tipoRecepcionExterno">{t("REC170_frm1_lbl_tipoRecepcionExterno")}</label>
                                                    <FieldSelect name="tipoRecepcionExterno" readOnly />
                                                    <StatusMessage for="tipoRecepcionExterno" />
                                                </div>
                                            </Col>
                                        </Row>
                                        <Row className="mb-2">
                                            <Col >
                                                <div className="form-group" style={{
                                                    marginTop: "-20px"
                                                }} >
                                                    <label htmlFor="autoCargarDetalle" />
                                                    <FieldCheckbox name="autoCargarDetalle" label={t("REG170_frm1_lbl_autoCargarDetalle")} readOnly />
                                                    <StatusMessage for="autoCargarDetalle" />
                                                </div>
                                            </Col>
                                        </Row>
                                    </Col>
                                    <Col lg={6}>
                                        <Row className="mb-2">
                                            <Col >
                                                <div className="form-group" >
                                                    <label htmlFor="fechaEntrega">{t("REC170_frm1_lbl_fechaEntrega")}</label>
                                                    <FieldDate name="fechaEntrega" />
                                                    <StatusMessage for="fechaEntrega" />
                                                </div>
                                            </Col>
                                            <Col>
                                                <div className="form-group" >
                                                    <label htmlFor="funcionarioAsignado">{t("REC170_frm1_lbl_funcionarioAsignado")}</label>
                                                    <FieldSelectAsync name="funcionarioAsignado" isClearable />
                                                    <StatusMessage for="funcionarioAsignado" />
                                                </div>
                                            </Col>
                                        </Row>
                                        <Row className="mb-2">
                                            <Col>
                                                <div className="form-group" >
                                                    <label htmlFor="placa">{t("REC170_frm1_lbl_placa")}</label>
                                                    <Field name="placa" maxLength="20" />
                                                    <StatusMessage for="placa" />
                                                </div>
                                            </Col>
                                        </Row>
                                    </Col>
                                </Row>

                                <Row>
                                    <Col>
                                        <hr></hr>
                                    </Col>
                                </Row>

                                <Row >
                                    <Col>
                                        <Row className="mb-2">
                                            <Col>
                                                <div className="form-group" style={{ display: referenciaLibre ? 'block' : 'none' }} >
                                                    <label htmlFor="referenciaLibre">{t("REC170_frm1_lbl_referenciaLibre")}</label>
                                                    <Field name="referenciaLibre" maxLength="20" />
                                                    <StatusMessage for="referenciaLibre" />
                                                </div>
                                                <div className="form-group" style={{ display: referenciaUnica ? 'block' : 'none' }} >
                                                    <label htmlFor="referencia">{t("REC170_frm1_lbl_buscarReferencia")}</label>
                                                    <FieldSelect name="referencia" />
                                                    <StatusMessage for="referencia" />
                                                </div>
                                            </Col>
                                            <Col></Col>
                                        </Row>
                                        <Row >
                                            <Col >
                                                <div className="form-group" style={{ display: referenciaMultiple ? 'block' : 'none', marginTop: "15px" }} >

                                                    <CheckboxList
                                                        name="listaSeleccionados"
                                                        items={itemsListSeleccionados}
                                                        allSelected={handleSelectAllSeleccionados}
                                                        onChange={handleChangeSeleccionados}
                                                        onSelectAllChange={handleSelectAllSeleccionados}
                                                        style={styleCheckList}
                                                    />
                                                </div>
                                            </Col>
                                        </Row>
                                    </Col>
                                </Row>
                            </Tab>

                            <Tab eventKey="anexos" title={t("REC170_frm1_tab_anexos")}>
                                <br></br>
                                <Row className="mb-2">
                                    <Col >
                                        <div className="form-group" >
                                            <label htmlFor="anexo1">{t("REC170_frm1_lbl_anexo1")}</label>
                                            <FieldTextArea name="anexo1" maxLength="200" />
                                            <StatusMessage for="anexo1" />
                                        </div>
                                    </Col>
                                </Row>
                                <Row className="mb-2">
                                    <Col>
                                        <div className="form-group" >
                                            <label htmlFor="anexo2">{t("REC170_frm1_lbl_anexo2")}</label>
                                            <FieldTextArea name="anexo2" maxLength="200" />
                                            <StatusMessage for="anexo2" />
                                        </div>
                                    </Col>
                                </Row>
                                <Row className="mb-2">
                                    <Col>
                                        <div className="form-group" >
                                            <label htmlFor="anexo3">{t("REC170_frm1_lbl_anexo3")}</label>
                                            <FieldTextArea name="anexo3" maxLength="200" />
                                            <StatusMessage for="anexo3" />
                                        </div>
                                    </Col>
                                </Row>
                                <Row className="mb-2">
                                    <Col>
                                        <div className="form-group" >
                                            <label htmlFor="anexo4">{t("REC170_frm1_lbl_anexo4")}</label>
                                            <FieldTextArea name="anexo4" maxLength="200" />
                                            <StatusMessage for="anexo4" />
                                        </div>
                                    </Col>
                                </Row>
                            </Tab>
                        </Tabs>

                    </Container>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("REC170_frm1_btn_cerrar")} </Button>
                    <SubmitButton id="btnSubmitGuardar" variant="primary" label="REC170_frm1_btn_guardar" />

                </Modal.Footer>
            </Form>

        </Page>
    );
}


export const REC170UpdateAgendaModal = withPageContext(InternalREC170UpdateAgendaModal);