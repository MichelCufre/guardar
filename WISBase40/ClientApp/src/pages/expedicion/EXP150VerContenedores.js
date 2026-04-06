import React, { useState } from 'react';
import { Form, Field, FieldSelect, SubmitButton, FormButton, StatusMessage } from '../../components/FormComponents/Form';
import { useTranslation } from 'react-i18next';
import { Modal, Button, Row, Col, Container } from 'react-bootstrap';
import * as Yup from 'yup';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { withPageContext } from '../../components/WithPageContext';
function InternalEXP150VerContenedores(props) {

    const { t } = useTranslation();

    const onAfterButtonAction = (context, data, nexus) => {
        props.onHide(context, data, props.nexus);
    };


    return (
        <Modal.Body>
            <Container fluid>
                <Row className='mt-3'>
                    <Col>
                        <Grid
                            id="EXP150_grid_1_VerContenedores"
                            application="EXP150VerContenedores"
                            rowsToFetch={30}
                            rowsToDisplay={15}
                            enableExcelExport
                            onBeforeInitialize={props.addParameters}
                            onBeforeFetch={props.addParameters}
                            onBeforeFetchStats={props.addParameters}                            
                            onAfterButtonAction={onAfterButtonAction}
                            onBeforeExportExcel={props.addParameters}
                        />
                    </Col>
                </Row>
            </Container>

        </Modal.Body>

    );

}
export const EXP150VerContenedores = withPageContext(InternalEXP150VerContenedores);