import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Modal, Row, Col, Container } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';


export default function REG605(props) {

    const { t } = useTranslation();

    const [infoProducto, setinfoProducto] = useState({
        CD_PRODUCTO: "", DS_PRODUCTO: "", NM_EMPRESA: "", CD_EMPRESA: ""
    });

    const [infoEmpresa, setInfoEmpresa] = useState({
        NM_EMPRESA: "", CD_EMPRESA: ""
    });

    const [isInfoProductoDisplayed, setinfoProductoDisplayed] = useState(false);

    const [isInfoEmpresaOnly, setIsInfoEmpresaOnly] = useState(false);


    const onAfterInitialize = (context, grid, parameters, nexus) => {


        if ((parameters.find(d => d.id === "REG605_CD_EMPRESA") != null) && (parameters.find(d => d.id === "REG605_CD_PRODUCTO") != null)) {

            setinfoProducto({
                CD_EMPRESA: parameters.find(d => d.id === "REG605_CD_EMPRESA").value,
                NM_EMPRESA: parameters.find(d => d.id === "REG605_NM_EMPRESA").value,
                CD_PRODUCTO: parameters.find(d => d.id === "REG605_CD_PRODUCTO").value,
                DS_PRODUCTO: parameters.find(d => d.id === "REG605_DS_PRODUCTO").value,
            });

            setinfoProductoDisplayed(true);

        } else if (parameters.find(d => d.id === "REG605_CD_EMPRESA") != null) {

            setInfoEmpresa({
                CD_EMPRESA: parameters.find(d => d.id === "REG605_CD_EMPRESA").value,
                NM_EMPRESA: parameters.find(d => d.id === "REG605_NM_EMPRESA").value,
            });



        } else {

            setinfoProductoDisplayed(false);
            setIsInfoEmpresaOnly(false);
        }

    };


    return (
        <Page
            title={t("REG605_Sec0_pageTitle_Titulo")}
            {...props}
        >

            <Container fluid style={{ display: isInfoProductoDisplayed || isInfoEmpresaOnly ? 'block' : 'none' }} >
                <Row>
                    <Col>
                        <Row>
                            <Col style={{ display: isInfoProductoDisplayed ? 'block' : 'none' }}>
                                <span style={{ fontWeight: "bold" }}>{t("REG603_Sec0_Info_Cabezal_Empresa")} </span>
                                <span> {`${infoProducto.CD_EMPRESA}`} - {`${infoProducto.NM_EMPRESA}`}</span>
                            </Col>
                            <Col style={{ display: isInfoProductoDisplayed ? 'block' : 'none' }}>
                                <span style={{ fontWeight: "bold", }}>{t("REG603_Sec0_Info_Cabezal_Producto")} </span>
                                <span> {`${infoProducto.CD_PRODUCTO}`} - {`${infoProducto.DS_PRODUCTO}`} </span>
                            </Col>
                            <Col>
                            </Col>
                        </Row>
                    </Col>
                </Row>
            </Container>
            <hr style={{ display: isInfoProductoDisplayed || isInfoEmpresaOnly ? 'block' : 'none' }}></hr>
            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="REG605_grid_1" rowsToFetch={30} rowsToDisplay={15} onAfterInitialize={onAfterInitialize} enableExcelExport={true}
                    />
                </div>
            </div>

        </Page>
    );
}