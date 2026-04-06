import React, { useState, useRef } from 'react';
import { Page } from '../../components/Page';
import { Grid } from '../../components/GridComponents/Grid';
import { Form, Field, FieldDate, FieldSelect, FieldSelectAsync, FieldTextArea, FormButton, SubmitButton, StatusMessage } from '../../components/FormComponents/Form';
import { Modal, Row, Col, FormGroup, Button, Tab, Tabs } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { REG240CreateVehiculoModal } from './REG240CreateVehiculo';
import { REG240UpdateVehiculoModal } from './REG240UpdateVehiculo';

export default function REG240(props) {
    const { t } = useTranslation();

    const [showModalAdd, setShowModalAdd] = useState(false);
    const [showModalUpdate, setShowModalUpdate] = useState(false);
    const [vehiculo, setVehiculo] = useState(null);

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
        if (data.buttonId === "btnEditar") {
            context.abortServerCall = true;

            const cellVehiculo = data.row.cells.find(d => d.column === "CD_VEICULO");

            setVehiculo(cellVehiculo.value);
            openFormUpdateDialog();
        }
    };

    const handleGridAfterButtonAction = (data, nexus) => {
        nexus.getGrid("REG240_grid_1").refresh();
    }

    return (

        <Page
            title={t("REG240_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div style={{ textAlign: "center" }}>
                <button className="btn btn-primary" onClick={openFormDialog}>{t("REG240_Sec0_btn_AgregarVehiculo")}</button>
            </div>

            <Grid
                id="REG240_grid_1"
                rowsToFetch={30}
                rowsToDisplay={15}
                enableExcelExport
                onBeforeButtonAction={handleGridBeforeButtonAction}
                onAfterButtonAction={handleGridAfterButtonAction}
                editable
            />
            <REG240CreateVehiculoModal show={showModalAdd} onHide={closeFormDialog} />
            <REG240UpdateVehiculoModal show={showModalUpdate} onHide={closeFormUpdateDialog} vehiculo={vehiculo} />
        </Page>
    );
}