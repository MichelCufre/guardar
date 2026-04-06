import React, { useContext, useEffect } from 'react';
import { FormTabContext } from './WithFormTabContext';
import { useTranslation } from 'react-i18next';


export function FormTabStep(props) {
    const context = useContext(FormTabContext);
    const { t } = useTranslation("translation", { useSuspense: false });

    const className = `form-tab-step ${context.currentTab === props.id ? "" : "hidden"}`;

    useEffect(() => context.addStep({ id: props.id, label: t(props.label)}), []);

    return (
        <div className={className}>
            {props.children}
        </div>
    );
}