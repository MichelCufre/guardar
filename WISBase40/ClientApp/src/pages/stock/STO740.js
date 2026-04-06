import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import Accordion from '@mui/material/Accordion';
import AccordionDetails from '@mui/material/AccordionDetails';
import AccordionSummary from '@mui/material/AccordionSummary';
import { forEach } from 'lodash';
import React, { useEffect, useState } from 'react';
import { Col, Row } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { FieldSelectAsync, Form, FormButton, StatusMessage } from '../../components/FormComponents/Form';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';

export default function STO740(props) {

    const { t } = useTranslation();

    const initialValues = {
        tipo: "",
    };

    const validationSchema = {
        tipo: Yup.string(),
    };

    const [filtro, setFiltro] = useState({ TipoLpn: null, AtributosCabezal: [], AtributosDetalle: [] });
    const [nexus, setNexus] = useState(null);
    const [tipoLpn, setTipoLpn] = useState(null);

    useEffect(() => {
        if (nexus) {
            nexus.getGrid('STO740_grid_LpnDet').refresh();
        }
    }, [filtro]);

    useEffect(() => {
        if (nexus) {
            nexus.getGrid('STO740_grid_AtrCab').refresh();
            nexus.getGrid('STO740_grid_AtrDet').refresh();
        }
    }, [tipoLpn]);

    const onBeforeFetch = (context, data, nexus) => {
        applyAttributeParameters(context, data, nexus);

        if (data.rowsToSkip == 0) {
            if (data.gridId == "STO740_grid_AtrCab") {
                setFiltro({ TipoLpn: tipoLpn, AtributosCabezal: [], AtributosDetalle: filtro.AtributosDetalle });
            } else if (data.gridId == "STO740_grid_AtrDet") {
                setFiltro({ TipoLpn: tipoLpn, AtributosCabezal: filtro.AtributosCabezal, AtributosDetalle: [] });
            }
        }
    }

    const onAfterApplySort = (context, data, nexus) => {
        applyAttributeParameters(context, data, nexus);

        if (data.gridId == "STO740_grid_AtrCab") {
            setFiltro({ TipoLpn: tipoLpn, AtributosCabezal: [], AtributosDetalle: filtro.AtributosDetalle });
        } else if (data.gridId == "STO740_grid_AtrDet") {
            setFiltro({ TipoLpn: tipoLpn, AtributosCabezal: filtro.AtributosCabezal, AtributosDetalle: [] });
        }
    }

    const onAfterValidateField = (context, form, query, nexus) => {
        const tipo = query.parameters.find(p => p.id === "tipo").value;

        if (tipo !== filtro.TipoLpn) {
            setNexus(nexus);
            setTipoLpn(tipo);
        }
    }

    const onBeforeButtonAction = (context, form, query, nexus) => {
        var filtro = { TipoLpn: tipoLpn, AtributosCabezal: [], AtributosDetalle: [] };

        forEach(nexus.getGrid('STO740_grid_AtrCab').getModifiedRows(), row => {
            var atributo = row.cells.find(c => c.column == 'VL_ATRIBUTO' && c.value != '');

            if (atributo) {
                filtro.AtributosCabezal.push({ Id: row.id, Value: atributo.value });
            }
        });

        forEach(nexus.getGrid('STO740_grid_AtrDet').getModifiedRows(), row => {
            var atributo = row.cells.find(c => c.column == 'VL_ATRIBUTO' && c.value != '');

            if (atributo) {
                filtro.AtributosDetalle.push({ Id: row.id, Value: atributo.value });
            }
        });

        setNexus(nexus);
        setFiltro(filtro);
    }

    const applyAttributeParameters = (context, data, nexus) => {
        data.parameters = [
            { id: "tipo", value: tipoLpn }
        ];
    };

    const applyLpnDetailParameters = (context, data, nexus) => {
        data.parameters = [
            { id: "filtro", value: JSON.stringify(filtro) }
        ];
    };

    return (
        <Page
            title={t("STO740_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <Form
                id="STO740_frm_1"
                application="STO740"
                initialValues={initialValues}
                validationSchema={validationSchema}
                onAfterValidateField={onAfterValidateField}
                onBeforeButtonAction={onBeforeButtonAction}
            >
                <Accordion>
                    <AccordionSummary
                        expandIcon={<ExpandMoreIcon />}
                        aria-controls="panel1-content"
                        id="panel1-header"
                    >
                        {t("STO740_Sec0_lbl_LpnFiltros")}
                    </AccordionSummary>
                    <AccordionDetails>
                        <Row>
                            <Col>
                                <div className="form-group" >
                                    <label htmlFor="tipo">{t("STO740_frm1_lbl_tipo")}</label>
                                    <FieldSelectAsync name="tipo" isClearable={true} />
                                    <StatusMessage for="tipo" />
                                </div>
                            </Col>
                        </Row>
                        <Row>
                            <Col>
                                <h4 className='form-title'>{t("STO740_Sec0_title_LpnDetAtrCab")}</h4>
                            </Col>
                            <Col>
                                <h4 className='form-title'>{t("STO740_Sec0_title_LpnDetAtrDet")}</h4>
                            </Col>
                        </Row>
                        <Row>
                            <Col>
                                <div className="row mb-4">
                                    <div className="col-12">
                                        <Grid
                                            id="STO740_grid_AtrCab"
                                            application="STO740"
                                            rowsToFetch={30}
                                            rowsToDisplay={5}
                                            enableExcelExport={true}
                                            editable={true}
                                            onBeforeFetch={onBeforeFetch}
                                            onAfterApplySort={onAfterApplySort}
                                            onBeforeInitialize={applyAttributeParameters}
                                            onBeforeFetchStats={applyAttributeParameters}
                                            onBeforeMenuItemAction={applyAttributeParameters}
                                            onBeforeApplyFilter={applyAttributeParameters}
                                            onBeforeExportExcel={applyAttributeParameters}
                                        />
                                    </div>
                                </div>
                            </Col>
                            <Col>
                                <div className="row mb-4">
                                    <div className="col-12">
                                        <Grid
                                            id="STO740_grid_AtrDet"
                                            application="STO740"
                                            rowsToFetch={30}
                                            rowsToDisplay={5}
                                            enableExcelExport={true}
                                            editable={true}
                                            onBeforeFetch={onBeforeFetch}
                                            onAfterApplySort={onAfterApplySort}
                                            onBeforeInitialize={applyAttributeParameters}
                                            onBeforeFetchStats={applyAttributeParameters}
                                            onBeforeMenuItemAction={applyAttributeParameters}
                                            onBeforeApplyFilter={applyAttributeParameters}
                                            onBeforeExportExcel={applyAttributeParameters}
                                        />
                                    </div>
                                </div>
                            </Col>
                        </Row>
                        <div className="row mb-4 text-center">
                            <div className="col-12">
                                <FormButton id="btnAplicarFiltro" variant="primary" value={t("General_Sec0_btn_AplicarFiltro")} />
                            </div>
                        </div>
                    </AccordionDetails>
                </Accordion>

                <br />

                <div className="row mb-4">
                    <div className="col-12">
                        <h4 className='form-title'>{t("STO740_Sec0_title_LpnDet")}</h4>
                        <Grid
                            id="STO740_grid_LpnDet"
                            application="STO740"
                            rowsToFetch={30}
                            rowsToDisplay={15}
                            enableExcelExport={true}
                            onBeforeInitialize={applyLpnDetailParameters}
                            onBeforeFetch={applyLpnDetailParameters}
                            onBeforeFetchStats={applyLpnDetailParameters}
                            onBeforeMenuItemAction={applyLpnDetailParameters}
                            onBeforeApplyFilter={applyLpnDetailParameters}
                            onBeforeApplySort={applyLpnDetailParameters}
                            onBeforeExportExcel={applyLpnDetailParameters}
                        />
                    </div>
                </div>
            </Form>
        </Page>
    );
}