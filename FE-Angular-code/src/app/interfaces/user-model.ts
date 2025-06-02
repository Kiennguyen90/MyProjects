export interface UserModel {
    id: string,
    email: string,
    fullName: string,
    phoneNumber: string,
    avatar: string,
    services: userServiceModel[] | undefined,
    userRole: string,
}

export interface userServiceModel {
    serviceId: string,
    roleId: string,
}
