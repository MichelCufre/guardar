import React, { useState} from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Row, Col } from 'react-bootstrap';
import { Form, Field, StatusMessage } from '../../components/FormComponents/Form';
import { useTranslation } from 'react-i18next';

export default function INV411(props) {
    const { t } = useTranslation();
    const initialValues = {};
    const [nuInventario, setInventario] = useState(null);

    const onAfterPageLoad = (data) => {
        setInventario(data.parameters.find(d => d.id === "inventario").value);
    };
    
    const onAfterMenuItemAction = (context, data, nexus) => {
        nexus.getGrid("INV411_grid_1").refresh();
        nexus.getGrid("INV411_grid_2").refresh();
    };

    const appendParameters = (context, data, nexus) => {
        data.parameters = [
            { id: "nuInventario", value: nuInventario }
        ];
    };

    const onBeforeInitialize = (context, form, data, nexus) => {
        data.parameters = [
            { id: "nuInventario", value: nuInventario }
        ];
    };

    return (

        <Page
            icon="fas fa-copy"
            title={t("INV411_Sec0_pageTitle_Titulo")}
            {...props}
            application="INV411"
            onAfterLoad={onAfterPageLoad}
        >

            <Form
                id="INV411_form_1"                
                application="INV411"
                initialValues={initialValues}
                onBeforeInitialize={onBeforeInitialize}
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
            </Form>

            <Row >
                <Col lg={6}>
                    <h3>{t("INV410_Sec0_lbl_UbicacionesDisponibles")}</h3>
                    <Grid
                        id="INV411_grid_1"
                        application="INV411"
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
                    <h3>{t("INV410_Sec0_lbl_UbicacionesSeleccionadas")}</h3>
                    <Grid
                        id="INV411_grid_2"
                        application="INV411"
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
        </Page>
    );
}