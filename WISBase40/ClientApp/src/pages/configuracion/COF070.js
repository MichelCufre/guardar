import React, { useState } from 'react';
import { Col, Container, Row } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useReportDownloader } from '../../components/ReportHook';

export default function COF070(props) {
    const { t } = useTranslation();
    const reportDownloader = useReportDownloader();
    const [camion, setCamion] = useState(null);
    const [agenda, setAgenda] = useState(null);

    const beforeButtonAction = (context, data, nexus) => {
        if (data.buttonId === "btnDescargar") {
            context.abortServerCall = true;
            reportDownloader.downloadReport(data.row.cells.find(d => d.column === "NU_REPORTE").value);
        }
    }

    const afterButtonAction = (context, nexus) => {
        nexus.getGrid("COF070_grid_1").refresh();
    }

    const loadHeader = (context, grid, params, nexus) => {
        if (params.length > 0) {
            params[0].id === "camion" ? setCamion(params[0].value) : null;
            params[0].id === "agenda" ? setAgenda(params[0].value) : null;
        }
    }

    return (
        <Page
            load
            title={t("COF070_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <Container fluid>
                <Row>
                    <Col className={camion ? "d-block" : "d-none"}>
                        <Row>
                            <Col sm={3}>
                                <span style={{ fontWeight: "bold" }}>{t("COF070_frm1_lbl_CD_CAMION")}:</span>
                            </Col>
                            <Col className='p-0'>
                                <span>{camion}</span>
                            </Col>
                        </Row>
                    </Col>
                    
                    <Col className={agenda ? "d-none" : "d-block"}>
                        <Row>
                            <Col sm={3}>
                            </Col>
                            <Col className='p-0'>
                            </Col>
                        </Row>
                    </Col>
                    <Col className={agenda ? "d-block" : "d-none"}>
                        <Row>
                            <Col sm={3}>
                                <span style={{ fontWeight: "bold" }}>{t("COF070_frm1_lbl_NU_AGENDA")}:</span>
                            </Col>
                            <Col className='p-0'>
                                <span> {agenda}</span>
                            </Col>
                        </Row>
                    </Col>
                    <Col className={camion ? "d-none" : "d-block"}>
                        <Row>
                            <Col sm={3}>
                            </Col>
                            <Col className='p-0'>
                            </Col>
                        </Row>
                    </Col>
                </Row>
            </Container>
            <hr></hr>
            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="COF070_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                        onAfterInitialize={loadHeader}
                        onAfterButtonAction={afterButtonAction}
                        onBeforeButtonAction={beforeButtonAction}
                    />
                </div>
            </div>
        </Page>
    )
}