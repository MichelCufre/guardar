import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { Modal, Row, Col, Container } from 'react-bootstrap';

export default function STO721(props) {

    const { t } = useTranslation();

    const [infoContexto, setInfoContexto] = useState({
        codigoEmpresa: "",
        empresa: "",
        numeroLpn: "",
        tipoLpn: "",
        tipo: "",
    });

    const onAfterInitialize = (context, grid, parameters, nexus) => {

        if (parameters.find(d => d.id === "numeroLpn") != null) {

            setInfoContexto({
                codigoEmpresa: parameters.find(d => d.id === "codigoEmpresa").value,
                empresa: parameters.find(d => d.id === "empresa").value,
                numeroLpn: parameters.find(d => d.id === "numeroLpn").value,
                tipoLpn: parameters.find(d => d.id === "tipoLpn").value,
                tipo: parameters.find(d => d.id === "tipo").value,
            });

        }
    };

    const applyParameters = (context, data, nexus) => {
        data.parameters = [{ id: "numeroLpn", value: infoContexto.numeroLpn },];
    };

    return (
        <Page
            title={t("STO721_Sec0_pageTitle_Titulo")}
            {...props}
        >

            <Container fluid style={{ display: 'block'}} >
                <Row>
                    <Col>
                        <Row>
                            <Col>
                                <span style={{ fontWeight: "bold", }}>{t("REG603_Sec0_Info_Cabezal_Lpn")} </span>
                                <span> {`${infoContexto.numeroLpn}`}</span>
                            </Col>
                            <Col>
                                <span style={{ fontWeight: "bold", }}>{t("STO721_Sec0_Info_Cabezal_Tipo")} </span>
                                <span> {`${infoContexto.tipoLpn}`} - {`${infoContexto.tipo}`} </span>
                            </Col>
                            <Col>
                                <span style={{ fontWeight: "bold" }}>{t("STO721_Sec0_Info_Cabezal_Empresa")} </span>
                                <span> {`${infoContexto.codigoEmpresa}`} - {`${infoContexto.empresa}`}</span>
                            </Col>
                        </Row>
                    </Col>
                </Row>
            </Container>
            <br/>
            <div className="row mb-4">
                <div className="col-12">
                    <h4 className='form-title'>{t("STO721_Sec0_logs_TituloCabezal")}</h4>
                    <Grid
                        id="STO721_grid_Cabezal"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport={true}
                        onAfterInitialize={onAfterInitialize}
                        onBeforeFetch={applyParameters}
                        onBeforeFetchStats={applyParameters}
                        onBeforeApplyFilter={applyParameters}
                        onBeforeApplySort={applyParameters}
                        onBeforeExportExcel={applyParameters}
                    />
                </div>
            </div>

            <div className="row mb-4">
                <div className="col-12">
                    <h4 className='form-title'>{t("STO721_Sec0_logs_TituloAtributosCabezal")}</h4>
                    <Grid
                        id="STO721_grid_AtributosCabezal"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport={true}
                        onBeforeFetch={applyParameters}
                        onBeforeFetchStats={applyParameters}
                        onBeforeApplyFilter={applyParameters}
                        onBeforeApplySort={applyParameters}
                        onBeforeExportExcel={applyParameters}
                    />
                </div>
            </div>

        </Page>
    );
}