import { MoveObject } from "./move-object";

export class CopyCut {
    public from = "";
    public isCopy = false;
    public destinationId = "";
    public destinationSecretCode = "";
    public sourceSecretCode = "";
    public moveObjects: MoveObject[] = [];
}