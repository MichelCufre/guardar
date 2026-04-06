import React, { useState } from 'react';
import { Form, Field, FieldSelect, SubmitButton, FormButton, StatusMessage } from '../../components/FormComponents/Form';
import { useTranslation } from 'react-i18next';
import { Modal, fieldset, Button, Row, Col, Container } from 'react-bootstrap';
import * as Yup from 'yup';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { withPageContext } from '../../components/WithPageContext';
function InternalPRE340InfDetallePedido(props) {

    const { t } = useTranslation();
    const handleFormAfterSubmit = (context, form, query, nexus) => {
        if (context.responseStatus === "OK") {
            props.onHide(query);
        }
    };
    return (
        <Form
            id="PRE340_Forms1"
            application="PRE340InfDetallePedido"
            onBeforeInitialize={props.addParametersForm}
            onAfterSubmit={handleFormAfterSubmit}
        >

            <Modal.Body>
                <div>

                    <Row>
                        <Col>
                            <div className="form-group">
                                <label className="form-control-sm" htmlFor="NU_PEDIDO">{t("PRE340_frm1_lbl_NU_PEDIDO")}</label>
                                <Field className="form-control-sm" name="NU_PEDIDO" isClearable={true} disabled={true} />
                                <StatusMessage for="NU_PEDIDO" />
                            </div>
                        </Col>
                        <Col>
                            <div className="form-group">
                                <label className="form-control-sm" htmlFor="TP_PEDIDO">{t("PRE340_frm1_lbl_TP_PEDIDO")}</label>
                                <Field className="form-control-sm" name="TP_PEDIDO" isClearable={true} disabled={true} />
                                <StatusMessage for="TP_PEDIDO" />
                            </div>
                        </Col>
                        <Col>
                            <div className="form-group">
                                <label className="form-control-sm" htmlFor="CD_SITUACAO">{t("PRE340_frm1_lbl_CD_SITUACAO")}</label>
                                <Field className="form-control-sm" name="CD_SITUACAO" isClearable={true} disabled={true} />
                                <StatusMessage for="CD_SITUACAO" />
                            </div>
                        </Col>
                        <Col>
                            <div className="form-group">
                                <label className="form-control-sm" htmlFor="AUX_CD_SITUACAO">{t("PRE340_frm1_lbl_DS_SITUACAO")}</label>
                                <Field className="form-control-sm" name="AUX_CD_SITUACAO" isClearable={true} disabled={true} />
                                <StatusMessage for="AUX_CD_SITUACAO" />
                            </div>
                        </Col>

                    </Row>
                    <Row>
                        <Col>
                            <div className="form-group">
                                <label className="form-control-sm" htmlFor="CD_CLIENTE">{t("PRE340_frm1_lbl_CD_CLIENTE")}</label>
                                <Field className="form-control-sm" name="CD_CLIENTE" isClearable={true} disabled={true} />
                                <StatusMessage for="CD_CLIENTE" />
                            </div>
                        </Col>
                        <Col>
                            <div className="form-group">
                                <label className="form-control-sm" htmlFor="AUX_CD_SITUACAO">{t("PRE340_frm1_lbl_DS_CLIENTE")}</label>
                                <Field className="form-control-sm" name="DS_CLIENTE" isClearable={true} disabled={true} />
                                <StatusMessage for="DS_CLIENTE" />
                            </div>
                        </Col>
                        <Col>
                            <div className="form-group">
                                <label className="form-control-sm" htmlFor="DT_ADDROW">{t("PRE340_frm1_lbl_DT_ADDROW")}</label>
                                <Field className="form-control-sm" name="DT_ADDROW" isClearable={true} disabled={true} />
                                <StatusMessage for="DT_ADDROW" />
                            </div>
                        </Col>
                        <Col>
                            <div className="form-group">
                                <label className="form-control-sm" htmlFor="CD_CONDICION_LIBERACION">{t("PRE340_frm1_lbl_CONDICION_LIB")}</label>
                                <Field className="form-control-sm" name="CD_CONDICION_LIBERACION" isClearable={true} disabled={true} />
                                <StatusMessage for="CD_CONDICION_LIBERACION" />
                            </div>
                        </Col>


                    </Row>
                    <Row>
                        <Col>
                            <div className="form-group">
                                <label className="form-control-sm" htmlFor="CD_EMPRESA">{t("PRE340_frm1_lbl_CD_EMPRESA")}</label>
                                <Field className="form-control-sm" name="CD_EMPRESA" isClearable={true} disabled={true} />
                                <StatusMessage for="CD_EMPRESA" />
                            </div>
                        </Col>
                        <Col>
                            <div className="form-group">
                                <label className="form-control-sm" htmlFor="AUX_CD_SITUACAO">{t("PRE340_frm1_lbl_DS_EMPRESA")}</label>
                                <Field className="form-control-sm" name="DS_EMPRESA" isClearable={true} disabled={true} />
                                <StatusMessage for="DS_EMPRESA" />
                            </div>
                        </Col>
                        <Col>
                            <div className="form-group">
                                <label className="form-control-sm" htmlFor="DT_ENTREGA">{t("PRE340_frm1_lbl_DT_ENTREGA")}</label>
                                <Field className="form-control-sm" name="DT_ENTREGA" isClearable={true} disabled={true} />
                                <StatusMessage for="DT_ENTREGA" />
                            </div>
                        </Col>
                        <Col>
                            <div className="form-group">
                                <label className="form-control-sm" htmlFor="NU_ULT_PREPARACION">{t("PRE340_frm1_lbl_NU_ULT_PREPARACION")}</label>
                                <Field className="form-control-sm" name="NU_ULT_PREPARACION" isClearable={true} disabled={true} />
                                <StatusMessage for="NU_ULT_PREPARACION" />
                            </div>
                        </Col>


                    </Row>
                    <Row>
                        <Col>
                            <div className="form-group">
                                <label className="form-control-sm" htmlFor="DS_ENDERECO">{t("PRE340_frm1_lbl_DS_ENDERECO")}</label>
                                <Field className="form-control-sm" name="DS_ENDERECO" isClearable={true} disabled={true} />
                                <StatusMessage for="DS_ENDERECO" />
                            </div>
                        </Col>

                        <Col>
                            <div className="form-group">
                                <label className="form-control-sm" htmlFor="CD_ROTA">{t("PRE340_frm1_lbl_CD_ROTA")}</label>
                                <Field className="form-control-sm" name="CD_ROTA" isClearable={true} disabled={true} />
                                <StatusMessage for="CD_ROTA" />
                            </div>
                        </Col>
                        <Col>
                            <div className="form-group">
                                <label className="form-control-sm" htmlFor="AUX_CD_ROTA">{t("PRE340_frm1_lbl_DS_ROTA")}</label>
                                <Field className="form-control-sm" name="AUX_CD_ROTA" isClearable={true} disabled={true} />
                                <StatusMessage for="AUX_CD_ROTA" />
                            </div>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <div className="form-group">
                                <label className="form-control-sm" htmlFor="DS_ANEXO4">{t("PRE340_frm1_lbl_DS_ANEXO4")}</label>
                                <Field className="form-control-sm" name="DS_ANEXO4" isClearable={true} disabled={true} />
                                <StatusMessage for="DS_ANEXO4" />
                            </div>
                        </Col>
                        <Col >
                            <div className="form-group">
                                <label className="form-control-sm" htmlFor="CD_TRANSPORTISTA">{t("PRE340_frm1_lbl_CD_TRANSPORTISTA")}</label>
                                <Field className="form-control-sm" name="CD_TRANSPORTISTA" isClearable={true} disabled={true} />
                                <StatusMessage for="CD_TRANSPORTISTA" />
                            </div>
                        </Col>
                        <Col>
                            <div className="mt-3">
                                <SubmitButton className="form-control-sm summit mt-4" id="btnSubmit" variant="primary" label="PRE340_frm_btn_InfoCamiones" />
                            </div>
                        </Col>

                    </Row>
                </div>
                <Container fluid>
                    <fieldset className="form-group border p-2 grid" >
                        <legend align="center" className="w-auto">{t("PRE340Seleccion_grid1_title_DetalleDelPedido")}</legend>
                        <Row className='mt-3'>
                            <Col>
                                <Grid
                                    id="PRE340_grid_1_PedidoDetalle"
                                    rowsToFetch={10}
                                    rowsToDisplay={5}
                                    enableExcelExport
                                    onBeforeInitialize={props.addParameters}
                                    onBeforeFetch={props.addParameters}
                                    onBeforeApplyFilter={props.addParameters}
                                    onBeforeApplySort={props.addParameters}
                                    onBeforeExportExcel={props.addParameters}
                                    onBeforeFetchStats={props.addParameters}
                                    application="PRE340InfDetallePedidoGrid1"
                                />
                            </Col>
                        </Row>
                    </fieldset>
                </Container>
                <Container fluid>
                    <fieldset className="form-group border p-2 grid" >
                        <legend align="center" className="w-auto">{t("PRE340Seleccion_grid2_title_ContenedoresDelPedido")}</legend>
                        <Row className='mt-3'>
                            <Col>
                                <Grid
                                    id="PRE340_grid_2_ContenedorPedido"
                                    rowsToFetch={10}
                                    rowsToDisplay={5}
                                    enableExcelExport
                                    onBeforeInitialize={props.addParameters}
                                    onBeforeFetch={props.addParameters}
                                    onBeforeApplyFilter={props.addParameters}
                                    onBeforeApplySort={props.addParameters}
                                    onBeforeExportExcel={props.addParameters}
                                    onBeforeFetchStats={props.addParameters}
                                    application="PRE340InfDetallePedidoGrid2"
                                />
                            </Col>
                        </Row>
                    </fieldset>
                </Container>

            </Modal.Body>
        </Form>

    );

}
export const PRE340InfDetallePedido = withPageContext(InternalPRE340InfDetallePedido);