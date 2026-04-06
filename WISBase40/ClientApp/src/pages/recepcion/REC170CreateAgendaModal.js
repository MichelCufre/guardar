import React, { useState } from 'react';
import { Page } from '../../components/Page';
import { Modal, Button, Row, Col, Tab, Tabs, Container } from 'react-bootstrap';
import { useCustomTranslation } from '../../components/TranslationHook';
import { Form, Field, FieldSelect, FieldSelectAsync, FieldTextArea, StatusMessage, SubmitButton, FieldDate, FieldCheckbox } from '../../components/FormComponents/Form';
import * as Yup from 'yup';
import { withPageContext } from '../../components/WithPageContext';
import { CheckboxList } from '../../components/CheckboxList';
import { AddRemovePanel } from '../../components/AddRemovePanel';
import { useToaster } from '../../components/ToasterHook';



function InternalREC170CreateAgendaModal(props) {

    const { t } = useCustomTranslation();
    const toaster = useToaster();

    const [referenciaLibre, setReferenciaLibre] = useState(false);
    const [referenciaUnica, setReferenciaUnica] = useState(false);
    const [referenciaMultiple, setReferenciaMultiple] = useState(false);
    const [currentData, setCurrentData] = useState(null);
    const [displayDetalles, setDisplayDetalles] = useState(false);

    const [itemsListDisponibles, setItemsListDisponibles] = useState([]);
    const [itemsListSeleccionados, setItemsListSeleccionados] = useState([]);
    const [allSelectedDisponibles, setAllSelectedDisponibles] = useState(false);
    const [allSelectedSeleccionados, setAllSelectedSeleccionados] = useState(false);


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

    const handleClose = () => {
        props.onHide();
    };


    const handleFormBeforeValidateField = (context, form, query, nexus) => {

    }

    const handleFormAfterValidateField = (context, form, query, nexus) => {

        if (query.fieldId == "codigoInternoAgente") {

            setReferenciaLibre(false);
            setReferenciaUnica(false);
            setReferenciaMultiple(false);
        }
        else if (query.fieldId == "tipoRecepcionExterno") {

            if (form.fields.find(s => s.id === "autoCargarDetalle").value == "False") {
                setDisplayDetalles(true);
            } else {
                setDisplayDetalles(false);
            }

            const tipo = query.parameters.find(p => p.id === "tipoSeleccion");
            const tipoRecepcion = form.fields.find(f => f.id === "tipoRecepcionExterno")?.value;
            const idEmpresa = form.fields.find(f => f.id === "idEmpresa")?.value;
            const codigoInternoAgente = form.fields.find(f => f.id === "codigoInternoAgente")?.value;

            const current= {
                tipoRecepcion: tipoRecepcion,
                idEmpresa: idEmpresa,
                codigoInternoAgente: codigoInternoAgente
            };

            if (tipo) {

                if (JSON.stringify(current) !== JSON.stringify(currentData)) {
                    setCurrentData(current);

                    if (tipo.value == "Fail" || tipo.value == "FACTURA" || tipo.value == "BOLSA") {

                        setReferenciaLibre(false);
                        setReferenciaUnica(false);
                        setReferenciaMultiple(false);
                    }
                    else if (tipo.value == "LIBRE" || tipo.value == "LPN") {

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
                        setItemsListDisponibles([]);
                        setItemsListSeleccionados([]);

                        let jsonAdded = query.parameters.find(w => w.id === "referenciasDisponibles").value;
                        var arrayAdded = JSON.parse(jsonAdded.toString());

                        setItemsListDisponibles(arrayAdded);

                        let jsonAsociadas = query.parameters.find(w => w.id === "referenciasAsociadas").value;
                        var arrayAsociadas = JSON.parse(jsonAsociadas.toString());

                        setItemsListSeleccionados(arrayAsociadas);
                    }
                }
            }
        }
    }


    const handleChangeDisponibles = (event, id) => {

        let listDisponibles = [...itemsListDisponibles];

        let item = listDisponibles.find(d => d.id === id);

        item.selected = !item.selected;

        setItemsListDisponibles(listDisponibles);
    };

    const handleChangeSelecciondas = (event, id) => {

        let listSeleccionados = [...itemsListSeleccionados];

        let item = listSeleccionados.find(d => d.id === id);

        item.selected = !item.selected;

        setItemsListSeleccionados(listSeleccionados);
    };

    const handleSelectAllDisponibles = (selected) => {
        setAllSelectedDisponibles(selected);

        setItemsListDisponibles(itemsListDisponibles.map(d => ({ ...d, selected: selected })));
    }

    const handleSelectAllSeleccionados = (selected) => {
        setAllSelectedSeleccionados(selected);

        setItemsListSeleccionados(itemsListSeleccionados.map(d => ({ ...d, selected: selected })));
    }

    const handleAdd = (evt, nexus) => {

        let seleccionadosParaAgregar = itemsListDisponibles.filter(d => d.selected === true).map(d => ({ ...d, selected: false }));
        let listSeleccionados = [...itemsListSeleccionados, ...seleccionadosParaAgregar];

        // Agrego los seleccionados disponibles a los seleccinados para asociar
        setItemsListSeleccionados(listSeleccionados);

        // Elimino de la lista de disponibles
        let noSeleccionados = itemsListDisponibles.filter(d => d.selected === false);
        setItemsListDisponibles(noSeleccionados);

        setAllSelectedDisponibles(false);

        evt.preventDefault();
    };

    const handleRemove = (evt, nexus) => {

        let seleccionadosParaQuitar = itemsListSeleccionados.filter(d => d.selected === true).map(d => ({ ...d, selected: false }));
        let listDisponibles = [...itemsListDisponibles, ...seleccionadosParaQuitar];

        // Agrego los seleccionados para asociar a los seleccinados disponibles
        setItemsListDisponibles(listDisponibles);

        // Elimino de la lista de selecciondas
        let noSeleccionados = itemsListSeleccionados.filter(d => d.selected === false);
        setItemsListSeleccionados(noSeleccionados);

        setAllSelectedSeleccionados(false);

        evt.preventDefault();
    };

    const handleFormBeforeSubmit = (context, form, query, nexus) => {

        query.parameters.push({ id: "listaSeleccion", value: JSON.stringify(itemsListSeleccionados) });
        query.parameters.push({ id: "isSubmit", value: true });

    }

    const handleFormAfterSubmit = (context, form, query, nexus) => {

        context.showErrorMessage = true;

        if (context.responseStatus === "OK") {

            if (query.buttonId == "btnSubmitConfirmarIrDetalle") {

                props.nexus.getGrid("REC170_grid_1").refresh();

                const tipo = query.parameters.find(p => p.id === "tipoRecepcion");
                if (tipo.value == "LPN") {
                    props.onHide(query.parameters.find(a => a.id === "idAgenda").value, "seleccionLpn");
                } else {
                    props.onHide(query.parameters.find(a => a.id === "idAgenda").value, "detalles");
                }
            }
            else {

                props.nexus.getGrid("REC170_grid_1").refresh();
                props.onHide(null, null);

            }
        }
    }

    const handleFormAfterInitialize = (context, form, query, nexus) => {

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
                application="REC170Create"
                id="REC170Create_form_1"
                initialValues={initialValues}
                validationSchema={validationSchema}
                onBeforeSubmit={handleFormBeforeSubmit}
                onAfterSubmit={handleFormAfterSubmit}
                onBeforeValidateField={handleFormBeforeValidateField}
                onAfterValidateField={handleFormAfterValidateField}
                onAfterInitialize={handleFormAfterInitialize}
            >
                <Modal.Header closeButton>
                    <Modal.Title>{t("REC170_Sec0_mdlCreate_Titulo")}</Modal.Title>
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
                                                    <FieldSelectAsync name="idEmpresa" />
                                                    <StatusMessage for="idEmpresa" />
                                                </div>
                                            </Col>
                                            <Col>
                                                <div className="form-group" >
                                                    <label htmlFor="codigoInternoAgente">{t("REC170_frm1_lbl_codigoInternoAgente")}</label>
                                                    <FieldSelectAsync name="codigoInternoAgente" />
                                                    <StatusMessage for="codigoInternoAgente" />
                                                </div>
                                            </Col>
                                        </Row>
                                        <Row className="mb-2">
                                            <Col>
                                                <div className="form-group" >
                                                    <label htmlFor="numeroPredio">{t("REC170_frm1_lbl_numeroPredio")}</label>
                                                    <FieldSelect name="numeroPredio" />
                                                    <StatusMessage for="numeroPredio" />
                                                </div>
                                            </Col>
                                            <Col>
                                                <div className="form-group" >
                                                    <label htmlFor="tipoRecepcionExterno">{t("REC170_frm1_lbl_tipoRecepcionExterno")}</label>
                                                    <FieldSelect name="tipoRecepcionExterno" />
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
                                                    <FieldCheckbox name="autoCargarDetalle" label={t("REG170_frm1_lbl_autoCargarDetalle")} />
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
                                                    <FieldSelectAsync name="referencia" />
                                                    <StatusMessage for="referencia" />
                                                </div>
                                            </Col>
                                            <Col></Col>
                                        </Row>
                                        <Row >
                                            <Col >
                                                <div className="form-group" style={{ display: referenciaMultiple ? 'block' : 'none', marginTop: "15px" }} >
                                                    <AddRemovePanel
                                                        onAdd={handleAdd}
                                                        onRemove={handleRemove}

                                                        from={(
                                                            <CheckboxList
                                                                name="listaDisponibles"
                                                                items={itemsListDisponibles}
                                                                allSelected={allSelectedDisponibles}
                                                                onChange={handleChangeDisponibles}
                                                                onSelectAllChange={handleSelectAllDisponibles}
                                                                style={styleCheckList}
                                                            />
                                                        )}
                                                        to={(
                                                            <CheckboxList
                                                                name="listaSeleccionadas"
                                                                items={itemsListSeleccionados}
                                                                allSelected={allSelectedSeleccionados}
                                                                onChange={handleChangeSelecciondas}
                                                                onSelectAllChange={handleSelectAllSeleccionados}
                                                                style={styleCheckList}
                                                            />
                                                        )}
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
                    <SubmitButton id="btnSubmitConfirmar" variant="primary" label="REC170_frm1_btn_confirmar" />
                    <SubmitButton id="btnSubmitConfirmarIrDetalle" variant="primary" label="REC170_frm1_btn_confirmarIrDetalle" />
                </Modal.Footer>

            </Form >

        </Page>
    );
}

export const REC170CreateAgendaModal = withPageContext(InternalREC170CreateAgendaModal);