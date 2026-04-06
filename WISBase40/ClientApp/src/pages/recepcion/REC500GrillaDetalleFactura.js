import React from 'react';
import { Page } from '../../components/Page';
import { Grid } from '../../components/GridComponents/Grid';
import { useTranslation } from 'react-i18next';

export default function REC500GrillaDetalleFactura(props) {
    const { t } = useTranslation();

    const facturaParams = props.factura || [];

    const onBeforeFetch = (context, data, nexus) => {
        data.parameters = [];
    };

    const onAfterInitialize = (context, rows, parameters, nexus) => {
            
    };


    return (
        <Page
            title={t("REC500GrillaDetalleFactura_Sec0_pageTitle_Titulo")}
            {...props}
            load
        >
            <div className="row mb-4">
                <div className="col-12">
                    <Grid
                        application="REC500GrillaDetalleFactura"
                        id="REC500GrillaDetalleFactura_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        onBeforeFetch={onBeforeFetch}
                        onBeforeFetchStats={onBeforeFetch}
                        onBeforeApplyFilter={onBeforeFetch}
                        onBeforeApplySort={onBeforeFetch}
                        onBeforeExportExcel={onBeforeFetch}
                        onAfterInitialize={onAfterInitialize}
                        enableExcelExport
                        autofocus={true}
                    />
                </div>
            </div>
        </Page>
    );
}
