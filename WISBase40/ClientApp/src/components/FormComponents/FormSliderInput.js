import { styled } from '@mui/system';
import Slider from '@mui/material/Slider';

export const FormSliderInput = styled(Slider)({
    root: {
        color: '#007bff',
        height: 4,
    },
    thumb: {
        height: 15,
        width: 15,
        backgroundColor: '#fff',
        border: '2px solid currentColor',
        marginTop: -7,
        marginLeft: -10,
        '&:focus,&:hover,&$active': {
            boxShadow: 'inherit',
        },
    },
    mark: {
        height: 6,
        width: 1,
        backgroundColor: 'gray',
        marginTop: '-1px'
    },
    active: {},
    valueLabel: {
        left: 'calc(-50% - 4px)',
    },
    track: {
        height: 4,
        borderRadius: 4,
    },
    rail: {
        height: 4,
        borderRadius: 4,
        backgroundColor: 'gray'
    },
});