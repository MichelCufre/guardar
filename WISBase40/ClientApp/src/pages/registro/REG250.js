import React, { useState, useRef } from 'react';
import { Page } from '../../components/Page';
import { Grid } from '../../components/GridComponents/Grid';
import { Form, Field, FieldDate, FieldSelect, FieldSelectAsync, FieldTextArea, FormButton, SubmitButton, StatusMessage } from '../../components/FormComponents/Form';
import { Modal, Row, Col, FormGroup, Button, Tab, Tabs } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { REG250CreateTipoVehiculoModal } from './REG250CreateTipoVehiculo';
import { REG250UpdateTipoVehiculoModal } from './REG250UpdateTipoVehiculo';

export default function REG250(props) {

    const { t } = useTranslation();

    const [showModalAdd, setShowModalAdd] = useState(false);
    const [showModalUpdate, setShowModalUpdate] = useState(false);
    const [tipoVehiculo, setTipoVehiculo] = useState(null);

    const openFormDialog = () => {
        setShowModalAdd(true);
    }

    const closeFormDialog = () => {
        setShowModalAdd(false);
    }

    const openFormUpdateDialog = () => {
        setShowModalUpdate(true);
    }

    const closeFormUpdateDialog = () => {
        setShowModalUpdate(false);
    }

    const handleGridBeforeButtonAction = (context, data, nexus) => {
        context.abortServerCall = true;

        const cellVehiculo = data.row.cells.find(d => d.column === "CD_TIPO_VEICULO");

        setTipoVehiculo(cellVehiculo.value);
        openFormUpdateDialog();
    };

    return (

        <Page
            title={t("REG250_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div style={{ textAlign: "center" }}>
                <button className="btn btn-primary" onClick={openFormDialog}>{t("REG250_Sec0_btn_AgregarTipo")}</button>
            </div>

            <Grid
                id="REG250_grid_1"
                rowsToFetch={30}
                rowsToDisplay={15}
                enableExcelExport
                onBeforeButtonAction={handleGridBeforeButtonAction}
                editable
            />
            <REG250CreateTipoVehiculoModal show={showModalAdd} onHide={closeFormDialog} />
            <REG250UpdateTipoVehiculoModal tipoVehiculo={tipoVehiculo} show={showModalUpdate} onHide={closeFormUpdateDialog} />
        </Page>
    );
}