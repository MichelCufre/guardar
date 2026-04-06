import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { withPageContext } from '../../components/WithPageContext';
import { Form, Field, FieldSelect, FieldSelectAsync, FieldTextArea, StatusMessage, SubmitButton, FieldDate, FieldDateTime } from '../../components/FormComponents/Form';
import { Modal, Button, Row, Col, Tab, Tabs, Container } from 'react-bootstrap';
import * as Yup from 'yup';

function InternalCON010ParametrosConfig(props) {

    const { t } = useTranslation();

    const handleClose = () => {
        props.onHide(props.nexus);
    };

    const applyParameters = (context, data, nexus) => {
        if (props.ParametroConfiguracion) {

            let parameters =
                [
                    { id: "cdParametro", value: props.ParametroConfiguracion.find(x => x.id === "cdParametro").value },
                    { id: "doEntidad", value: props.ParametroConfiguracion.find(x => x.id === "doEntidad").value },
                    { id: "dsDominio", value: props.ParametroConfiguracion.find(x => x.id === "dsDominio").value },
                ];

            data.parameters = parameters;
        }
    };

    const handleBeforeSelectSearch = (context, row, query, nexus) => {
        query.parameters = [
            { id: "cdParametro", value: props.ParametroConfiguracion.find(x => x.id === "cdParametro").value },
            { id: "doEntidad", value: props.ParametroConfiguracion.find(x => x.id === "doEntidad").value },
            { id: "dsDominio", value: props.ParametroConfiguracion.find(x => x.id === "dsDominio").value },
        ];
    }

    const onBeforeCommit = (context, data, nexus) => {
        for (var i = 0; i < data.rows.length; i++) {
            var aux = 0;
            var valorEntidad = data.rows[i].cells.find(a => a.column === "ND_ENTIDAD").value;

            for (var y = 0; y < data.rows.length; y++) {
                if (data.rows[y].cells.find(a => a.column === "ND_ENTIDAD").value == valorEntidad) {
                    aux++;
                    if (aux > 1) {
                        context.abortServerCall = true;
                        throw new Error(t("General_Sec0_Error_Er006_LineasDuplicadas"));
                    }
                }
            }
        }
        data.parameters = [
            { id: "cdParametro", value: props.ParametroConfiguracion.find(x => x.id === "cdParametro").value },
            { id: "doEntidad", value: props.ParametroConfiguracion.find(x => x.id === "doEntidad").value },
            { id: "dsDominio", value: props.ParametroConfiguracion.find(x => x.id === "dsDominio").value },
        ];
    }

    return (
         <Modal show={props.show} onHide={handleClose} dialogClassName="modal-90w" backdrop="static">
            <Modal.Header closeButton>
                <Modal.Title>{t("CON010ParametrosConfig_Sec0_modalTitle_Titulo")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Grid
                    application="CON010ParametrosConfig"
                    id="CON010ParametrosConfig_grid_1"
                    rowsToFetch={30}
                    rowsToDisplay={15}
                    enableExcelExport={true}
                    enableExcelImport={false}
                    notifySelection={true}
                    onBeforeInitialize={applyParameters}
                    onBeforeFetch={applyParameters}
                    onBeforeExportExcel={applyParameters}
                    onBeforeCommit={onBeforeCommit}
                    onBeforeButtonAction={applyParameters}
                    onBeforeSelectSearch={handleBeforeSelectSearch}
                    onBeforeApplyFilter={applyParameters}
                    onBeforeApplySort={applyParameters}
                    onAfterValidateRow={applyParameters}
                    />
            </Modal.Body>
        </Modal>
    );
}

export const CON010ParametrosConfig = withPageContext(InternalCON010ParametrosConfig);