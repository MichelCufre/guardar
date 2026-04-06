import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { Container, Row, Col, Modal, Tab, Tabs, Button } from 'react-bootstrap';


export default function PRE110DetPedidoLpnAtr(props) {

    const { t } = useTranslation();

    const addParameter = (context, data, nexus) => {

        if (props.Pedido !== undefined) {

            data.parameters = [
                { id: "Pedido", value: props.Pedido },
                { id: "Empresa", value: props.Empresa },
                { id: "Cliente", value: props.Cliente },
                { id: "IdEspecificaIdentificador", value: props.IdEspecificaIdentificador },
                { id: "IdLpnExteno", value: props.IdLpnExteno },
                { id: "LpnTipo", value: props.LpnTipo },
                { id: "Producto", value: props.Producto },
                { id: "Identificador", value: props.Identificador },
                { id: "Faixa", value: props.Faixa },
                {id: "NuDetPedSaiAtrib", value: props.NuDetPedSaiAtrib}
            
            ];
        }
    };

    return (

        <Page
            title={t("PRE110DetPedidoLpnAtr_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <Modal.Header closeButton>
                <Modal.Title>{t("PRE110DetPedidoLpnAtr_Sec0_pageTitle_Titulo")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
            <div className="row mb-3">
                <div className="col-12">
                    <Grid id="PRE110DetPedidoLpnAtr_grid_1"
                        application="PRE110DetPedidoLpnAtr"
                        rowsToFetch={30} rowsToDisplay={15} enableExcelExport
                        onBeforeInitialize={addParameter}
                        onBeforeFetch={addParameter}
                    />
                </div>
                </div>
            </Modal.Body>
        </Page>
    );
}