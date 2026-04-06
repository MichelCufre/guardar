import React, { useState, useRef } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Form, Field, FieldDateTime, FormButton, FieldCheckbox, StatusMessage, FieldSelectAsync } from '../../components/FormComponents/Form';
import { Row, Col, FormGroup } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';

export default function REC220(props) {
    const { t } = useTranslation();
    const [listCargas, setCargas] = useState("[]");
    const [isHiddenConfirmar, setIsHiddenConfirmar] = useState(true);
    const classBtnConfirmar = isHiddenConfirmar ? "hidden" : "";
    const onAfterButtonAction = (context, form, query, nexus) => {

        if (query.buttonId === "btnFinalizar") {
            if (form.fields.find(w => w.id === "listCargas") != null)
                setCargas(form.fields.find(w => w.id === "listCargas").value);

            nexus.getGrid("REC220_grid_1").refresh();
        }

    };
    const onBeforeValidateField = (context, form, query, nexus) => {

        setIsHiddenConfirmar(form.buttons.some(w => w.id == "btnFinalizar" && w.disabled == false));

    };
    const addParameters = (context, data, nexus) => {
        data.parameters = [
            { id: "listCargas", value: listCargas },
        ];
    };
    const initialValues = {
    };
    const validationSchema = {
        CD_EMPRESA: Yup.string().required(),
    };

    const onAfterValidateField = (context, form, query, nexus) => {
        setIsHiddenConfirmar(form.buttons.some(w => w.id == "btnFinalizar" && w.disabled == true));
    }
    return (
        <Page
            load
            title={t("REC220_Sec0_pageTitle_Titulo")}
            application="REC220"
            {...props}
        >
            <Form
                id="REC220_form_1"
                application="REC220"
                initialValues={initialValues}
                validationSchema={validationSchema}
                onAfterButtonAction={onAfterButtonAction}
                onBeforeValidateField={onBeforeValidateField}
                onAfterValidateField={onAfterValidateField}>
                <Row>
                    <Col lg={2}>
                        <FormGroup>
                            <label htmlFor="CD_EMPRESA">{t("WREC220_frm1_lbl_CD_EMPRESA")}</label>
                            <FieldSelectAsync name="CD_EMPRESA" isClearable />
                            <StatusMessage for="CD_EMPRESA" />
                        </FormGroup>
                    </Col>
                    <Col lg={2}>
                        <FormGroup>
                            <Field name="listCargas" isClearable hidden />
                        </FormGroup>
                    </Col>
                </Row>
                <Row>
                    <Col>
                        <FormGroup>
                            <FormButton className={classBtnConfirmar} id="btnFinalizar" label="General_Sec0_btn_LIBERAR" />
                        </FormGroup>
                    </Col>
                </Row>
            </Form>
            <Row>
                <Col >
                    <h1>{t("REC220_Sec0_pageTitle_Titulo2")}</h1>
                </Col>
            </Row>
            <Row className="mb-4">
                <Col>
                    <Grid
                        id="REC220_grid_1"
                        application="REC220"
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