import React, { useState, useEffect } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { withPageContext } from '../../components/WithPageContext';
import { Form, Field, FieldSelect, FieldCheckbox, FieldSelectAsync, FieldTextArea, StatusMessage, SubmitButton, FieldDate, FieldDateTime } from '../../components/FormComponents/Form';
import { Modal, Button, Row, Col, Tab, Tabs, Container, Div } from 'react-bootstrap';
import * as Yup from 'yup';
import { CheckboxList } from '../../components/CheckboxList';

function InternalSEG030CreateModal(props) {
    const { t } = useTranslation();
    const [editarEnable, setEditarEnable] = useState(false);
    const [mostrarAsignarAutoEmpresas, setMostrarAsignarAutoEmpresas] = useState(false);
    const [opcional, setOpcional] = useState(null);
    const [itemsList, setItemsList] = useState([]);
    const [allSelectedDisponibles, setAllSelectedDisponibles] = useState(false);

    const validationSchema = {

        nomUsuario: Yup.string().required(),
        nomCompleto: Yup.string().required(),
        email: Yup.string().required(),
        tipoUsu: Yup.string().required(),
        idioma: Yup.string(),
        asignarAutoEmpresas: Yup.string()
    };

    const handleFormAfterSubmit = (context, form, query, nexus) => {

        context.showErrorMessage = true;
        if (context.responseStatus === "OK") {
            if (query.notifications && query.notifications.find(x => x.type === 2)) {
                if (query.buttonId == "btnSubmitConfirmarRecursos") {

                    props.onHide(query.parameters, null, props.nexus);

                } else if (query.buttonId == "btnSubmitConfirmarAsignar") {

                    props.onHide(query.parameters, true, props.nexus);

                } else {

                    props.onHide(null, null, props.nexus);
                }
            } else {

                props.onHide(null, null, props.nexus);
            }
        }
    }

    const handleFormBeforeInitialize = (context, form, query, nexus) => {

        if (props.usuario) {

            query.parameters.push({ id: "idUsuario", value: props.usuario.find(x => x.id === "idUsuario").value });

            if (props.accionEditar) {
                query.parameters.push({ id: "btnEditar", value: "true" })
                setEditarEnable(true);
            }
        }

    }

    const handleFormBeforeSubmit = (context, form, query, nexus) => {
        if (props.usuario) {
            query.parameters = [(
                { id: "idUsuario", value: props.usuario.find(x => x.id === "idUsuario").value }
            )];
        }

        query.parameters.push({ id: "listaPerfiles", value: JSON.stringify(itemsList) },
            { id: "editarUsuario", value: editarEnable });
    }

    const handleAfterValidate = (context, form, query, nexus) => {

    }

    const handleBeforeValidate = (context, form, query, nexus) => {
        if (props.usuario)
            query.parameters.push({ id: "idUsuario", value: props.usuario.find(x => x.id === "idUsuario").value });

    }

    const handleOnAfterInitialize = (context, form, query, nexus) => {

        let jsonAdded = query.parameters.find(w => w.id === "ListItems").value;
        var arrayAdded = JSON.parse(jsonAdded.toString());
        setItemsList(arrayAdded);

        let jsonParameterPermiso = query.parameters.find(w => w.id === "modificarAsignarAutoEmpresas").value;
        var permiso = JSON.parse(jsonParameterPermiso.toString());
        setMostrarAsignarAutoEmpresas(permiso);
    }

    const handleClose = () => {
        props.onHide(null, null, props.nexus);
    };

    const handleChange = (event, id) => {

        let itemsModificar = [...itemsList];

        let item = itemsModificar.find(d => d.id === id);

        item.selected = !item.selected;

        setItemsList(itemsModificar);
    };

    const handleSelectAllDisponibles = (selected) => {
        setAllSelectedDisponibles(selected);

        setItemsList(itemsList.map(d => ({ ...d, selected: selected })));
    }

    return (

        <Form
            application="SEG030"
            id="SEG030_form_1"
            validationSchema={validationSchema}
            onAfterSubmit={handleFormAfterSubmit}
            onBeforeSubmit={handleFormBeforeSubmit}
            onBeforeInitialize={handleFormBeforeInitialize}
            onAfterInitialize={handleOnAfterInitialize}
            onAfterValidateField={handleAfterValidate}
            onBeforeValidateField={handleBeforeValidate}
        >
            <Modal.Header closeButton>
                <Modal.Title>{editarEnable ? t("SEG030_Sec0_mdlCreate_TituloEdit") : t("SEG030_Sec0_mdlCreate_Titulo")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Container fluid>
                    <Row>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="nomUsuario">{t("SEG030_frm1_lbl_nomUsuario")}</label>
                                <Field name="nomUsuario" />
                                <StatusMessage for="nomUsuario" />
                            </div>
                            <div className="form-group" >
                                <label htmlFor="nomCompleto">{t("SEG030_frm1_lbl_nomCompleto")}</label>
                                <Field name="nomCompleto" />
                                <StatusMessage for="nomCompleto" />
                            </div>
                            <div className="form-group" >
                                <label htmlFor="email">{t("SEG030_frm1_lbl_email")}</label>
                                <Field name="email" />
                                <StatusMessage for="email" />
                            </div>
                            <div className="form-group" >
                                <label htmlFor="tipoUsu">{t("SEG030_frm1_lbl_tipoUsu")}</label>
                                <FieldSelect name="tipoUsu" />
                                <StatusMessage for="tipoUsu" />
                            </div>
                            <div className="form-group" >
                                <label htmlFor="idioma">{t("SEG030_frm1_lbl_idioma")}</label>
                                <FieldSelect name="idioma" />
                                <StatusMessage for="idioma" />
                            </div>
                        </Col>
                        <Col>
                            <div style={{ display: mostrarAsignarAutoEmpresas ? 'block' : 'none' }}>
                                <label htmlFor="asignarAutoEmpresas">{t("SEG030_frm1_lbl_asignarAutoEmpresas")}</label>
                                <FieldSelect name="asignarAutoEmpresas" />
                                <StatusMessage for="asignarAutoEmpresas" />
                            </div>

                            <label>{t("SEG030_frm1_lbl_selectPerfiles")}</label>
                            <CheckboxList
                                items={itemsList}
                                onChange={handleChange}
                                maxHeight={opcional}
                                className={opcional}
                                allSelected={allSelectedDisponibles}
                                onSelectAllChange={handleSelectAllDisponibles}
                            />
                        </Col>
                    </Row>
                </Container>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("SEG030_frm1_btn_cerrar")} </Button>
                <SubmitButton id="btnSubmitConfirmar" variant="primary" label="SEG030_frm1_btn_confirmar" />
                {/*<SubmitButton id="btnSubmitConfirmarRecursos" variant="primary" label="SEG030_frm1_btn_confirmarRecursos" />*/}
                {/*<SubmitButton id="btnSubmitConfirmarAsignar" variant="primary" label="SEG030_frm1_btn_confirmarAsignar" />*/}
            </Modal.Footer>
        </Form>
    );
}

export const SEG030CreateModal = withPageContext(InternalSEG030CreateModal);