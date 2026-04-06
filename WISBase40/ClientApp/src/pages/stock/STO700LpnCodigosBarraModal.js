import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { withPageContext } from '../../components/WithPageContext';
import { Form, Field, StatusMessage, SubmitButton, FormButton } from '../../components/FormComponents/Form';
import { Modal, Button, Row, Col, Container } from 'react-bootstrap';
import * as Yup from 'yup';
import { Grid } from '../../components/GridComponents/Grid';


function InternalSTO700LpnCodigosBarraModal(props) {

    const { t } = useTranslation();

    const initialValues = {
        numeroLpn: "",
        tipoLpn: "",
    };

    const handleClose = () => {
        props.onHide(null, props.nexus);
    };

    const applyParameters = (context, data, nexus) => {
        data.parameters = [
            { id: "numeroLpn", value: props.numeroLpn },
        ];
    };

    const handleFormBeforeInitialize = (context, form, query, nexus) => {
        query.parameters = [
            { id: "numeroLpn", value: props.numeroLpn },
        ];
    };

    return (

        <Form
            application="STO700ConsultaBarrasLpn"
            id="STO700_form_CodigosBarraModal"
            initialValues={initialValues}
            onBeforeInitialize={handleFormBeforeInitialize}
        >
            <Modal.Header closeButton>
                <Modal.Title>{t("STO700_Sec0_mdlEdit_ConsultaBarras")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Container fluid>
                    <Row>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="numeroLpn">{t("STO700_Sec0_mdl_NumeroLpn")}</label>
                                <Field name="numeroLpn" readOnly />
                                <StatusMessage for="numeroLpn" />
                            </div>
                        </Col>

                        <Col>
                            <div className="form-group" >
                                <label htmlFor="tipoLpn">{t("STO700_Sec0_mdl_TipoLpn")}</label>
                                <Field name="tipoLpn" />
                                <StatusMessage for="tipoLpn" />
                            </div>
                        </Col>
                    </Row>

                    <div className="row mb-4" >
                        <div className="col-12">
                            <Grid application="STO700ConsultaBarrasLpn"
                                id="STO700_grid_4"
                                rowsToFetch={30}
                                rowsToDisplay={15}
                                enableExcelExport={true}
                                onBeforeFetch={applyParameters}
                                onBeforeInitialize={applyParameters}
                                onBeforeExportExcel={applyParameters}
                                onBeforeFetchStats={applyParameters}
                                onBeforeApplyFilter={applyParameters}
                                onBeforeApplySort={applyParameters}
                            />
                        </div>
                    </div>
                </Container>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("STO700_frm1_btn_cerrar")} </Button>
            </Modal.Footer>
        </Form>
    );
}

export const STO700LpnCodigosBarraModal = withPageContext(InternalSTO700LpnCodigosBarraModal);