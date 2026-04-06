import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { Page } from '../../components/Page';

import { Grid } from '../../components/GridComponents/Grid';

export default function EVT001(props) {
    const { t } = useTranslation();

    const [ipCompartida, setIpCompartida] = useState(null);

    const GridOnAfterInitialize = (context, grid,parameters, nexus) => {

        setIpCompartida(parameters.find(d => d.id === "IP_COMPARTIDA").value);

    };


    const GridOnBeforeButtonAction = (context, data, nexus) => {

        context.abortServerCall = true;

        if (data.buttonId === "btnVer") {
            var link = data.row.cells.find(w => w.column == "LK_RUTA").value;
            window.open(ipCompartida + link.replace(/#/gi, '%23'), "_newtab");
        }

    };

    return (

        <Page
            title={t("EVT001_Sec0_pageTitle_Titulo")}
            {...props}
        >

            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="EVT001_grid_1" rowsToFetch={30} rowsToDisplay={15} enableExcelExport
                        onAfterInitialize={GridOnAfterInitialize}
                        onBeforeButtonAction={GridOnBeforeButtonAction}
                    />
                </div>
            </div>

        </Page>
    );
}
