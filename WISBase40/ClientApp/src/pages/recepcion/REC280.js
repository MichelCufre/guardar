import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { REC280DetallesSugerenciaModal } from './REC280DetallesSugerenciaModal';

export default function REC280(props) {
    const { t } = useTranslation();

    const [isDetallesSugerenciaOpen, setIsDetallesSugerenciaOpen] = useState(false);

    const [estrategia, setEstrategia] = useState(null);
    const [descripcionEstrategia, setDescripcionEstrategia] = useState(null);
    const [predio, setPredio] = useState(null);
    const [tipoOperativa, setTipoOperativa] = useState(null);
    const [codigoOperativa, seCodigoOperativa] = useState(null);
    const [codigoClase, setCodigoClase] = useState(null);
    const [codigoGrupo, setCodigoGrupo] = useState(null);
    const [empresa, setEmpresa] = useState(null);
    const [producto, setProducto] = useState(null);
    const [codigoReferencia, setCodigoReferencia] = useState(null);
    const [codigoAgrupador, setCodigoAgrupador] = useState(null);
    const [enderecoSugerido, setEnderecoSugerido] = useState(null);
    const [nuAlmSugerencia, setNuAlmSugerencia] = useState(null);


    const handleBeforeButtonAction = (context, data, nexus) => {
        if (data.buttonId === "btnDetalles") {
            context.abortServerCall = true;

            setEstrategia(data.row.cells.find(w => w.column == "NU_ALM_ESTRATEGIA").value);
            setDescripcionEstrategia(data.row.cells.find(w => w.column == "DS_ALM_ESTRATEGIA").value);
            setPredio(data.row.cells.find(w => w.column == "NU_PREDIO").value);
            setTipoOperativa(data.row.cells.find(w => w.column == "TP_ALM_OPERATIVA_ASOCIABLE").value);
            seCodigoOperativa(data.row.cells.find(w => w.column == "CD_ALM_OPERATIVA_ASOCIABLE").value);
            setCodigoClase(data.row.cells.find(w => w.column == "CD_CLASSE").value);
            setCodigoGrupo(data.row.cells.find(w => w.column == "CD_GRUPO").value);
            setEmpresa(data.row.cells.find(w => w.column == "CD_EMPRESA_PRODUTO").value);
            setProducto(data.row.cells.find(w => w.column == "CD_PRODUTO").value);
            setCodigoReferencia(data.row.cells.find(w => w.column == "CD_REFERENCIA").value);
            setCodigoAgrupador(data.row.cells.find(w => w.column == "CD_AGRUPADOR").value);
            setEnderecoSugerido(data.row.cells.find(w => w.column == "CD_ENDERECO_SUGERIDO").value);
            setNuAlmSugerencia(data.row.cells.find(w => w.column == "NU_ALM_SUGERENCIA").value);
            setIsDetallesSugerenciaOpen(true);
        }
    }

    return (
        <Page
            title={t("REC280_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <h4 className="form-title">{t("REC280_frm1_tlt_Sugerencia")}</h4>
            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="REC280_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport={true}
                        onBeforeButtonAction={handleBeforeButtonAction}
                    />
                </div>
            </div>

            <h4 className="form-title">{t("REC280_frm1_tlt_SugerenciaReabas")}</h4>
            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="REC280_grid_2"
                        application="REC280SugerenciaReabas"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport={true}
                        onBeforeButtonAction={handleBeforeButtonAction}
                    />
                </div>
            </div>

            <REC280DetallesSugerenciaModal
                show={isDetallesSugerenciaOpen}
                onHide={() => setIsDetallesSugerenciaOpen(false)}
                estrategia={estrategia}
                descripcionEstrategia={descripcionEstrategia}
                predio={predio}
                tipoOperativa={tipoOperativa}
                codigoOperativa={codigoOperativa}
                codigoClase={codigoClase}
                codigoGrupo={codigoGrupo}
                empresa={empresa}
                producto={producto}
                codigoReferencia={codigoReferencia}
                codigoAgrupador={codigoAgrupador}
                enderecoSugerido={enderecoSugerido}
                nuAlmSugerencia={nuAlmSugerencia}
            />
        </Page>
    );
}