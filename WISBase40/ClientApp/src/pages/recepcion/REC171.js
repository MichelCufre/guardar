import React, { useState } from 'react';
import { Page } from '../../components/Page';
import { Grid } from '../../components/GridComponents/Grid';
import { useTranslation } from 'react-i18next';
import { Row, Col } from 'react-bootstrap';
import { Container } from 'react-bootstrap';
import {FieldToggle, Form, StatusMessage} from "../../components/FormComponents/Form";
import * as Yup from "yup";
import REC171ImprimirEtiquetasModal from "./REC171ImprimirEtiquetasModal";

export default function REC171(props) {

    const { t } = useTranslation();

    const [infoDetalle, setInfoDetalle] = useState({
        NU_AGENDA: "", CD_EMPRESA: "", NM_EMPRESA: "", CD_AGENTE: "", DS_TIPO_AGENTE: "", DS_AGENTE: ""
    });

    const [infoDetalleProd, setInfoDetalleProd] = useState({
        NU_AGENDA: "", CD_EMPRESA: "", NM_EMPRESA: "", CD_PRODUTO: "", DS_PRODUTO: ""
    });

    const [isInfoDetalleDisplayed, setIsInfoDetalleDisplayed] = useState(false);
    const [isInfoDetalleProdDisplayed, setIsInfoDetalleProdDisplayed] = useState(false);
    const [sinEtiqueta, setSinEtiqueta] = useState(false);
    const [showImprimirPopup, setShowImprimirPopup] = useState(false);

    const [selectedKeys, setSelectedKeys] = useState([]);

    const initialValues = {
        flPrdSinEtiqueta: false,
    };

    const validationSchema = {
        flPrdSinEtiqueta: Yup.boolean()
    };

    const closeFormDialog = () => setShowImprimirPopup(false);

    const onAfterInitialize = (context, grid, parameters, nexus) => {

        if (parameters.find(d => d.id === "REC171_CD_PRODUTO") != null) {
            setInfoDetalleProd({
                NU_AGENDA: parameters.find(d => d.id === "REC171_NU_AGENDA").value,
                CD_PRODUTO: parameters.find(d => d.id === "REC171_CD_PRODUTO").value,
                DS_PRODUTO: parameters.find(d => d.id === "REC171_DS_PRODUTO").value,
                CD_EMPRESA: parameters.find(d => d.id === "REC171_CD_EMPRESA").value,
                NM_EMPRESA: parameters.find(d => d.id === "REC171_NM_EMPRESA").value,
            });

            setIsInfoDetalleProdDisplayed(true);

            setIsInfoDetalleDisplayed(false);
        }
        else if (parameters.find(d => d.id === "REC171_CD_AGENTE") != null) {

            setInfoDetalle({
                NU_AGENDA: parameters.find(d => d.id === "REC171_NU_AGENDA").value,
                CD_AGENTE: parameters.find(d => d.id === "REC171_CD_AGENTE").value,
                DS_AGENTE: parameters.find(d => d.id === "REC171_DS_AGENTE").value,
                DS_TIPO_AGENTE: parameters.find(d => d.id === "REC171_DS_TIPO_AGENTE").value,
                CD_EMPRESA: parameters.find(d => d.id === "REC171_CD_EMPRESA").value,
                NM_EMPRESA: parameters.find(d => d.id === "REC171_NM_EMPRESA").value,
            });

            setIsInfoDetalleDisplayed(true);

            setIsInfoDetalleProdDisplayed(false);

        } else {

            setIsInfoDetalleProdDisplayed(false);

            setIsInfoDetalleDisplayed(false);
        }
    };

    const onBeforeValidateField = (context, form, query, nexus) => {
        let etiqueta = nexus.getForm("REC171_form_1").getFieldValue("flPrdSinEtiqueta");
        setSinEtiqueta(etiqueta);
        nexus.getGrid("REC171_grid_1").refresh();
    };

    const addParameters = (context, data, nexus) => {

        if (data.parameters == null) {
            data.parameters = [];
        }
        data.parameters.push({ id: "SIN_ETIQUETA", value: sinEtiqueta });        
    };

    const hadleOnBeforeMenuItemAction = (context, data, nexus) => {

        addParameters(context, data, nexus);

        if (data.buttonId === "btnImprimir") {

            data.parameters.push ({
                id: "MODIFIED_ROWS",
                value: JSON.stringify (nexus
                    .getGrid ("REC171_grid_1")
                    .getModifiedRows ())
            });
        }
    };

    const hadleOnAfterMenuItemAction = (context, data, nexus) => {

        if (data.parameters.some(x => x.id === "SELECTED_KEYS")) { 
            setSelectedKeys(data.parameters.find(x => x.id === "SELECTED_KEYS").value);
            setShowImprimirPopup(true);
        }
    };

    return (
        <Page
            title={t("REC171_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <Container fluid style={{ display: isInfoDetalleDisplayed ? 'block' : 'none' }} >
                <Row>
                    <Col>
                        <Row>
                            <Col sm={4}>
                                <span style={{ fontWeight: "bold" }}>{t("REC171_Sec0_Info_Cabezal_Agenda")} </span>
                            </Col>
                            <Col className='p-0'>
                                <span> {`${infoDetalle.NU_AGENDA}`}</span>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={4}>
                            </Col>
                            <Col className='p-0'>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={4}>
                            </Col>
                            <Col className='p-0'>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col sm={4}>
                            </Col>
                            <Col className='p-0'>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={4}>
                                <span style={{ fontWeight: "bold" }}>{t("REC171_Sec0_Info_Cabezal_Empresa")} </span>
                            </Col>
                            <Col className='p-0'>
                                <span> {`${infoDetalle.CD_EMPRESA}`} - {`${infoDetalle.NM_EMPRESA}`}</span>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={4}>
                            </Col>
                            <Col className='p-0'>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col sm={4}>
                            </Col>
                            <Col className='p-0'>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={4}>
                            </Col>
                            <Col className='p-0'>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={4}>
                                <span style={{ fontWeight: "bold" }}>{t("REC171_Sec0_Info_Cabezal_Agente")} </span>
                            </Col>
                            <Col className='p-0'>
                                <span>{`${infoDetalle.DS_TIPO_AGENTE}`} - {`${infoDetalle.CD_AGENTE}`} - {`${infoDetalle.DS_AGENTE}`}</span>
                            </Col>
                        </Row>
                    </Col>
                </Row>
            </Container>

            <Container fluid style={{ display: isInfoDetalleProdDisplayed ? 'block' : 'none' }} >
                <Row>
                    <Col>
                        <Row>
                            <Col sm={4}>
                                <span style={{ fontWeight: "bold" }}>{t("REC171_Sec0_Info_Cabezal_Agenda")} </span>
                            </Col>
                            <Col className='p-0'>
                                <span> {`${infoDetalleProd.NU_AGENDA}`}</span>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={4}>
                            </Col>
                            <Col className='p-0'>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={4}>
                            </Col>
                            <Col className='p-0'>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col sm={4}>
                            </Col>
                            <Col className='p-0'>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={4}>
                                <span style={{ fontWeight: "bold" }}>{t("REC171_Sec0_Info_Cabezal_Empresa")} </span>
                            </Col>
                            <Col className='p-0'>
                                <span> {`${infoDetalleProd.CD_EMPRESA}`} - {`${infoDetalleProd.NM_EMPRESA}`}</span>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={4}>
                            </Col>
                            <Col className='p-0'>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col sm={4}>
                            </Col>
                            <Col className='p-0'>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={4}>
                            </Col>
                            <Col className='p-0'>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={4}>
                                <span style={{ fontWeight: "bold" }}>{t("REC171_Sec0_Info_Cabezal_Producto")} </span>
                            </Col>
                            <Col className='p-0'>
                                <span>{`${infoDetalleProd.CD_PRODUTO}`} - {`${infoDetalleProd.DS_PRODUTO}`}</span>
                            </Col>
                        </Row>
                    </Col>
                </Row>
            </Container>

            <hr style={{ display: isInfoDetalleDisplayed || isInfoDetalleProdDisplayed ? 'block' : 'none' }}></hr>
            <Row>
                <Col sm={5}>
                    <Form
                        id="REC171_form_1"
                        onBeforeValidateField={onBeforeValidateField}
                        initialValues={initialValues}
                        validationSchema={validationSchema}
                    >
                        <div className="form-group">
                            <FieldToggle name="flPrdSinEtiqueta" label={t("REC171_frm1_lbl_FiltroProductosSinEtiqueta")} />
                            <StatusMessage for="flPrdSinEtiqueta" />
                        </div>
                    </Form>
                </Col>
            </Row>
            <Grid
                id="REC171_grid_1"
                enableExcelExport
                enableSelection
                onAfterInitialize={onAfterInitialize}
                onBeforeApplyFilter={addParameters}
                onBeforeApplySort={addParameters}
                onBeforeExportExcel={addParameters}
                onBeforeFetch={addParameters}
                onBeforeFetchStats={addParameters}
                onBeforeInitialize={addParameters}
                onBeforeMenuItemAction={hadleOnBeforeMenuItemAction}
                onAfterMenuItemAction={hadleOnAfterMenuItemAction}
                rowsToDisplay={15}
                rowsToFetch={30}
            />
            <REC171ImprimirEtiquetasModal
                show={ showImprimirPopup }
                onHide={ closeFormDialog }
                selectedKeys={selectedKeys}
            />
        </Page>
    );
}
