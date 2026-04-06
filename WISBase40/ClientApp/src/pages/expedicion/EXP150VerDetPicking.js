import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Modal, Button, Row, Col, Container } from 'react-bootstrap';
import * as Yup from 'yup';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { withPageContext } from '../../components/WithPageContext';

function InternalEXP150VerDetPicking(props) {

    const { t } = useTranslation();
    return (
        <Modal.Body>
            <Container fluid>
                <Row className='mt-3'>
                    <Col>
                        <Grid
                            id="EXP150_grid_1_VerDetPickCont"
                            application="EXP150VerDetPicking"
                            rowsToFetch={30}
                            rowsToDisplay={15}
                            enableExcelExport
                            onBeforeInitialize={props.addParameters}
                            onBeforeFetch={props.addParameters}
                            onBeforeFetchStats={props.addParameters}
                            onBeforeExportExcel={props.addParameters}
                        />
                    </Col>
                </Row>
            </Container>
        </Modal.Body>

    );

}
export const EXP150VerDetPicking = withPageContext(InternalEXP150VerDetPicking);