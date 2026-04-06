import React, { useState, useRef } from 'react';
import { Modal, Button, Row, Col, Tab, Tabs, Container } from 'react-bootstrap';
import { Grid } from '../../components/GridComponents/Grid';
import { useCustomTranslation } from '../../components/TranslationHook';
import { Form, Field, FieldSelect, FieldSelectAsync, FieldTextArea, StatusMessage, SubmitButton, FieldCheckbox } from '../../components/FormComponents/Form';
import { notificationType } from '../../components/Enums';
import * as Yup from 'yup';
import { withPageContext } from '../../components/WithPageContext';

function InternalREG040CreateUbicacionModal(props) {

    const { t } = useCustomTranslation();

    const infoCantidadColumnas = useRef({});
    const infoCantidadAlturas = useRef({});

    const refCantidadColumnas = useRef({});
    const refCantidadAlturas = useRef({});

    const infoCantidadUbicaciones = useRef({});

    const refCamposSubmit = useRef({});

    const [isInfoCantidadColumnasDisplayed, setInfoCantidadColumnasDisplayed] = useState(false);
    const [isInfoCantidadAlturasDisplayed, setInfoCantidadAlturasDisplayed] = useState(false);
    const [isInfoCantidadUbicacionesDisplayed, setInfoCantidadUbicacionesDisplayed] = useState(false);


    const initialValues = {
        idEmpresa: "",
        idArea: "",
        idTipoUbicacion: "",
        idProductoClase: "",
        idProductoFamilia: "",
        idProductoRotatividad: "",
        numeroPredio: "",
        codigoBloque: "",
        codigoCalle: "",
        columnaDesde: "",
        columnaHasta: "",
        columnaSalto: "",
        alturaDesde: "",
        alturaHasta: "",
        alturaSalto: "",
        omitirPares: "",
        idZonaUbicacion: "",
        controlAcceso: "",

    };

    const validationSchema = {

        idEmpresa: Yup.string().required(),
        idArea: Yup.string().required(),
        idTipoUbicacion: Yup.string().required(),
        idProductoClase: Yup.string().required(),
        idProductoFamilia: Yup.string().required(),
        idProductoRotatividad: Yup.string().required(),
        numeroPredio: Yup.string().required(),
        codigoBloque: Yup.string().required(),
        codigoCalle: Yup.string().required(),
        columnaDesde: Yup.string().required(),
        columnaHasta: Yup.string().required(),
        columnaSalto: Yup.string().required(),
        alturaDesde: Yup.string().required(),
        alturaHasta: Yup.string().required(),
        alturaSalto: Yup.string().required(),
        idZonaUbicacion: Yup.string(),
        omitirPares: Yup.string(),
        ubicacionBaja: Yup.boolean(),
        controlAcceso: Yup.string().nullable(),
    };

    const handleClose = () => {
        props.onHide();
    };



    const handleFormBeforeValidateField = (context, form, query, nexus) => {


        if (query.fieldId == "columnaSalto") {

            setInfoCantidadColumnasDisplayed(false);
            refCantidadColumnas.current = 0;
        }
        if (query.fieldId == "alturaSalto") {

            setInfoCantidadAlturasDisplayed(false);
            refCantidadAlturas.current = 0;
        }
        if (query.fieldId == "columnaSalto" || query.fieldId == "alturaSalto") {

            setInfoCantidadUbicacionesDisplayed(false);
        }
    }

    const handleFormAfterValidateField = (context, form, query, nexus) => {

        if (query.fieldId == "columnaSalto") {

            const infoColumnas = query.parameters.find(p => p.id === "infoColumnas");
            const cantidadColumnas = query.parameters.find(p => p.id === "cantidadColumnas");

            if (cantidadColumnas) {

                refCantidadColumnas.current = cantidadColumnas.value;
                infoCantidadColumnas.current = t(infoColumnas.value, [cantidadColumnas.value]);

                setInfoCantidadColumnasDisplayed(true);
            }
        }

        if (query.fieldId == "alturaSalto") {

            const infoAlturas = query.parameters.find(p => p.id === "infoAlturas");
            const cantidadAlturas = query.parameters.find(p => p.id === "cantidadAlturas");

            if (cantidadAlturas) {

                refCantidadAlturas.current = cantidadAlturas.value;
                infoCantidadAlturas.current = t(infoAlturas.value, [cantidadAlturas.value]);

                setInfoCantidadAlturasDisplayed(true);
            }
        }

        if (query.fieldId == "columnaSalto" || query.fieldId == "alturaSalto") {

            if (refCantidadColumnas.current > 0 && refCantidadAlturas.current > 0) {

                let cantidad = (refCantidadColumnas.current * refCantidadAlturas.current);

                infoCantidadUbicaciones.current = t("REG040_Sec0_Info_CantidadUbicaciones", [cantidad]);

                setInfoCantidadUbicacionesDisplayed(true);
            }

        }
    }

    const handleFormBeforeSubmit = (context, form, query, nexus) => {

        query.parameters = [
            { id: "isSubmit", value: true },
        ];

        refCamposSubmit.current = {
            idEmpresa: form.fields.find(d => d.id === "idEmpresa").value,
            idArea: form.fields.find(d => d.id === "idArea").value,
            idTipoUbicacion: form.fields.find(d => d.id === "idTipoUbicacion").value,
            idProductoClase: form.fields.find(d => d.id === "idProductoClase").value,
            idProductoFamilia: form.fields.find(d => d.id === "idProductoFamilia").value,
            idProductoRotatividad: form.fields.find(d => d.id === "idProductoRotatividad").value,
            numeroPredio: form.fields.find(d => d.id === "numeroPredio").value,
            codigoBloque: form.fields.find(d => d.id === "codigoBloque").value,
        };

    }
    const handleFormonBeforeInitialize = (context, form, query, nexus) => {

        if (refCamposSubmit.current) {

            query.parameters = [
                { id: "idEmpresa", value: refCamposSubmit.current.idEmpresa },
                { id: "idArea", value: refCamposSubmit.current.idArea },
                { id: "idTipoUbicacion", value: refCamposSubmit.current.idTipoUbicacion },
                { id: "idProductoClase", value: refCamposSubmit.current.idProductoClase },
                { id: "idProductoFamilia", value: refCamposSubmit.current.idProductoFamilia },
                { id: "idProductoRotatividad", value: refCamposSubmit.current.idProductoRotatividad },
                { id: "numeroPredio", value: refCamposSubmit.current.numeroPredio },
                { id: "codigoBloque", value: refCamposSubmit.current.codigoBloque },
            ];

        }
        refCamposSubmit.current = null;
    };


    const handleFormAfterSubmit = (context, form, query, nexus) => {

        if (query.resetForm) {

            nexus.getGrid("REG040_grid_1").refresh();

            setInfoCantidadUbicacionesDisplayed(false);
            setInfoCantidadAlturasDisplayed(false);
            setInfoCantidadColumnasDisplayed(false);

            // props.onHide();

            // TODO Hacer foco en el campo calle
        }
    }

    const onBeforeValidateField = (context, form, query, nexus) => {
        context.abortServerCall = true;
    }

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-50w" backdrop="static">

            <Form
                application="REG040Create"
                id="REG040Create_form_1"
                initialValues={initialValues}
                onBeforeInitialize={handleFormonBeforeInitialize}
                validationSchema={validationSchema}
                onBeforeSubmit={handleFormBeforeSubmit}
                onAfterSubmit={handleFormAfterSubmit}
                onBeforeValidateField={handleFormBeforeValidateField}
                onAfterValidateField={handleFormAfterValidateField}
            >

                <Modal.Header closeButton>
                    <Modal.Title>{t("REG040_Sec0_mdlCreate_Titulo")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>

                    <Container fluid>
                        <Row>
                            <Col>
                                <h6 className="form-title">{t("REG040_frm1_lbl_legendConfiguracion")}</h6>
                                <Row>

                                    <Col>
                                        <div className="form-group" >
                                            <label htmlFor="idEmpresa">{t("REG040_frm1_lbl_idEmpresa")}</label>
                                            <FieldSelectAsync name="idEmpresa" />
                                            <StatusMessage for="idEmpresa" />
                                        </div>
                                    </Col>
                                    <Col>
                                        <div className="form-group" >
                                            <label htmlFor="idArea">{t("REG040_frm1_lbl_idArea")}</label>
                                            <FieldSelect name="idArea" />
                                            <StatusMessage for="idArea" />
                                        </div>
                                    </Col>
                                    <Col>
                                        <div className="form-group" >
                                            <label htmlFor="idTipoUbicacion">{t("REG040_frm1_lbl_idTipoUbicacion")}</label>
                                            <FieldSelect name="idTipoUbicacion" />
                                            <StatusMessage for="idTipoUbicacion" />
                                        </div>
                                    </Col>
                                </Row>

                                <Row>
                                    <Col>
                                        <div className="form-group" >
                                            <label htmlFor="idProductoClase">{t("REG040_frm1_lbl_idProductoClase")}</label>
                                            <FieldSelect name="idProductoClase" />
                                            <StatusMessage for="idProductoClase" />
                                        </div>
                                    </Col>
                                    <Col>
                                        <div className="form-group" >
                                            <label htmlFor="idProductoFamilia">{t("REG040_frm1_lbl_idProductoFamilia")}</label>
                                            <FieldSelectAsync name="idProductoFamilia" />
                                            <StatusMessage for="idProductoFamilia" />
                                        </div>
                                    </Col>
                                    <Col>
                                        <div className="form-group" >
                                            <label htmlFor="idProductoRotatividad">{t("REG040_frm1_lbl_idProductoRotatividad")}</label>
                                            <FieldSelect name="idProductoRotatividad" />
                                            <StatusMessage for="idProductoRotatividad" />
                                        </div>
                                    </Col>
                                </Row>

                                <h6 className="form-title">{t("REG040_frm1_lbl_legendUbicacion")}</h6>

                                <Row>
                                    <Col>
                                        <div className="form-group" >
                                            <label htmlFor="numeroPredio">{t("REG040_frm1_lbl_numeroPredio")}</label>
                                            <FieldSelect name="numeroPredio" />
                                            <StatusMessage for="numeroPredio" />
                                        </div>
                                    </Col>
                                    <Col>
                                        <div className="form-group" >
                                            <label htmlFor="codigoBloque">{t("REG040_frm1_lbl_codigoBloque")}</label>
                                            <Field name="codigoBloque" />
                                            <StatusMessage for="codigoBloque" />
                                        </div>
                                    </Col>
                                    <Col>
                                        <div className="form-group" >
                                            <label htmlFor="codigoCalle">{t("REG040_frm1_lbl_codigoCalle")}</label>
                                            <Field name="codigoCalle" />
                                            <StatusMessage for="codigoCalle" />
                                        </div>
                                    </Col>
                                </Row>

                                <Row>
                                    <Col>
                                        <div className="form-group" >
                                            <label htmlFor="columnaDesde">{t("REG040_frm1_lbl_columnaDesde")}</label>
                                            <Field name="columnaDesde" />
                                            <StatusMessage for="columnaDesde" />
                                        </div>
                                    </Col>
                                    <Col>
                                        <div className="form-group" >
                                            <label htmlFor="columnaHasta">{t("REG040_frm1_lbl_columnaHasta")}</label>
                                            <Field name="columnaHasta" />
                                            <StatusMessage for="columnaHasta" />
                                        </div>
                                    </Col>
                                    <Col>
                                        <div className="form-group" >
                                            <label htmlFor="columnaSalto">{t("REG040_frm1_lbl_columnaSalto")}</label>
                                            <Field name="columnaSalto" />
                                            <StatusMessage for="columnaSalto" maxLength="5" />
                                            <div style={{ display: isInfoCantidadColumnasDisplayed ? 'block' : 'none' }}>
                                                <span className="text-info">
                                                    {`${infoCantidadColumnas.current}`}
                                                </span>
                                            </div>
                                        </div>
                                    </Col>
                                </Row>

                                <Row>
                                    <Col>
                                        <div className="form-group" >
                                            <label htmlFor="alturaDesde">{t("REG040_frm1_lbl_alturaDesde")}</label>
                                            <Field name="alturaDesde"/>
                                            <StatusMessage for="alturaDesde" />
                                        </div>
                                    </Col>
                                    <Col>
                                        <div className="form-group" >
                                            <label htmlFor="alturaHasta">{t("REG040_frm1_lbl_alturaHasta")}</label>
                                            <Field name="alturaHasta"/>
                                            <StatusMessage for="alturaHasta" />
                                        </div>
                                    </Col>
                                    <Col>
                                        <div className="form-group" >
                                            <label htmlFor="alturaSalto">{t("REG040_frm1_lbl_alturaSalto")}</label>
                                            <Field name="alturaSalto" maxLength="5" />
                                            <StatusMessage for="alturaSalto" />
                                            <div style={{ display: isInfoCantidadAlturasDisplayed ? 'block' : 'none' }}>
                                                <span className="text-info">
                                                    {`${infoCantidadAlturas.current}`}
                                                </span>
                                            </div>
                                        </div>
                                    </Col>
                                </Row>

                                <Row>
                                    <Col>
                                        <div className="form-group" >
                                            <label htmlFor="idZonaUbicacion">{t("REG040_frm1_lbl_idZonaUbicacion")}</label>
                                            <FieldSelect name="idZonaUbicacion" />
                                            <StatusMessage for="idZonaUbicacion" />
                                        </div>
                                    </Col>
                                    <Col >
                                        <div className="form-group" >
                                            <label htmlFor="omitirPares">{t("REG040_frm1_lbl_omitirPares")}</label>
                                            <FieldSelect name="omitirPares" />
                                            <StatusMessage for="omitirPares" />
                                        </div>
                                    </Col>
                                    <Col >
                                        <div className="form-group" >
                                            <label htmlFor="controlAcceso">{t("REG040_frm1_lbl_controlAcceso")}</label>
                                            <FieldSelect name="controlAcceso" />
                                            <StatusMessage for="controlAcceso" />
                                        </div>
                                    </Col>
                                    <Col >
                                        <div className="form-group" style={{
                                            marginTop: "15px"
                                        }} >
                                            <label htmlFor="ubicacionBaja" />
                                            <FieldCheckbox name="ubicacionBaja" label={t("REG040_frm1_lbl_ubicacionBaja")} />
                                            <StatusMessage for="ubicacionBaja" />
                                        </div>
                                    </Col>
                                    <Col style={{
                                        textAlign: "left",
                                        justifyContent: 'center'
                                    }}>

                                        <div style={{ display: isInfoCantidadUbicacionesDisplayed ? 'block' : 'none' }}>
                                            <span className="text-info">
                                                {`${infoCantidadUbicaciones.current}`}
                                            </span>
                                        </div>

                                    </Col>
                                </Row>
                            </Col>
                        </Row>
                    </Container>

                </Modal.Body>
                <Modal.Footer>
                    <Button variant="btn btn-outline-secondary" onClick={handleClose}>{t("REG040_frm1_btn_CANCELAR")}</Button>
                    <SubmitButton id="btnSubmitCreateUbicacion" variant="primary" label="REG040_frm1_btn_CREAR" />
                </Modal.Footer>
            </Form>
        </Modal>
    );
}

export const REG040CreateUbicacionModal = withPageContext(InternalREG040CreateUbicacionModal);