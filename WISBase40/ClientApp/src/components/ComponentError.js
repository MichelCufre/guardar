export class ComponentError extends Error {
    constructor(messageArguments, ...params) {
        super(params);

        this.name = "ComponentError";

        this.messageArguments = messageArguments;
    }
}