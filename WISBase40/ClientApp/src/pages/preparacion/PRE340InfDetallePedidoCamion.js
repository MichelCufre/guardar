import React, { useState } from 'react';
import { Form, Field, FieldSelect, SubmitButton, FormButton, StatusMessage } from '../../components/FormComponents/Form';
import { useTranslation } from 'react-i18next';
import { Modal, Button, Row, Col, Container, fieldset } from 'react-bootstrap';
import * as Yup from 'yup';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { withPageContext } from '../../components/WithPageContext';
function InternalPRE340InfDetallePedidoCamion(props) {

    const { t } = useTranslation();
    const fieldSetStyle = { border: "1px solid #ddd", margin: "2px" };
    const [CamionRowSelected, setCamionRowSelected] = useState("");
    const onAfterButtonAction = (data, nexus) => {

        if (data.parameters.some(x => x.id == "AUX_ROW_SELECTED_CD_CAMION")) {
            var valueCamionCont = data.parameters.find(x => x.id == "AUX_ROW_SELECTED_CD_CAMION").value;
            setCamionRowSelected(valueCamionCont);
            nexus.getGrid("PRE340_grid_1_Camiones").refresh();
            nexus.getGrid("PRE340_grid_2_ProductoCamiones").refresh();
        }
    }

    const onBeforeFetch = (context, data, nexus) => {

        props.addParameters(context, data, nexus);

        data.parameters.push(
            { id: "CD_CAMION", value: CamionRowSelected },

        );
    };

    const onAfterInitialize = (context, grid, parameters, nexus) => {
        var valueCamionCont = parameters.find(x => x.id == "AUX_ROW_SELECTED_CD_CAMION").value;
        setCamionRowSelected(valueCamionCont);
        nexus.getGrid("PRE340_grid_2_ProductoCamiones").refresh();
    };

    return (
        <Form
            id="PRE340_Forms2"
            application="PRE340InfDetallePedidoCamion"
            onBeforeInitialize={props.addParametersForm}
        >
            <Modal.Body>

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
                            <label className="form-control-sm" htmlFor="DS_CLIENTE">{t("PRE340_frm1_lbl_DS_CLIENTE")}</label>
                            <Field className="form-control-sm" name="DS_CLIENTE" isClearable={true} disabled={true} />
                            <StatusMessage for="DS_CLIENTE" />
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
                            <label className="form-control-sm" htmlFor="DS_EMPRESA">{t("PRE340_frm1_lbl_DS_EMPRESA")}</label>
                            <Field className="form-control-sm" name="DS_EMPRESA" isClearable={true} disabled={true} />
                            <StatusMessage for="DS_EMPRESA" />
                        </div>
                    </Col>
                </Row>
                <Container fluid>
                    <fieldset className="form-group border p-2">
                        <legend align="center" className="w-auto">{t("PRE340_grid1_title_CamionesPedido")}</legend>
                        <Row className='mt-3'>
                            <Col>
                                <Grid
                                    id="PRE340_grid_1_Camiones"
                                    rowsToFetch={10}
                                    rowsToDisplay={5}
                                    enableExcelExport
                                    onBeforeInitialize={props.addParameters}
                                    onBeforeApplyFilter={props.addParameters}
                                    onBeforeApplySort={props.addParameters}
                                    onBeforeFetch={onBeforeFetch}
                                    onAfterInitialize={onAfterInitialize}
                                    onAfterButtonAction={onAfterButtonAction}
                                    onBeforeExportExcel={props.addParameters}
                                    onBeforeFetchStats={props.addParameters}
                                    application="PRE340InfPedidoCamionGrid1"
                                />
                            </Col>
                        </Row>
                    </fieldset>
                </Container>

                <Container fluid>
                    <fieldset className="form-group border p-2" style={fieldSetStyle}>
                        <legend align="center" className="w-auto">{t("PRE340_grid2_title_ProductosDelCamion")}</legend>
                        <Row className='mt-3'>
                            <Col>
                                <Grid
                                    id="PRE340_grid_2_ProductoCamiones"
                                    rowsToFetch={10}
                                    rowsToDisplay={5}
                                    enableExcelExport
                                    onBeforeInitialize={props.addParameters}
                                    onBeforeApplyFilter={onBeforeFetch}
                                    onBeforeApplySort={onBeforeFetch}
                                    onBeforeFetch={onBeforeFetch}
                                    onBeforeExportExcel={props.addParameters}
                                    onBeforeFetchStats={props.addParameters}
                                    application="PRE340InfPedidoCamionGrid2"
                                />
                            </Col>
                        </Row>
                    </fieldset>
                </Container>

                <Container fluid>
                    <fieldset className="form-group border p-2" style={fieldSetStyle}>
                        <legend align="center" className="w-auto">{t("PRE340_grid3_title_ProductosSinCamion")}</legend>
                        <Row className='mt-3'>
                            <Col>
                                <Grid
                                    id="PRE340_grid_3_ProdSinCamion"
                                    rowsToFetch={15}
                                    rowsToDisplay={5}
                                    enableExcelExport
                                    onBeforeInitialize={props.addParameters}
                                    onBeforeApplyFilter={props.addParameters}
                                    onBeforeApplySort={props.addParameters}
                                    onBeforeFetch={props.addParameters}
                                    onBeforeExportExcel={props.addParameters}
                                    onBeforeFetchStats={props.addParameters}
                                    application="PRE340InfPedidoCamionGrid3"
                                />
                            </Col>
                        </Row>
                    </fieldset>
                </Container>
            </Modal.Body>

        </Form>

    );

}
export const PRE340InfDetallePedidoCamion = withPageContext(InternalPRE340InfDetallePedidoCamion);