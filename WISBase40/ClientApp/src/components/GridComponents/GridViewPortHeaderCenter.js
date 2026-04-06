import React from 'react';
import { ScrollSyncPane } from '../ScrollSyncComponent';

function ViewPortHeaderCenter(props) {
    return (
        <div className="gr-vp-header-center">
            <ScrollSyncPane group="horizontal">
                {props.children}
            </ScrollSyncPane>
        </div>
    );
}

export { ViewPortHeaderCenter };