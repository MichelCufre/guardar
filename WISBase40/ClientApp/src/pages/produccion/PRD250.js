import React, { useRef } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Form, Field, FieldSelect, FieldSelectAsync, FieldDate, SubmitButton, Button, StatusMessage } from '../../components/FormComponents/Form';
import { Page } from '../../components/Page';
import * as Yup from 'yup';
import { useTranslation } from 'react-i18next';

export default function PRD250(props) {
    const { t } = useTranslation();

    return (

        <Page
            icon="fas fa-file"
            title={t("PRD250_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div className="row mb-4">
                <div className="col-12">
                    <Grid
                        id="PRD250_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                    />
                </div>
            </div>
        </Page>
    );
}