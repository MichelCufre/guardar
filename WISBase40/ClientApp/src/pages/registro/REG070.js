import React, { useState, useRef } from 'react';
import { Page } from '../../components/Page';
import { Grid } from '../../components/GridComponents/Grid';
import { Form, Field, FieldDate, FieldSelect, FieldSelectAsync, FieldTextArea, FormButton, SubmitButton, StatusMessage } from '../../components/FormComponents/Form';
import { Modal, Row, Col, FormGroup, Button, Tab, Tabs } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { REG070CreateZonaModal } from './REG070CreateZonaModal';
import { REG070UpdateZonaModal } from './REG070UpdateZonaModal';

export default function REG070(props) {

    const { t } = useTranslation();

    const [showPopupAdd, setShowPopupAdd] = useState(false);
    const [showPopupUpdate, setShowPopupUpdate] = useState(false);
    const [keyZona, setkeyZona] = useState(null);

    const openFormDialog = () => {
        setShowPopupAdd(true);
    }

    const closeFormDialog = () => {
        setShowPopupAdd(false);
    }

    const openFormUpdateDialog = () => {
        setShowPopupUpdate(true);
    }

    const closeFormUpdateDialog = () => {
        setShowPopupUpdate(false);
    }

    const GridOnBeforeButtonAction = (context, data, nexus) =>
    {
        if (data.buttonId === "btnEditar")
        {
            context.abortServerCall = true;
            setkeyZona([{ id: "idZonaUbicacion", value: data.row.cells.find(w => w.column == "CD_ZONA_UBICACION").value }]);
            openFormUpdateDialog();
        }
    };

    const handleAfterMenuItemAction = (context, data, nexus) => {
        nexus.getGrid("REG070_grid_1").refresh();
    };

    const handleBeforeMenuItemAction = (context, data, nexus) => {        
        setkeyZona([{ id: "idZonaUbicacion", value: data.row.cells.find(w => w.column == "CD_ZONA_UBICACION").value }]);
    };

    return (

        <Page
            title={t("REG070_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div style={{ textAlign: "center" }}>
                <button className="btn btn-primary" onClick={openFormDialog}>{t("REG070_Sec0_btn_AgregarZona")}</button>
            </div>

            <Grid
                id="REG070_grid_1"
                rowsToFetch={30}
                rowsToDisplay={15}
                enableExcelExport
                onBeforeButtonAction={GridOnBeforeButtonAction}
                onAfterMenuItemAction={handleAfterMenuItemAction}
                enableSelection
            />
            <REG070CreateZonaModal show={showPopupAdd} onHide={closeFormDialog} />
            <REG070UpdateZonaModal show={showPopupUpdate} onHide={closeFormUpdateDialog} zona={keyZona} />
        </Page>
    );
}