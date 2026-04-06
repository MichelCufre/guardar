import { forEach } from 'lodash';
import React, { useState } from 'react';
import { Button, Col, Modal, Row } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { Field, FieldSelect, FieldSelectAsync, Form, StatusMessage } from '../../components/FormComponents/Form';
import { notificationType } from '../Enums';
import { withPageContext } from '../WithPageContext';
import withToaster from '../WithToaster';
import { GridImportExcelButtonAPI } from './GridImportExcelButtonAPI';

function GridImportExcelModalAPIInternal(props) {
    const { t } = useTranslation("translation", { useSuspense: false });
    const [isDraggingOver, setDragOver] = useState(false);
    const [isUploading, setUploading] = useState(false);
    const [api, setApi] = useState(500);
    const [empresa, setEmpresa] = useState(null);
    const [referencia, setReferencia] = useState("");

    const handleClose = () => {
        if (!isUploading)
            props.closeImportExcelModal();
    };

    const handleDrop = (e) => {
        e.preventDefault();
        e.stopPropagation();

        setDragOver(false);

        var f = e.dataTransfer.files[0];

        if (f.type !== "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") {
            props.toaster.toastError(t('IExcel_Modal_lbl_InvalidDataType'));
            return false;
        }

        var reader = new FileReader();

        setUploading(true);

        reader.onload = function (e) {

            props.importExcel(f.name, reader.result.replace(/^data:.+;base64,/, ''), api, empresa, referencia)
                .then((response) => {
                    setUploading(false);

                    if (response && response.Status === "OK")
                        props.closeImportExcelModal();
                });

            //TODO: Ver de mostrar el mensaje de error dentro del cuadro de subida para mejor UX
        };

        reader.readAsDataURL(f);
    }

    const handleClickDownloadTemplate = (e) => {
        console.log(api);
        e.preventDefault();
        props.generateExcelTemplate(api);
    }

    const handleDragOver = (e, context, data, nexus) => {
        setDragOver(true);
        e.preventDefault();
    }

    const handleDragLeave = (e) => {
        setDragOver(false);
        e.preventDefault();
    }

    const handleChange = (e) => {
        e.preventDefault();
        e.stopPropagation();
        const f = e.target.files[0];
        processFiles(f);
    }

    const processFiles = (file) => {
        setDragOver(false);
        console.log(file);

        /*if (f.type !== "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") {
            props.nexus.toast(notificationType.error, t('Tipo de archivo no válido'));
            return false;
        }*/

        const extension = file.name.substr(file.name.lastIndexOf('.') + 1)

        if (extension !== "xls" && extension !== "xlsx") {
            props.nexus.toast(notificationType.error, t('Tipo de archivo no válido'));
            return false;
        }

        const reader = new FileReader();

        setUploading(true);

        reader.onload = function (e) {

            props.importExcel(file.name, reader.result.replace(/^data:.+;base64,/, ''), api, empresa, referencia)
                .then((response) => {
                    setUploading(false);

                    if (response && response.Status === "OK")
                        props.closeImportExcelModal();
                });

            //TODO: Ver de mostrar el mensaje de error dentro del cuadro de subida para mejor UX
        };

        reader.readAsDataURL(file);
    }
    const handleDrag = (e) => {
        e.stopPropagation();
        e.preventDefault();
    }

    const initialValues = {

        api: "500",
        empresa: "",
        referencia: null,

    };

    const validationSchema = {

        api: Yup.string().required(),
        empresa: Yup.string().required(),
        referencia: Yup.string().required()
    };

    const handleFormAfterInitialize = (context, form, query, nexus) => {
        var api = nexus.getForm("INT050_form_1").getFieldValue("api");
        setApi(api);

        var empresa = query.parameters.find(w => w.id === "empresaUnica").value;
        setEmpresa(empresa);

        forEach(nexus.getForm("INT050_form_1").getFieldOptions("api"), option => {
            option.label = t(option.label);
        });

        console.log(api + ' - ' + empresa);
    };

    const handleFormAfterValidateField = (context, form, query, nexus) => {
        var api = nexus.getForm("INT050_form_1").getFieldValue("api");
        var empresa = nexus.getForm("INT050_form_1").getFieldValue("empresa");
        var referencia = nexus.getForm("INT050_form_1").getFieldValue("referencia");
        setApi(api);
        setEmpresa(empresa);
        setReferencia(referencia);
        console.log(api + ' - ' + empresa + ' - ' + referencia);
    }

    return (
        <Modal show={props.isImportExcelModalAPIOpen} size="lg" onHide={handleClose} className="gr-import-excel">
            <Modal.Header closeButton>
                <Modal.Title>{t("IExcel_Modal_lbl_Title")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Form
                    application="INT050"
                    id="INT050_form_1"
                    initialValues={initialValues}
                    validationSchema={validationSchema}
                    onAfterValidateField={handleFormAfterValidateField}
                    onAfterInitialize={handleFormAfterInitialize}
                >
                    <Row >
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="api">{t("APIs")}</label>
                                <FieldSelect name="api" />
                                <StatusMessage for="api" />
                            </div>
                        </Col>
                    </Row>
                    <Row className="mb-2">
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="empresa">{t("REC170_frm1_lbl_idEmpresa")}</label>
                                <FieldSelectAsync name="empresa" />
                                <StatusMessage for="empresa" />
                            </div>
                        </Col>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="referencia">{t("Referencia")}</label>
                                <Field name="referencia" maxLength="20" />
                                <StatusMessage for="referencia" />
                            </div>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <Button variant="outline-primary" block onClick={handleClickDownloadTemplate}>{t("IExcel_Modal_lbl_DownloadTemplate")}</Button>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <GridImportExcelButtonAPI
                                onDragOver={handleDragOver}
                                onDragLeave={handleDragLeave}
                                onDragEnter={handleDrag}
                                onDragEnd={handleDrag}
                                onDrop={handleDrop}
                                onChange={handleChange}
                                isDraggingOver={isDraggingOver}
                                isUploading={isUploading}
                            />
                        </Col>
                    </Row>
                </Form >
            </Modal.Body>
            <Modal.Footer>
                <Button variant="secondary" onClick={handleClose}>
                    {t("General_Sec0_btn_LOAD_FILTER_CLOSE")}
                </Button>
            </Modal.Footer>
        </Modal >
    );
}

export const GridImportExcelModalAPI = withToaster(withPageContext(GridImportExcelModalAPIInternal));