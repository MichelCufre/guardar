import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Form } from '../../components/FormComponents/Form';
import { useTranslation } from 'react-i18next';
import Table from 'react-bootstrap/Table';
import INV418 from './INV418';

export default function INV414(props) {
    const { t } = useTranslation();
    const initialValues = {};
    const [showPopupAtributos, setShowPopupAtributos] = useState(false);    
    const [nuInventarioDetalle, setNuInventarioDetalle] = useState("");

    const greenStyle = {
        backgroundColor: "#9FF781",
        color: "black"
    };

    const yellowStyle = {
        backgroundColor: "#FBFF90",
        color: "black"
    };

    const redStyle = {
        backgroundColor: "#F78181",
        color: "black"
    };

    const blueStyle = {
        backgroundColor: "#9CC1FF",
        color: "black"
    };

    const onAfterMenuItemAction = (context, data, nexus) => {
        nexus.getGrid("INV414_grid_1").refresh();
    };

    const openPopupAtributos = () => {
        setShowPopupAtributos(true);
    }

    const closePopupAtributos = () => {
        setShowPopupAtributos(false);
    }

    const onBeforeButtonAction = (context, data, nexus) => {
        if (data.buttonId === "btnAtributosDetalleTemporal") {
            context.abortServerCall = true;
            setNuInventarioDetalle(data.row.cells.find(w => w.column == "NU_INVENTARIO_ENDERECO_DET").value);
            openPopupAtributos();
        }
    }

    return (

        <Page
            icon="fas fa-copy"
            title={t("INV414_Sec0_pageTitle_Titulo")}
            {...props}
        >

            <Form
                id="INV414_form_1"
                initialValues={initialValues}
            >
                <Table className="table">
                    <thead>
                        <tr>
                            <th><h5><span class="badge" style={greenStyle}>{t("INV414_frm1_lbl_Actualizados")}</span></h5></th>
                            <th><h5><span class="badge" style={yellowStyle}>{t("INV414_frm1_lbl_Rechazados")}</span></h5></th>
                            <th><h5><span class="badge" style={redStyle}>{t("INV414_frm1_lbl_ConDif")}</span></h5></th>
                            <th><h5><span class="badge" style={blueStyle}>{t("INV414_frm1_lbl_Recontar")}</span></h5></th>
                        </tr>
                    </thead>
                </Table>

            </Form>

            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="INV414_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                        enableSelection
                        onAfterMenuItemAction={onAfterMenuItemAction}
                        onBeforeButtonAction={onBeforeButtonAction}
                    />
                </div>
            </div>

            <INV418 show={showPopupAtributos} onHide={closePopupAtributos} nuInventarioDetalle={nuInventarioDetalle}/>

        </Page>
    );
}