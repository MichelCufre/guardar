import React from 'react';
import { FieldRange } from './FormFieldRange';

export function FieldRangePercentage({ min, max, step, ...props }) {
    const tooltipFormat = (value) => {
        return value + "%";
    }

    return (
        <FieldRange tooltipFormat={tooltipFormat} min={min || 0} max={max || 100} step={step || 1} {...props} />
    );
}