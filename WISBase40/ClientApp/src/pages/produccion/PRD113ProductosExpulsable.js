import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Button, Row, Col } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { Form, Field, FieldSelect, FieldSelectAsync, StatusMessage, FieldTextArea } from '../../components/FormComponents/Form';
import * as Yup from 'yup';
import { withPageContext } from '../../components/WithPageContext';
import { Page } from '../../components/Page';

function InternalPRD113ProductosExpulsable(props) {
    const { t } = useTranslation();

    const [stockSelection, setStockSelection] = useState("");
    const [_nexus, setNexus] = useState(null);
    const [isHabilitada, setIsHabilitada] = useState(false); 
    const [isExplusarConTransferencia, setIsExplusarConTransferencia] = useState("");
    const [codigoImpresora, setCodigoImpresora] = useState("");
   
    const expulsarMasivo = (evt) => {
        setIsHabilitada(true);
        _nexus.getGrid("ProductosExpul_grid_1").triggerMenuAction("btnExpedir", false, evt.ctrlKey);
    }

    const expulsarMasivoTransferencia = (evt) => {
        setIsHabilitada(true);
        _nexus.getGrid("ProductosExpul_grid_1").triggerMenuAction("btnExpedirTransferencia", false, evt.ctrlKey);
    }

    const onAfterMenuItemAction = (context, data, nexus) => {
        
        setIsExplusarConTransferencia(data.parameters.find(w => w.id === "isExplusarConTransferencia").value);
        setStockSelection(data.parameters.find(w => w.id === "PRD113_ProductosExpulsable").value)
        context.abortUpdate = true;
        context.abortHideLoading = true;

        _nexus.getGrid("ProductosExpul_grid_1").commit(true, true);
    }

    const onAfterInitialize = (context, grid, parameters, nexus) => {
        setNexus(nexus);

    };
    const onBeforeCommit = (context, data, nexus) => {
        data.parameters.push({ id: "codigoImpresora", value: _nexus.getForm("PRD113Expulsable_form").getFieldValue("impresora") });
        data.parameters.push({ id: "PRD113_ProductosExpulsable", value: stockSelection });
        data.parameters.push({ id: "isExplusarConTransferencia", value: isExplusarConTransferencia });
    }

    const onAfterCommit = (context, rows, parameters, nexus) => {
        setIsHabilitada(false);
        if (context.status !== "ERROR") {
            nexus.getGrid("ProductosExpul_grid_1").refresh();
        }
    }
    const onAfterInitializeForm = (context, form, query, nexus) => {
 
        setIsHabilitada(false);
        
        if (query.parameters.some(x => x.id == "PRD113_HABILITADO_PRODUCCION")) {

            var habilitadoProduccion = query.parameters.find(x => x.id == "PRD113_HABILITADO_PRODUCCION").value;
            if (habilitadoProduccion == "N") {
                setIsHabilitada(true);
            }
        }

    };
    const handleFormBeforeSubmit = (context, form, query, nexus) => {
        context.abortServerCall = true;
    }
    return (

        <Form
            id="PRD113Expulsable_form"
            application="PRD113ProductosExpulsable"
            onAfterInitialize={onAfterInitializeForm}
            onBeforeSubmit={handleFormBeforeSubmit}
        >

            <br></br>
            <Row>
                <div className="col-4">
                    <Col>
                        <div className="form-group" >
                            <label htmlFor="impresora">{t("IMP080_frm1_lbl_impresora")}</label>
                            <FieldSelect name="impresora" isDisabled={isHabilitada}  />
                            <StatusMessage for="impresora" />
                        </div>
                    </Col>
                </div>
            </Row>
            <br></br>
            <Row>
                <Col span={6} style={{ maxWidth: "85%" }}>
                    <Grid
                        id="ProductosExpul_grid_1"
                        application="PRD113ProductosExpulsable"
                        rowsToFetch={30}
                        rowsToDisplay={10}
                        enableExcelExport
                        enableExcelImport={false}
                        enableSelection
                        onAfterInitialize={onAfterInitialize}
                        onAfterMenuItemAction={onAfterMenuItemAction}
                        onBeforeCommit={onBeforeCommit}
                        onAfterCommit={onAfterCommit}
                    />
                </Col>
                <Col span={6} style={{ display: 'flex', flexDirection: 'column', justifyContent: 'center', alignItems: 'center', height: '200px', maxWidth: '15%', marginTop: '90px' }}>
                    <Row style={{ marginBottom: '10px', width: '100%', justifyContent: 'center' }}>
                        <Button variant="primary" onClick={expulsarMasivo} disabled={isHabilitada}>
                            {t("PRD112_Sec0_btn_Expulsar")}
                        </Button>
                    </Row>
                    <Row style={{ marginBottom: '10px', width: '100%', justifyContent: 'center' }}>
                        <Button variant="primary" onClick={expulsarMasivoTransferencia} disabled={isHabilitada}>
                            {t("PRD112_Sec0_btn_ExpulsarEtiqueta")}
                        </Button>
                    </Row>
                </Col>
            </Row>
        </Form>

    );
}

export const PRD113ProductosExpulsable = withPageContext(InternalPRD113ProductosExpulsable);