import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Form, FieldSelectAsync, FieldDate, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';

export default function STO500(props) {

    const { t } = useTranslation();

    return (
        <Page
            icon="fas fa-cubes"
            title={t("STO500_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div className="row mb-4">
                <div className="col">
                    <Grid
                        id="STO500_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                    />
                </div>
            </div>
        </Page>
    );
}