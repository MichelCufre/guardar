import React from 'react';
import { useTranslation } from 'react-i18next';
import { Grid } from '../../components/GridComponents/Grid';
import { withPageContext } from '../../components/WithPageContext';

function InternalREG700DetallesRecorrido(props) {

    const { t } = useTranslation();

    //--------------GRID--------------



    //----------------------------------------

    return (
        <div>
            <Grid
                application="REG700DetallesRecorrido"
                id={props.id}
                onBeforeInitialize={props.onBeforeInitialize}
                onBeforeFetch={props.onBeforeFetch}
                onBeforeFetchStats={props.onBeforeFetchStats}
                onBeforeExportExcel={props.onBeforeExportExcel}
                onBeforeImportExcel={props.onBeforeExportExcel}
                onBeforeApplyFilter={props.onBeforeApplyFilter}
                onBeforeApplySort={props.onBeforeApplySort}
                rowsToFetch={30}
                rowsToDisplay={props.rowsToDisplay}
                enableExcelExport
                enableExcelImport={props.isImportEnabled}
            />
        </div>
    );
}

export const REG700DetallesRecorrido = withPageContext(InternalREG700DetallesRecorrido);