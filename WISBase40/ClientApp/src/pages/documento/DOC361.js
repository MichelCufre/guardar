import React from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';

export default function DOC361(props) {
    const { t } = useTranslation();

    const onAfterMenuItemAction = (context, data, nexus) => {
        if (data.redirect) {  
            window.location = data.redirect; 
        } 
    }

    return (

        <Page
            icon="fas fa-file"
            title={t("DOC361_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="DOC361_grid_1"
                        onAfterMenuItemAction={onAfterMenuItemAction}
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableSelection={true}
                        enableExcelExport />
                </div>
            </div>
        </Page>
    );
}