import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { Tab, Tabs } from 'react-bootstrap';

export default function STO060(props) {

    const { t } = useTranslation();

    const [keyTab, setKeyTab] = useState("etiquetas");

    const onAfterGrid_1MenuItemAction = (context, data, nexus) => {
        nexus.getGrid("STO060_grid_1").refresh();
    }

    const onAfterGrid_2MenuItemAction = (context, data, nexus) => {
        nexus.getGrid("STO060_grid_2").refresh();
    }

    return (
        <Page
            icon="fas fa-cubes"
            title={t("STO060_Sec0_pageTitle_Titulo")}
            {...props}
        >

            <Tabs defaultActiveKey="etiquetas" transition={false} id="noanim-tab-example"
                activeKey={keyTab}
                onSelect={(k) => setKeyTab(k)}
            >
                <Tab eventKey="etiquetas" title={t("STO060_frm1_tab_etiquetas")}>
                    <br></br>
                    <div className="row mb-4">
                        <div className="col">
                            <Grid
                                id="STO060_grid_1"
                                rowsToFetch={30}
                                rowsToDisplay={15}
                                enableSelection
                                enableExcelExport
                                onAfterMenuItemAction={onAfterGrid_1MenuItemAction}
                            />
                        </div>
                    </div>
                </Tab>
                <Tab eventKey="ubicaciones" title={t("STO060_frm1_tab_ubicaciones")}>
                    <br></br>
                    <div className="row mb-4">
                        <div className="col">
                            <Grid
                                id="STO060_grid_2"
                                rowsToFetch={30}
                                rowsToDisplay={15}
                                enableSelection
                                enableExcelExport
                                onAfterMenuItemAction={onAfterGrid_2MenuItemAction}
                            />
                        </div>
                    </div>
                </Tab>
            </Tabs>

        </Page>
    );
}