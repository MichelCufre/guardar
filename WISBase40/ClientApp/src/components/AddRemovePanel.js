import React from 'react';
import { withPageContext } from './WithPageContext';
import { useCustomTranslation } from './TranslationHook';

function InternalAddRemovePanel(props) {
    const { t } = useCustomTranslation("translation", { useSuspense: false });

    const handleAdd = (evt) => {
        props.onAdd(evt, props.nexus)
    };

    const handleRemove = (evt) => {
        props.onRemove(evt, props.nexus)
    };

    const labelAdd = t(props.labelAdd || "General_Sec0_btn_Agregar");
    const labelRemove = t(props.labelRemove || "General_Sec0_btn_Quitar");
    const btnDisabled = (props.BtnDisabled || false);

    return (
        <div className="add-remove-panel">
            <div className="add-remove-panel-from">{props.from}</div>
            <div className="add-remove-panel-button-column">
                <div className="add-remove-panel-button-add">
                    <button className="btn btn-lg btn-outline-primary" disabled={btnDisabled} title={labelAdd} onClick={handleAdd}><i className="fas fa-arrow-right" /></button>
                </div>
                <div className="add-remove-panel-button-remove">
                    <button className="btn btn-lg btn-outline-primary" disabled={btnDisabled} title={labelRemove} onClick={handleRemove}><i className="fas fa-arrow-left" /></button>
                </div>
            </div>
            <div className="add-remove-panel-to">{props.to}</div>
        </div>
    );
}

export const AddRemovePanel = withPageContext(InternalAddRemovePanel);