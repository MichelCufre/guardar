import React, { useRef } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Form, Field, FieldDate, SubmitButton, StatusMessage } from '../../components/FormComponents/Form';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
export default function DOC402(props) {
    const { t } = useTranslation();
    return (

        <Page
            icon="fas fa-file"
            title={t("DOC402_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div className="row">
                <div className="col-12">
                    <Grid id="DOC402_grid_documentos"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                        enableExcelImport={false}
                    />
                </div>
            </div>

        </Page>
    );
}