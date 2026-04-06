import React, { useState, useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { Page } from '../../components/Page';
import { Grid } from '../../components/GridComponents/Grid';
import { Container, Row, Col } from 'react-bootstrap';
import INT103Modal from './INT103Modal';

export default function INT103(props) {
    const { t } = useTranslation();
    const [showModal, setShowModal] = useState(false);
    const [nuInterfazEjecucion, setNuInterfazEjecucion] = useState('');
    const [referencia, setReferencia] = useState('');

    const onAfterButtonAction = (data, nexus) => {
        if (data.buttonId == "btnViewDetail") {
            setShowModal(true);
        }
    }

    const onHide = () => {
        setShowModal(false);
    }

    const onBeforeInitialize = (context, data, nexus) => {
        if (data.parameters.length > 0) {
            setNuInterfazEjecucion(data.parameters.find(e => e.id === "interfaz").value);
            // setReferencia(data.parameters.find(e => e.id === "referencia").value);
        }
    }

    return (
        <Page
            title={t("INT103_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <Container fluid>
                <Row>
                    <Col>
                        <Row>
                            <Col sm={1}>
                                <span style={{ fontWeight: "bold" }}>{t("INT_lbl_NU_INTERFAZ_EJECUCION")}:</span>
                            </Col>
                            <Col className='p-0'>
                                <span>{nuInterfazEjecucion}</span>
                            </Col>
                        </Row>
                    </Col>
                </Row>
            </Container>
            <hr></hr>
            <Grid
                id="INT103_grid_1"
                rowsToFetch={30}
                rowsToDisplay={15}
                enableExcelExport
                onAfterButtonAction={onAfterButtonAction}
                onBeforeInitialize={onBeforeInitialize}
            />
            <INT103Modal show={showModal} onHide={onHide} nuInterfaz={nuInterfazEjecucion} />
        </Page>
    );
}