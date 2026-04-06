import React, { useState, useRef } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Form, Field, FieldDateTime, FormButton, FieldCheckbox, StatusMessage, FieldSelectAsync } from '../../components/FormComponents/Form';
import { Row, Col, FormGroup } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';

export default function REC220(props) {
    const { t } = useTranslation();
    const [isHiddenConfirmar, setIsHiddenConfirmar] = useState(true);
    const [Agenda, setAgenda] = useState("-1");
    const classBtnConfirmar = isHiddenConfirmar ? "hidden" : "";
    const [noPuedeTransferir, setNoPuedeTransferir] = useState(false);

    const onAfterButtonAction = (context, form, query, nexus) => {

        setNoPuedeTransferir(false);

        if (query.buttonId === "btnTransferir") {
            setAgenda(form.fields.find(w => w.id === "NU_AGENDA").value);
            nexus.getGrid("REC270_grid_1").refresh();
            nexus.getGrid("REC270_grid_2").refresh();
            nexus.getGrid("REC270_grid_3").refresh();
        }

    };

    const onBeforeButtonAction = (context, form, query, nexus) => {

        if (query.buttonId == "btnTransferir" && noPuedeTransferir) {
            context.abortServerCall = true;
        }
        else {
            setNoPuedeTransferir(true);
        }
    };

    const onBeforeValidateField = (context, form, query, nexus) => {
        setAgenda(form.fields.find(w => w.id === "NU_AGENDA").value);
        setIsHiddenConfirmar(form.buttons.some(w => w.id == "btnTransferir" && w.disabled == false));
        nexus.getGrid("REC270_grid_1").refresh();
        nexus.getGrid("REC270_grid_2").refresh();
        nexus.getGrid("REC270_grid_3").refresh();
    };

    const addParameters = (context, data, nexus) => {
        data.parameters = [
            { id: "Agenda", value: Agenda },
        ];
    };
    const initialValues = {
    };
    const validationSchema = {
        NU_AGENDA: Yup.string().required(),
    };

    const onAfterValidateField = (context, form, query, nexus) => {
        setIsHiddenConfirmar(form.buttons.some(w => w.id == "btnTransferir" && w.disabled == true));
    }
    return (
        <Page
            load
            title={t("REC270_Sec0_pageTitle_Titulo")}
            application="REC270"
            {...props}
        >
            <Form
                id="REC220_form_1"
                application="REC270"
                initialValues={initialValues}
                validationSchema={validationSchema}
                onAfterButtonAction={onAfterButtonAction}
                onBeforeButtonAction={onBeforeButtonAction}
                onBeforeValidateField={onBeforeValidateField}
                onAfterValidateField={onAfterValidateField}>
                <Row>
                    <Col lg={6}>
                        <FormGroup>
                            <label htmlFor="NU_AGENDA">{t("WREC220_frm1_lbl_NU_AGENDA")}</label>
                            <FieldSelectAsync name="NU_AGENDA" isClearable />
                            <StatusMessage for="NU_AGENDA" />
                        </FormGroup>
                    </Col>
                </Row>
                <Row>
                    <Col>
                        <FormGroup>
                            <FormButton className={classBtnConfirmar} id="btnTransferir" label="General_Sec0_btn_TRANSFERIR" />
                        </FormGroup>
                    </Col>
                </Row>
            </Form>
            <Row>
                <Col >
                    <h1>{t("REC270_Sec0_pageTitle_Titulo1")}</h1>
                </Col>
            </Row>
            <Row className="mb-4">
                <Col>
                    <Grid
                        id="REC270_grid_1"
                        application="REC270"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                        onBeforeFetch={addParameters}
                    />
                </Col>
            </Row>
            <Row>
                <Col >
                    <h1>{t("REC270_Sec0_pageTitle_Titulo2")}</h1>
                </Col>
            </Row>
            <Row className="mb-4">
                <Col>
                    <Grid
                        id="REC270_grid_2"
                        application="REC270"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                        onBeforeFetch={addParameters}
                    />
                </Col>
            </Row>
            <Row>
                <Col >
                    <h1>{t("REC270_Sec0_pageTitle_Titulo3")}</h1>
                </Col>
            </Row>
            <Row className="mb-4">
                <Col>
                    <Grid
                        id="REC270_grid_3"
                        application="REC270"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                        onBeforeFetch={addParameters}
                    />
                </Col>
            </Row>
        </Page>
    );
}