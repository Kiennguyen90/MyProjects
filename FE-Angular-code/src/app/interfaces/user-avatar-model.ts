export interface UserAvatarModel {
    userId: string,
    fileName: string,
    content: Uint8Array,
    uploadedAt: Date,
    fileType: string,
    fileSize: number
}