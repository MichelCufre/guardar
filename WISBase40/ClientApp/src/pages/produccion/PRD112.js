import React, { useState } from 'react';
import { Col, Row } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { Field, Form, SubmitButton } from '../../components/FormComponents/Form';
import { FormWarningMessage } from '../../components/FormComponents/FormWarningMessage';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import '../../css/RowColors.css';
import { PRD112PedidosModal } from "./PRD112PedidosModal";
export default function PRD112(props) {
    const { t } = useTranslation();

    const [idIngreso, setIdIngreso] = useState(null);
    const [tieneEspacioAsignado, setTieneEspacoAsignado] = useState(false);
    const [isPedidosModalOpen, setIsPedidosModalOpen] = useState(false);
    const [clickButtonId, setClickButtonId] = useState("");
    const [empresa, setEmpresa] = useState("");

    const handleFormBeforeInitialize = (context, form, query, nexus) => { query.parameters = [{ id: "idIngreso", value: idIngreso }] }

    const handlePageAfterLoad = (data) => {

        var nuIngreso = data.parameters.find(x => x.id == "nuIngresoProduccion").value;
        var tieneEspacioAsignado = data.parameters.find(x => x.id == "tieneEspacioAsignado").value;

        setIdIngreso(nuIngreso);
        setTieneEspacoAsignado(tieneEspacioAsignado == "S");
    }

    const applyGridParameters = (context, data, nexus) => {

        data.parameters =
            [{ id: "idIngreso", value: idIngreso }]
    }

    const onBeforeCommit = (context, data, nexus) => {

        data.parameters = [
            { id: "idIngreso", value: idIngreso },
            { id: "btnGenerarPedidoLiberar", value: clickButtonId === "btnGenerarPedidoLiberar" ? "S" : "N" },
            { id: "btnGenerarPedido", value: clickButtonId === "btnGenerarPedido" ? "S" : "N" },
            { id: "isSubmit", value: "S" },
            { id: "empresa", value: empresa }
        ]

    }
    const applyGridParametersCommit = (context, rows, params, nexus) => {
        nexus.getGrid("PRD112_grid_1").refresh();
    }

    const onBeforeSubmit = (context, form, data, nexus) => {
        if (data.buttonId !== "btnVerPedidos") {
            if (tieneEspacioAsignado) {
                setClickButtonId(data.buttonId);
                nexus.getGrid("PRD112_grid_1").commit(true, true);
            } else {
                setClickButtonId(data.buttonId);
                nexus.getGrid("PRD112_grid_2").commit(true, true);
            }
        } else {
            setIsPedidosModalOpen(true);
        }
        context.abortServerCall = true;
    };
    const GridOnAfterInitialize = (context, grid, parameters, nexus) => {

        setEmpresa(parameters.find(d => d.id === "empresa").value);

    };
    const handleBeforeValidateRow = (context, data, nexus) => {
        data.parameters = [
            { id: "empresa", value: empresa },
            { id: "idIngreso", value: idIngreso }
        ];
    }

    const onBeforeSelectSearch = (context, rows, query, nexus) => {
        query.parameters = [
            { id: "empresa", value: empresa },
            { id: "idIngreso", value: idIngreso }
        ];
    }

    return (
        <Page icon="fas fa-file" title={t("PRD112_Sec0_pageTitle_Titulo")}
            {...props}
            application="PRD112"
            onAfterLoad={handlePageAfterLoad}
        >
            <Form id="PRD112enPed_form_1"
                application="PRD112"
                onBeforeInitialize={handleFormBeforeInitialize}
                onBeforeSubmit={onBeforeSubmit}>
                <Row>
                    <Col>
                        <Row>
                            <Col>
                                <div className="form-group" >
                                    <label htmlFor="idProduccion">{t("PRD112GenPed_form1_label_IdProduccion")}</label>
                                    <Field name="idProduccion" className="form-control-sm" />
                                </div>
                            </Col>
                            <Col>
                                <div className="form-group" >
                                    <label htmlFor="idProduccionExterno">{t("PRD112GenPed_form1_label_IdProduccionExterno")}</label>
                                    <Field name="idProduccionExterno" className="form-control-sm" />
                                </div>
                            </Col>
                            <Col>
                                <div className="form-group" >
                                    <label htmlFor="tpProduccion">{t("PRD112GenPed_form1_label_TipoProduccion")}</label>
                                    <Field name="tpProduccion" className="form-control-sm" />
                                </div>
                            </Col>
                        </Row>
                        <Row hidden={!tieneEspacioAsignado}>
                            <Col>
                                <div className="form-group" >
                                    <label htmlFor="idEspacio">{t("PRD112GenPed_form1_label_IdEspacio")}</label>
                                    <Field name="idEspacio" className="form-control-sm" />
                                </div>
                            </Col>
                            <Col>
                                <div className="form-group" >
                                    <label htmlFor="dsEspacio">{t("PRD112GenPed_form1_label_DsEspacio")}</label>
                                    <Field name="dsEspacio" className="form-control-sm" />
                                </div>
                            </Col>
                            <Col>
                                <div className="form-group" >
                                    <label htmlFor="nuProduccionEspacio">{t("PRD112GenPed_form1_label_NuProduccionEspacio")}</label>
                                    <Field name="nuProduccionEspacio" className="form-control-sm" />
                                </div>
                            </Col>
                        </Row>

                    </Col>
                </Row>
                <Row>
                    <FormWarningMessage message={t("PRD112_Sec0_Error_Er001_OmisioCantidadPedir")} show={tieneEspacioAsignado} />
                </Row>
                <Row>
                    <FormWarningMessage message={t("PRD112_Sec0_Error_Er002_OmisioCantidadPedir")} show={!tieneEspacioAsignado} />
                </Row>
                <Row>
                    <Col>

                        <Row >
                            <div className="col-10" hidden={!tieneEspacioAsignado}>
                                <Grid
                                    id="PRD112_grid_1"
                                    application="PRD112"
                                    rowsToFetch={100}
                                    rowsToDisplay={20}
                                    enableExcelExport={true}
                                    enableExcelImport={false}
                                    onBeforeInitialize={applyGridParameters}
                                    onBeforeFetch={applyGridParameters}
                                    onBeforeFetchStats={applyGridParameters}
                                    onBeforeApplyFilter={applyGridParameters}
                                    onBeforeApplySort={applyGridParameters}
                                    onBeforeExportExcel={applyGridParameters}
                                    onAfterCommit={applyGridParametersCommit}
                                    onBeforeCommit={onBeforeCommit}
                                    onAfterInitialize={GridOnAfterInitialize}
                                    onBeforeValidateRow={handleBeforeValidateRow}
                                    onBeforeSelectSearch={onBeforeSelectSearch}

                                />
                            </div>
                            <div className="col-10" hidden={tieneEspacioAsignado}>
                                <Grid
                                    id="PRD112_grid_2"
                                    application="PRD112"
                                    rowsToFetch={100}
                                    rowsToDisplay={20}
                                    enableExcelExport={true}
                                    enableExcelImport={false}
                                    onBeforeInitialize={applyGridParameters}
                                    onBeforeFetch={applyGridParameters}
                                    onBeforeFetchStats={applyGridParameters}
                                    onBeforeApplyFilter={applyGridParameters}
                                    onBeforeApplySort={applyGridParameters}
                                    onBeforeExportExcel={applyGridParameters}
                                    onAfterCommit={applyGridParametersCommit}
                                    onBeforeCommit={onBeforeCommit}
                                    onAfterInitialize={GridOnAfterInitialize}
                                    onBeforeValidateRow={handleBeforeValidateRow}
                                    onBeforeSelectSearch={onBeforeSelectSearch}

                                />
                            </div>
                            <div className="col-2">
                                <br />
                                <br />
                                <br />
                                <br />
                                <div className="col-11">
                                    <Row hidden={!tieneEspacioAsignado}>
                                        <SubmitButton id="btnGenerarReserva" variant="primary" label="PRD112_grid1_btn_GenerarReserva" />
                                    </Row>
                                </div>
                                <br />
                                <div className="col-11">
                                    <Row >
                                        <SubmitButton id="btnGenerarPedido" variant="primary" label={tieneEspacioAsignado ? "PRD112_grid1_btn_GenerarPedidoReserva" : "PRD112_grid1_btn_GenerarPedido"} />
                                    </Row>
                                </div>
                                <br />
                                <div className="col-11">
                                    <Row >
                                        <SubmitButton id="btnGenerarPedidoLiberar" variant="primary" label={tieneEspacioAsignado ? "PRD112_grid1_btn_GenerarPedidoReservaLiberar" : "PRD112_grid1_btn_GenerarPedidoLiberar"} />
                                    </Row>
                                </div>
                                <br />
                                <div className="col-11">
                                    <Row >
                                        <SubmitButton id="btnVerPedidos" variant="primary" label="PRD112_form1_btn_VerPedidosGenerados" />
                                    </Row>
                                </div>
                            </div>
                        </Row>
                    </Col>
                </Row>
            </Form>

            <PRD112PedidosModal PRD112PedidosModal show={isPedidosModalOpen} onHide={() => setIsPedidosModalOpen(false)} ingresoEditar={idIngreso} />

        </Page>
    );
}