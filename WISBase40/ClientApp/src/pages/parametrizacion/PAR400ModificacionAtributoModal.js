import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { withPageContext } from '../../components/WithPageContext';
import { Form, Field, FieldSelect, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';
import { Modal, Button, Row, Col, Container } from 'react-bootstrap';
import * as Yup from 'yup';


function InternalPAR400ModificacionAtributoModal(props) {

    const { t, i18n } = useTranslation();

    const [isTipoFecha, setIsTipoFecha] = useState("hidden");
    const [isTipoNumerico, setIsTipoNumerico] = useState("hidden");
    const [isTipoDominio, setIsTipoDominio] = useState("hidden");
    const [isTipoHora, setIsTipoHora] = useState("hidden");
    const [isTipoSistemaWis, setIsTipoSistemaWis] = useState("hidden");

    const initialValues = {

        nombre: "",
        tipo: "",
        descripcion: "",
        mascaraIngresoFecha: "",
        mascaraIngresoHora: "",
        displayFecha: "",
        displayHora: "",
        dominio: "",
        campo: "",
        largo: "",
        decimales: "",
        separador: "",
    };

    const validationSchema = {
        nombre: Yup.string().required(),
        tipo: Yup.string().required(),
        descripcion: Yup.string().required(),
        mascaraIngresoFecha: Yup.string(),
        mascaraIngresoHora: Yup.string(),
        displayFecha: Yup.string(),
        displayHora: Yup.string(),
        dominio: Yup.string(),
        campo: Yup.string(),
        largo: Yup.string(),
        decimales: Yup.string(),
        separador: Yup.string(),
    };

    const handleClose = () => {
        props.onHide(props.nexus);
    };

    const handleFormBeforeInitialize = (context, form, query, nexus) => {
        query.parameters = [
            { id: "codigoAtributo", value: props.codigoAtributo },
        ];

        if (props.tipoAtributo) {
            switch (props.tipoAtributo) {
                case '2':
                    setIsTipoFecha("");
                    setIsTipoNumerico("hidden");
                    setIsTipoDominio("hidden");
                    setIsTipoHora("hidden");
                    break;
                case '1':
                    setIsTipoFecha("hidden");
                    setIsTipoNumerico("");
                    setIsTipoDominio("hidden");
                    setIsTipoHora("hidden");
                    break;
                case '5':
                    setIsTipoFecha("hidden");
                    setIsTipoNumerico("hidden");
                    setIsTipoDominio("");
                    setIsTipoHora("hidden");
                    break;
                case '3':
                    setIsTipoFecha("hidden");
                    setIsTipoNumerico("hidden");
                    setIsTipoDominio("hidden");
                    setIsTipoHora("");
                    break;
                case '6':
                    setIsTipoFecha("hidden");
                    setIsTipoNumerico("hidden");
                    setIsTipoDominio("hidden");
                    setIsTipoHora("hidden");
                    setIsTipoSistemaWis("");
                    break;
            }
        }
    };

    const onAfterSubmit = (context, form, query, nexus) => {
        context.showErrorMessage = true;

        if (context.responseStatus === "OK") {
            nexus.getGrid("PAR400_grid_1").refresh();
            props.onHide(props.nexus);
        }
    }

    const onBeforeSubmit = (context, form, query, nexus) => {
        const parameters = [
            {
                id: "codigoAtributo",
                value: props.codigoAtributo
            }
        ];

        query.parameters = parameters;
    };

    // allow keys to be phrases having `:`
    i18n.options.nsSeparator = '::';

    return (

        <Form
            application="PAR400ModificarAtributo"
            id="PAR400ModificarAtributo_form_1"
            initialValues={initialValues}
            validationSchema={validationSchema}
            onAfterSubmit={onAfterSubmit}
            onBeforeSubmit={onBeforeSubmit}
            onBeforeInitialize={handleFormBeforeInitialize}
            onBeforeInitialize={handleFormBeforeInitialize}
            onBeforeValidateField={onBeforeSubmit}
        >
            <Modal.Header closeButton>
                <Modal.Title>{t("PAR400_Sec0_mdlEdit_Modificar")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Container fluid>
                    <Row>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="nombre">{t("PAR400_Sec0_Info_Cabezal_Nombre")}</label>
                                <Field name="nombre" />
                                <StatusMessage for="nombre" />
                            </div>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="tipo">{t("PAR400_Sec0_Info_Cabezal_Tipo")}</label>
                                <Field name="tipo" />
                                <StatusMessage for="tipo" />
                            </div>
                        </Col>
                    </Row>
                    <Row>
                        <Col className={isTipoFecha}>
                            <div className="form-group" >
                                <label htmlFor="mascaraIngresoFecha">{t("PAR400_form_colname_mascaraIngresoFecha")}</label>
                                <FieldSelect name="mascaraIngresoFecha" />
                                <StatusMessage for="mascaraIngresoFecha" />
                            </div>
                        </Col>
                        <Col className={isTipoNumerico}>
                            <div className="form-group" >
                                <label htmlFor="largo">{t("PAR400_form_colname_largo")}</label>
                                <Field name="largo" />
                                <StatusMessage for="largo" />
                            </div>
                        </Col>
                        <Col className={isTipoHora}>
                            <div className="form-group" >
                                <label htmlFor="mascaraIngresoHora">{t("PAR400_form_colname_mascaraIngresoHora")}</label>
                                <FieldSelect name="mascaraIngresoHora" />
                                <StatusMessage for="mascaraIngresoHora" />
                            </div>
                        </Col>
                        <Col className={isTipoDominio}>
                            <div className="form-group" >
                                <label htmlFor="dominio">{t("PAR400_form_colname_Dominio")}</label>
                                <FieldSelect name="dominio" />
                                <StatusMessage for="dominio" />
                            </div>
                        </Col>
                        <Col className={isTipoSistemaWis}>
                            <div className="form-group" >
                                <label htmlFor="campo">{t("PAR400_form_colname_Campo")}</label>
                                <FieldSelect name="campo" />
                                <StatusMessage for="campo" />
                            </div>
                        </Col>
                    </Row>
                    <Row>
                        <Col className={isTipoFecha}>
                            <div className="form-group" >
                                <label htmlFor="displayFecha">{t("PAR400_form_colname_displayFecha")}</label>
                                <FieldSelect name="displayFecha" />
                                <StatusMessage for="displayFecha" />
                            </div>
                        </Col>
                        <Col className={isTipoNumerico}>
                            <div className="form-group" >
                                <label htmlFor="decimales">{t("PAR400_form_colname_decimales")}</label>
                                <Field name="decimales" />
                                <StatusMessage for="decimales" />
                            </div>
                        </Col>
                        <Col className={isTipoHora}>
                            <div className="form-group" >
                                <label htmlFor="displayHora">{t("PAR400_form_colname_displayHora")}</label>
                                <FieldSelect name="displayHora" />
                                <StatusMessage for="displayHora" />
                            </div>
                        </Col>
                    </Row>
                    <Row>
                        <Col className={isTipoNumerico}>
                            <div className="form-group" >
                                <label htmlFor="separador">{t("PAR400_form_colname_separador")}</label>
                                <FieldSelect name="separador" />
                                <StatusMessage for="separador" />
                            </div>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="descripcion">{t("PAR400_Sec0_Info_Cabezal_descripcion")}</label>
                                <Field name="descripcion" />
                                <StatusMessage for="descripcion" />
                            </div>
                        </Col>
                    </Row>
                </Container>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("FAC251_frm1_btn_cerrar")} </Button>
                <SubmitButton id="btnSubmitConfirmar" variant="primary" label="FAC251_frm1_btn_confirmar" />

            </Modal.Footer>
        </Form>
    );
}

export const PAR400ModificacionAtributoModal = withPageContext(InternalPAR400ModificacionAtributoModal);