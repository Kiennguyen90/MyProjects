export interface CryptoUserModel {
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

export interface ListUserResponeModel {
    listUser: CryptoUserModel[];
    message: string;
}