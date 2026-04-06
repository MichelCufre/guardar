import React, { useState, useRef, useEffect, useLayoutEffect } from 'react';
import { useTranslation } from 'react-i18next';

export function CellValue(props) {
    //const t = useTranslation("translation", { useSuspense: false });
    //console.log(props.translate);
    const value = props.translate ? props.translator(props.value) : props.value;
    //const value = props.value;

    return (
        <React.Fragment>
            {value}
        </React.Fragment>
    );
}