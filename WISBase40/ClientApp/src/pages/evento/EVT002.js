import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { Page } from '../../components/Page';

import { Grid } from '../../components/GridComponents/Grid';

export default function EVT002(props) {
    const { t } = useTranslation();


    const onAfterButtonAction = (data, nexus) => {

        if (data.buttonId === "btnBorrar" || data.buttonId === "btnActivar") {
            nexus.getGrid("EVT002_grid_1").refresh();
        }
    };

    const GridOnBeforeButtonAction = (context, data, nexus) => {

        context.abortServerCall = true;

        //if (data.buttonId === "btnVer") {
        //    var link = data.row.cells.find(w => w.column == "LK_RUTA").value;
        //    window.open(ipCompartida + link.replace(/#/gi, '%23'), "_newtab");
        //}
        //else
        if (data.buttonId === "btnDetalles") {
            context.abortServerCall = false;
        }
        else if (data.buttonId === "btnBorrar") {
            context.abortServerCall = false;

        }
        else if (data.buttonId === "btnActivar") {
            context.abortServerCall = false;

        }

    };

    return (

        <Page
            title={t("EVT002_Sec0_pageTitle_Titulo")}
            {...props}
        >

            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="EVT002_grid_1" rowsToFetch={30} rowsToDisplay={15} enableExcelExport
                        onBeforeButtonAction={GridOnBeforeButtonAction}
                        onAfterButtonAction={onAfterButtonAction}

                    />
                </div>
            </div>

        </Page>
    );
}
