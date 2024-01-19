import { CommonConstant } from "src/app/shared/constants/common.constant";
import { BaseModel } from "src/app/models/base/base-model";

export class RefreshTokenModel
{
    public userId = CommonConstant.ZERO_GUID;

    public refreshToken = "";

    public expriedTime!: Date;
}
