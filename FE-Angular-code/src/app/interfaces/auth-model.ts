export interface AuthModel {
    userInformation: UserInformation;
    accessToken: string;
    refreshToken: string;
    error: string;
}

export interface UserInformation {
    userId: string;
    email: string;
    userName : string;
}
