import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Form } from '../../components/FormComponents/Form';
import { Page } from '../../components/Page';
import { Modal, Row, Col, Container } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';


export default function FAC009(props) {

    const { t } = useTranslation();

    const [infoFacturacion, setInfoFacturacion] = useState({});

    const onAfterInitialize = (context, form, query, nexus) => {
        if (query.parameters.find(x => x.id === "nuEjecucion") != null) {
            setInfoFacturacion({
                NuEjecucion: query.parameters.find(x => x.id === "nuEjecucion").value,
            });
        }
    };

    const onAfterButtonAction = (data, nexus) => {
        nexus.getGrid("FAC009_grid_1").refresh();
    }

    return (
        <Page
            title={t("FAC009_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <Form
                application="FAC009"
                id="FAC009_form_1"
                application="FAC009"
                onAfterInitialize={onAfterInitialize}
            >

                <Container fluid>
                    <div style={{ marginBottom: '6vh' }}>
                        <Row>
                            <Col sm={2}>
                                <span style={{ fontWeight: "bold" }}>{t("FAC009_frm1_lbl_NroEjecucion")}: </span>
                            </Col>
                            <Col className='p-0'>
                                <span>{`${infoFacturacion.NuEjecucion ? infoFacturacion.NuEjecucion : "-"}`}</span>
                            </Col>
                        </Row>
                    </div>
                </Container>

                <div className="row mb-4">
                    <div className="col-12">
                        <Grid id="FAC009_grid_1" rowsToFetch={30} rowsToDisplay={15} enableExcelExport enableExcelImport={false}
                            onAfterButtonAction={onAfterButtonAction}
                        />
                    </div>
                </div>

            </Form>
        </Page>
    );
}