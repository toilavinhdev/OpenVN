import { SocketType } from "src/app/shared/enumerations/socket-type.enum";

export class SocketMessage {
    public type = SocketType.Message;
    public message: any;
}