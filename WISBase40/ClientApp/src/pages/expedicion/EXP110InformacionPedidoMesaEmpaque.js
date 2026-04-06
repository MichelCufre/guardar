import React, { useState } from 'react';
import { Form, Field, FieldSelect, SubmitButton, FormButton, StatusMessage } from '../../components/FormComponents/Form';
import { useTranslation } from 'react-i18next';
import { Modal, Button, Row, Col, Container } from 'react-bootstrap';
import Table from 'react-bootstrap/Table';
import * as Yup from 'yup';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { withPageContext } from '../../components/WithPageContext';
function InternalEXP110PedidoMesaEmpaqueModal(props) {

    const { t } = useTranslation();

    const onAfterButtonAction = (context, data, nexus) => {
        if (context.parameters.some(x => x.id == "BTNID") && context.parameters.find(x => x.id == "BTNID").value == "btnInfoPedido") {

            props.onHide(context, data, props.nexus);

        } else {
            data.getGrid("EXP110InfPedidoEmpaque_grid_1").refresh();
        }

    };

    const blueStyle = {
        backgroundColor: "#9cc1ff",
        color: "black",
        fontFamily: "PTSans"
    };

    const redStyle = {
        backgroundColor: "#EC7063",
        color: "black",
        fontFamily: "PTSans"
    };

    const greenStyle = {
        backgroundColor: "#7fff54",
        color: "black",
        fontFamily: "PTSans"
    };

    const yellowStyle = {
        backgroundColor: "#f4f870",
        color: "black",
        fontFamily: "PTSans"
    };

    return (
        <Modal.Body>
            <Form
            >
                <div className="col-12">
                    <Row>
                        <Col>
                            <h5><span class="badge" style={greenStyle}>{t("EXP110_grid1_lbl_PedidoCompletador")}</span></h5>
                        </Col>
                        <Col>
                            <h5><span class="badge" style={yellowStyle}>{t("EXP110_grid1_lbl_PedidoNoEstaTotalmentePreparado")}</span></h5>
                        </Col>
                        <Col>
                            <h5><span class="badge" style={redStyle}>{t("EXP110_grid1_lbl_PedidoNoLiberadoCompletamente")}</span></h5>
                        </Col>
                        <Col>
                            <h5><span class="badge" style={blueStyle}>{t("EXP110_grid1_lbl_PedidoNoEmpaquetadoCompletamente")}</span></h5>
                        </Col>
                    </Row>

                    <Row className='mt-3'>
                        <Col>
                            <Grid
                                application="EXP110InfPedidoEmpaque"
                                id="EXP110InfPedidoEmpaque_grid_1"
                                rowsToFetch={20}
                                rowsToDisplay={10}
                                enableExcelExport
                                onBeforeInitialize={props.addParameters}
                                onBeforeFetch={props.addParameters}
                                onBeforeApplySort={props.addParameters}
                                onBeforeApplyFilter={props.addParameters}
                                onAfterButtonAction={onAfterButtonAction}
                                onBeforeExportExcel={props.addParameters}
                                onBeforeFetchStats={props.addParameters}
                            />
                        </Col>
                    </Row>
                </div>
            </Form >
        </Modal.Body >
    );

}
export const EXP110InformacionPedidoMesaEmpaque = withPageContext(InternalEXP110PedidoMesaEmpaqueModal);