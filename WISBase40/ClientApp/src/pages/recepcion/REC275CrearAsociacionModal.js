import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { withPageContext } from '../../components/WithPageContext';
import { Form, Field, StatusMessage, FieldSelectAsync, SubmitButton, FieldSelect } from '../../components/FormComponents/Form';
import { Modal, Button, Row, Col, Container } from 'react-bootstrap';
import * as Yup from 'yup';
import { Grid } from '../../components/GridComponents/Grid';

function InternalREC275CrearAsociacionModal(props) {

    const { t } = useTranslation();
    const [isAsociarClase, setAsociarClase] = useState("hidden");
    const [isAsociarGrupo, setAsociarGrupo] = useState("hidden");
    const [isAsociarProducto, setAsociarProducto] = useState("hidden");
    const [predio, setPredio] = useState("");
    const [tipoEntidad, setTipoEntidad] = useState("");
    const [codigoEntidad, setCodigoEntidad] = useState("");
    const [descripcionEntidad, setDescripcionEntidad] = useState("");
    const [codigoEmpresa, setCodigoEmpresa] = useState("");
    const [descripcionEmpresa, setDescripcionEmpresa] = useState("");

    const initialValues = {
        estrategia: "",
        predio: "",
        asociacion: "",
        clase: "",
        grupo: "",
        empresa: "",
        producto: "",
    };

    const validationSchema = {
        estrategia: Yup.string().required(),
        predio: Yup.string().required(),
        asociacion: Yup.string().required(),
        clase: Yup.string(),
        grupo: Yup.string(),
        empresa: Yup.string(),
        producto: Yup.string(),
    };

    const handleClose = () => {
        props.onHide(props.nexus);
    };

    const onAfterSubmit = (context, form, query, nexus) => {
        context.showErrorMessage = true;

        if (context.responseStatus === "OK") {
            props.onHide(props.nexus);
        }
    }

    const onBeforeInitialize = (context, form, query, nexus) => {
        query.parameters = [
            { id: "codigoEstrategia", value: props.codigoEstrategia },
        ];
    };

    const onBeforeSubmit = (context, form, query, nexus) => {

        const rowsEntrada = nexus.getGrid("REC275_grid_6").getModifiedRows();

        query.parameters = [
            { id: "rowsEntrada", value: JSON.stringify(rowsEntrada) },
            { id: "codigoEstrategia", value: props.codigoEstrategia },
        ];
    }

    const onBeforeValidateField = (context, form, query, nexus) => {
        var clase = form.fields.find(w => w.id === "clase");
        var grupo = form.fields.find(w => w.id === "grupo");
        var producto = form.fields.find(w => w.id === "producto");
        var empresa = form.fields.find(w => w.id === "empresa");

        setPredio(form.fields.find(w => w.id === "predio").value);
        setCodigoEmpresa(empresa.value);

        var optEmpresa = empresa.options.find(o => o.value === empresa.value);
        setDescripcionEmpresa(optEmpresa ? optEmpresa.label : '');

        setTipoEntidad("PRODUCTO");
        setCodigoEntidad(producto.value);

        var optProducto = producto.options.find(o => o.value === producto.value);
        setDescripcionEntidad(optProducto ? optProducto.label : '');

        if (!producto.value) {
            setTipoEntidad("GRUPO");
            setCodigoEntidad(grupo.value);

            var optGrupo = grupo.options.find(o => o.value === grupo.value);
            setDescripcionEntidad(optGrupo ? optGrupo.label : '');
        }

        if (!grupo.value) {
            setTipoEntidad("CLASE");
            setCodigoEntidad(clase.value);

            var optClase = clase.options.find(o => o.value === clase.value);
            setDescripcionEntidad(optClase ? optClase.label : '');
        }
    };

    const onAfterValidateField = (context, form, query, nexus) => {

        const clase = query.parameters.find(p => p.id === "Clase");
        const grupo = query.parameters.find(p => p.id === "Grupo");
        const producto = query.parameters.find(p => p.id === "Producto");

        if (clase && clase.value == "true") {
            setAsociarClase("");
            setAsociarGrupo("hidden");
            setAsociarProducto("hidden");

            form.fields.find(f => f.id === "producto").value = "";
            form.fields.find(f => f.id === "empresa").value = "";
            form.fields.find(f => f.id === "grupo").value = "";
        }

        if (query.fieldId === "clase" || query.fieldId === "predio"
            || query.fieldId === "asociacion" || query.fieldId === "grupo"
            || query.fieldId === "empresa" || query.fieldId === "producto") {

            let grid = nexus.getGrid("REC275_grid_6");
            if (grid != undefined) {
                nexus.getGrid("REC275_grid_6").refresh();
            }
        }


        if (grupo && grupo.value == "true") {
            setAsociarGrupo("");
            setAsociarClase("hidden");
            setAsociarProducto("hidden");

            form.fields.find(f => f.id === "producto").value = "";
            form.fields.find(f => f.id === "empresa").value = "";
            form.fields.find(f => f.id === "clase").value = "";

        }

        if (producto && producto.value == "true") {
            setAsociarProducto("");
            setAsociarGrupo("hidden");
            setAsociarClase("hidden");

            form.fields.find(f => f.id === "grupo").value = "";
            form.fields.find(f => f.id === "clase").value = "";
        }
    };

    const applyParameters = (context, data, nexus) => {

        data.parameters = [
            { id: "estrategia", value: props.codigoEstrategia },
            { id: "predio", value: predio },
            { id: "tipoEntidad", value: tipoEntidad },
            { id: "codigoEntidad", value: codigoEntidad },
            { id: "descripcionEntidad", value: descripcionEntidad },
            { id: "codigoEmpresa", value: codigoEmpresa },
            { id: "descripcionEmpresa", value: descripcionEmpresa },
        ];
    };

    return (

        <Form
            application="REC275CrearAsociacion"
            id="REC275FormModalCrearAsociacion"
            initialValues={initialValues}
            validationSchema={validationSchema}
            onBeforeSubmit={onBeforeSubmit}
            onAfterSubmit={onAfterSubmit}
            onBeforeInitialize={onBeforeInitialize}
            onBeforeValidateField={onBeforeValidateField}
            onAfterValidateField={onAfterValidateField}
        >
            <Modal.Header closeButton>
                <Modal.Title>{t("REC275_Sec0_title_CrearAsociacion")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Container fluid>
                    <Row>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="estrategia">{t("REC275_form_colname_Estrategia")}</label>
                                <Field name="estrategia" readOnly />
                                <StatusMessage for="estrategia" />
                            </div>
                        </Col>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="descripcionEstrategia">{t("REC275_form_lbl_DescripcionEstrategia")}</label>
                                <Field name="descripcionEstrategia" readOnly />
                                <StatusMessage for="descripcionEstrategia" />
                            </div>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="predio">{t("REC275_form_colname_Predio")}</label>
                                <FieldSelect name="predio" />
                                <StatusMessage for="predio" />
                            </div>
                        </Col>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="asociacion">{t("REC275_form_colname_Asociacion")}</label>
                                <FieldSelect name="asociacion" />
                                <StatusMessage for="asociacion" />
                            </div>
                        </Col>
                    </Row>

                    <Row className={isAsociarClase}>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="clase">{t("REC275_form_colname_Clase")}</label>
                                <FieldSelect name="clase" />
                                <StatusMessage for="clase" />
                            </div>
                        </Col>
                    </Row>

                    <Row className={isAsociarGrupo}>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="grupo">{t("REC275_form_colname_Grupo")}</label>
                                <FieldSelect name="grupo" />
                                <StatusMessage for="grupo" />
                            </div>
                        </Col>
                    </Row>

                    <Row className={isAsociarProducto}>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="empresa">{t("REC275_form_colname_Empresa")}</label>
                                <FieldSelectAsync name="empresa" />
                                <StatusMessage for="empresa" />
                            </div>
                        </Col>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="producto">{t("REC275_form_colname_Producto")}</label>
                                <FieldSelectAsync name="producto" />
                                <StatusMessage for="producto" />
                            </div>
                        </Col>
                    </Row>

                </Container>
                <br />
                <div className="col-12">
                    <Grid application="REC275CrearAsociacion" id="REC275_grid_6" rowsToFetch={30} rowsToDisplay={15} enableExcelExport={true}
                        onBeforeFetch={applyParameters}
                        onBeforeInitialize={applyParameters}
                        onBeforeExportExcel={applyParameters}
                        onBeforeFetchStats={applyParameters}
                        onBeforeApplyFilter={applyParameters}
                        onBeforeApplySort={applyParameters}
                    />
                </div>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("PRE052_frm1_btn_cerrar")} </Button>
                <SubmitButton id="btnSubmitConfirmarAsociacion" variant="primary" label="REC275_frm1_btn_Confirmar" />
            </Modal.Footer>
        </Form>
    );
}

export const REC275CrearAsociacionModal = withPageContext(InternalREC275CrearAsociacionModal);