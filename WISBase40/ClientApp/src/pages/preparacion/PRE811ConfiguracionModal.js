import React, { useEffect, useLayoutEffect, useRef, useState } from 'react';
import { Button, Modal, Tab, Tabs } from 'react-bootstrap';
import { AddRemovePanel } from '../../components/AddRemovePanel';
import { Form } from '../../components/FormComponents/Form';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useCustomTranslation } from '../../components/TranslationHook';
import { withPageContext } from '../../components/WithPageContext';

function InternalPRE811ConfiguracionModal(props) {
    const { t } = useCustomTranslation();
    const [tabsEnabled, setTabsEnabled] = useState(null);
    const [keyTab, setKeyTab] = useState(null);
    const _nexus = useRef(null);

    const tabConfigs = [
        { key: "PRE811PreferenciaCtrlAcceso", title: t("PRE811CoAc_Sec0_modal_title"), base: "ControlAcceso" },
        { key: "PRE811PrefereciaClase", title: t("PRE811Clas_Sec0_modal_title"), base: "Classe" },
        { key: "PRE811PreferenciaFamilia", title: t("PRE811Fam_Sec0_modal_title"), base: "Familia" },
        { key: "PRE811PreferenciaEmpresa", title: t("PRE811Emp_Sec0_modal_title"), base: "Empresa" },
        { key: "PRE811PreferenciaCliente", title: t("PRE811Cli_Sec0_modal_title"), base: "Cliente" },
        { key: "PRE811PreferenciaRuta", title: t("PRE811Ruta_Sec0_modal_title"), base: "Ruta" },
        { key: "PRE811PreferenciaZona", title: t("PRE811Zona_Sec0_modal_title"), base: "Zona" },
        { key: "PRE811PreferenciaCondLibe", title: t("PRE811PreferenciaCondLibe_Sec0_modal_title"), base: "CondLiberacion" },
        { key: "PRE811PreferenciaTpPedido", title: t("PRE811TpPe_Sec0_modal_title"), base: "TpPedido" },
        { key: "PRE811PreferenciaTpExpedicion", title: t("PRE811TpEx_Sec0_modal_title"), base: "TpExpedicion" },
    ];

    useEffect(() => {
        if (tabsEnabled && !keyTab) {
            const firstEnabled = tabConfigs.find(tab => tabsEnabled[tab.base]);
            if (firstEnabled) {
                setKeyTab(firstEnabled.key);
            }
        }
    }, [tabsEnabled]);

    useEffect(() => {

        const firstEnabled = tabConfigs.find(tab => tab.key === keyTab);
        if (firstEnabled) {
            _nexus.current.getGrid(`Agregar${firstEnabled.base}_grid_1`)?.reset();
            _nexus.current.getGrid(`Quitar${firstEnabled.base}_grid_2`)?.reset();
        }

    }, [keyTab]);

    const onBeforePageLoad = (data) => {
        data.parameters = [{ id: "keyPreferencia", value: props.preferencia }];
    }

    const onAfterPageLoad = (data) => {

        const param = data.parameters.find(d => d.id === "TabsEnabled");
        const tabsEnabled = param ? JSON.parse(param.value) : {};

        setTabsEnabled(tabsEnabled);
    }
    const applyParameters = (context, data, nexus) => {
        _nexus.current = nexus;

        if (keyTab !== data.application) {
            context.abortServerCall = true;
        }

        data.parameters = [{ id: "keyPreferencia", value: props.preferencia }];
    };

    const handleAfterMenuItemAction = (context, data, nexus) => {
        const firstEnabled = tabConfigs.find(tab => tab.key === keyTab);
        if (firstEnabled) {
            _nexus.current.getGrid(`Agregar${firstEnabled.base}_grid_1`)?.refresh();
            _nexus.current.getGrid(`Quitar${firstEnabled.base}_grid_2`)?.refresh();
        }
    };

    const handleClose = (update) => {
        _nexus.current = null;
        setKeyTab(null);

        if (update)
            props.onHide(props.preferencia);
        else
            props.onHide(null);
    };

    return (
        <Modal show={props.show} onHide={() => handleClose()} dialogClassName="modal-70w" backdrop="static">

            <Modal.Header closeButton>
                <Modal.Title>{t("PRE811_lbl_title_Configuracion")}: {`${props.preferencia}`}</Modal.Title>

            </Modal.Header>
            <Modal.Body>
                <Page
                    {...props}
                    application="PRE811Configuracion"
                    onBeforeLoad={onBeforePageLoad}
                    onAfterLoad={onAfterPageLoad}
                >
                        {keyTab ? (
                            <Tabs transition={false} id="noanim-tab-example" activeKey={keyTab} onSelect={setKeyTab}>
                                {tabConfigs.map(tab => (
                                    <Tab
                                        key={tab.key}
                                        eventKey={tab.key}
                                        title={tab.title}
                                        disabled={!tabsEnabled?.[tab.base]}
                                    >
                                        <br />
                                        <AddRemovePanel
                                            onAdd={(evt, nexus) =>
                                                nexus.getGrid(`Agregar${tab.base}_grid_1`).triggerMenuAction("btnAgregar", false, evt.ctrlKey)
                                            }
                                            onRemove={(evt, nexus) =>
                                                nexus.getGrid(`Quitar${tab.base}_grid_2`).triggerMenuAction("btnQuitar", false, evt.ctrlKey)
                                            }
                                            from={(
                                                <Grid
                                                    application={tab.key}
                                                    id={`Agregar${tab.base}_grid_1`}
                                                    rowsToFetch={30}
                                                    rowsToDisplay={15}
                                                    onBeforeInitialize={applyParameters}
                                                    onBeforeFetch={applyParameters}
                                                    onBeforeApplyFilter={applyParameters}
                                                    onBeforeApplySort={applyParameters}
                                                    onBeforeMenuItemAction={applyParameters}
                                                    onAfterMenuItemAction={handleAfterMenuItemAction}
                                                    onBeforeExportExcel={applyParameters}
                                                    onBeforeFetchStats={applyParameters}
                                                    enableExcelExport
                                                    enableSelection
                                                />
                                            )}
                                            to={(
                                                <Grid
                                                    application={tab.key}
                                                    id={`Quitar${tab.base}_grid_2`}
                                                    rowsToFetch={30}
                                                    rowsToDisplay={15}
                                                    onBeforeInitialize={applyParameters}
                                                    onBeforeFetch={applyParameters}
                                                    onBeforeApplyFilter={applyParameters}
                                                    onBeforeApplySort={applyParameters}
                                                    onBeforeMenuItemAction={applyParameters}
                                                    onAfterMenuItemAction={handleAfterMenuItemAction}
                                                    onBeforeExportExcel={applyParameters}
                                                    onBeforeFetchStats={applyParameters}
                                                    enableExcelExport
                                                    enableSelection
                                                />
                                            )}
                                        />
                                    </Tab>
                                ))}
                            </Tabs>
                        ) : (
                            <div className="alert alert-danger" align="center">{t("PRE811_frm1_msg_SinParametrosActivos")}</div>
                        )}

                </Page>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="btn btn-outline-secondary" onClick={() => handleClose()}> {t("PRE811_frm1_btn_cerrar")} </Button>
                <Button variant="primary" onClick={() => handleClose(true)}> {t("PRE811_frm1_btn_EditarPreferencia")} </Button>
            </Modal.Footer>
        </Modal >

    );
}

export const PRE811ConfiguracionModal = withPageContext(InternalPRE811ConfiguracionModal);