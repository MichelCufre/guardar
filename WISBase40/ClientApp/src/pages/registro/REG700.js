import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { REG700CreateRecorridoModal } from './REG700CreateRecorridoModal';
import { REG700UpdateRecorridoModal } from './REG700UpdateRecorridoModal';
import { REG700DetallesModal } from './REG700DetallesModal';
import { REG700UbicacionesSinAsociarModal } from './REG700UbicacionesSinAsociarModal';

import { REG700AsociarAplicacionModal } from './REG700AsociarAplicacionModal';
import { REG700AsociarAplicacionUsuarioModal } from './REG700AsociarAplicacionUsuarioModal';

export default function REG700(props) {

    const { t } = useTranslation();

    const [showPopupAdd, setShowPopupAdd] = useState(false);
    const [showPopupUpdate, setShowPopupUpdate] = useState(false);
    const [showPopupDetalles, setShowPopupDetalles] = useState(false);
    const [showPopupUbicacionesSinAsociar, setShowPopupUbicacionesSinAsociar] = useState(false);
    const [isImportEnabled, setIsImportEnabled] = useState(false);
    const [isBtnCreateDisabled, setBtnCreateDisabled] = useState(true);
    const [numeroRecorrido, setNumeroRecorrido] = useState(null);
    const [nombreRecorrido, setNombreRecorrido] = useState(null);
    const [predio, setPredio] = useState(null);
    const [isRecorridoDefaul, setIsRecorridoDefault] = useState(null);
    const [_nexus, setNexus] = useState(false);

    const [showPopupAsociarAplicacion, setShowPopupAsociarAplicacion] = useState(false);
    const [showPopupAsociarAplicacionUsuario, setShowPopupAsociarAplicacionUsuario] = useState(false);

    const openAddDialog = () => {
        setShowPopupAdd(true);
    }

    const closeAddDialog = (recorrido,detalleImportHabilitado) => {
        setShowPopupAdd(false);

        if (recorrido) {
            setNumeroRecorrido(recorrido);

            var shouldDetailsModalImportExcel = detalleImportHabilitado === "S";

            setIsImportEnabled(shouldDetailsModalImportExcel);
            setShowPopupDetalles(true);
        } else
            setNumeroRecorrido(null);

        if (_nexus) _nexus.getGrid("REG700_grid_1").refresh();
    }

    const closeUpdateDialog = () => {
        setShowPopupUpdate(false);

        if (_nexus) _nexus.getGrid("REG700_grid_1").refresh();
    }

    const closeViewDialog = () => {
        setShowPopupDetalles(false);
    }

    const closeDialogUbicacionesSinAsociar = () => {
        setShowPopupUbicacionesSinAsociar(false);
    }

    const closeAsociarAplicacionUsuarioDialog = () => {
        setShowPopupAsociarAplicacionUsuario(false);
    }

    const closeAsociarAplicacionDialog = () => {
        setShowPopupAsociarAplicacion(false);
    }

    const onAfterPageLoad = (data) => {
        if (data.parameters.find(f => f.id === "REG700_CREAR_RECORRIDO_HABILITADO")) {
            var recorridoHabilitado = data.parameters.find(f => f.id === "REG700_CREAR_RECORRIDO_HABILITADO").value === "S";

            setBtnCreateDisabled(!recorridoHabilitado)
        }
    };

    const onBeforeInitialize = (context, data, nexus) => {
        setNexus(nexus);
    }

    const onAfterInitialize = (context, grid, parameters, nexus) => {

    };

    const gridOnAfterButtonAction = (data, nexus) => {

        setNumeroRecorrido(data.parameters.find(f => f.id === "REG700_NU_RECORRIDO").value);
        setNombreRecorrido(data.parameters.find(f => f.id === "REG700_NM_RECORRIDO").value);
        setIsRecorridoDefault(data.parameters.find(f => f.id === "REG700_FL_DEFAULT").value);
        setPredio(data.parameters.find(f => f.id === "REG700_NU_PREDIO").value);

        if (data.buttonId === "btnEditar")
            setShowPopupUpdate(true);
        else if (data.buttonId === "btnViewDetails") {
            var shouldDetailsModalImportExcel = data.parameters.find(f => f.id === "REG700_DETALLE_IMPORT_HABILITADO") &&
                data.parameters.find(f => f.id === "REG700_DETALLE_IMPORT_HABILITADO").value === "S";

            setIsImportEnabled(shouldDetailsModalImportExcel);
            setShowPopupDetalles(true);
        }
        else if (data.buttonId === "btnUbicacionesSinAsociar")
            setShowPopupUbicacionesSinAsociar(true);
        else if (data.buttonId === "btnAsociarAplicaciones")
            setShowPopupAsociarAplicacion(true);
        else if (data.buttonId === "btnAsociarAplicacionesUsuario")
            setShowPopupAsociarAplicacionUsuario(true);
    }

    return (

        <Page
            title={t("REG700_Sec0_pageTitle_Titulo")}
            onAfterLoad={onAfterPageLoad}
            {...props}
        >
            <div style={{
                textAlign: "center",
            }}>
                <button className="btn btn-primary" onClick={openAddDialog} disabled={isBtnCreateDisabled} >{t("REG700_Sec0_btn_Crear")}</button>
            </div>

            <Grid
                application="REG700"
                id="REG700_grid_1"
                rowsToFetch={30}
                rowsToDisplay={15}
                enableExcelExport
                onBeforeInitialize={onBeforeInitialize}
                onAfterInitialize={onAfterInitialize}
                onAfterButtonAction={gridOnAfterButtonAction}
            />

            <REG700CreateRecorridoModal show={showPopupAdd} onHide={closeAddDialog} numeroRecorrido={numeroRecorrido} />
            <REG700UpdateRecorridoModal show={showPopupUpdate} onHide={closeUpdateDialog} numeroRecorrido={numeroRecorrido} />
            <REG700DetallesModal show={showPopupDetalles} onHide={closeViewDialog} numeroRecorrido={numeroRecorrido} isImportEnabled={isImportEnabled} />
            <REG700UbicacionesSinAsociarModal show={showPopupUbicacionesSinAsociar} onHide={closeDialogUbicacionesSinAsociar} numeroRecorrido={numeroRecorrido} />
            <REG700AsociarAplicacionModal show={showPopupAsociarAplicacion} onHide={closeAsociarAplicacionDialog} numeroRecorrido={numeroRecorrido} nombreRecorrido={nombreRecorrido} isRecorridoDefaul={isRecorridoDefaul} predio ={predio} />
            <REG700AsociarAplicacionUsuarioModal show={showPopupAsociarAplicacionUsuario} onHide={closeAsociarAplicacionUsuarioDialog} numeroRecorrido={numeroRecorrido} nombreRecorrido={nombreRecorrido} isRecorridoDefaul={isRecorridoDefaul} predio={predio} />
        </Page>
    );
}
