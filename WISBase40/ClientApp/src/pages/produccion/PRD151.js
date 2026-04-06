import React, { useState, useRef } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { Row, Col, FormGroup } from 'react-bootstrap';

export default function PRD110(props) {
    const { t } = useTranslation();
    const ingreso = useRef({});
    const [isLoaded, setLoaded] = useState(false);

    const onAfterPageLoad = (data) => {
        console.log("onAfterPageLoad");
        if (data && data.parameters) {
            let params = data.parameters.reduce((ing, param) => { ing[param.id] = param.value || "-"; return ing; }, {});

            if (params && params.ingreso != null && params.ingreso != undefined)
                ingreso.current = JSON.parse(params.ingreso);

            setLoaded(true);
        }
    }
    return (

        <Page
            icon="fas fa-file"
            title={t("KIT151_sec0_lbl_PageTitle")}
            onAfterLoad={onAfterPageLoad}
            load
            {...props}
        >
            <Row>
                <div className="row col-12">

                    <div className="row col-6">
                        <table className="table table-bordered table-striped">
                            <tbody>
                                <tr>
                                    <td><strong>{t("KIT151_frm1_lbl_NU_PRDC_INGRESO")}</strong></td>
                                    <td>{ingreso.current.NU_PRDC_INGRESO}</td>
                                    <td>{ingreso.current.DS_TIPO_INGRESO}</td>
                                </tr>
                                <tr>
                                    <td><strong>{t("KIT151_frm1_lbl_CD_PRDC_DEFINICION")}</strong></td>
                                    <td colSpan="2">{ingreso.current.NM_PRDC_DEFINICION}</td>
                                </tr>

                                <tr>
                                    <td><strong>{t("KIT151_frm1_lbl_CD_EMPRESA")}</strong></td>
                                    <td>{ingreso.current.CD_EMPRESA}</td>
                                    <td>{ingreso.current.NM_EMPRESA}</td>
                                </tr>

                                <tr>

                                    <td><strong>{t("KIT151_frm1_lbl_CD_SITUACAO")}</strong></td>
                                    <td>{ingreso.current.CD_SITUACAO}</td>
                                    <td>{ingreso.current.DS_SITUACAO}</td>
                                </tr>

                                <tr>
                                    <td><strong>{t("KIT151_frm1_lbl_ID_GENERAR_PEDIDO")}</strong></td>
                                    {ingreso.current.ID_GENERAR_PEDIDO == 'S'
                                        ? <td colSpan="2">{t("KIT151_frm1_opt_Generar")}</td>
                                        : <td colSpan="2">{t("KIT151_frm1_opt_NoGenerar")}</td>
                                    }
                                </tr>

                            </tbody>
                        </table>
                    </div>

                    <div className="row col-6">
                        <table className="table table-bordered table-striped">
                            <tbody>
                                <tr>
                                    <td><strong>{t("KIT151_frm1_lbl_CD_FUNCIONARIO")}</strong></td>
                                    <td>{ingreso.current.CD_FUNCIONARIO}</td>
                                    <td>{ingreso.current.NM_FUNCIONARIO}</td>
                                </tr>

                                <tr>
                                    <td><strong>{t("KIT151_frm1_lbl_DT_ADDROW")}</strong></td>
                                    <td colSpan="2" >{ingreso.current.DT_ADDROW}</td>
                                </tr>

                                <tr>
                                    <td><strong>{t("KIT151_frm1_lbl_DS_ANEXO1")}</strong></td>
                                    <td colSpan="2">{ingreso.current.DS_ANEXO1}</td>
                                </tr>

                                <tr>
                                    <td><strong>{t("KIT151_frm1_lbl_DS_ANEXO2")}</strong></td>
                                    <td colSpan="2">{ingreso.current.DS_ANEXO2}</td>
                                </tr>

                                <tr>
                                    <td><strong>{t("KIT151_frm1_lbl_DS_ANEXO3")}</strong></td>
                                    <td colSpan="2">{ingreso.current.DS_ANEXO3}</td>
                                </tr>

                                <tr>
                                    <td><strong>{t("KIT151_frm1_lbl_DS_ANEXO4")}</strong></td>
                                    <td colSpan="2">{ingreso.current.DS_ANEXO4}</td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
            </Row>

            {ingreso.current.ND_TIPO == "TPINGPR_BLACKBOX"
                ? <div>
                    <h4 className="form-title">{t("KIT151_sec0_lbl_HeaderProducidoBB")}</h4>
                    <Grid id="PRD151_grid_3" rowsToFetch={30} rowsToDisplay={15} enableExcelExport />

                    <h4 className="form-title">{t("KIT151_sec0_lbl_HeaderConsumidoBB")}</h4>
                    <Grid id="PRD151_grid_4" rowsToFetch={30} rowsToDisplay={15} enableExcelExport />
                </div>
                : <div>
                    <h4 className="form-title">{t("KIT151_sec0_lbl_HeaderConsumido")}</h4>
                    <Grid id="PRD151_grid_1" rowsToFetch={30} rowsToDisplay={15} enableExcelExport />

                    <h4 className="form-title">{t("KIT151_sec0_lbl_HeaderProducido")}</h4>
                    <Grid id="PRD151_grid_2" rowsToFetch={30} rowsToDisplay={15} enableExcelExport />
                </div>
            }
        </Page >
    );
}