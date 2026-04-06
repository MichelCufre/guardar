import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { REG015CreateOrUpdate } from './REG015CreateOrUpdateModal';


export default function REG015(props) {
    const { t } = useTranslation();
    const [isUpdate, setIsUpdate] = useState(false);
    const [showPopup, setShowPopup] = useState(false);
    const [empresa, setEmpresa] = useState(null);
    const [cliente, setCliente] = useState(null);
    const [producto, setProducto] = useState(null);

    const openFormDialog = () => {
        setShowPopup(true);
    }

    const closeFormDialog = (nexus) => {
        setShowPopup(false);
        setIsUpdate(false);
    }

    const handleBeforeButtonAction = (context, data, nexus) => {
        if (data.buttonId === "btnEditar") {
            context.abortServerCall = true;
            setEmpresa(data.row.cells.find(d => d.column === "CD_EMPRESA").value);
            setCliente(data.row.cells.find(d => d.column === "CD_CLIENTE").value);
            setProducto(data.row.cells.find(d => d.column === "CD_PRODUTO").value);
            setIsUpdate(true);
            openFormDialog(true);
        }
    }
    return (
        <Page
            title={t("REG015_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div style={{ textAlign: "center" }}>
                <button className="btn btn-primary" onClick={openFormDialog}>{t("REG015_Sec0_btn_CrearProductoProveedor")}</button>
            </div>
            <div className="row mb-4">
                <div className="col-12">
                    <Grid
                        id="REG015_grid_1"
                        application="REG015"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                        onBeforeButtonAction={handleBeforeButtonAction}
                    />
                </div>
            </div>

            <REG015CreateOrUpdate show={showPopup} onHide={closeFormDialog} isUpdate={isUpdate} empresa={empresa} cliente={cliente} producto={producto}  />
        </Page>
    );
}