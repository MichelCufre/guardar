import React, { useRef, useState} from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Form, Field, FieldDate, SubmitButton, StatusMessage } from '../../components/FormComponents/Form';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
export default function DOC410(props) {
    const { t } = useTranslation();
    const onAfterMenuItemAction = (context, data, nexus) => {
        nexus.getGrid("DOC410_grid_documentos").refresh();
    }
    return (

        <Page
            icon="fas fa-file"
            title={t("DOC410_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div className="row">
                <div className="col-12">
                    <Grid id="DOC410_grid_documentos"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                        enableSelection
                        onAfterMenuItemAction={onAfterMenuItemAction}
                        enableExcelImport={false}
                    />
                </div>
            </div>

        </Page>
    );
}