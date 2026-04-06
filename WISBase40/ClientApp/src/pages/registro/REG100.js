import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { REG100CreateEmpresaModal } from './REG100CreateEmpresaModal';
import { REG100UpdateReferenciaExternaModal } from './REG100ReferenciaExternalModal';
import { REG100UpdateEmpresaModal } from './REG100UpdateEmpresaModal';
import { REG100CodigosMultidatoModal } from './REG100CodigosMultidatoModal';
import { REG100AsociarCodigoMultidatoModal } from './REG100AsociarCodigoMultidatoModal';
export default function REG100(props) {

    const { t } = useTranslation();

    const [showPopupAdd, setShowPopupAdd] = useState(false);
    const [showPopupUpdate, setShowPopupUpdate] = useState(false);
    const [showPopupCodigosMultidato, setShowPopupCodigosMultidato] = useState(false);

    const [showPopupRef, setShowPopupRef] = useState(false);
    const [keyAgente, setkeyAgente] = useState(null);
    const [empresa, setEmpresa] = useState(null);

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

    const openPopupRef = () => {
        setShowPopupRef(true);
    }

    const closePopupRef = () => {
        setShowPopupRef(false);
    }

    const openPopupCodigos = () => {
        setShowPopupCodigosMultidato(true);
    }

    const closePopupCodigos = () => {
        setShowPopupCodigosMultidato(false);
    }

    const GridOnBeforeButtonAction = (context, data, nexus) => {

        if (data.buttonId === "btnEditar") {

            context.abortServerCall = true;

            setkeyAgente(
                [
                    { id: "IdEmpresa", value: data.row.cells.find(w => w.column == "CD_EMPRESA").value }
                ]
            );

            openFormUpdateDialog();

        } else if (data.buttonId === "btnVerTipoRecepcion") {

            context.abortServerCall = true;

            setkeyAgente(
                [
                    { id: "empresa", value: data.row.cells.find(w => w.column == "CD_EMPRESA").value }
                ]
            );

            openPopupRef();
        }
        else if (data.buttonId === "btnVerCodigosMutiDato") {

            context.abortServerCall = true;

            setEmpresa(data.row.cells.find(d => d.column === "CD_EMPRESA").value);

            openPopupCodigos();
        }

    };

    const openPopRefBoton = () => {
        setkeyAgente([]);
        openPopupRef();
    }

    return (

        <Page
            title={t("REG100_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div style={{ textAlign: "center" }}>
                <button className="btn btn-primary" onClick={openFormDialog}>{t("REG100_Sec0_btn_AgregarLinea")}</button>
                &nbsp;
                <button className="btn btn-primary ml-2" onClick={openPopRefBoton}>{t("REG100_Sec0_btn_TiposRec")}</button>
            </div>

            <Grid
                id="REG100_grid_1"
                rowsToFetch={30}
                rowsToDisplay={15}
                enableExcelExport
                onBeforeButtonAction={GridOnBeforeButtonAction}
            />

            <REG100CreateEmpresaModal show={showPopupAdd} onHide={closeFormDialog} />
            <REG100UpdateEmpresaModal show={showPopupUpdate} onHide={closeFormUpdateDialog} agente={keyAgente} />
            <REG100UpdateReferenciaExternaModal show={showPopupRef} onHide={closePopupRef} empresa={keyAgente} />
            <REG100AsociarCodigoMultidatoModal show={showPopupCodigosMultidato} onHide={closePopupCodigos} empresa={empresa} />
        </Page>
    );
}
