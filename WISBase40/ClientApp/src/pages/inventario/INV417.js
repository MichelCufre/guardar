import React, { useState, useEffect } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Form, Field, FormButton, StatusMessage } from '../../components/FormComponents/Form';
import { Row, Col } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import Accordion from '@mui/material/Accordion';
import AccordionSummary from '@mui/material/AccordionSummary';
import AccordionDetails from '@mui/material/AccordionDetails';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import { forEach } from 'lodash';
import { AddRemovePanel } from '../../components/AddRemovePanel';

export default function INV417(props) {
    const { t } = useTranslation();

    const [nuInventario, setInventario] = useState(null);
    const [nexus, setNexus] = useState(null);
    const [filtro, setFiltro] = useState({ AtributosCabezal: [], AtributosDetalle: [] });
    const initialValues = {};

    const onAfterPageLoad = (data) => {
        setInventario(data.parameters.find(d => d.id === "inventario").value);
    };

    useEffect(() => {
        if (nexus) {
            nexus.getGrid('INV417_grid_1').refresh();
            nexus.getGrid('INV417_grid_2').refresh();
        }
    }, [filtro]);

    const onBeforeFetch = (context, data, nexus) => {
        if (data.rowsToSkip == 0) {
            if (data.gridId == "INV417_grid_DetalleLpnAtrCab") {
                setFiltro({ AtributosCabezal: [], AtributosDetalle: filtro.AtributosDetalle });
            }
            else if (data.gridId == "INV417_grid_DetalleLpnAtrDet") {
                setFiltro({ AtributosCabezal: filtro.AtributosCabezal, AtributosDetalle: [] });
            }
        }
    }

    const onAfterApplySort = (context, rows, parameters) => {
        if (data.gridId == "INV417_grid_DetalleLpnAtrCab") {
            setFiltro({ AtributosCabezal: [], AtributosDetalle: filtro.AtributosDetalle });
        }
        else if (data.gridId == "INV417_grid_DetalleLpnAtrDet") {
            setFiltro({ AtributosCabezal: filtro.AtributosCabezal, AtributosDetalle: [] });
        }
    }

    const appendParameters = (context, data, nexus) => {
        data.parameters = [
            { id: "filtro", value: JSON.stringify(filtro) },
            { id: "nuInventario", value: nuInventario }
        ];
    };

    const onAfterMenuItemAction = (context, data, nexus) => {

        nexus.getGrid('INV417_grid_1').refresh();
        nexus.getGrid('INV417_grid_2').refresh();
    };

    const onBeforeValidateField = (context, form, query, nexus) => {
        context.abortServerCall = true;
    };

    const onBeforeButtonAction = (context, form, query, nexus) => {
        context.abortServerCall = true;

        if (query.buttonId === "btnAplicarFiltro") {
            var filtro = { AtributosCabezal: [], AtributosDetalle: [] };

            forEach(nexus.getGrid('INV417_grid_DetalleLpnAtrCab').getModifiedRows(), row => {
                var atributo = row.cells.find(c => c.column == 'VL_ATRIBUTO' && c.value != '');

                if (atributo) {
                    filtro.AtributosCabezal.push({ Id: row.id, Value: atributo.value });
                }
            });

            forEach(nexus.getGrid('INV417_grid_DetalleLpnAtrDet').getModifiedRows(), row => {
                var atributo = row.cells.find(c => c.column == 'VL_ATRIBUTO' && c.value != '');

                if (atributo) {
                    filtro.AtributosDetalle.push({ Id: row.id, Value: atributo.value });
                }
            });

            setNexus(nexus);
            setFiltro(filtro);
        }
    };

    const onBeforeSubmit = (context, form, query, nexus) => {
        context.abortServerCall = true;
    };

    const onBeforeInitialize = (context, form, data, nexus) => {
        data.parameters = [
            { id: "filtro", value: JSON.stringify(filtro) },
            { id: "nuInventario", value: nuInventario }
        ];
    };

    return (
        <Page
            title={t("INV417_Sec0_pageTitle_Titulo")}
            {...props}
            application="INV417"
            onAfterLoad={onAfterPageLoad}
        >
            <Form
                id="INV417_form_1"
                application="INV417"
                initialValues={initialValues}
                onBeforeInitialize={onBeforeInitialize}
                onBeforeValidateField={onBeforeValidateField}
                onBeforeButtonAction={onBeforeButtonAction}
                onBeforeSubmit={onBeforeSubmit}
            >
                <div className="row col-12">
                    <div className="col-4">
                        <div className="form-group">
                            <label htmlFor="nuInventario">{t("INV_frm1_lbl_NU_INVENTARIO")}</label>
                            <Field name="nuInventario" readOnly />
                            <StatusMessage for="nuInventario" />
                        </div>
                    </div>
                    <div className="col-4">
                        <div className="form-group">
                            <label htmlFor="descInventario">{t("INV_frm1_lbl_DS_INVENTARIO")}</label>
                            <Field name="descInventario" readOnly />
                            <StatusMessage for="descInventario" />
                        </div>
                    </div>
                    <div className="col-4">
                        <div className="form-group">
                            <label htmlFor="tipoInventario">{t("INV_frm1_lbl_TP_INVENTARIO")}</label>
                            <Field name="tipoInventario" readOnly />
                            <StatusMessage for="tipoInventario" />
                        </div>
                    </div>
                </div>
                <div className="row col-12">
                    <div className="col-4">
                        <div className="form-group">
                            <label htmlFor="empresa">{t("General_frm1_lbl_CD_EMPRESA")}</label>
                            <Field name="empresa" readOnly />
                            <StatusMessage for="empresa" />
                        </div>
                    </div>
                    <div className="col-4">
                        <div className="form-group">
                            <label htmlFor="descEmpresa">{t("General_frm1_lbl_NM_EMPRESA")}</label>
                            <Field name="descEmpresa" readOnly />
                            <StatusMessage for="descEmpresa" />
                        </div>
                    </div>
                    <div className="col-4">
                        <div className="form-group">
                            <label htmlFor="predio">{t("INV_frm1_lbl_NU_PREDIO")}</label>
                            <Field name="predio" readOnly />
                            <StatusMessage for="predio" />
                        </div>
                    </div>
                </div>

                <Accordion>
                    <AccordionSummary
                        expandIcon={<ExpandMoreIcon />}
                        aria-controls="panel1-content"
                        id="panel1-header"
                    >
                        {t("INV410_Sec0_lbl_LpnFiltros")}
                    </AccordionSummary>
                    <AccordionDetails>
                        <Row>
                            <Col lg={6}>
                                <h4 className='form-title'>{t("INV410_Sec0_title_LpnDetAtrCab")}</h4>
                            </Col>
                            <Col lg={6}>
                                <h4 className='form-title'>{t("INV410_Sec0_title_LpnDetAtrDet")}</h4>
                            </Col>
                        </Row>
                        <Row>
                            <Col lg={6}>
                                <Grid
                                    id="INV417_grid_DetalleLpnAtrCab"
                                    application="INV417"
                                    rowsToFetch={30}
                                    rowsToDisplay={5}
                                    enableExcelExport
                                    editable={true}
                                    onBeforeFetch={onBeforeFetch}
                                    onAfterApplySort={onAfterApplySort}
                                />
                            </Col>
                            <Col lg={6}>
                                <Grid
                                    id="INV417_grid_DetalleLpnAtrDet"
                                    application="INV417"
                                    rowsToFetch={30}
                                    rowsToDisplay={5}
                                    enableExcelExport
                                    editable={true}
                                    onBeforeFetch={onBeforeFetch}
                                    onAfterApplySort={onAfterApplySort}
                                />
                            </Col>
                        </Row>
                        <Row>
                            <div className="row mb-4 text-center">
                                <div className="col-12">
                                    <FormButton id="btnAplicarFiltro" variant="primary" value={t("General_Sec0_btn_AplicarFiltro")} />
                                </div>
                            </div>
                        </Row>
                    </AccordionDetails>
                </Accordion>

                <br />

                <Row>

                    <Col lg={6}>
                        <h3>{t("INV410_Sec0_lbl_RegistrosDisponibles")}</h3>
                        <Grid
                            id="INV417_grid_1"
                            application="INV417"
                            rowsToFetch={30}
                            rowsToDisplay={15}
                            enableSelection
                            enableExcelExport
                            onBeforeFetch={appendParameters}
                            onBeforeFetch={appendParameters}
                            onBeforeApplyFilter={appendParameters}
                            onBeforeApplySort={appendParameters}
                            onBeforeMenuItemAction={appendParameters}
                            onBeforeFetchStats={appendParameters}
                            onBeforeExportExcel={appendParameters}
                            onBeforeInitialize={appendParameters}
                            onAfterMenuItemAction={onAfterMenuItemAction}
                        />
                    </Col>
                    <Col lg={6}>
                        <h3>{t("INV410_Sec0_lbl_RegistrosSeleccionados")}</h3>
                        <Grid
                            id="INV417_grid_2"
                            application="INV417"
                            rowsToFetch={30}
                            rowsToDisplay={15}
                            enableSelection
                            enableExcelExport
                            onBeforeFetch={appendParameters}
                            onBeforeApplyFilter={appendParameters}
                            onBeforeApplySort={appendParameters}
                            onBeforeMenuItemAction={appendParameters}
                            onBeforeFetchStats={appendParameters}
                            onBeforeInitialize={appendParameters}
                            onBeforeExportExcel={appendParameters}
                            onAfterMenuItemAction={onAfterMenuItemAction}
                        />
                    </Col>
                </Row>
            </Form>
        </Page>
    );
}