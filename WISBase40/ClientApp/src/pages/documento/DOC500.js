import React from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Form, Field, StatusMessage } from '../../components/FormComponents/Form';
import { useTranslation } from 'react-i18next';

export default function DOC500(props) {
    const { t } = useTranslation();

    const fieldSetStyle = { border: "1px solid #ddd", margin: "10px", width: "100%" };

    const initialValues = {
      
    };
    return (

        <Page
            icon="fas fa-file"
            title={t("DOC500_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="DOC500_grid_1" rowsToFetch={30} rowsToDisplay={15}
                        enableExcelExport
                        enableExcelImport={false}
                    />
                </div>
            </div>
        </Page>
    );
}