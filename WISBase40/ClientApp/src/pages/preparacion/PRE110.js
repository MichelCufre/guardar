import React from 'react';
import { useTranslation } from 'react-i18next';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';

export default function PRE110(props) {

    const { t } = useTranslation();
    const onAfterCommit = (context, rows, parameters, nexus) => {
        nexus.getGrid("PRE110_grid_1").refresh();
    }
    const onAfterMenuItemAction = (context, data, nexus) => {
        nexus.getGrid("PRE110_grid_1").refresh();
    }
    return (

        <Page
            title={t("PRE110_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="PRE110_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                        onAfterCommit={onAfterCommit}
                        onAfterMenuItemAction={onAfterMenuItemAction}
                        enableSelection
                    />
                </div>
            </div>
        </Page>
    );
}