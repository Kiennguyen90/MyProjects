export interface AuthModel {
    userInformation: BaseUserInformationModel;
    accessToken: string;
    refreshToken: string;
    error: string;
}

export interface BaseUserInformationModel {
    userId: string;
    email: string;
    userName : string;
}
