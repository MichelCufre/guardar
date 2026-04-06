import React, { useState } from 'react';
import { Col, Container, Row } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { FieldToggle, Form, StatusMessage } from '../../components/FormComponents/Form';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';

export default function STO720(props) {
    const { t } = useTranslation();
    const [lpnsActivos, setLpnsActivos] = useState(true);

    const initialValues = {
        lpnsActivos: true,
    };

    const validationSchema = {
        lpnsActivos: Yup.string()
    };

    const [infoContenidoLpn, setInfoContenidoLpn] = useState({
        numeroLpn: "", tipoLpn: "", idLpnExterno: ""
    });

    const [isInfoContenidoLpnDisplayed, setIsInfoContenidoLpnDisplayed] = useState(false);

    const onAfterInitialize = (context, grid, parameters, nexus) => {

        if (parameters.find(d => d.id === "STO720_NU_LPN") != null) {
            setInfoContenidoLpn({
                numeroLpn: parameters.find(d => d.id === "STO720_NU_LPN").value,
                tipoLpn: parameters.find(d => d.id === "STO720_TP_LPN_TIPO").value,
                idLpnExterno: parameters.find(d => d.id === "STO720_ID_LPN_EXTERNO").value,
            });

            setIsInfoContenidoLpnDisplayed(true);
        }
        else {

            setIsInfoContenidoLpnDisplayed(false);

        }
    };

    const addParameters = (context, data, nexus) => {

        data.parameters.push({ id: "lpnsActivos", value: lpnsActivos });
    }

    const onBeforeValidateField = (context, form, query, nexus) => {
        var lpnsActivos = nexus.getForm("STO720_form_1").getFieldValue("lpnsActivos");
        setLpnsActivos(lpnsActivos);

        query.parameters.push({ id: "lpnsActivos", value: lpnsActivos });

        nexus.getGrid("STO720_grid_1").refresh();
    };

    return (
        <Page
            title={t("STO720_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <Row>
                <Col lg={3}>
                    <Form id="STO720_form_1"
                        initialValues={initialValues}
                        validationSchema={validationSchema}
                        onBeforeValidateField={onBeforeValidateField}
                    >
                        <div className="form-group">
                            <FieldToggle name="lpnsActivos" label={t("STO700_frm1_lbl_activos")} />
                            <StatusMessage for="lpnsActivos" />
                        </div>
                    </Form>
                </Col>
                <Col lg={9} style={{ display: isInfoContenidoLpnDisplayed ? 'block' : 'none' }}>
                    <Row>
                        <Col>
                            <span style={{ fontWeight: "bold" }}>{t("STO720_Sec0_Info_NumeroLPN")}: </span>
                            <span> {`${infoContenidoLpn.numeroLpn}`}</span>
                        </Col>
                        <Col>
                            <span style={{ fontWeight: "bold" }}>{t("STO720_Sec0_Info_TipoLPN")}: </span>
                            <span> {`${infoContenidoLpn.tipoLpn}`}</span>
                        </Col>
                        <Col>
                            <span style={{ fontWeight: "bold" }}>{t("STO720_Sec0_Info_IdExterno")}: </span>
                            <span>{`${infoContenidoLpn.idLpnExterno}`}</span>
                        </Col>
                    </Row>
                </Col>
            </Row>

            <div className="row mb-4">
                <div className="col-12">
                    <Grid
                        id="STO720_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                        onBeforeInitialize={addParameters}
                        onBeforeFetch={addParameters}
                        onBeforeFetchStats={addParameters}
                        onBeforeExportExcel={addParameters}
                        onBeforeApplyFilter={addParameters}
                        onBeforeApplySort={addParameters}
                        onBeforeMenuItemAction={addParameters}
                        onAfterInitialize={onAfterInitialize}
                    />
                </div>
            </div>

        </Page>
    );
}