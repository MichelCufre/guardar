import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Form, FieldSelectAsync, FieldDate, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { STO005ProductoContent } from './STO005ProductoContent';

export default function STO005(props) {
    const { t } = useTranslation();

    const [producto, setProducto] = useState(null);
    const [empresa, setEmpresa] = useState(null);

    const [stockGeneral, setStockGeneral] = useState({});


    const initialValues = {
        CD_EMPRESA: "",
        NM_EMPRESA: "",
        CD_PRODUTO: "",
        DS_PRODUTO: ""
    };

    const validationSchema =
    {
        CD_EMPRESA: Yup.string().required(),
        CD_PRODUTO: Yup.string().required().max(40),
        DT_INICIO: Yup.string().nullable(),
        DT_FIN: Yup.string().nullable()
    };

    const onAfterSubmit = (context, form, query, nexus) => {
        if (context.responseStatus === "OK") {
            const productoParam = query.parameters.find(d => d.id === "CD_PRODUTO");
            const empresaParam = query.parameters.find(d => d.id === "CD_EMPRESA");

            setProducto(productoParam ? productoParam.value : null);
            setEmpresa(empresaParam ? empresaParam.value : null);

            nexus.getGrid("STO005_grid_1").refresh();
        }
    };

    const onBeforeFetch = (context, data, nexus) => {
        data.parameters = [
            { id: "CD_PRODUTO", value: producto },
            { id: "CD_EMPRESA", value: empresa }
        ];
    };

    const onAfterFetch = (context, newRows, parameters, nexus) => {
        const datosStockGeneral = parameters.find(d => d.id === "datosStockGeneral");

        if (datosStockGeneral && datosStockGeneral.value)
            setStockGeneral(JSON.parse(datosStockGeneral.value));
    };

    return (
        <Page
            title={t("STO005_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div className="row mb-4">
                <div className="col">
                    <Form id="STO005_form_1" initialValues={initialValues}
                        validationSchema={validationSchema}
                        onAfterSubmit={onAfterSubmit}
                    >
                        <div className="row">
                            <div className="col">
                                <div className="row">
                                    <div className="col">
                                        <div className="form-group">
                                            <label htmlFor="CD_EMPRESA">{t("STO005_frm1_lbl_CD_EMPRESA")} <span className="required-badge">*</span></label>
                                            <FieldSelectAsync name="CD_EMPRESA" />
                                            <StatusMessage for="CD_EMPRESA" />
                                        </div>
                                    </div>
                                </div>
                                <div className="row">
                                    <div className="col">
                                        <div className="form-group">
                                            <label htmlFor="CD_PRODUTO">{t("STO005_frm1_lbl_CD_PRODUTO")} <span className="required-badge">*</span></label>
                                            <FieldSelectAsync name="CD_PRODUTO" />
                                            <StatusMessage for="CD_PRODUTO" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>                        
                        <div className="row">
                            <div className="col">
                                <SubmitButton id="btnConfirmar" value={t("STO005_frm1_btn_Filtrar")} />
                            </div>
                        </div>                        
                    </Form>
                </div>
            </div>
            <hr />
            <STO005ProductoContent stockGeneral={stockGeneral} />
            <div className="row mb-4">
                <div className="col-12">
                    <Grid
                        id="STO005_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                        onBeforeFetch={onBeforeFetch}
                        onBeforeExportExcel={onBeforeFetch}
                        onAfterFetch={onAfterFetch}
                        onBeforeApplyFilter={onBeforeFetch}
                    />
                </div>
            </div>
        </Page>
    );
}