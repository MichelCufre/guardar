import React from 'react';

export function InternalLayoutMain(props) {
    return (
        <main>
            <div className="container-fluid mt-3">
                {props.children}
            </div>
        </main>
    );
}

export const LayoutMain = React.memo((props) => <InternalLayoutMain {...props} />, (oldProps, newProps) => !newProps.isMenuOpening && oldProps.doneLoading);