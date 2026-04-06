import React, { useState, useEffect } from 'react';
import { Button, Col, Modal, Row } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { Field, FieldSelect, FieldSelectAsync, FieldToggle, Form, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';
import { Grid } from '../../components/GridComponents/Grid';
import { notificationType } from '../../components/Enums';

export function EVT040ModificarInstanciaModal(props) {
    const { t } = useTranslation();

    const [nexus, setNexus] = useState(null);
    const [eventOptions, setEventOptions] = useState([]);
    const [notificationTypeOptions, setNotificationTypeOptions] = useState([]);
    const [templateOptions, setTemplateOptions] = useState([]);
    const [evento, setEvento] = useState("");
    const [tipoNotificacion, setTipoNotificacion] = useState("");

    const [tipoAgente, setTipoAgente] = useState(null);
    const [empresa, setEmpresa] = useState(null);

    const initialValues = {
        evento: "",
        instancia: "",
        descripcion: "",
        tipoNotificacion: "",
        plantilla: "",
        habilitado: true,

    };

    const validationSchema = {
        evento: Yup.string().required(),
        instancia: Yup.string().required(),
        descripcion: Yup.string().required(),
        tipoNotificacion: Yup.string(),
        plantilla: Yup.string(),
        habilitado: Yup.boolean().required(),
    };

    const handleClose = () => {
        setNexus(null);
        setEvento(null);
        setTipoNotificacion(null);

        props.onHide();
    };

    useEffect(() => {
        if (nexus && evento && tipoNotificacion) {
            nexus.getGrid("EVT040_grid_Parametros").refresh();
        }
    }, [evento, tipoNotificacion]);

    const handleFormBeforeInitialize = (context, form, query, nexus) => {
        let parameters = [
            { id: "instancia", value: props.instancia }
        ];

        query.parameters = parameters;
    }

    const handleFormAfterInitialize = (context, form, query, nexus) => {
        setNexus(nexus);
        setEvento(form.fields.find(f => f.id === "evento").value);
        setTipoNotificacion(form.fields.find(f => f.id === "tipoNotificacion").value);
    }

    const handleFormAfterValidateField = (context, form, query, nexus) => {
        setNexus(nexus);
        if (query.fieldId === "evento") {
            setEvento(form.fields.find(f => f.id === query.fieldId).value);
        } else if (query.fieldId === "tipoNotificacion") {
            setTipoNotificacion(form.fields.find(f => f.id === query.fieldId).value);
        }
    };

    const handleFormAfterSubmit = (context, form, query, nexus) => {
        var grid = nexus.getGrid("EVT040_grid_Parametros");

        if (context.responseStatus === "OK") {
            if (grid.hasError()) {
                context.abortServerCall = true;

                nexus.toast(notificationType.error, "General_Sec0_Error_Error07");

                return false;
            } else {
                grid.commit(true, true);
            }
        }
    };

    const onAfterInitialize = (context, form, parameters, nexus) => {
        setNexus(nexus);
    }

    const onAfterCommit = (context, rows, parameters, nexus) => {
        if (context.status === "ERROR") return;

        setNexus(null);
        setEvento(null);
        setTipoNotificacion(null);

        props.onHide();
    };

    const applyParameters = (context, data, nexus) => {
        const form = nexus.getForm("EVT040_form_ModificarInstancia");
        data.parameters = [
            { id: "plantilla", value: form.getFieldValue("plantilla") },
            { id: "instancia", value: form.getFieldValue("instancia") },
            { id: "descripcion", value: form.getFieldValue("descripcion") },
            { id: "habilitado", value: form.getFieldValue("habilitado") },
            { id: "evento", value: evento },
            { id: "tipoNotificacion", value: tipoNotificacion },
            { id: "tipoAgente", value: tipoAgente },
            { id: "empresa", value: empresa },
        ];
    };

    const onBeforeValidateRow = (context, data, nexus) => {
        var rows = nexus.getGrid("EVT040_grid_Parametros").getAllRows();

        var manejaParametroTipoAgente = rows.find(r => r.id === 'TP_AGENTE') ? true : false;
        var manejaParametroEmpresa = rows.find(r => r.id === 'CD_EMPRESA') ? true : false;

        if (manejaParametroTipoAgente) {
            var tipoAgente = rows.find(r => r.id === 'TP_AGENTE').cells.find(c => c.column === 'VL_PARAMETRO').value;
            setTipoAgente(tipoAgente);
        }

        if (manejaParametroEmpresa) {
            var empresa = rows.find(r => r.id === 'CD_EMPRESA').cells.find(c => c.column === 'VL_PARAMETRO').value;
            setEmpresa(empresa);
        }

        let parameters = [
            {
                id: "empresa",
                value: empresa
            },
            {
                id: "tipoAgente",
                value: tipoAgente
            }
        ];

        data.parameters = parameters;
    }

    const onBeforeSubmit = (context, form, query, nexus) => {
        var grid = nexus.getGrid("EVT040_grid_Parametros");
        var rows = nexus.getGrid("EVT040_grid_Parametros").getAllRows();

        rows.forEach((row) => {
            row.isModified = true;
            grid.validateRow(row);
        });

        query.parameters.push({ id: "isSubmit", value: true });
    }

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-50w" backdrop="static">
            <Form
                application="EVT040ModificarInstancia"
                id="EVT040_form_ModificarInstancia"
                initialValues={initialValues}
                validationSchema={validationSchema}
                onBeforeInitialize={handleFormBeforeInitialize}
                onAfterInitialize={handleFormAfterInitialize}
                onAfterValidateField={handleFormAfterValidateField}
                onAfterSubmit={handleFormAfterSubmit}
                onBeforeSubmit={onBeforeSubmit}
            >
                <Modal.Header closeButton>
                    <Modal.Title>{t("EVT040_Sec0_modalTitle_TituloModificar")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Row>
                        <Col lg={4}>
                            <div className="form-group">
                                <label htmlFor="instancia">{t("EVT040_frm_lbl_Instancia")}</label>
                                <Field name="instancia" readOnly />
                            </div>
                        </Col>
                        <Col lg={8}>
                            <div className="form-group">
                                <label htmlFor="descripcion">{t("EVT040_frm_lbl_Descripcion")}</label>
                                <Field name="descripcion" />
                                <StatusMessage for="descripcion" />
                            </div>
                        </Col>
                    </Row>
                    <Row>
                        <Col lg={4}>
                            <div className="form-group">
                                <label htmlFor="evento">{t("EVT040_frm_lbl_Evento")}</label>
                                <FieldSelect name="evento" readOnly />
                                <StatusMessage for="evento" />
                            </div>
                        </Col>
                        <Col lg={4}>
                            <div className="form-group">
                                <label htmlFor="tipoNotificacion">{t("EVT040_frm_lbl_Notificacion")}</label>
                                <FieldSelect name="tipoNotificacion" readOnly />
                                <StatusMessage for="tipoNotificacion" />
                            </div>
                        </Col>
                        <Col lg={4}>
                            <div className="form-group">
                                <label htmlFor="plantilla">{t("EVT040_frm_lbl_Plantilla")}</label>
                                <FieldSelect name="plantilla" />
                                <StatusMessage for="plantilla" />
                            </div>
                        </Col>
                    </Row>
                    <Row>
                        <Col lg={4}>
                            <div className="form-group" style={{ marginTop: "30px" }}>
                                <FieldToggle name="habilitado" label={t("EVT040_frm_lbl_Habilitado")} />
                                <StatusMessage for="habilitado" />
                            </div>
                        </Col>
                    </Row>
                    <Row>
                        <Col lg={12}>
                            <Grid id="EVT040_grid_Parametros"
                                application="EVT040ModificarInstancia"
                                rowsToFetch={30}
                                rowsToDisplay={7}
                                validateAllRows
                                enableExcelExport
                                onAfterInitialize={onAfterInitialize}
                                onBeforeFetch={applyParameters}
                                onAfterCommit={onAfterCommit}
                                onBeforeCommit={applyParameters}
                                onBeforeFetchStats={applyParameters}
                                onBeforeApplyFilter={applyParameters}
                                onBeforeApplySort={applyParameters}
                                onBeforeExportExcel={applyParameters}
                                onBeforeValidateRow={onBeforeValidateRow}
                            />
                        </Col>
                    </Row>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="btn btn-outline-secondary" onClick={handleClose}>
                        {t("REG240_frm_btn_Cancelar")}
                    </Button>
                    <SubmitButton id="btnSubmit" variant="primary" label="EVT040_frm_btn_Confirmar" />
                </Modal.Footer>
            </Form>
        </Modal>
    );
}
