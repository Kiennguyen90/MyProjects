import { BaseResponseModel } from "../base-respone-model";

export interface UserInformationModel {
    userId: string;
    email: string;
    userName: string;
    phoneNumber: string;
    avatar: string;
    status: string;
    balance: number;
    profit: number;
    totalDeposit: number;
    totalWithdraw: number;
    groupId: string;
    groupAdminId: string;
}

export interface ListUserResponeModel extends BaseResponseModel {
    listUser: UserInformationModel[];
}