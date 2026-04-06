import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';

export default function DOC082(props) {
    const { t } = useTranslation();

    const [grid2State, setgrid2State] = useState("hidden");
    const [grid3State, setgrid3State] = useState("hidden");

    const GridOnAfterInitialize = (context, form, parameters) => {

        const mostrarGrid2 = parameters.find(p => p.id === "mostrarGrid2");
        const mostrarGrid3 = parameters.find(p => p.id === "mostrarGrid3");


        if (mostrarGrid2 && mostrarGrid2.value === "true")
            setgrid2State("");

        if (mostrarGrid3 && mostrarGrid3.value === "true")
            setgrid3State("");
    }

    return (

        <Page
            icon="fas fa-file"
            title={t("DOC082_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div className="mb-4">
                <div className="col-12">
                    <legend>{t("DOC082_frm1_lbl_legend1")}</legend>
                    <Grid id="DOC082_grid_1" rowsToFetch={30} rowsToDisplay={15} enableExcelExport />
                </div>
            </div>
            <div className="mb-4">
                <div className={grid2State}>
                    <div className="col-12">
                        <legend>{t("DOC082_frm1_lbl_legend2")}</legend>
                        <Grid id="DOC082_grid_2" rowsToFetch={30} rowsToDisplay={15} onAfterInitialize={GridOnAfterInitialize} enableExcelExport/>
                    </div>
                </div>
            </div>
            <div className="mb-4">
                <div className={grid3State}>

                    <div className="col-12">
                        <legend>{t("DOC082_frm1_lbl_legend3")}</legend>
                        <Grid id="DOC082_grid_3" rowsToFetch={30} rowsToDisplay={15} onAfterInitialize={GridOnAfterInitialize} enableExcelExport/>
                    </div>
                </div>
            </div>
        </Page >
    );
}